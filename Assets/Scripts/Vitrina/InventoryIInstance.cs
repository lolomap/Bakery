using UnityEngine;

[System.Serializable]
public class ItemInstance {
    public ProductData data;
    public float currentFreshness;

    public ItemInstance(ProductData data) {
        this.data = data;
        this.currentFreshness = data.maxShelfLife;
    }

    // Рассчитываем привлекательность (0.0 - 1.0)
    public float GetAttractiveness() {
        return currentFreshness / data.maxShelfLife;
    }
}