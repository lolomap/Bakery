using UnityEngine;

[CreateAssetMenu(fileName = "NewProduct", menuName = "Bakery/Product")]
public class ProductData : ScriptableObject {
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject modelPrefab;
    public float basePrice;
    public float maxShelfLife = 100f; // В условных единицах свежести
}