using UnityEngine;
using System.Collections.Generic;

public class Event
{
    public EventType type;
    public string description;
    public Dictionary<LocationType, int> locationsEnergyModifier;
    
    public Event(EventType type, string description)
    {
        this.type = type;
        this.description = description;
        locationsEnergyModifier = new Dictionary<LocationType, int>();
    }
    
    public void AddEnergyModifier(LocationType locationType, int value)
    {
        if (!locationsEnergyModifier.ContainsKey(locationType))
        {
            locationsEnergyModifier.Add(locationType, value);
        }
        else
        {
            locationsEnergyModifier[locationType] = value;
        }
    }

    public int GetEnergyModifier(LocationType locationType)
    {
        return locationsEnergyModifier.ContainsKey(locationType) ? locationsEnergyModifier[locationType] : 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is Event) 
            return description.Equals(((Event) obj).description);
        return false;
    }

    public override int GetHashCode()
    {
        return description.GetHashCode();
    }
}

public enum EventType
{
    NO_IMPACT,
    SOME_LOCATIONS,
    WHOLE_CITY,
}

