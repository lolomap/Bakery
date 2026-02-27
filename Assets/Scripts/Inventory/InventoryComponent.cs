using System;
using GameData;
using UI;
using UnityEngine;
using UnityEngine.UI;
namespace Inventory
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryComponent<TItem> : MonoBehaviour where TItem : ItemSO
    {
        public enum CellSizeMode
        {
            Original,
            Overriden,
            Adjust
        }
        
        public Vector2Int Size;
        public CellUI CellPrefab;
        public CellSizeMode CellSize;
        
        public BaseInventory<TItem> Content { get; private set; }

        private RectTransform _rectTransform;
        private GridLayoutGroup _gridUI;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridUI = GetComponent<GridLayoutGroup>();

            _gridUI.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridUI.constraintCount = Size.x;
            
            switch (CellSize)
            {
                case CellSizeMode.Original:
                    _gridUI.cellSize = CellPrefab.GetComponent<Image>().rectTransform.rect.size;
                    break;
                case CellSizeMode.Overriden:
                    break;
                case CellSizeMode.Adjust:
                    _gridUI.cellSize = _rectTransform.rect.size / Size - _gridUI.spacing;
                    break;
            }
            
            CellUI[,] cells = new CellUI[Size.x, Size.y];
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    cells[x, y] = Instantiate(CellPrefab, transform);
                }
            }
            
            Content = new(Size.x, Size.y, cells);
        }

        public Vector2 GridToLocalUI(Vector2Int pos) => GridToLocalUI(pos.x, pos.y);
        public Vector2 GridToLocalUI(int x, int y)
        {
            return new(x * _gridUI.cellSize.x, y * _gridUI.cellSize.y);
        }
        
        public Vector2Int LocalUItoGrid(Vector2 pos)
        {
            return new((int)(pos.x / _gridUI.cellSize.x), (int)(pos.y / _gridUI.cellSize.y));
        }
    }
}
