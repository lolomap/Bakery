using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Craft
{
    public static class Producer
    {
        private class IngredientStack
        {
            public int Count;
            public float AverageQuality;
        }
        
        private struct RecipeEntry
        {
            public string IngredientName;
            public int Min;
            public int Max;
            public int Perfect;
        }
        
        private static ProductData Craft(List<ProductData> ingredients, List<RecipeEntry> recipe)
        {
            // Calculate avarage quality and count of each ingredient 
            Dictionary<string, IngredientStack> composition = new();
            foreach (ProductData ingredient in ingredients)
            {
                if (!composition.ContainsKey(ingredient.itemName))
                {
                    composition[ingredient.itemName] = new() { Count = 1, AverageQuality = ingredient.Quality };
                }
                else
                {
                    IngredientStack stack = composition[ingredient.itemName];
                    stack.Count++;
                    stack.AverageQuality += (ingredient.Quality - stack.AverageQuality) / stack.Count;
                }
            }

            // Calculate result quality on ingredients quality
            float resultQuality = composition.Values.Sum(ingredient => ingredient.AverageQuality);
            resultQuality /= composition.Count;
            
            // Change result quality on recipe match
            foreach (RecipeEntry recipeEntry in recipe)
            {
                IngredientStack component = composition[recipeEntry.IngredientName];
                
                if (component.Count <= recipeEntry.Min)
                {
                    resultQuality -= Math.Abs(component.Count - recipeEntry.Min) * 1; //TODO: penalty modifier in SO
                }
                else if (component.Count >= recipeEntry.Max)
                {
                    resultQuality -= Math.Abs(component.Count - recipeEntry.Max) * 1; //TODO: penalty modifier in SO
                }
                else if (component.Count == recipeEntry.Perfect)
                {
                    resultQuality += 1; //TODO: penalty modifier in SO
                }

                composition.Remove(recipeEntry.IngredientName);
            }
            resultQuality = composition.Values.Aggregate(resultQuality, (current, ingredient) => current - ingredient.Count * 1); //TODO: penalty

            ProductData result = ScriptableObject.CreateInstance<ProductData>();
            result.Quality = resultQuality;

            return result;
        }

        private static bool CheckFailed(List<ProductData> ingredients, IEnumerable<Func<List<ProductData>, bool>> conditions)
        {
            return conditions.Any(condition => !condition(ingredients));
        }
        
        public static ProductData CraftDough(List<ProductData> ingredients, Func<List<ProductData>, ProductCategory> categoryCondition)
        {
            ProductData dough = Craft(ingredients, new() //TODO: recipes in SO
            {
                new() { IngredientName = "flour", Min = 1, Max = 2 },
                new() { IngredientName = "water", Min = 0, Max = 2 },
                new() { IngredientName = "oil", Min = 0, Max = 2 },
                new() { IngredientName = "yeast", Min = 0, Max = 2 },
                new() { IngredientName = "salt", Min = 0, Max = 2 },
                new() { IngredientName = "soda", Min = 0, Max = 2 },
                new() { IngredientName = "vinegar", Min = 0, Max = 2 },
                new() { IngredientName = "sugar", Min = 0, Max = 2 },
                new() { IngredientName = "egg", Min = 0, Max = 2 },
            });

            bool isFailed = CheckFailed(ingredients, new List<Func<List<ProductData>, bool>>
            {
                e =>
                {
                    return e.Any(ingredient => ingredient.itemName is "water" or "oil");
                }
            });

            dough.Category = isFailed ? ProductCategory.Failed : categoryCondition(ingredients);
            dough.Type = ProductType.Dough;
            
            return dough;
        }

        public static void CraftFormed(ProductData dough /* TODO: how forms are defined? */)
        {
            //TODO: logic to select result product
            dough.itemName = "cake";
            dough.itemName = "cookie";
            dough.itemName = "bread";

            dough.Type = ProductType.FormedBase;
            
            //TODO: quality changes
        }

        public static void Bake(ProductData formedBase, float time, float temperature, float targetTime, float targetTemperature)
        {
            formedBase.Type = ProductType.Baked;

            float error = Math.Abs(time - targetTime);
            error += Math.Abs(temperature - targetTemperature);

            formedBase.Quality -= error * 1; //TODO: penalty in SO
        }

        public static void AddToppings()
        {
            
        }
    }
}
