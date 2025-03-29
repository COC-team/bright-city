using Scenes.PowerStation.Scripts;
using UnityEngine;

public class CardsInUse : MonoBehaviour
{
    public GameObject cardPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var card in StationManager.Instance.cardsInUse.GetItems())
        {
            // if (card.card != null) continue;
            var newObject = Instantiate(cardPrefab, transform);
            var objectCard = newObject.GetComponent<Card>();
            objectCard.cardInfo = card;
            objectCard.SetText();
            // card.card = newObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
