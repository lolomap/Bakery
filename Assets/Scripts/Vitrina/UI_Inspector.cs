using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Inspector : MonoBehaviour {
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public Slider freshnessBar;

    public void ShowItem(ItemInstance item) {
        panel.SetActive(true);
        nameText.text = item.data.itemName;
        
        float attract = item.GetAttractiveness();
        string status = attract > 0.8f ? "Свежайшее!" : attract > 0.4f ? "Нормально" : "Черствое...";
        
        statsText.text = $"{item.data.description}\n\nСтатус: {status}";
        freshnessBar.value = attract; // Слайдер от 0 до 1
    }

    public void Close() => panel.SetActive(false);
}