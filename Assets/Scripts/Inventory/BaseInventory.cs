using GameData;
using UnityEngine;
using Grid;
using UI;
namespace Inventory
{
    public class BaseInventory<TItem> : MonoBehaviour where TItem : ItemSO
    {
        private Grid<TItem> _grid;
        private CellUI[] _cells;
        private ItemUI _items;

        public BaseInventory(int width, int height)
        {
            _grid = new(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    
                }
            }
        }
        
        public Vector2 GridToUI(int x, int y)
        {
            return Vector2.zero;
        }
        
        public Vector2Int UItoGrid(Vector2 pos)
        {
            return Vector2Int.zero;
        }

        public TItem Get(int x, int y) => _grid.Get(x, y);
        public bool Set(int x, int y, ItemUI item)
        {
            return false;
        }
    }
}
