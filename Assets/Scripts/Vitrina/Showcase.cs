using UnityEngine;
using System.Collections.Generic;

public class Showcase : MonoBehaviour 
{
    [Header("Физические слоты")]
    public Transform[] visualSlots; // Перетащи сюда пустые объекты-точки в инспекторе
    
    [Header("Настройки товаров")]
    public List<ProductData> availableProducts;
    public List<ItemInstance> itemsOnDisplay = new List<ItemInstance>();

    // Список для хранения созданных 3D моделей (чтобы удалять их потом)
    private List<GameObject> spawnedModels = new List<GameObject>();

    public void AddRandomProduct() 
    {
        // ПРОВЕРКА: Если товаров уже столько же, сколько слотов — ничего не делаем
        if (itemsOnDisplay.Count >= visualSlots.Length) {
            Debug.Log("Витрина полна!");
            return;
        }

        if (availableProducts.Count > 0) {
            ProductData data = availableProducts[Random.Range(0, availableProducts.Count)];
            itemsOnDisplay.Add(new ItemInstance(data));
            
            RefreshPhysicalModels(); // Обновляем модели на полке
        }
    }

    public void RefreshPhysicalModels()
    {
        // 1. Удаляем старые модели
        foreach (var model in spawnedModels) Destroy(model);
        spawnedModels.Clear();

        // 2. Спавним новые модели согласно списку товаров
        for (int i = 0; i < itemsOnDisplay.Count; i++)
        {
            if (i >= visualSlots.Length) break;

            GameObject prefab = itemsOnDisplay[i].data.modelPrefab;
            if (prefab != null)
            {
                GameObject newModel = Instantiate(prefab, visualSlots[i].position, visualSlots[i].rotation);
                newModel.transform.SetParent(visualSlots[i]);
                spawnedModels.Add(newModel);
            }
        }
    }
}