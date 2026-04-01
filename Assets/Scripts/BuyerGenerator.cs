 
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuyerGenerator : MonoBehaviour
{
    
    public TimeController TM;
    [Header("Настройки генерации")]
    [SerializeField] private float minPatienceTime = 30f;
    [SerializeField] private float maxPatienceTime = 120f;
    
    
    [Serializable]
    public struct GroupWeight
    {
        public BuyerGroup group;
        public AnimationCurve weightCurve; // Кривая: Time (0..1) -> Weight (0..1)
    }
    
    
    // Словарь, определяющий, как меняется популярность группы со временем
    // 0.0 - начало игры (сентябрь/октябрь 1917), 1.0 - конец (декабрь 1917)
    private Dictionary<BuyerGroup, Func<float, float>> _weightCalculators;
    
    [Header("Вероятности групп (%)")]
    
        
    [SerializeField]private List<BuyerGroup> GroupChances;

    void Awake()
    {
        foreach (BuyerGroup group in Enum.GetValues(typeof(BuyerGroup)))
            GroupChances.Add(group);
        InitWeightRules();
    }

    public BuyerData GenerateRandomBuyer()
    {
        BuyerData newBuyer = new BuyerData();
        
        float progress = TM.GetStoryProgress();
        
        newBuyer.group = GenerateWeightedGroup(progress);
        
        // 2. ОБЯЗАТЕЛЬНО создаем объект Demand, иначе будет ошибка NullReference
        newBuyer.demand = new Demand();
        newBuyer.demand.type = (DemandType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(DemandType)).Length);
        newBuyer.demand.basePrice = 10f;
        newBuyer.demand.quantity = UnityEngine.Random.Range(1, 4);

        newBuyer.buyerName = GenerateNameForGroup(newBuyer.group);
        newBuyer.maxPrice = CalculateMaxPrice(newBuyer.demand.basePrice, newBuyer.group);
        
        newBuyer.greetingPhrases = GenerateGreetingPhrases(newBuyer.group);
        newBuyer.demandPhrases = GenerateDemandPhrases(newBuyer.group, newBuyer.demand.type);
        newBuyer.angryPhrases = GenerateAngryPhrases(newBuyer.group);
        newBuyer.thankPhrases = GenerateThankPhrases(newBuyer.group);
        return newBuyer;
    }
    private BuyerGroup GenerateWeightedGroup(float progress)
    {
        // Считаем текущие веса всех групп исходя из времени
        Dictionary<BuyerGroup, float> currentWeights = _weightCalculators.ToDictionary(
            x => x.Key, 
            x => x.Value(progress)
        );

        float totalWeight = currentWeights.Values.Sum();
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var weight in currentWeights)
        {
            cumulativeWeight += weight.Value;
            if (randomValue <= cumulativeWeight)
            {
                return weight.Key;
            }
        }

        return BuyerGroup.Peasants; // Дефолт
    }
    
    private void InitWeightRules()
    {
        _weightCalculators = new Dictionary<BuyerGroup, Func<float, float>>
        {
            // ИМПЕРСКИЕ (Угасают к концу)
            { BuyerGroup.Landowners, (p) => Mathf.Lerp(1.0f, 0.05f, p) },
            { BuyerGroup.Bohemians, (p) => Mathf.Lerp(0.8f, 0.1f, p) },
            { BuyerGroup.Former, (p) => Mathf.Lerp(0.7f, 0.0f, p) },
            { BuyerGroup.ProvisionalGovernmentSupporters, (p) => Mathf.Lerp(1.0f, 0.0f, p) },

            // НЕЙТРАЛЬНЫЕ / ОБЩИЕ (Всегда есть, но могут чуть просесть или вырасти)
            { BuyerGroup.Peasants, (p) => 0.6f },
            { BuyerGroup.Beggars, (p) => Mathf.Lerp(0.3f, 0.8f, p) }, // Нищих становится больше
            { BuyerGroup.AmnestiedPrisoners, (p) => 0.4f },

            // СОВЕТСКИЕ (Растут к концу)
            { BuyerGroup.SovietSupporters, (p) => Mathf.Lerp(0.1f, 1.2f, p) },
            { BuyerGroup.Workers, (p) => Mathf.Lerp(0.4f, 1.0f, p) },
            { BuyerGroup.Speculators, (p) => Mathf.Lerp(0.2f, 1.0f, p) }, // Мешочники плодятся в разруху
            { BuyerGroup.Deserters, (p) => Mathf.Lerp(0.3f, 0.9f, p) }
        };
    }
    
    private BuyerGroup GenerateRandomGroup()
    {
        int randomValue = UnityEngine.Random.Range(0, Enum.GetValues(typeof(BuyerGroup)).Length - 1);
        return GroupChances[randomValue];
    }
    
    private string GenerateNameForGroup(BuyerGroup group)
    {
        Dictionary<BuyerGroup, List<string>> names = new Dictionary<BuyerGroup, List<string>>
        {
            { BuyerGroup.AmnestiedPrisoners , new List<string> { "Иван", "Анна", "Сергей", "Ольга", "Дмитрий", "Елена" } },
            { BuyerGroup.Beggars, new List<string> { "Алексей", "Катя", "Максим", "Аня", "Артем", "София" } },
            { BuyerGroup.Bohemians, new List<string> { "Николай", "Мария", "Петр", "Галина", "Владимир", "Татьяна" } },
            { BuyerGroup.Deserters, new List<string> { "Александр", "Виктория", "Андрей", "Наталья", "Константин", "Ирина" } },
            { BuyerGroup.Former, new List<string> { "Джон", "Эмили", "Карлос", "Софи", "Хироси", "Лин" } },
            { BuyerGroup.Landowners, new List<string> { "Костян", "Вован", "Димас", "Стас", "Рома" } },
            { BuyerGroup.Peasants, new List<string> { "Аристарх", "Аглая", "Тимофей", "Клеопатра" } },
            { BuyerGroup.ProvisionalGovernmentSupporters, new List<string> { "Незнакомец", "Тень", "Гость", "Посетитель" } },
            { BuyerGroup.SovietSupporters, new List<string> { "Незнакомец", "Тень", "Гость", "Посетитель" } },
            { BuyerGroup.Speculators, new List<string> { "Незнакомец", "Тень", "Гость", "Посетитель" } },
            { BuyerGroup.Workers, new List<string> { "Незнакомец", "Тень", "Гость", "Посетитель" } }
        };
        
        List<string> groupNames = names[group];
        return groupNames[UnityEngine.Random.Range(0, groupNames.Count)];
    }
    

    //################ TODO Если надо сделать предпочтения по покупкам у группировок #########################

    // private Demand GenerateDemandForGroup(BuyerGroup group)
    // {
    //    return new Demand ;
    // }
    
    private float CalculateMaxPrice(float basePrice, BuyerGroup group)
    {
        float multiplier = 1f;
        
        switch (group)
        {
            case BuyerGroup.AmnestiedPrisoners:
                multiplier = UnityEngine.Random.Range(0.6f, 0.8f); // У амнистированных денег не много
                break;
            case BuyerGroup.Beggars:
                multiplier = UnityEngine.Random.Range(0.3f, 0.5f); // У нищих еще меньше
                break;
            case BuyerGroup.Bohemians:

                multiplier = UnityEngine.Random.Range(0.5f, 1.0f); // Любят торговаться но разброс большой
                break;
            case BuyerGroup.Deserters:
                multiplier = UnityEngine.Random.Range(0.8f, 1.2f); // Не особо имеют проблемы
                break;
            case BuyerGroup.Former:
                multiplier = UnityEngine.Random.Range(1.0f, 1.8f); // Имеют деньги
                break;
            case BuyerGroup.Landowners:
                multiplier = UnityEngine.Random.Range(0.9f, 1.3f); // Они имеют и деньги но и поторговаться могут
                break;
            case BuyerGroup.Peasants:
                multiplier = UnityEngine.Random.Range(0.7f, 1.0f); // Денег мало но торговаться не умеют 
                break;
            case BuyerGroup.ProvisionalGovernmentSupporters:
                multiplier = UnityEngine.Random.Range(0.5f, 1.5f); // Деньги есть но могут запугивать
                break;
            case BuyerGroup.SovietSupporters:
                multiplier = UnityEngine.Random.Range(0.7f, 1.3f);
                break;
            case BuyerGroup.Speculators:
                multiplier = UnityEngine.Random.Range(0.1f, 0.8f); // Разводилы
                break;
            case BuyerGroup.Workers:
                multiplier = UnityEngine.Random.Range(0.7f, 1.0f); // Денег не сильно много, могут поторговаться но и понимающие
                break;
        }
        
        return basePrice * multiplier;
    }
    
    // private float GetPatienceMultiplier(BuyerGroup group)
    // {
    //     switch (group)
    //     {
    //         case BuyerGroup.Businessman: return 0.7f; // Нетерпеливы
    //         case BuyerGroup.Hooligan: return 0.5f; // Очень нетерпеливы
    //         case BuyerGroup.Pensioner: return 1.5f; // Терпеливы
    //         case BuyerGroup.Student: return 1.2f;
    //         default: return 1f;
    //     }
    // }
    
    private List<string> GenerateGreetingPhrases(BuyerGroup group)
    {
        switch (group)
        {
            case BuyerGroup.AmnestiedPrisoners:
                return new List<string> { "Че почем?", "Здарова!", "Добрый день!", "Вечер в хату!","Че как?" };
            case BuyerGroup.Beggars:
                return new List<string> { "Здравствуйте, сударь.", "Добрый день.", "Можно вас побеспокоить?", "Прошу прощения.", "Хлебушка бы кусочек" };
            case BuyerGroup.Bohemians:
                return new List<string> { "Доброго дня.", "Могу я взглянуть на что-нибудь приличное?", "Здравствуйте.", "Как обстановка" };
            case BuyerGroup.Deserters:
                return new List<string> { "Здорово, хозяин", "Есть что съестное?", "Как тут у вас с провизией?", "Только тихо", "Мне бы чего-нибудь достать по-быстрому" };
            case BuyerGroup.Former:
                return new List<string> { "Здравствуйте.", "Тише, пожалуйста.", "Надеюсь, нас не услышат...", "Что у вас есть хорошего?", "Как нынче дела?" };
            case BuyerGroup.Landowners:
                return new List<string> { "Мое почтение.", "Есть что-нибудь изысканное?", "Здравствуйте. Покажите мне ваш лучший товар.", "Надеюсь, у вас приличные манеры?", "Как дела, голубчик?" };
            case BuyerGroup.Peasants:
                return new List<string> { "Здорово, хозяин!", "Хлеба нет ли?", "Что почем нынче?", "Как торговля идет?"};
            case BuyerGroup.ProvisionalGovernmentSupporters:
                return new List<string> { "Здравствуйте.", "Как обстановка в городе?", "Есть ли свежие газеты?", "Надеюсь, порядок будет скоро наведен"};
            case BuyerGroup.SovietSupporters:
                return new List<string> { "Приветствую, товарищ!", "Как тут с распределением продуктов?", "Всё по справедливости?", "Дайте мне по карточке"};
            case BuyerGroup.Speculators:
                return new List<string> { "Что есть из-под полы?", "Товар есть?", "Тихонько, браток, дело есть", "Может, обменяемся?"};
            case BuyerGroup.Workers:
                return new List<string> { "Здравствуйте", "Товарищ, покажи вон ту вещь", "Почем хлеб?", "Нам бы по норме получить"};
            default:
                return new List<string> { "Здравствуйте!", "Добрый день.", "Приветствую." };
        }
    }
    
    private List<string> GenerateDemandPhrases(BuyerGroup group, DemandType Item)
    {
        string itemName = Item.ToString();
        var basePhrases = new List<string>
        {
            $"Мне нужно {itemName}.",
            $"Дайте {itemName}, пожалуйста.",
            $"Есть {itemName}?",
            $"Хочу купить {itemName}.",
            $"Можно мне {itemName}."
        };   
        return basePhrases;
    }
    
    private List<string> GenerateAngryPhrases(BuyerGroup group)
    {
        switch (group)
        {
            case BuyerGroup.AmnestiedPrisoners:
                return new List<string> {
                    "Ни крошки хлеба! Беспредел!",
                    "Опять кидалово! Даже хлеба нет!",
                    "Мы это так не оставим! Где хавчик?",
                    "Смотри у меня, булочник!",
                    "Эх, не видать нам свежей булки."
                };
            case BuyerGroup.Beggars:
                return new List<string> {
                    "И хлеба нет...",
                    "Хоть бы корочку дали!",
                    "Бог накажет за жадность!",
                    "Совсем обнищали люди...",
                    "Даже здесь пусто..."
                };
            case BuyerGroup.Bohemians:
                return new List<string> {
                    "Какое убожество! Даже хлеб кончился.",
                    "Ничего свежего, как всегда.",
                    "Ваша пекарня — торжество пошлости!",
                    "Опять вдохновения не видать...",
                    "Я разочарован отсутствием выпечки."
                };
            case BuyerGroup.Deserters:
                return new List<string> {
                    "Черт бы побрал эту разруху! Хлеба нет!",
                    "Опять жрать нечего!",
                    "Так и помереть можно с голоду!",
                    "Где обещанный хлеб, сволочи?",
                    "Сматываем удочки отсюда, тут пусто."
                };
            case BuyerGroup.Former:
                return new List<string> {
                    "Какое безобразие! Хлеба нет!",
                    "При новом режиме только пустые прилавки...",
                    "Всё разворовали!",
                    "Когда же это кончится?",
                    "Ужас, просто ужас! Даже хлеба нет!"
                };
            case BuyerGroup.Landowners:
                return new List<string> {
                    "Что за чернь вокруг! Даже хлеба приличного нет!",
                    "Ничего достойного моего внимания.",
                    "Всё растащили эти мужики!",
                    "Как можно этим торговать?",
                    "Прислуга и то лучше питалась!"
                };
            case BuyerGroup.Peasants:
                return new List<string> {
                    "Опять пусто! Хлеба нет!",
                    "Нет ни хлеба, ни муки!",
                    "Грабеж, а не торговля!",
                    "Эх, деревня, деревня...",
                    "Как дальше жить-то без хлеба?"
                };
            case BuyerGroup.ProvisionalGovernmentSupporters:
                return new List<string> {
                    "Опять анархия! Хлеба нет!",
                    "Где власть, где порядок?",
                    "Скоро и пекарен не останется!",
                    "Бардак, вот что это такое!",
                    "Ничего не меняется..."
                };
            case BuyerGroup.SovietSupporters:
                return new List<string> {
                    "Почему нет хлеба?",
                    "Контрреволюция, не иначе!",
                    "Разбазариваете народное добро!",
                    "Донесу в ЧК! Хлеба нет!",
                    "Где пайки, товарищи?"
                };
            case BuyerGroup.Speculators:
                return new List<string> {
                    "Ничего не достать легально...",
                    "Придется толкать свое.",
                    "Эх, упустили вы шанс заработать!",
                    "С вами каши не сваришь.",
                    "Пойду в другом месте поищу."
                };
            case BuyerGroup.Workers:
                return new List<string> {
                    "Почему нет хлеба?",
                    "Опять обман!",
                    "Нас морят голодом!",
                    "Устроим забастовку!",
                    "Все для фронта, всё для победы, а нам что?"
                };
            default:
                return new List<string> {
                    "Ничего нет...",
                    "Печально.",
                    "Полки пустые."
                };
        }

    }
    
    private List<string> GenerateThankPhrases(BuyerGroup group)
    {
        switch (group)
        {
            case BuyerGroup.AmnestiedPrisoners:
                return new List<string> {
                    "Вот это дело! Спасибо, хозяин!",
                    "Уважил, век воли не видать!",
                    "От души! Будем помнить добро.",
                    "Красава! Всё как надо сделал.",
                    "Фарту тебе!"
                };
            case BuyerGroup.Beggars:
                return new List<string> {
                    "Спаси Бог вас!",
                    "Дай Бог здоровья вам и вашим близким!",
                    "Храни вас Господь!",
                    "Спасибо за милость вашу.",
                    "Благодарю от всего сердца."
                };
            case BuyerGroup.Bohemians:
                return new List<string> {
                    "Истинно, вы творите искусство!",
                    "Благодарю за этот нектар и амброзию.",
                    "Восхитительно! Мое почтение мастеру!",
                    "Спасибо за вдохновение!",
                    "Вы спасли мой вечер!"
                };
            case BuyerGroup.Deserters:
                return new List<string> {
                    "Спасибо, выручил, браток!",
                    "Будем живы, не помрем!",
                    "Век не забудем твоей доброты!",
                    "Хороший ты человек, хозяин.",
                    "Спасибо за съестное!"
                };
            case BuyerGroup.Former:
                return new List<string> {
                    "Благодарю вас от всей души.",
                    "Приятно иметь дело с порядочным человеком.",
                    "Как хорошо, что остались еще честные люди!",
                    "Ваша лавка — оазис порядка.",
                    "Большое спасибо за ваш труд."
                };
            case BuyerGroup.Landowners:
                return new List<string> {
                    "Благодарю, голубчик.",
                    "Приятно видеть хороший сервис.",
                    "Ваш товар превосходен!",
                    "Вы знаете свое дело, сударь.",
                    "Мое почтение за качество."
                };
            case BuyerGroup.Peasants:
                return new List<string> {
                    "Спасибо, хозяин, выручил!",
                    "Слава Богу, теперь с хлебом будем!",
                    "Вот это торговля честная!",
                    "Спасибо за соль/хлеб!",
                    "Дай Бог здоровья вам!"
                };
            case BuyerGroup.ProvisionalGovernmentSupporters:
                return new List<string> {
                    "Спасибо! Порядок должен быть во всем!",
                    "Благодарю вас за службу!",
                    "Приятно видеть, что дело движется.",
                    "Спасибо за свежие новости и хлеб!",
                    "Надежда еще есть."
                };
            case BuyerGroup.SovietSupporters:
                return new List<string> {
                    "Спасибо, товарищ! Всё по справедливости.",
                    "Отличная работа по распределению!",
                    "Благодарю за выполнение плана!",
                    "Запишем в книгу почета!",
                    "Вот это я понимаю, советская торговля!"
                };
            case BuyerGroup.Speculators:
                return new List<string> {
                    "Дело сделали! Молодцом!",
                    "Спасибо, хозяин! Выручка будет!",
                    "Все по высшему разряду!",
                    "Еще зайдем, если что!",
                    "Хороший гешефт провернули!"
                };
            case BuyerGroup.Workers:
                return new List<string> {
                    "Спасибо, товарищ, за хлеб!",
                    "Теперь можно и поработать!",
                    "Вот это забота о пролетариате!",
                    "Благодарим за выполнение норм!",
                    "Ура! Хлеб есть!"
                };
            default:
                return new List<string> {
                    "Спасибо.",
                    "Благодарю.",
                    "Отлично."
                };
        }

    }
}
