using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public DynamicList dropList;
    public DynamicList cardDeck;
    
    public void OnDrop(PointerEventData eventData)
    {
        // The object that was dragged
        GameObject droppedObject = eventData.pointerDrag;

        // Optionally, you can check if the dropped object has a specific tag or component
        if (droppedObject != null && droppedObject.GetComponent<Card>() != null)
        {
            Debug.Log("Object dropped on the drop zone!");
            // Handle the drop, e.g., move the object to the drop zone
            droppedObject.transform.position = transform.position;
            dropList.AddItem(droppedObject.GetComponent<Card>());
            cardDeck.RemoveItem(droppedObject.GetComponent<Card>());
        }
    }
}
