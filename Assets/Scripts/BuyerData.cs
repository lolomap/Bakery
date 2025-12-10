using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuyerData
{
    public string buyerName;
    public BuyerGroup group;
    public Demand demand;
    public float maxPrice; // Максимальная цена, которую готов заплатить
    public float patienceTime; // Время до того, как покупатель уйдет
    public List<string> greetingPhrases; // Фразы приветствия
    public List<string> demandPhrases; // Фразы при запросе товара
    public List<string> angryPhrases; // Фразы при уходе/раздражении
    public List<string> thankPhrases; // Фразы благодарности
    public Color nameColor; // Цвет имени (для визуального различия групп)
    
    public string GetRandomGreeting() => greetingPhrases[Random.Range(0, greetingPhrases.Count)];
    public string GetRandomDemand() => demandPhrases[Random.Range(0, demandPhrases.Count)];
    public string GetRandomAngry() => angryPhrases[Random.Range(0, angryPhrases.Count)];
    public string GetRandomThank() => thankPhrases[Random.Range(0, thankPhrases.Count)];
}

// Группировки покупателей
public enum BuyerGroup
{
    Deserters,     // Дезертиры
    Beggars,  // Нищие
    Peasants,      // Крестьяне
    Workers,    // Рабочие
    Bohemians,      // Богема
    Landowners,     // Помещики
    SovietSupporters,    // Сторонники Советов
    ProvisionalGovernmentSupporters, // Сторонники Временного правительства
    AmnestiedPrisoners, //Амнистированные заключенные
    Former, //Бывшие 
    Speculators  // Мешочники и спекулянты
}

// Типы товаров/требований
public enum DemandType
{
    Bun, //Булка
    Baguette, //Багет
    Cracker, //Cухарь
    Cake, // Пирожное
    Croissant,  //Круассан
    Pretzels // Крендель / сушка
}

[System.Serializable]
public class Demand
{
    public DemandType type;
    public int quantity = 1;
    public float basePrice; // Базовая цена товара
}
