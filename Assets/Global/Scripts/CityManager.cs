using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CityManager : MonoBehaviour
{
    public static CityManager Instance { get; private set; }  // Singleton Instance

    public int cityPopulation;
    public int maxCityDaysAmount;
    
    public TextMeshProUGUI dayCounterText;
    public TextMeshProUGUI finalMessage;

    private bool isGameOver = false;
    public bool popupOpened = false;
    public City city;
    private Dictionary<int, List<Event>> eventsByDay;
    private Dictionary<int, LocationType> enablingLocationsByDay;
    private Dictionary<int, string> baseMessageByDay;
    private int previousDayEnergyDifference = 0;

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

        city = new City();
        city.population = cityPopulation;
        city.maxDaysAmount = maxCityDaysAmount;
        StartCoroutine(WaitForSceneLoaderAndLoadScene());
        InitializeEvents();
        InitializeEnablingLocationsByDay();
        InitializeBaseMessagesByDay();

        UpdateEnergyAmountUI();
        UpdateDayCounterUI();
        UpdatePopulationAmountUI();
    }
    
    private IEnumerator WaitForSceneLoaderAndLoadScene()
    {
        // Wait until the next frame to ensure SceneLoader is initialized
        yield return null;

        // Ensure SceneLoader instance is available
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneAndCollectLocations("Town");
        }
        else
        {
            Debug.LogError("SceneLoader instance is not available!");
        }

        // Log locations after loading the scene (if needed)
        LogAllLocations();
    }

    private void Update()
    {
    }
    
    public void WinGame()
    {
        Debug.Log("You win!");
        isGameOver = true;
        UpdateFinalMessageUI("You win!!!!");
        SceneLoader.Instance.LoadNewScene("Win");
    }
    
    public void LooseGame()
    {
        Debug.Log("You lose!");
        isGameOver = true;
        UpdateFinalMessageUI("You lose!!!!");
        SceneLoader.Instance.LoadNewScene("Lose");
    }
    
    public void AddLocation(Location location)
    {
        city.locations.Add(location);
    }

    public void FinishDay()
    {
        if (isGameOver)
        {
            Debug.Log("Game is already over. Cannot finish day.");
            return;
        }
        int actualEnergy = StationManager.Instance.GetEnergy();
        StationManager.Instance.ClearCards();
        int neededEnergy = 0;
        foreach (var location in city.getEnabledLocations())
        {
            neededEnergy += location.currentDayEnergyAmount;
        }
        
        previousDayEnergyDifference = Math.Abs(actualEnergy - neededEnergy);
        Debug.Log("Finished day required energy: " + neededEnergy);
        if (previousDayEnergyDifference != 0)
        {
            city.population -= previousDayEnergyDifference;
            UpdatePopulationAmountUI();
        }
        
        if (city.population <= 0)
        {
            LooseGame();
            return;
        }

        SwitchNextDay();
    }
    
    public void SwitchNextDay()
    {
        city.SwitchNextDay();
        if (city.IsLastDayPassed())
        {
            WinGame();
            Debug.Log("All days have passed.");
            return;
        }
        else
        {
            Debug.Log("Day " + city.currentDay + " has passed.");
        }
        
        UpdateDayCounterUI();
        var unlockedLocation = EnableCurrentDayLocation();
        
        StationManager.Instance.AddCardsOfDay(city.currentDay);
        UpdateEnergyAmountUI();

        var baseMessage = "Good morning citizens!";;
        if (baseMessageByDay.ContainsKey(city.currentDay))
        {
            baseMessage = baseMessageByDay[city.currentDay];
        }
        
        if (eventsByDay.ContainsKey(city.currentDay))
        {
            Debug.Log($"Day {city.currentDay}: Events Occurring");
            var events = eventsByDay[city.currentDay];
            NewsManager.Instance.ShowNews(baseMessage, events, previousDayEnergyDifference, unlockedLocation);
            city.ApplyEvents(events);
        }
        else
        {
            NewsManager.Instance.ShowNews(baseMessage, null, previousDayEnergyDifference, unlockedLocation);
        }

        LogAllLocations();
    }
    
    private void LogAllLocations()
    {
        foreach (var location in city.getEnabledLocations())
        {
            Debug.Log($"Location: {location.type}, Base Energy: {location.baseEnergyAmount}, Current Day Energy: {location.currentDayEnergyAmount}");
        }
    }
    
    public void UpdateDayCounterUI()
    {
        // Debug.Log("Updating day counter UI");
        // if (dayCounterText != null)
        // {
        //     Debug.Log("Day counter text is not null");
        //     dayCounterText.text = "Day: " + city.currentDay;  // Update text here
        // }
        //
        GameObject targetObject = GameObject.Find("DaysCounter");

        // Check if the GameObject was found
        if (targetObject != null)
        {
            // Get the component from the GameObject
            TextMeshProUGUI component = targetObject.GetComponent<TextMeshProUGUI>();
            component.text = "Day: " + city.currentDay;  // Update text here
        }
    }
    
    public void UpdateEnergyAmountUI()
    {
        GameObject targetObject = GameObject.Find("EnergyAmount");

        // Check if the GameObject was found
        if (targetObject != null)
        {
            // Get the component from the GameObject
            TextMeshProUGUI component = targetObject.GetComponent<TextMeshProUGUI>();
            component.text = StationManager.Instance.GetEnergy().ToString();
        }
    }
    
    public void UpdatePopulationAmountUI()
    {
        GameObject targetObject = GameObject.Find("Population");

        // Check if the GameObject was found
        if (targetObject != null)
        {
            // Get the component from the GameObject
            TextMeshProUGUI component = targetObject.GetComponent<TextMeshProUGUI>();
            component.text = city.population.ToString();
        }
    }
    
    private void UpdateFinalMessageUI(string message)
    {
        Debug.Log("Updating final message UI");
        if (finalMessage != null)
        {
            Debug.Log("Final message text is not null");
            finalMessage.text = message;  // Update text here
        }
    }

    private LocationType? EnableCurrentDayLocation()
    {
        if (!enablingLocationsByDay.ContainsKey(city.currentDay))
        {
            Debug.LogError($"No location to enable for day {city.currentDay}");
            return null;
        }
        LocationType locationTypeToEnable = enablingLocationsByDay[city.currentDay];
        foreach (var location in city.locations)
        {
            if (location.type == locationTypeToEnable)
            {
                location.EnableLocation();
                return location.type;
            }
        }

        return null;
    }
    
    private void InitializeEvents()
    {
        eventsByDay = new Dictionary<int, List<Event>>();
        // Initialize some events
        Event chillDay = new Event(EventType.NO_IMPACT, "Chill day, no events.");
        
        Event earthquake = new Event(EventType.SOME_LOCATIONS, "Earthquake hits the city! -50 energy at Factory and -30 at Hospital.");
        earthquake.locationsEnergyModifier = new Dictionary<LocationType, int>
        {
            { LocationType.Factory, -50 },
            { LocationType.Hospital, -30 }
        };

        Event protest = new Event(EventType.SOME_LOCATIONS, "Protest at the School. +20 energy.");
        protest.locationsEnergyModifier = new Dictionary<LocationType, int>
        {
            { LocationType.School, 20 }
        };

        // Adding events to specific days
        eventsByDay[1] = new List<Event> { chillDay };
        eventsByDay[2] = new List<Event> { earthquake };
        eventsByDay[3] = new List<Event> { protest };
        eventsByDay[4] = new List<Event> { chillDay };
        eventsByDay[5] = new List<Event> { chillDay };
    }

    private void InitializeEnablingLocationsByDay()
    {
        enablingLocationsByDay = new Dictionary<int, LocationType>();
        enablingLocationsByDay[1] = LocationType.Hospital;
        enablingLocationsByDay[2] = LocationType.School;
        enablingLocationsByDay[3] = LocationType.Club;
        enablingLocationsByDay[4] = LocationType.Cinema;
        enablingLocationsByDay[5] = LocationType.Supermarket;
    }
    
    private void InitializeBaseMessagesByDay()
    {
        baseMessageByDay = new Dictionary<int, string>();
        baseMessageByDay[1] = "Welcome to the city! Day 1.";
        baseMessageByDay[2] = "Day 2: Things are getting interesting.";
        baseMessageByDay[3] = "Day 3: Keep an eye on the energy levels.";
        baseMessageByDay[4] = "Day 4: The city is growing!";
        baseMessageByDay[5] = "Final Day: Make it count!";
    }
}