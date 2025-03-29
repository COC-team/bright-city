using System.Collections.Generic;
using Scenes.PowerStation.Scripts;


public class DynamicList
{
    // public Transform contentPanel; // Assign Panel (with Vertical Layout Group)
    private List<CardInfo> items = new List<CardInfo>();

    void Start()
    {
        // Card[] cards =  contentPanel.GetComponentsInChildren<Card>();
        // foreach (var card in cards)
        // {
        //     AddItem(card);
        // }
    }
    public void AddItem(CardInfo newCard)
    {
        items.Add(newCard);
    }
    

    public bool Contains(CardInfo card)
    {
        return items.Contains(card);
    }

    public void RemoveItem(CardInfo item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public List<CardInfo> GetItems()
    {
        return items;
    }

    public void DestroyAllItems()
    {
        items.Clear();
    }
}
