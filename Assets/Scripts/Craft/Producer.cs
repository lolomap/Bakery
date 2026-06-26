using System;
using System.Collections.Generic;
using System.Linq;
using BakerySO;
using Kitchen;
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

        private static ProducerConfig _config;

        public static void Init()
        {
            _config = KitchenManager.Instance.Configuration;
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
                IngredientStack component = composition[recipeEntry.Ingredient.itemName];
                
                if (component.Count <= recipeEntry.Min)
                {
                    resultQuality -= Math.Abs(component.Count - recipeEntry.Min) * _config.BaseShortagePenaltyModifier;
                }
                else if (component.Count >= recipeEntry.Max)
                {
                    resultQuality -= Math.Abs(component.Count - recipeEntry.Max) * _config.BaseSurplusPenaltyModifier;
                }
                else if (component.Count == recipeEntry.Perfect)
                {
                    resultQuality += _config.BasePerfectBonusModifier;
                }

                composition.Remove(recipeEntry.Ingredient.itemName);
            }
            resultQuality = composition.Values.Aggregate(resultQuality,
                (current, ingredient) => current - ingredient.Count * _config.BaseRedundantPenaltyModifier);

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
            ProductData dough = Craft(ingredients, _config.DoughRecipe.Ingredients);

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

            formedBase.Quality -= error * _config.BaseBakingPenaltyModifier;
        }

        public static void AddToppings()
        {
            
        }
    }
}
