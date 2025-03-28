using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicList : MonoBehaviour
{
    public GameObject itemPrefab;  // Assign item prefab in Inspector
    public Transform contentPanel; // Assign Panel (with Vertical Layout Group)
    private List<Card> items = new List<Card>();

    public void AddItem(Card newCard)
    {
        // GameObject newItem = Instantiate(itemPrefab, contentPanel);
        // newItem.GetComponentInChildren<Text>().text = text; // Set text if using UI
        items.Add(newCard);
        newCard.transform.SetParent(contentPanel);
    }

    public void RemoveLastItem()
    {
        if (items.Count > 0)
        {
            Card lastItem = items[-1];
            items.Remove(lastItem);
            Destroy(lastItem);
        }
    }

    public void RemoveItem(Card item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Destroy(item);
        }
    }
}
