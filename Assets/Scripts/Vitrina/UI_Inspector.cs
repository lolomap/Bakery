using UnityEngine;
using System.Collections.Generic;

public class UI_Inspector : MonoBehaviour
{
    public GameObject panel;
    public Transform container; // Объект с Vertical Layout Group
    public GameObject itemRowPrefab; // Тот самый префаб из шага 2

    public void ShowShowcase(List<ItemInstance> items)
    {
        panel.SetActive(true);
        
        // Очищаем старые строки
        foreach (Transform child in container) Destroy(child.gameObject);

        // Создаем новые
        foreach (var item in items)
        {
            GameObject row = Instantiate(itemRowPrefab, container);
            row.GetComponent<UI_ItemRow>().Setup(item);
        }
    }

    public void Close() => panel.SetActive(false);
}