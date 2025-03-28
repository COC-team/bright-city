using UnityEngine;
using System.Collections.Generic;

public class CityManager : MonoBehaviour
{
    public int cityPopulation;
    public int maxCityDaysAmount;
    
    private City city;
    private Dictionary<int, List<Event>> eventsByDay;

    private void Awake()
    {
        city = new City();
        city.population = cityPopulation;
        city.maxDaysAmount = maxCityDaysAmount;
        city.locations = new List<Location>
        {
            new Location(LocationType.HOSPITAL, 50),
            new Location(LocationType.SCHOOL, 20),
            new Location(LocationType.SUPERMARKET, 30),
            new Location(LocationType.FACTORY, 100),
        };
        
        eventsByDay = new Dictionary<int, List<Event>>();
        // Initialize some events
        Event chillDay = new Event(EventType.NO_IMPACT, "Chill day, no events.");
        
        Event earthquake = new Event(EventType.SOME_LOCATIONS, "Earthquake hits the city!");
        earthquake.locationsEnergyModifier = new Dictionary<LocationType, int>
        {
            { LocationType.FACTORY, -50 },
            { LocationType.HOSPITAL, -30 }
        };

        Event protest = new Event(EventType.SOME_LOCATIONS, "Protest at the School.");
        protest.locationsEnergyModifier = new Dictionary<LocationType, int>
        {
            { LocationType.SCHOOL, 20 }
        };

        // Adding events to specific days
        eventsByDay[1] = new List<Event> { chillDay };
        eventsByDay[2] = new List<Event> { earthquake };
        eventsByDay[3] = new List<Event> { protest };
    }

    private void Update()
    {
        // Simulate a day passing when pressing the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchNextDay();
        }
    }

    public void SwitchNextDay()
    {
        city.SwitchNextDay();
        
        if (eventsByDay.ContainsKey(city.currentDay))
        {
            Debug.Log($"Day {city.currentDay}: Events Occurring");
            foreach (var gameEvent in eventsByDay[city.currentDay])
            {
                Debug.Log($"Event: {gameEvent.description}, Type: {gameEvent.type}");
                city.ApplyEvent(gameEvent);
            }
        }
        else
        {
            Debug.Log($"Day {city.currentDay}: No events scheduled.");
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
    
    public void WinGame()
    {
        Debug.Log("You win!");
    }
    
    private void LogAllLocations()
    {
        foreach (var location in city.locations)
        {
            Debug.Log($"Location: {location.type}, Base Energy: {location.baseEnergyAmount}, Current Day Energy: {location.currentDayEnergyAmount}");
        }
    }
}