using UnityEngine;
using UnityEngine.EventSystems;

public class HoverCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D hoverCursor;
    public Texture2D defaultCursor;
    
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("PointerExit");
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
}
