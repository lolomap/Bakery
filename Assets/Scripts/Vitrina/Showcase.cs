using UnityEngine;
using System.Collections.Generic;

public class Showcase : MonoBehaviour {
    public Transform[] slots; 
    private Dictionary<int, ItemInstance> displayedItems = new Dictionary<int, ItemInstance>();
    private GameObject[] spawnedModels;

    void Awake() {
        spawnedModels = new GameObject[slots.Length];
    }

    // Метод для выкладки товара на конкретный слот
    public void PlaceItem(int slotIndex, ProductData product) {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        // Удаляем старую модель, если была
        if (spawnedModels[slotIndex] != null) Destroy(spawnedModels[slotIndex]);

        ItemInstance newItem = new ItemInstance(product);
        displayedItems[slotIndex] = newItem;

        // Спавним визуал
        GameObject model = Instantiate(product.modelPrefab, slots[slotIndex].position, slots[slotIndex].rotation);
        model.transform.SetParent(slots[slotIndex]);
        spawnedModels[slotIndex] = model;
    }

    // Каджое обновление времени продукты чуть-чуть портятся
    void Update() {
        foreach (var item in displayedItems.Values) {
            if (item.currentFreshness > 0) {
                item.currentFreshness -= Time.deltaTime * 0.5f; // Скорость порчи
            }
        }
    }

    public ItemInstance GetItemAtSlot(int index) => displayedItems.ContainsKey(index) ? displayedItems[index] : null;
}