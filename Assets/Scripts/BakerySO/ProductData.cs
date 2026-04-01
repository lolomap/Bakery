using UnityEngine;


[CreateAssetMenu(fileName = "NewProduct", menuName = "Bakery/Product")]
public class ProductData : ItemTetrisSO {
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public Sprite worldSprite;
    public float basePrice;
    public float maxShelfLife = 100f; // В условных единицах свежести
}