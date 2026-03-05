using System.Collections.Generic;
using GameData;
using UnityEngine;
using Grid;
using UI;
namespace Inventory
{
    public interface IInventory
    {
        public bool CanPlace(int x, int y, ItemSO item);
        public ItemSO Get(int x, int y);
        public bool Set(int x, int y, ItemSO item);
        public void Clear(int x, int y, ItemSO item);
    }
    
    public class BaseInventory<TItem> : IInventory where TItem : ItemSO
    {
        private readonly Grid<TItem> _grid;
        private CellUI[,] _cells;
        private ItemUI[] _items;

        public BaseInventory(int width, int height, CellUI[,] cells)
        {
            _grid = new(width, height);
            _cells = cells;

            foreach (CellUI cell in _cells)
            {
                cell.Inventory = this;
            }
        }

        public bool CanPlace(int x, int y, ItemSO item) => _grid.CanPlace(x, y, (TItem)item);
        public ItemSO Get(int x, int y) => _grid.Get(x, y);
        public bool Set(int x, int y, ItemSO item)
        {
            if (item.GetType() != typeof(TItem)) return false;
            TItem itemData = (TItem)item;

            List<Vector2Int> setPositions = new();
            foreach (Vector2Int occupationPos in itemData.Shape)
            {
                int occupationX = x + occupationPos.x;
                int occupationY = y + occupationPos.y;

                if (!_grid.CanPlace(occupationX, occupationY, itemData))
                {
                    return false;
                }
                
                setPositions.Add(new(occupationX, occupationY));
            }

            foreach (Vector2Int position in setPositions)
            {
                _grid.Set(position.x, position.y, itemData);
            }
            
            return true;
        }
        
        public void Clear(int x, int y, ItemSO item)
        {
            TItem dummy = (TItem)item;
            foreach (Vector2Int occupationPos in item.Shape)
            {
                int occupationX = x + occupationPos.x;
                int occupationY = y + occupationPos.y;
                
                if (_grid.CanPlace(occupationX, occupationY, dummy))
                    _grid.Set(occupationX, occupationY, null);
            }
        }
    }
}
