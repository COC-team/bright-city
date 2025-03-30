using System.Collections.Generic;
using UnityEngine;

public class StationManager  : MonoBehaviour
{
    public static StationManager Instance { get; private set; }  // Singleton Instance
    public GameObject cardDeckPanel;
    public GameObject cardsInUsePanel;
    public DynamicList cardDeck = new DynamicList();
    public DynamicList cardsInUse = new DynamicList();
    public GameObject cardPrefab;
    private Dictionary<int, List<CardInfo>> cardsPerDay = new Dictionary<int, List<CardInfo>>();

    private int energyAmount = 0;

    private void Start()
    {
        // cardDeck = cardDeckPanel.GetComponent<DynamicList>();
        // cardsInUse = cardsInUsePanel.GetComponent<DynamicList>();
        InitCards();
        AddCardsOfDay(0);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures the CityManager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances of the CityManager
            return;
        }
    }

    public void ClearCards()
    {
        // cardDeck.DestroyAllItems();
        cardsInUse.DestroyAllItems();
    }
    
    
    public List<Event> GetEvents()
    {
        List<Event> events = new List<Event>();
        foreach (var card in cardsInUse.GetItems())
        {
            if (card.electricity == 0)
            {
                events.Add(card.cityEvent);
            }
        }

        return events;
    }
    public int GetEnergy()
    {
        int producedPower = 0;
        List<CardInfo> usedCards = cardsInUse.GetItems();
        foreach (var usedCard in usedCards)
        {
            if (usedCard.electricity > 0)
            {
                producedPower += usedCard.electricity;
            }
        }
        return producedPower;
    }

    void InitCards()
    {
        List<CardInfo> cards = new List<CardInfo>();
        cards.Add(CreateCard("5", "Gives 1 power to city", 5));
        cardsPerDay.Add(0, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("15", "Gives 5 power to city", 15));
        cards.Add(CreateCard("20", "Gives 10 power to city", 20));
        cardsPerDay.Add(1, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("5", "Gives 10 power to city", 5));
        cards.Add(CreateCard("15", "Gives 10 power to city", 15));
        cards.Add(CreateCard("20", "Gives 10 power to city", 20));
        cards.Add(CreateCard("30", "Gives 10 power to city", 30));
        cardsPerDay.Add(2, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("15", "Gives 10 power to city", 15));
        cards.Add(CreateCard("20", "Gives 10 power to city", 20));
        cards.Add(CreateCard("30", "Gives 10 power to city", 30));
        cardsPerDay.Add(3, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("30", "Gives 10 power to city", 30));
        cards.Add(CreateCard("30", "Gives 10 power to city", 30));
        cards.Add(CreateCard("30", "Gives 10 power to city", 30));
        cards.Add(CreateCard("30", "Gives 10 power to city", 30));
        cards.Add(CreateCard("200", "Gives 10 power to city", 200));
        cardsPerDay.Add(4, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("15", "Gives 10 power to city", 15));
        cards.Add(CreateCard("20", "Gives 10 power to city", 20));
        cards.Add(CreateCard("50", "Gives 10 power to city", 50));
        cards.Add(CreateCard("60", "Gives 10 power to city", 60));
        cardsPerDay.Add(5, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("5", "Gives 10 power to city", 5));
        cardsPerDay.Add(6, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("50", "Gives 10 power to city", 50));
        cards.Add(CreateCard("50", "Gives 10 power to city", 50));
        cards.Add(CreateCard("50", "Gives 10 power to city", 50));
        cards.Add(CreateCard("50", "Gives 10 power to city", 50));
        cards.Add(CreateCard("50", "Gives 10 power to city", 50));
        cardsPerDay.Add(7, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("100", "Gives 10 power to city", 100));
        cards.Add(CreateCard("100", "Gives 10 power to city", 100));
        cards.Add(CreateCard("100", "Gives 10 power to city", 100));
        cardsPerDay.Add(8, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("5", "Gives 10 power to city", 5));
        cards.Add(CreateCard("200", "Gives 10 power to city", 200));
        cardsPerDay.Add(9, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("50", "Gives 10 power to city", 50));
        cards.Add(CreateCard("100", "Gives 10 power to city", 100));
        cards.Add(CreateCard("200", "Gives 10 power to city", 200));
        cardsPerDay.Add(10, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("10", "Gives 10 power to city", 10));
        cards.Add(CreateCard("30", "Gives 10 power to city", 30 ));
        cards.Add(CreateCard("100", "Gives 10 power to city", 100));

        cardsPerDay.Add(11, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("10", "Gives 10 power to city", 10));
        cards.Add(CreateCard("30", "Gives 10 power to city", 30 ));
        cards.Add(CreateCard("200", "Gives 10 power to city", 200));
        cardsPerDay.Add(12, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("30", "Gives 10 power to city", 30));
        cards.Add(CreateCard("100", "Gives 10 power to city", 100));
        cards.Add(CreateCard("200", "Gives 10 power to city", 200));
        cardsPerDay.Add(13, cards);
        cards = new List<CardInfo>();
        cards.Add(CreateCard("10", "Gives 10 power to city", 10));
        cards.Add(CreateCard("100", "Gives 10 power to city", 100));
        cards.Add(CreateCard("200", "Gives 10 power to city", 200));
        cardsPerDay.Add(14, cards);
    }

    public void AddCardsOfDay(int day)
    {
        if (cardsPerDay.ContainsKey(day))
        {
            foreach (var card in cardsPerDay[day])
            {
                cardDeck.AddItem(card);
            }
        }
    }

    CardInfo CreateCard(string cardName, string description, int electricity = 0, Event cardEvent = null)
    {
        var card = new CardInfo();
        card.electricity = electricity;
        card.cityEvent = cardEvent;
        card.cardName = cardName;
        card.cardDescription = description;
        return card;
    }
}
