using UnityEngine;

public class Location
{
    public LocationType type;
    public int baseEnergyAmount;
    public int currentDayEnergyAmount;
    
    public Location(LocationType type, int baseEnergyAmount = 0)
    {
        this.type = type;
        this.baseEnergyAmount = baseEnergyAmount;
        currentDayEnergyAmount = baseEnergyAmount;
    }
    
    public void applyEnergyModifier(int energyModifier)
    {
        currentDayEnergyAmount += energyModifier;
    }
    
    public void ResetDayEnergy()
    {
        currentDayEnergyAmount = baseEnergyAmount;
    }
}

public enum LocationType
{
    HOSPITAL,
    SCHOOL,
    SUPERMARKET,
    CINEMA,
    CLUB,
    FACTORY,
    UNIVERSITY,
    CASINO,
    PARK,
    FARM,
}
