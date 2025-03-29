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
    
    private List<Event> usedEvents = new List<Event>(); // List to track used events

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
        SceneLoader.Instance.LoadNewScene("Win", true);
    }
    
    public void LooseGame()
    {
        Debug.Log("You lose!");
        isGameOver = true;
        UpdateFinalMessageUI("You lose!!!!");
        SceneLoader.Instance.LoadNewScene("Lose", true);
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
        
        eventsByDay[city.currentDay] = new List<Event>();
        
        if (eventsByDay.ContainsKey(city.currentDay))
        {
            Debug.Log($"Day {city.currentDay}: Events Occurring");
            if (city.currentDay == 0)
            {
                NewsManager.Instance.ShowNews(baseMessage, new List<Event>(), previousDayEnergyDifference, unlockedLocation);
            }
            var events = GenerateRandomEventsForDay();
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
            component.text = city.currentDay.ToString();  // Update text here
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
        
        Event chillDay = new Event(EventType.NO_IMPACT, "Chill day, no events.");
        
        /*eventsByDay[1] = new List<Event> { chillDay };
        eventsByDay[2] = GenerateRandomEventsForDay();*/
        /*eventsByDay[3] = new List<Event> { chillDay };
        eventsByDay[4] = new List<Event> { chillDay };
        eventsByDay[5] = new List<Event> { chillDay };*/
    }
    
    private List<Event> GenerateRandomEventsForDay()
    {
        List<Location> enabledLocations = city.getEnabledLocations();
        Debug.Log("Generating random events");
        Debug.Log(enabledLocations.Count);
        List<Event> possibleEvents = new List<Event>();

        // Select random number of events (0 to 3)
        int eventCount = UnityEngine.Random.Range(0, 4);

        while (possibleEvents.Count < eventCount)
        {
            // Pick a random enabled location
            Location randomLocation = enabledLocations[UnityEngine.Random.Range(0, enabledLocations.Count)];

            // Generate a random event for the selected location
            Event randomEvent = GetRandomEventForLocation(randomLocation.type);

            // Ensure event is not repeated
            if (!usedEvents.Contains(randomEvent))
            {
                possibleEvents.Add(randomEvent);
                usedEvents.Add(randomEvent); // Mark event as used
            }
        }

        return possibleEvents;
    }
    
    public Event GetRandomEventForLocation(LocationType locationType)
    {
        switch (locationType)
        {
            case LocationType.Hospital:
                List<Event> hospitalEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Medical Crisis. Due to a power shortage, life-support systems shut down, endangering patients in critical condition.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Hospital, 5 } } },
                    new Event(EventType.SOME_LOCATIONS, "Hospital Strike. Nurses and doctors go on strike, demanding better pay and working conditions. Large parts of the hospital cease operations.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Hospital, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Modern Equipment. The hospital acquires new medical devices that require stable electricity supply, greatly improving the quality of care.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Hospital, 6 } } }
                };
                return hospitalEvents[UnityEngine.Random.Range(0, hospitalEvents.Count)];

            case LocationType.School:
                List<Event> schoolEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "School Program Delays. The new educational program rollout is delayed. Some schools are working with limited resources.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.School, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Teacher Shortage. Schools temporarily lack enough teachers, reducing energy usage due to fewer classes.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.School, 1 } } },
                    new Event(EventType.SOME_LOCATIONS, "Student Growth. The city sees an increase in the number of school-aged children, requiring additional equipment for classrooms.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.School, 4 } } },
                    new Event(EventType.SOME_LOCATIONS, "Digital Learning Implementation. Interactive boards and computers are installed in the school to enhance the educational process.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.School, 3 } } }
                };
                return schoolEvents[UnityEngine.Random.Range(0, schoolEvents.Count)];

            case LocationType.Supermarket:
                List<Event> supermarketEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Supply Issues. Due to global logistical problems, deliveries to the supermarket are delayed, leaving some shelves empty. This reduces energy usage.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Supermarket, 2 } } },
                    new Event(EventType.SOME_LOCATIONS, "Low Sales. Due to the economic crisis, supermarket sales drop, temporarily reducing electricity consumption.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Supermarket, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Hypermarket Opening. A new massive hypermarket opens in the city, offering a wide range of products and convenient shopping conditions.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Supermarket, 5 } } },
                    new Event(EventType.SOME_LOCATIONS, "Cooling Storage System. New refrigeration units are installed for product storage, requiring additional power.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Supermarket, 4 } } }
                };
                return supermarketEvents[UnityEngine.Random.Range(0, supermarketEvents.Count)];

            case LocationType.Cinema:
                List<Event> cinemaEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Poor Box Office. The new film is underperforming, causing a drop in ticket sales and minimal energy usage.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Cinema, 2 } } },
                    new Event(EventType.SOME_LOCATIONS, "Film Festival. A major film festival is happening, requiring substantial energy for equipment and lighting.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Cinema, 5 } } },
                    new Event(EventType.SOME_LOCATIONS, "Blockbuster Premiere. The cinema shows the long-awaited film. The theaters are packed, and late-night screenings are popular.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Cinema, 3 } } },
                    new Event(EventType.SOME_LOCATIONS, "Equipment Upgrade. Modern screens and sound systems are installed, improving the viewing experience.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Cinema, 4 } } }
                };
                return cinemaEvents[UnityEngine.Random.Range(0, cinemaEvents.Count)];

            case LocationType.Club:
                List<Event> clubEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Loud Party. A noisy celebration with concerts and dancing is taking place, increasing energy consumption for lights and sound.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Club, 6 } } },
                    new Event(EventType.SOME_LOCATIONS, "Power Outage. The club is experiencing electrical issues, halting all events and reducing energy consumption.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Club, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Neon Lighting and Laser Show. Powerful lasers and dynamic lighting are installed to create a spectacular show.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Club, 4 } } }
                };
                return clubEvents[UnityEngine.Random.Range(0, clubEvents.Count)];

            case LocationType.Factory:
                List<Event> factoryEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Environmental Crisis. The factory faces an environmental disaster due to improper waste disposal, halting production temporarily.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Factory, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Factory Strike. Factory workers go on strike, demanding better pay. Production halts, lowering energy consumption.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Factory, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Industrial Boom. Due to a stable electricity supply, the factory expands its production, creating new jobs.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Factory, 6 } } },
                    new Event(EventType.SOME_LOCATIONS, "Process Automation. New robotic production lines are introduced, increasing efficiency but requiring more energy.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Factory, 7 } } }
                };
                return factoryEvents[UnityEngine.Random.Range(0, factoryEvents.Count)];

            case LocationType.Casino:
                List<Event> casinoEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Gaming Crisis. Multiple machines in the casino malfunction, leading to fewer customers and reduced energy use.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Casino, 2 } } },
                    new Event(EventType.SOME_LOCATIONS, "Casino Closure. The casino is closed for maintenance, reducing energy usage in the area.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Casino, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "International Poker Tournament. The casino hosts a prestigious tournament, attracting players from around the world. The atmosphere is electric.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Casino, 4 } } },
                    new Event(EventType.SOME_LOCATIONS, "VIP Room with Slot Machines. The casino opens a luxurious room for wealthy clients, equipped with high-tech slot machines.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Casino, 5 } } }
                };
                return casinoEvents[UnityEngine.Random.Range(0, casinoEvents.Count)];

            case LocationType.Park:
                List<Event> parkEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Seasonal Work. The park is undergoing seasonal work to plant new trees and shrubs, requiring extra energy for watering and lighting.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Park, 4 } } },
                    new Event(EventType.SOME_LOCATIONS, "City Cleanup. The park is being cleaned manually, without electricity, lowering energy consumption.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Park, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "City Festival. A music and food festival is held in the park, with locals enjoying the festive atmosphere.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Park, 3 } } },
                    new Event(EventType.SOME_LOCATIONS, "Night Illumination. Decorative lanterns are installed to light the paths and create a cozy ambiance.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Park, 2 } } }
                };
                return parkEvents[UnityEngine.Random.Range(0, parkEvents.Count)];

            case LocationType.Farm:
                List<Event> farmEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Farmers' Strike. Farmers go on strike due to low wages and poor working conditions, halting farm production.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Crop Failure. A poor harvest season leads to a halt in farm production, temporarily lowering energy consumption.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Farm Expansion. Farmers expand their agricultural lands, increasing food production for the city.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, 5 } } },
                    new Event(EventType.SOME_LOCATIONS, "Irrigation Systems. Modern irrigation systems are installed, allowing the farm to produce larger crops.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, 6 } } }
                };
                return farmEvents[UnityEngine.Random.Range(0, farmEvents.Count)];

            default:
                return new Event(EventType.NO_IMPACT, "No events available for this location.");
        }
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