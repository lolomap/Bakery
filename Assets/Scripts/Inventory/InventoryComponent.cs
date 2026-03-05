using GameData;
using UI;
using UnityEngine;
using UnityEngine.UI;
namespace Inventory
{
    public interface IInventoryComponent
    {
        public RectTransform RectTransform { get; }
        
        /*public Vector2 GridToLocalUI(Vector2Int pos) => GridToLocalUI(pos.x, pos.y);
        public Vector2 GridToLocalUI(int x, int y);
        public Vector2Int LocalUItoGrid(Vector2 pos);*/
    }
    
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryComponent<TItem> : MonoBehaviour, IInventoryComponent where TItem : ItemSO
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
        public RectTransform RectTransform { get; private set; }

        private GridLayoutGroup _gridUI;
        private Vector2 _cellSpacedSize;
        
        private void Start()
        {
            RectTransform = GetComponent<RectTransform>();
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
                    _gridUI.cellSize = RectTransform.rect.size / Size - _gridUI.spacing;
                    break;
            }
            _cellSpacedSize = _gridUI.cellSize + _gridUI.spacing;
            
            CellUI[,] cells = new CellUI[Size.x, Size.y];
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    cells[x, y] = Instantiate(CellPrefab, transform);
                    cells[x, y].InventoryComponent = this;
                    cells[x, y].GridPosition = new(x, y);
                }
            }
            
            Content = new(Size.x, Size.y, cells);
        }

        /*public Vector2 GridToLocalUI(int x, int y)
        {
            return new(x * _cellSpacedSize.x, y * _cellSpacedSize.y);
        }
        public Vector2Int LocalUItoGrid(Vector2 pos)
        {
            Vector2Int res = new((int)(pos.x / _cellSpacedSize.x), (int)(pos.y / _cellSpacedSize.y));
            return res;
        }*/
    }
}
