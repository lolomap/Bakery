using UnityEngine;

[System.Serializable]
public class ItemInstance {
    public ProductData data;
    
    // Время (в секундах от начала игры), когда предмет испортится полностью
    public float expirationTime; 
    // Время, когда предмет был создан/испечен
    public float creationTime;

    public ItemInstance(ProductData data) {
        this.data = data;
        this.creationTime = Time.time;
        // Предмет "умрет" через время, указанное в ProductData
        this.expirationTime = Time.time + data.maxShelfLife;
    }

    // Рассчитываем свежесть (0.0 - 1.0) в ЛЮБОЙ момент времени
    public float GetAttractiveness() {
        if (Time.time >= expirationTime) return 0; // Уже испорчен

        float totalLife = expirationTime - creationTime;
        float remainingLife = expirationTime - Time.time;

        return Mathf.Clamp01(remainingLife / totalLife);
    }
}