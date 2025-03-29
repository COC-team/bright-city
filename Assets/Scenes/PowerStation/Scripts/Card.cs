using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : HoverCursor, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject card;

    public CardInfo cardInfo;
    // public Event cityEvent;
    // public int electricity;
    // public string cardName;
    // public string cardDescription;
    private Vector3 startPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (card == null) return;
        // TMP_Text[] cardTexts = card.GetComponentsInChildren<TMP_Text>();
        // foreach (var cardText in cardTexts)
        // {
        //     cardText.text = cardText.name switch
        //     {
        //         "Card name" => cardName,
        //         "Card description" => cardDescription,
        //         _ => cardText.text
        //     };
        // }
        
    }

    public void SetText()
    {
        TMP_Text[] cardTexts = card.GetComponentsInChildren<TMP_Text>();
        foreach (var cardText in cardTexts)
        {
            cardText.text = cardText.name switch
            {
                "Card name" => cardInfo.cardName,
                "Card description" => cardInfo.cardDescription,
                _ => cardText.text
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
    }
    

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = card.transform.position; // Store initial position
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(card.transform.position).z; // Preserve the card's z-position
        card.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        base.OnPointerExit(null);
        var dropZone = GetDropZoneUnderMouse();
        if (dropZone != null)
        {
            if (dropZone.GetComponent<DropZone>().OnDrop(this))
                return;
        }
        card.transform.position = startPosition;
    }
    
    private GameObject GetDropZoneUnderMouse()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("DropZone"))
            {
                return result.gameObject;
            }
        }

        return null;
    }

    public void OnDestroy()
    {
        // Destroy(card);
    }
}
