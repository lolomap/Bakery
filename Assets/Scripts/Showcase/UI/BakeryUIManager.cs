using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BakeryUIManager : MonoBehaviour
{
    public static BakeryUIManager Instance;
    
    [Header("Settings")]
    public GameObject rowPrefab; // Оставим префаб для строки, так удобнее настраивать дизайн

    private GameObject mainPanel;
    private Transform contentContainer;
    private Showcase currentShowcase;

    void Awake() {
        Instance = this;
        CreateUIStructure();
        mainPanel.SetActive(false);
    }

    // Создаем структуру UI программно
    void CreateUIStructure() {
        // 1. Создаем фон панели
        mainPanel = new GameObject("BakeryPanel", typeof(Image));
        mainPanel.transform.SetParent(this.transform, false);
        RectTransform rt = mainPanel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 600);
        mainPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);

        // 2. Заголовок
        GameObject title = new GameObject("Title", typeof(TextMeshProUGUI));
        title.transform.SetParent(mainPanel.transform, false);
        title.GetComponent<TextMeshProUGUI>().text = "ВИТРИНА";
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 270);

        // 3. Контейнер для списка (Scroll View аналог)
        GameObject scrollObj = new GameObject("ListContainer", typeof(RectTransform));
        scrollObj.transform.SetParent(mainPanel.transform, false);
        contentContainer = scrollObj.transform;
        VerticalLayoutGroup vlg = scrollObj.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = false;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 10;
        
        RectTransform scrollRT = scrollObj.GetComponent<RectTransform>();
        scrollRT.sizeDelta = new Vector2(380, 450);

        // Внутри BakeryUIManager.cs в методе CreateUI
        CreateButton("ВЫЛОЖИТЬ ТОВАР", new Vector2(0, -260), () => {
            if (currentShowcase != null) {
                currentShowcase.AddRandomProduct(); // Добавляет данные
                currentShowcase.RefreshPhysicalModels(); // Спавнит 3D модель
                RefreshList(); // Обновляет список в UI
            }
        });
                
        // 5. Кнопка "Закрыть"
        CreateButton("X", new Vector2(180, 280), () => mainPanel.SetActive(false), new Vector2(40, 40));
    }

    void CreateButton(string label, Vector2 pos, UnityEngine.Events.UnityAction action, Vector2? size = null) {
        GameObject btnObj = new GameObject("Button_" + label, typeof(Image), typeof(Button));
        btnObj.transform.SetParent(mainPanel.transform, false);
        btnObj.GetComponent<RectTransform>().anchoredPosition = pos;
        btnObj.GetComponent<RectTransform>().sizeDelta = size ?? new Vector2(160, 40);
        btnObj.GetComponent<Button>().onClick.AddListener(action);
        
        GameObject txt = new GameObject("Text", typeof(TextMeshProUGUI));
        txt.transform.SetParent(btnObj.transform, false);
        txt.GetComponent<TextMeshProUGUI>().text = label;
        txt.GetComponent<TextMeshProUGUI>().color = Color.black;
        txt.GetComponent<TextMeshProUGUI>().fontSize = 18;
        txt.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
    }

    public void OpenShowcase(Showcase showcase) {
        currentShowcase = showcase;
        mainPanel.SetActive(true);
        RefreshList();
    }

    void RefreshList() {
        foreach (Transform child in contentContainer) Destroy(child.gameObject);
        foreach (var item in currentShowcase.itemsOnDisplay) {
            GameObject row = Instantiate(rowPrefab, contentContainer);
            row.GetComponent<UI_ItemRow>().Setup(item);
        }
    }
}