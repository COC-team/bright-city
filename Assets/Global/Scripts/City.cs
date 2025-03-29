using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class City
{
    public int population;
    public int maxDaysAmount;
    public int currentDay = 0;
    public List<Location> locations = new List<Location>();
    
    public void SwitchNextDay()
    {
        currentDay++;
        ResetLocationsDayEnergy();
    }
    
    public bool IsLastDayPassed()
    {
        return currentDay > maxDaysAmount;
    }
    
    public List<Location> getEnabledLocations()
    {
        List<Location> enabledLocations = new List<Location>();
        foreach (Location location in locations)
        {
            if (location.enabledLocation)
            {
                enabledLocations.Add(location);
            }
        }
        return enabledLocations;
    }
    
    public void ApplyEvents(List<Event> eventsToApply)
    {
        foreach (var eventToApply in eventsToApply)
        {
            Debug.Log($"Event: {eventToApply.description}, Type: {eventToApply.type}");
            if (eventToApply.type == EventType.WHOLE_CITY)
            {
                foreach (Location location in getEnabledLocations())
                {
                    location.ApplyEnergyModifier(eventToApply.GetEnergyModifier(location.type));
                }
            }
            else if (eventToApply.type == EventType.SOME_LOCATIONS)
            {
                foreach (var location in getEnabledLocations())
                {
                    if (eventToApply.locationsEnergyModifier.ContainsKey(location.type))
                    {
                        location.ApplyEnergyModifier(eventToApply.GetEnergyModifier(location.type));
                    }
                }
            }
        }
    }
    
    public void ResetLocationsDayEnergy()
    {
        foreach (Location location in getEnabledLocations())
        {
            location.ResetDayEnergy();
        }
    }
}
