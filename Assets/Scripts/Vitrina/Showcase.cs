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
    foreach (var model in spawnedModels) Destroy(model);
    spawnedModels.Clear();

    for (int i = 0; i < itemsOnDisplay.Count; i++)
    {
        if (i >= visualSlots.Length) break;

        Sprite itemSprite = itemsOnDisplay[i].data.worldSprite; // Берем спрайт
        if (itemSprite != null)
        {
            // Создаем новый объект для спрайта
            GameObject newSpriteObj = new GameObject("ItemSprite_" + i);
            
            // Добавляем SpriteRenderer и назначаем спрайт
            SpriteRenderer sr = newSpriteObj.AddComponent<SpriteRenderer>();
            sr.sprite = itemSprite;
            
            // Настройка для вида сверху/изометрии
            // Спрайты должны стоять вертикально или лежать (зависит от твоей камеры)
            newSpriteObj.transform.position = visualSlots[i].position;
            newSpriteObj.transform.rotation = visualSlots[i].rotation;
            newSpriteObj.transform.SetParent(visualSlots[i]);
            
            spawnedModels.Add(newSpriteObj);
        }
    }
}
}