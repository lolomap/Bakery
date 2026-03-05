using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Grid
{
    public class Cell
    {
        private readonly int _x;
        private readonly int _y;
        private GridItemBase _gridItemBase;
        private Image _backgroundImage;
        private ItemType _type;
        public bool IsEmpty => _gridItemBase == null;
        public GridItemBase GridItemBase
        {
            get => _gridItemBase;
            set => _gridItemBase = value;
        }
        
        public ItemType Type
        {
            get => _type;
            set => _type = value;
        }


        public Cell(int x, int y)
        {
            _x = x;
            _y = y;
            _gridItemBase = null;
            _backgroundImage = null;
            _type = ItemType.Default;
        }

        public void SetBackgroundImage(Image image)
        {
            _backgroundImage = image;
        }

        public void Highlight(Color color)
        {
            if (_backgroundImage)
            {
                _backgroundImage.color = color;
            }
            else
            {
                Debug.LogWarning("No image attached to this cell");
            }
        }

        public void ClearHighlight()
        {
            if (!_backgroundImage) return;
            _backgroundImage.color = Colors.DefaultCellBackgroundColor;
        }

        public override string ToString()
        {
            return _x + ", " + _y + "\n" + _gridItemBase;
        }

        public void ClearPlacedObject()
        {
            _gridItemBase = null;
        }

    }
}