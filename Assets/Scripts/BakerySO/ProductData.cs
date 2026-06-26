using UnityEngine;
using UnityEngine.Serialization;

public enum ProductType
{
    Default,
        
    Ingredient,
    Topping,
    
    Dough,
    FormedBase,
    Baked
}

public enum ProductCategory
{
    Default,
        
    Pastry,
    Bread,
    Confectionery,
    Failed
}

[CreateAssetMenu(fileName = "NewProduct", menuName = "Bakery/Product", order = -999)]
public class ProductData : ItemTetrisSO {
    //TODO: Remove itemName and sprites, they are described in ItemTetrisSO 
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public Sprite worldSprite;
    public float basePrice;
    public float maxShelfLife = 100f; // В условных единицах свежести

    public ProductType Type;
    public ProductCategory Category;
    public float Quality;
}