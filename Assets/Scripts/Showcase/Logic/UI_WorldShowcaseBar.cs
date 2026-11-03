using UnityEngine;
using UnityEngine.UI;

public class UI_WorldShowcaseBar : MonoBehaviour
{
    public Showcase targetShowcase;
    private Slider appealSlider;
    private Image fillImage;

    void Start()
    {

        CreateWorldSlider();
    }

    void CreateWorldSlider()
    {
        // 1. Создаем Canvas
        GameObject canvasObj = new GameObject("AppealCanvas");
        canvasObj.transform.SetParent(this.transform);
        // Смещение вверх над полкой
        canvasObj.transform.localPosition = new Vector3(0, 1.5f, 0); 
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRT = canvasObj.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(200, 50); // Используем нормальные пиксельные размеры
        canvasRT.localScale = new Vector3(0.01f, 0.01f, 0.01f); // И уменьшаем масштаб всего Canvas

        // 2. Фон (Background)
        GameObject bgObj = new GameObject("Background", typeof(Image));
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.GetComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.6f);
        bgObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);

        // 3. Слайдер
        GameObject sliderObj = new GameObject("AppealSlider", typeof(Slider));
        sliderObj.transform.SetParent(canvasObj.transform, false);
        appealSlider = sliderObj.GetComponent<Slider>();
        RectTransform sliderRT = sliderObj.GetComponent<RectTransform>();
        sliderRT.sizeDelta = new Vector2(200, 30);

        // 4. Важно: Fill Area (Контейнер для заполнения)
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform faRT = fillArea.GetComponent<RectTransform>();
        faRT.anchorMin = Vector2.zero;
        faRT.anchorMax = Vector2.one;
        faRT.sizeDelta = new Vector2(-10, -10); // Отступы внутри рамки

        // 5. Сам Fill (Зеленая полоска)
        GameObject fill = new GameObject("Fill", typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        fillImage = fill.GetComponent<Image>();
        fillImage.color = Color.green;
        
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.sizeDelta = Vector2.zero; // Обнуляем, чтобы он зависел только от слайдера

        // Настройка компонента Slider
        appealSlider.fillRect = fillRT;
        appealSlider.minValue = 0;
        appealSlider.maxValue = 1;
        appealSlider.transition = Selectable.Transition.None;
        appealSlider.interactable = false;
    }

    void Update()
    {
        if (targetShowcase == null || appealSlider == null) return;

        // Берем значение из Showcase.cs
        float appeal = targetShowcase.GetTotalShowcaseAttractiveness();
        
        // Ограничиваем значение, чтобы оно не было выше 1 или ниже 0
        appeal = Mathf.Clamp01(appeal);

        
        appealSlider.value = appeal;
        fillImage.color = Color.Lerp(Color.red, Color.green, appeal);

        // Поворот к камере
        if (Camera.main != null)
        {
            appealSlider.transform.parent.LookAt(appealSlider.transform.parent.position + Camera.main.transform.forward);
        }
    }
}