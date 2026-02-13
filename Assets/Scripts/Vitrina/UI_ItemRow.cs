using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemRow : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public Slider freshnessBar;
    public Image fillImage;

    private ItemInstance currentItem; // Запоминаем ссылку на предмет

    public void Setup(ItemInstance item)
    {
        currentItem = item;
        nameText.text = item.data.itemName;
        icon.sprite = item.data.icon;
        UpdateVisuals(); // Выносим обновление визуала в отдельный метод
    }

    // Этот метод будет обновлять слайдер и цвет
    public void UpdateVisuals()
    {
        if (currentItem == null) return;

        float freshness = currentItem.GetAttractiveness();
        freshnessBar.value = freshness;
       // fillImage.color = Color.Lerp(Color.red, Color.green, freshness);
    }

    // Каждый кадр обновляем полоску, если строка видна на экране
    void Update()
    {
        UpdateVisuals();
    }
}