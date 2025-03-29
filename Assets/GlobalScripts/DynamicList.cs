using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicList : MonoBehaviour
{
    public Transform contentPanel; // Assign Panel (with Vertical Layout Group)
    private List<Card> items = new List<Card>();

    void Start()
    {
        Card[] cards =  contentPanel.GetComponentsInChildren<Card>();
        foreach (var card in cards)
        {
            AddItem(card);
        }
    }
    public void AddItem(Card newCard)
    {
        items.Add(newCard);
        newCard.transform.SetParent(contentPanel);
    }
    

    public bool Contains(Card card)
    {
        return items.Contains(card);
    }

    public void RemoveItem(Card item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public List<Card> GetItems()
    {
        return items;
    }

    public void DestroyAllItems()
    {
        foreach (var card in items)
        {
            Destroy(card);
        }
        items.Clear();
    }
}
