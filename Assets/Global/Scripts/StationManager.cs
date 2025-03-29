using System;
using System.Collections.Generic;
using UnityEngine;

public class StationManager  : MonoBehaviour
{
    public static StationManager Instance { get; private set; }  // Singleton Instance
    public GameObject cardDeckPanel;
    public GameObject cardsInUsePanel;
    private DynamicList cardDeck;
    private DynamicList cardsInUse;
    public GameObject cardPrefab;
    private Dictionary<int, List<GameObject>> cardsPerDay = new Dictionary<int, List<GameObject>>();

    private int energyAmount = 0;

    private void Start()
    {
        cardDeck = cardDeckPanel.GetComponent<DynamicList>();
        cardsInUse = cardsInUsePanel.GetComponent<DynamicList>();
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
        return; // TODO: Remove this
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
        return 0; // TODO: Remove this
        int producedPower = 0;
        List<Card> usedCards = cardsInUse.GetItems();
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
        List<GameObject> cards = new List<GameObject>();
        cards.Add(CreateCard("1 power", "Gives 1 power to city", 1));
        cards.Add(CreateCard("2 power", "Gives 2 power to city", 2));
        cards.Add(CreateCard("3 power", "Gives 3 power to city", 3));
        cards.Add(CreateCard("4 power", "Gives 4 power to city", 4));
        cardsPerDay.Add(0, cards);
        cards = new List<GameObject>();
        cards.Add(CreateCard("5 power", "Gives 5 power to city", 5));
        cards.Add(CreateCard("10 power", "Gives 10 power to city", 10));
        cards.Add(CreateCard("15 power", "Gives 15 power to city", 15));
        cards.Add(CreateCard("20 power", "Gives 20 power to city", 20));
        
        cardsPerDay.Add(1, cards);
    }

    public void AddCardsOfDay(int day)
    {
        if (cardsPerDay.ContainsKey(day))
        {
            foreach (var card in cardsPerDay[day])
            {
                card.SetActive(true);
            }
        }
    }

    GameObject CreateCard(string cardName, string description, int electricity = 0, Event cardEvent = null)
    {
        var cardObject = Instantiate(cardPrefab, cardDeckPanel.transform);
        cardObject.SetActive(false);
        var card = cardObject.GetComponent<Card>();
        card.electricity = electricity;
        card.cityEvent = cardEvent;
        card.cardName = cardName;
        card.cardDescription = description;
        return cardObject;
    }
}
