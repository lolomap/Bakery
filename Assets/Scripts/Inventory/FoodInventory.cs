using System;
using GameData;
using UnityEngine;
namespace Inventory
{
    public class FoodInventory : MonoBehaviour
    {
        public Vector2Int Size;
        
        public BaseInventory<FoodSO> Food { get; private set; }

        private void Awake()
        {
            Food = new(Size.x, Size.y);
        }
    }
}
