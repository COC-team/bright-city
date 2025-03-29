using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour
{
    // public string dropZoneName;
    public DynamicList to;
    public DynamicList from;
    private GameObject toPanel;
    private GameObject fromPanel;


    void Start()
    {
        if (this.name == "CardDeckDropZone")
        {
            to = StationManager.Instance.cardDeck;
            from = StationManager.Instance.cardsInUse;
            toPanel = GameObject.Find("CardDeck");
            fromPanel = GameObject.Find("CardsInUse");
        } else if (this.name == "CardsInUseDropZone")
        {
            to = StationManager.Instance.cardsInUse;
            from = StationManager.Instance.cardDeck;
            toPanel = GameObject.Find("CardsInUse");
            fromPanel = GameObject.Find("CardDeck");
        }
    }
    public bool OnDrop(Card card)
    {
        // The object that was dragged
        // Optionally, you can check if the dropped object has a specific tag or component
     
        // Handle the drop, e.g., move the object to the drop zone
        if (!to.Contains(card.cardInfo))
        {   
            to.AddItem(card.cardInfo);
            from.RemoveItem(card.cardInfo);
            card.card.transform.SetParent(toPanel.transform);
            card.transform.position = transform.position;
            CityManager.Instance.UpdateEnergyAmountUI();
            return true;
        }

        return false;
    }
}
