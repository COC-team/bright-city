using System.Collections.Generic;
using UnityEngine;

public class NewsManager : MonoBehaviour
{
    public PopupController popupController;
    
    public void ShowNews(List<Event> events)
    {
        if (events == null || events.Count == 0)
        {
            popupController.ShowPopup("No news today.");
            return;
        }
        
        string newsText = "Good morning citizens! Today's Events:\n";
        foreach (var eventItem in events)
        {
            newsText += $"- {eventItem.description}\n";
        }
        popupController.ShowPopup(newsText);
    }
}
