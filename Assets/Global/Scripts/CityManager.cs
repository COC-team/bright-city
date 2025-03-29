using System;
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
    private City city;
    private Dictionary<int, List<Event>> eventsByDay;
    private Dictionary<int, LocationType> enablingLocationsByDay;
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
        city.locations = new List<Location>(FindObjectsByType<Location>(FindObjectsSortMode.None));
        InitializeEvents();
        InitializeEnablingLocationsByDay();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }
        
        // Simulate a day passing when pressing the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FinishDay();
        }
    }
    
    public void WinGame()
    {
        Debug.Log("You win!");
        isGameOver = true;
        UpdateFinalMessageUI("You win!!!!");
    }
    
    public void LooseGame()
    {
        Debug.Log("You lose!");
        isGameOver = true;
        UpdateFinalMessageUI("You lose!!!!");
    }
    
    public void AddLocation(Location location)
    {
        city.locations.Add(location);
    }

    private void FinishDay()
    {
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
        UpdateDayCounterUI();
        EnableCurrentDayLocation();
        StationManager.Instance.AddCardsOfDay(city.currentDay);
        
        if (eventsByDay.ContainsKey(city.currentDay))
        {
            Debug.Log($"Day {city.currentDay}: Events Occurring");
            var events = eventsByDay[city.currentDay];
            NewsManager.Instance.ShowNews(events, previousDayEnergyDifference);
            city.ApplyEvents(events);
        }
        else
        {
            NewsManager.Instance.ShowNews(null, previousDayEnergyDifference);
        }

        LogAllLocations();
        
        if (city.IsLastDayPassed())
        {
            WinGame();
            Debug.Log("All days have passed.");
        }
        else
        {
            Debug.Log("Day " + city.currentDay + " has passed.");
        }
    }
    
    private void LogAllLocations()
    {
        foreach (var location in city.getEnabledLocations())
        {
            Debug.Log($"Location: {location.type}, Base Energy: {location.baseEnergyAmount}, Current Day Energy: {location.currentDayEnergyAmount}");
        }
    }
    
    private void UpdateDayCounterUI()
    {
        if (dayCounterText != null)
        {
            dayCounterText.text = "Day: " + city.currentDay;  // Update text here
        }
    }
    
    private void UpdateFinalMessageUI(string message)
    {
        if (finalMessage != null)
        {
            finalMessage.text = message;  // Update text here
        }
    }

    private void EnableCurrentDayLocation()
    {
        LocationType locationTypeToEnable = enablingLocationsByDay[city.currentDay];
        foreach (var location in city.locations)
        {
            if (location.type == locationTypeToEnable)
            {
                location.EnableLocation();
                Debug.Log($"Location {locationTypeToEnable} enabled for day {city.currentDay}");
            }
        }
    }
    
    private void InitializeEvents()
    {
        eventsByDay = new Dictionary<int, List<Event>>();
        // Initialize some events
        Event chillDay = new Event(EventType.NO_IMPACT, "Chill day, no events.");
        
        Event earthquake = new Event(EventType.SOME_LOCATIONS, "Earthquake hits the city!");
        earthquake.locationsEnergyModifier = new Dictionary<LocationType, int>
        {
            { LocationType.Factory, -50 },
            { LocationType.Hospital, -30 }
        };

        Event protest = new Event(EventType.SOME_LOCATIONS, "Protest at the School.");
        protest.locationsEnergyModifier = new Dictionary<LocationType, int>
        {
            { LocationType.School, 20 }
        };

        // Adding events to specific days
        eventsByDay[1] = new List<Event> { chillDay };
        eventsByDay[2] = new List<Event> { earthquake };
        eventsByDay[3] = new List<Event> { protest };
    }

    private void InitializeEnablingLocationsByDay()
    {
        enablingLocationsByDay = new Dictionary<int, LocationType>();
        enablingLocationsByDay[1] = LocationType.Hospital;
        enablingLocationsByDay[2] = LocationType.School;
        enablingLocationsByDay[3] = LocationType.Supermarket;
        enablingLocationsByDay[4] = LocationType.Cinema;
        enablingLocationsByDay[5] = LocationType.Club;
    }
}