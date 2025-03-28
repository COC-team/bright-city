using UnityEngine;
using System.Collections.Generic;

public class City
{
    public int population;
    public int maxDaysAmount;
    public int currentDay = 0;
    public List<Location> locations;
    
    public void SwitchNextDay()
    {
        currentDay++;
        ResetLocationsDayEnergy();
    }
    
    public bool IsLastDayPassed()
    {
        return currentDay > maxDaysAmount;
    }
    
    public void ApplyEvent(Event eventToApply)
    {
        if (eventToApply.type == EventType.WHOLE_CITY)
        {
            foreach (Location location in locations)
            {
                location.applyEnergyModifier(eventToApply.GetEnergyModifier(location.type));
            }
        }
        else if (eventToApply.type == EventType.SOME_LOCATIONS)
        {
            foreach (var location in locations)
            {
                if (eventToApply.locationsEnergyModifier.ContainsKey(location.type))
                {
                    location.applyEnergyModifier(eventToApply.GetEnergyModifier(location.type));
                }
            }
        }
    }
    
    public void ResetLocationsDayEnergy()
    {
        foreach (Location location in locations)
        {
            location.ResetDayEnergy();
        }
    }
}
