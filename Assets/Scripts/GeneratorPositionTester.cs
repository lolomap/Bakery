using UnityEngine;
using System.Collections.Generic;

public class GeneratorPositionTester : MonoBehaviour
{
    public BuyerGenerator generator;
    public int testCount = 1000; // Сколько раз "приглашаем" покупателя для статистики

    [ContextMenu("Run History Test")]
    public void RunTest()
    {
        if (generator == null) generator = GetComponent<BuyerGenerator>();
        if (generator.TM == null) { Debug.LogError("Назначь TimeController в генератор!"); return; }

        Debug.Log("=== НАЧАЛО ТЕСТА ГЕНЕРАЦИИ (1917 ГОД) ===");
        
        // Тестируем три точки времени: Начало (0), Середина (0.5), Конец (1.0)
        TestAtProgress(0f, "НАЧАЛО ИГРЫ (Империя)");
        TestAtProgress(0.5f, "СЕРЕДИНА (Брожение)");
        TestAtProgress(1.0f, "КОНЕЦ (Революция)");
        
        Debug.Log("=== ТЕСТ ЗАВЕРШЕН ===");
    }

    private void TestAtProgress(float progress, string label)
    {
        // Принудительно ставим прогресс в TimeController для теста
        // В TimeController прогресс считается от 274 до 365 дня
        int day = (int)(Mathf.Lerp(274, 365, progress));
        generator.TM.SetStoryProgress(day);

        Dictionary<BuyerGroup, int> stats = new Dictionary<BuyerGroup, int>();

        for (int i = 0; i < testCount; i++)
        {
            BuyerData buyer = generator.GenerateRandomBuyer();
            if (stats.ContainsKey(buyer.group)) stats[buyer.group]++;
            else stats[buyer.group] = 1;
        }

        string result = $"<color=yellow>[{label}]</color> Результаты из {testCount} заходов:\n";
        foreach (var pair in stats)
        {
            float percent = (pair.Value / (float)testCount) * 100f;
            result += $"- {pair.Key}: {pair.Value} ({percent:F1}%)\n";
        }
        Debug.Log(result);
    }
}