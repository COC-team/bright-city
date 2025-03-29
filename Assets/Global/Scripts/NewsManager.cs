using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewsManager : MonoBehaviour
{
    public static NewsManager Instance { get; private set; }  // Singleton Instance

    private PopupController popupController;

    void Start()
    {
        PopupController[] controllers = FindObjectsOfType<PopupController>(true);
        foreach (var controller in controllers)
        {
            popupController = controller;

        }
        
    }
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

    public void ShowNews(String baseMessage, List<Event> events, int neededEnergy, int actualEnergy, int previousDayEnergyDifference, LocationType? locationType)
    {
        Debug.Log("Showing news...");
        string newsText = baseMessage + "\n";
        newsText += $"Previous day statistics:\n - used energy {actualEnergy}/{neededEnergy}\n";
        if (previousDayEnergyDifference != 0)
        {
            newsText += $"- people left the town: {previousDayEnergyDifference}\n";
        }
        
        if (locationType != null)
        {
            newsText += $"Today you unlocked: {locationType}.\n";
        }
        
        if (events == null || events.Count == 0)
        {
            newsText += "No news today.";
        }
        else
        {
            newsText += "Today's news:\n";
            foreach (var eventItem in events)
            {
                newsText += $"  - {eventItem.description}\n";
            }            
        }
        
        GameObject targetObject = FindInactiveGameObject("Popup");

        // Check if the GameObject was found
        if (targetObject == null)
        {
            Debug.LogError("Popup GameObject not found in the scene.");
            return;
        }
        if (targetObject != null)
        {
            // Get the component from the GameObject
            PopupController component = targetObject.GetComponent<PopupController>();
            component.ShowPopup(newsText);
        }
    }

    public void ShowFirst(string message)
    {
        GameObject targetObject = FindInactiveGameObject("Popup");

        // Check if the GameObject was found
        if (targetObject == null)
        {
            Debug.LogError("Popup GameObject not found in the scene.");
            return;
        }
        if (targetObject != null)
        {
            // Get the component from the GameObject
            PopupController component = targetObject.GetComponent<PopupController>();
            component.ShowPopup(message);
        }
    }
    
    public GameObject FindInactiveGameObject(string name)
    {
        // Get all root GameObjects in the scene
        GameObject[] allRootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (var rootObj in allRootObjects)
        {
            GameObject found = FindInHierarchy(rootObj.transform, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private GameObject FindInHierarchy(Transform parent, string name)
    {
        // Check if the current object matches the name
        if (parent.name == name)
        {
            return parent.gameObject;
        }

        // Recursively search the children
        foreach (Transform child in parent)
        {
            GameObject found = FindInHierarchy(child, name);
            if (found != null)
            {
                return found;
            }
        }

        return null; // If not found
    }
}
