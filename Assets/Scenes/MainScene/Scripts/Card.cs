using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : HoverCursor, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject card;
    public GameObject cityEvent;
    public string cardName;
    public string cardDescription;
    private Vector3 startPosition;
    
    public Texture2D cursorPointer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TMP_Text[] cardTexts = card.GetComponentsInChildren<TMP_Text>();
        foreach (var cardText in cardTexts)
        {
            cardText.text = cardText.name switch
            {
                "Card name" => cardName,
                "Card description" => cardDescription,
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
        print("Card clicked " + cardName);
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
        card.transform.position += (Vector3) eventData.delta; // Move object with mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        card.transform.position = startPosition;
    }
}
