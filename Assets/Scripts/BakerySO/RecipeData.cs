using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace BakerySO
{
    [Serializable]
    public struct RecipeEntry
    {
        public ProductData Ingredient;
        public int Min;
        public int Max;
        public int Perfect;
    }
    
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Bakery/Recipe", order = -999)]
    public class RecipeData : ScriptableObject
    {
        public string Tag;
        public ProductType ResultType;
        public List<RecipeEntry> Ingredients;

        private void OnValidate()
        {
            foreach (RecipeEntry entry in Ingredients.Where(entry => entry.Ingredient.Type == ProductType.Baked))
            {
                Debug.LogWarning($"Recipe ${Tag} contains final product as ingredient: ${entry.Ingredient.itemName}");
            }
        }
    }
}
