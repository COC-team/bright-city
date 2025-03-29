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
        cards.Add(CreateCard("5 power", "Gives 5 power to city", 5));
        cards.Add(CreateCard("10 power", "Gives 5 power to city", 10));
        cards.Add(CreateCard("15 power", "Gives 5 power to city", 15));
        cards.Add(CreateCard("20 power", "Gives 5 power to city", 20));
        
        cardsPerDay.Add(0, cards);
    }

    void AddCardsOfDay(int day)
    {
        if (cardsPerDay.ContainsKey(day))
        {
            foreach (var card in cardsPerDay)
            {
                Debug.Log("dsfds");
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
        card.name = cardName;
        card.cardDescription = description;
        return cardObject;
    }
    
    // public void AddEnergy(int amount)
    // {
    //     energyAmount += amount;
    // }
    //
    // public void RemoveEnergy(int amount)
    // {
    //     energyAmount -= amount;
    //     if (energyAmount < 0)
    //     {
    //         energyAmount = 0;
    //     }
    // }
    //
    // public void ResetEnergy()
    // {
    //     energyAmount = 0;
    // }
}
