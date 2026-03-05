using System.Collections.Generic;
using UnityEngine;
namespace GameData
{
    [CreateAssetMenu(menuName = "Item/Base", order = -1000)]
    public class ItemSO : ScriptableObject
    {
        public List<Vector2Int> Shape = new() {Vector2Int.zero};
        
    }
}
