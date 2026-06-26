using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public TMP_Text flourText;
    public TMP_Text waterText;
    public TMP_Text oilText;
    public TMP_Text sodaText;
    public TMP_Text vinegarText;
    public TMP_Text yeastText;
    public TMP_Text saltText;
    public TMP_Text sugarText;
    public TMP_Text eggsText;
    public TMP_Text milkText;
    public TMP_Text butterText;

    public TMP_Text smallDoughText;
    public TMP_Text smallFormedText;
    public TMP_Text pastryText;

    public TMP_Text breadDoughText;
    public TMP_Text breadFormedText;
    public TMP_Text breadText;

    public TMP_Text sweetDoughText;
    public TMP_Text sweetFormedText;
    public TMP_Text sweetCakeText;
    public TMP_Text creamText;
    public TMP_Text sweetFinalText;

    public int flour = 4;
    public int water = 5;
    public int oil = 3;
    public int soda = 2;
    public int vinegar = 2;
    public int yeast = 2;
    public int salt = 2;
    public int sugar = 3;
    public int eggs = 2;
    public int milk = 3;
    public int butter = 3;

    public int smallDough = 0;
    public int smallFormed = 0;
    public int pastry = 0;

    public int breadDough = 0;
    public int breadFormed = 0;
    public int bread = 0;

    public int sweetDough = 0;
    public int sweetFormed = 0;
    public int sweetCake = 0;
    public int cream = 0;
    public int sweetFinal = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        UpdateAllTexts();
    }

    public void UpdateAllTexts()
    {
        flourText.text = "Мука: " + flour;
        waterText.text = "Вода: " + water;
        oilText.text = "Масло: " + oil;
        sodaText.text = "Сода: " + soda;
        vinegarText.text = "Уксус: " + vinegar;
        yeastText.text = "Дрожжи: " + yeast;
        saltText.text = "Соль: " + salt;
        sugarText.text = "Сахар: " + sugar;
        eggsText.text = "Яйца: " + eggs;
        milkText.text = "Молоко: " + milk;
        butterText.text = "Масло слив.: " + butter;

        smallDoughText.text = "Тесто для печенья: " + smallDough;
        smallFormedText.text = "Заготовка для печенья: " + smallFormed;
        pastryText.text = "Печенье: " + pastry;

        breadDoughText.text = "Тесто для хлеба: " + breadDough;
        breadFormedText.text = "Заготовка для хлеба: " + breadFormed;
        breadText.text = "Хлеб: " + bread;

        sweetDoughText.text = "Тесто для сладостей: " + sweetDough;
        sweetFormedText.text = "Заготовка для сладостей: " + sweetFormed;
        sweetCakeText.text = "Сладости: " + sweetCake;
        creamText.text = "Крем: " + cream;
        sweetFinalText.text = "Торт: " + sweetFinal;
    }

    public bool HasEnough(int flourNeeded, int waterNeeded, int oilNeeded, int sodaNeeded, int vinegarNeeded)
    {
        return flour >= flourNeeded &&
               water >= waterNeeded &&
               oil >= oilNeeded &&
               soda >= sodaNeeded &&
               vinegar >= vinegarNeeded;
    }

    public void SpendResources(int flourNeeded, int waterNeeded, int oilNeeded, int sodaNeeded, int vinegarNeeded)
    {
        flour -= flourNeeded;
        water -= waterNeeded;
        oil -= oilNeeded;
        soda -= sodaNeeded;
        vinegar -= vinegarNeeded;
        UpdateAllTexts();
    }

    public void MakeSmallDough()
    {
        if (HasEnough(1, 1, 1, 1, 1))
        {
            SpendResources(1, 1, 1, 1, 1);
            smallDough += 1;
            UpdateAllTexts();

            Debug.Log("Kneaded the cookie dough!");
        }

        else
        {
            Debug.Log("Not enough ingredients!");
        }
    }

    public void MakeSmallFormed()
    {
        if (smallDough >= 1)
        {
            smallDough -= 1;

            smallFormed += 1;

            UpdateAllTexts();
            Debug.Log("molded cookie blanks");
        }
        else
        {
            Debug.Log("no cookie dough!");
        }
    }

    public void MakeSmallPastry()
    {
        if (smallFormed >= 1)
        {
            smallFormed -= 1;

            pastry += 1;

            UpdateAllTexts();
            Debug.Log("baked cookies");
        }
        else
        {
            Debug.Log("there are no cookie blanks for baking!");
        }
    }

    public void MakeBreadDough()
    {
        if (flour >= 2 && water >= 1 && oil >= 1 && yeast >= 1 && salt >= 1)
        {
            flour -= 2;
            water -= 1;
            oil -= 1;
            yeast -= 1;
            salt -= 1;
            breadDough += 1;
            UpdateAllTexts();
            Debug.Log("Замесили тесто для хлеба!");
        }
        else
        {
            Debug.Log("Не хватает ингредиентов для хлеба!");
        }
    }

    public void MakeBreadFormed()
    {
        if (breadDough >= 1)
        {
            breadDough -= 1;
            breadFormed += 1;
            UpdateAllTexts();
            Debug.Log("Слепили формованный хлеб!");
        }
        else
        {
            Debug.Log("Нет теста для хлеба!");
        }
    }

    public void MakeBreadBaked()
    {
        if (breadFormed >= 1)
        {
            breadFormed -= 1;
            bread += 1;
            UpdateAllTexts();
            Debug.Log("Испекли хлеб!");
        }
        else
        {
            Debug.Log("Нет формованного хлеба!");
        }
    }

    // Методы для сладостей
    public void MakeSweetDough()
    {
        if (flour >= 1 && sugar >= 1 && eggs >= 1 && oil >= 1)
        {
            flour -= 1;
            sugar -= 1;
            eggs -= 1;
            oil -= 1;
            sweetDough += 1;
            UpdateAllTexts();
            Debug.Log("Замесили сладкое тесто!");
        }
        else
        {
            Debug.Log("Не хватает ингредиентов для сладостей!");
        }
    }

    public void MakeSweetFormed()
    {
        if (sweetDough >= 1)
        {
            sweetDough -= 1;
            sweetFormed += 1;
            UpdateAllTexts();
            Debug.Log("Слепили сладкую заготовку!");
        }
        else
        {
            Debug.Log("Нет сладкого теста!");
        }
    }

    public void MakeSweetBaked()
    {
        if (sweetFormed >= 1)
        {
            sweetFormed -= 1;
            sweetCake += 1;
            UpdateAllTexts();
            Debug.Log("Испекли сладость!");
        }
        else
        {
            Debug.Log("Нет сладких заготовок!");
        }
    }
}
