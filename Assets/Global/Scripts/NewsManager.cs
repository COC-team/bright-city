using System.Collections.Generic;
using UnityEngine;

public class NewsManager : MonoBehaviour
{
    public static NewsManager Instance { get; private set; }  // Singleton Instance

    public PopupController popupController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures the CityManager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances of the CityManager
            return;
        }
    }

    public void ShowNews(List<Event> events, int previousDayEnergyDifference)
    {
        string newsText = "Good morning citizens!\n";
        if (previousDayEnergyDifference != 0)
        {
            newsText += $"Yesterday, died {previousDayEnergyDifference} citizens. :D\n";
        }
        
        if (events == null || events.Count == 0)
        {
            newsText += "No news today.";
        }
        else
        {
            newsText = "Today's news:\n";
            foreach (var eventItem in events)
            {
                newsText += $"  - {eventItem.description}\n";
            }            
        }
        
        // popupController.ShowPopup(newsText);
    }
}
