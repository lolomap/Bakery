using GameData;
using UnityEngine;
using Grid;
using UI;
namespace Inventory
{
    public class BaseInventory<TItem> where TItem : ItemSO
    {
        private readonly Grid<TItem> _grid;
        private CellUI[,] _cells;
        private ItemUI[] _items;

        public BaseInventory(int width, int height, CellUI[,] cells)
        {
            _grid = new(width, height);
            _cells = cells;
        }

        public TItem Get(int x, int y) => _grid.Get(x, y);
        public bool Set(int x, int y, ItemUI item)
        {
            return false;
        }
    }
}
