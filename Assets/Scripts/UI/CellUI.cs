using System;
using Grid;
using Inventory;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CellUI : MonoBehaviour
    {
        public Image Background;
        public Color PlacableHintColor;
        public Color NotPlacableHintColor;
        
        public IInventory Inventory { get; set; }
        public IInventoryComponent InventoryComponent { get; set; }
        public Vector2Int GridPosition { get; set; }
        
        private ICell _data;

        private void Awake()
        {
        }

        public void Highlight(Color color)
        {
            Background.color = color;
        }

        public void OnHover(bool canPlace)
        {
            Highlight(canPlace? PlacableHintColor : NotPlacableHintColor);
        }

        public void OnNotHover()
        {
            Highlight(Color.white);
        }
    }
}
