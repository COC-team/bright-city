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
    
    public bool firstPowerStation = false;
    public bool firstTown = false;

    private bool isGameOver = false;
    public bool popupOpened = false;
    public City city;
    private Dictionary<int, List<Event>> eventsByDay;
    private Dictionary<int, LocationType> enablingLocationsByDay;
    private Dictionary<int, string> baseMessageByDay;
    private int previousDayEnergyDifference = 0;
    private int previousDayNeededEnergy = 0;
    private int previousDayActualEnergy = 0;
    private string lastNewsMessage;
    
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
        
        StartCoroutine(WaitAndShowFirst());
    }
    
    IEnumerator WaitAndShowFirst()
    {
        yield return new WaitForSeconds(1f);
        lastNewsMessage = "DO NOT SKIP!\n\nWelcome new manager!\n\nThis is your first day in the city. You will be responsible for managing the energy supply and ensuring the well-being of the citizens. Good luck! Visit the Power Station and the Town to get started.";
        NewsManager.Instance.ShowFirst(lastNewsMessage);
    }
    
    public void DisplayLastNewsMessage()
    {
        if (lastNewsMessage != null)
        {
            NewsManager.Instance.ShowLastMessage(lastNewsMessage);
        }
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
        SceneLoader.Instance.LoadNewScene("Win", true);
    }
    
    public void LooseGame()
    {
        Debug.Log("You lose!");
        isGameOver = true;
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
        previousDayActualEnergy = StationManager.Instance.GetEnergy();
        StationManager.Instance.ClearCards();
        previousDayNeededEnergy = 0;
        foreach (var location in city.getEnabledLocations())
        {
            previousDayNeededEnergy += location.currentDayEnergyAmount;
        }
        
        previousDayEnergyDifference = previousDayNeededEnergy - previousDayActualEnergy;
        if (previousDayEnergyDifference < 0)
        {
            previousDayEnergyDifference = 0;
        }

        if (previousDayEnergyDifference != 0)
        {
            city.population -= previousDayEnergyDifference;
            if (city.population < 0)
            {
                city.population = 0;
            }
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
        
        Debug.Log($"Day {city.currentDay}: Events Occurring");
        var events = GenerateRandomEventsForDay();
        lastNewsMessage = NewsManager.Instance.ShowNews(baseMessage, events, previousDayNeededEnergy, previousDayActualEnergy, previousDayEnergyDifference, unlockedLocation);
        
        city.ApplyEvents(events);

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

        // Select number of events (0 to 3)
        int eventCount;
        int randomValue = UnityEngine.Random.Range(0, 100);
        if (city.currentDay is 0 or 1)
        {
            eventCount = 0;
        }
        else if (city.currentDay is 2)
        {
            eventCount = 1;
        }
        
        else if (city.currentDay <= 5 )
        {
            if (randomValue < 20) eventCount = 0;      // 15%
            else if (randomValue < 80) eventCount = 1; // 25%
            /*else if (randomValue < 95) eventCount = 2; // 45%*/
            else eventCount = 2;                       // 15%
        }
        else
        {
            if (randomValue < 10) eventCount = 0;      // 15%
            else if (randomValue < 30) eventCount = 1; // 25%
            /*else if (randomValue < 70) eventCount = 2; // 45%*/
            else eventCount = 2;                       // 15%
        }


        while (possibleEvents.Count < eventCount)
        {
            // Pick a random enabled location
            Location randomLocation = enabledLocations[UnityEngine.Random.Range(0, enabledLocations.Count)];

            // Generate a random event for the selected location
            Event randomEvent = GetRandomEventForLocation(randomLocation.type);

            if (usedEvents.Count == 30)
            {
                usedEvents = new List<Event>();
            }
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
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Hospital, 7 } } },
                    new Event(EventType.SOME_LOCATIONS, "Hospital Strike. Nurses and doctors go on strike, demanding better pay and working conditions. Hospital stops working.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Hospital, -60 } } },
                    new Event(EventType.SOME_LOCATIONS, "Modern Equipment. The hospital acquires new medical devices that require stable electricity supply, greatly improving the quality of care.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Hospital, 6 } } }
                };
                return hospitalEvents[UnityEngine.Random.Range(0, hospitalEvents.Count)];

            case LocationType.School:
                List<Event> schoolEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "School Program Delays. The new educational program rollout is delayed. School is not working.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.School, -30 } } },
                    new Event(EventType.SOME_LOCATIONS, "Teacher Shortage. Schools temporarily lack enough teachers, reducing energy usage due to fewer classes.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.School, 3 } } },
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
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Supermarket, -20 } } },
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
                    new Event(EventType.SOME_LOCATIONS, "Power Outage. The club is experiencing electrical issues. No more fun for today") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Club, -30 } } },
                    new Event(EventType.SOME_LOCATIONS, "Neon Lighting and Laser Show. Powerful lasers and dynamic lighting are installed to create a spectacular show.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Club, 4 } } }
                };
                return clubEvents[UnityEngine.Random.Range(0, clubEvents.Count)];

            case LocationType.Factory:
                List<Event> factoryEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Environmental Crisis. The factory faces an environmental disaster due to improper waste disposal, pausing production temporarily.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Factory, -80 } } },
                    new Event(EventType.SOME_LOCATIONS, "Factory Strike. Factory workers go on strike, demanding better pay. Production stopping, lowering energy consumption.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Factory, -80 } } },
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
                    new Event(EventType.SOME_LOCATIONS, "Casino Closure. The casino is closed for maintenance.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Casino, -25 } } },
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
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Park, -10 } } },
                    new Event(EventType.SOME_LOCATIONS, "City Festival. A music and food festival is held in the park, with locals enjoying the festive atmosphere.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Park, 3 } } },
                    new Event(EventType.SOME_LOCATIONS, "Night Illumination. Decorative lanterns are installed to light the paths and create a cozy ambiance.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Park, 2 } } }
                };
                return parkEvents[UnityEngine.Random.Range(0, parkEvents.Count)];

            case LocationType.Farm:
                List<Event> farmEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Farmers' Strike. Farmers go on strike due to low wages and poor working conditions.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, -20 } } },
                    new Event(EventType.SOME_LOCATIONS, "Crop Failure. A poor harvest season leads to a pause in farm production.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, -20 } } },
                    new Event(EventType.SOME_LOCATIONS, "Farm Expansion. Farmers expand their agricultural lands, increasing food production for the city.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, 5 } } },
                    new Event(EventType.SOME_LOCATIONS, "Irrigation Systems. Modern irrigation systems are installed, allowing the farm to produce larger crops.") 
                    { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.Farm, 6 } } }
                };
                return farmEvents[UnityEngine.Random.Range(0, farmEvents.Count)];
            case LocationType.University:
                List<Event> universityEvents = new List<Event>
                {
                    new Event(EventType.SOME_LOCATIONS, "Professor Strike. Professors demand better salaries, pausing university operations.") 
                        { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.University, -30 } } },
                    new Event(EventType.SOME_LOCATIONS, "Research Breakthrough. Scientists at the university make a major discovery, boosting technological progress.") 
                        { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.University, 5 } } },
                    new Event(EventType.SOME_LOCATIONS, "Government Funding. The university receives additional funding, improving education quality.") 
                        { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.University, 0 } } },
                    new Event(EventType.SOME_LOCATIONS, "Student Protest. Students rally for reforms, causing temporary disruptions in studies.") 
                        { locationsEnergyModifier = new Dictionary<LocationType, int> { { LocationType.University, 2 } } }
                };
                return universityEvents[UnityEngine.Random.Range(0, universityEvents.Count)];

            default:
                return new Event(EventType.NO_IMPACT, "No events available for this location.");
        }
    }

    private void InitializeEnablingLocationsByDay()
    {
        enablingLocationsByDay = new Dictionary<int, LocationType>();
        enablingLocationsByDay[1] = LocationType.Farm;
        enablingLocationsByDay[2] = LocationType.Cinema;
        enablingLocationsByDay[3] = LocationType.Park;
        enablingLocationsByDay[4] = LocationType.Hospital;
        enablingLocationsByDay[5] = LocationType.University;
        enablingLocationsByDay[6] = LocationType.Casino;
        enablingLocationsByDay[7] = LocationType.School;
        enablingLocationsByDay[8] = LocationType.Factory;
        enablingLocationsByDay[9] = LocationType.Supermarket;
        enablingLocationsByDay[10] = LocationType.Club;
    }
    
    private void InitializeBaseMessagesByDay()
    {
        baseMessageByDay = new Dictionary<int, string>();
        baseMessageByDay[1] = "Day 1: You will unlock locations throughout next 14 days.\nREMEMBER: you need to get as close as possible to electricity consuming level or \nPEOPLE WILL DIE\n";
        baseMessageByDay[2] = "Day 2: Things are getting interesting.\n \nREMEMBER: you can always open this menu by clicking at the calender icon.\n";
        baseMessageByDay[3] = "Day 3: Keep an eye on the energy levels.";
        baseMessageByDay[4] = "Day 4: The city is evolving, stay vigilant.";
        baseMessageByDay[5] = "Day 5: Challenges arise, plan wisely.";
        baseMessageByDay[6] = "Day 6: The citizens rely on your decisions.";
        baseMessageByDay[7] = "Day 7: A week has passed, stability is key.";
        baseMessageByDay[8] = "Day 8: New opportunities and risks emerge.";
        baseMessageByDay[9] = "Day 9: Keep an eye on resources and morale.";
        baseMessageByDay[10] = "Day 10: The city is growing, manage it well.";
        baseMessageByDay[11] = "Day 11: Strategic planning will pay off.";
        baseMessageByDay[12] = "Day 12: The future of the city is in your hands.";
        baseMessageByDay[13] = "Day 13: A crucial time, make wise choices.";
        baseMessageByDay[14] = "Final Day: Two weeks in, your leadership is tested.";
    }
}