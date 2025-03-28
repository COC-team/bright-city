using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour
{
    public DynamicList to;
    public DynamicList from;
    
    public bool OnDrop(Card card)
    {
        // The object that was dragged
        print("On drop");
        // Optionally, you can check if the dropped object has a specific tag or component
     
        Debug.Log("Object dropped on the drop zone!");
        // Handle the drop, e.g., move the object to the drop zone
        if (!to.Contains(card))
        {   
            print("card moving");
            to.AddItem(card);
            from.RemoveItem(card);
            card.transform.position = transform.position;
            return true;
        }

        return false;
    }
}
