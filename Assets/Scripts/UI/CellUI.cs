using System;
using Grid;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    [RequireComponent(typeof(Image))]
    public class CellUI : MonoBehaviour
    {
        public Image Background { get; private set; }

        private ICell _data;

        private void Awake()
        {
            Background = GetComponent<Image>();
        }

        public void Highlight()
        {
            
        }

        public void OnHover(ItemUI item)
        {
            
        }
    }
}
