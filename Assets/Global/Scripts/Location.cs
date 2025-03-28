using UnityEngine;

public class Location : MonoBehaviour
{
    public LocationType type;
    public int baseEnergyAmount;
    public int currentDayEnergyAmount;
    public bool enabled = false;
    
    public Location(LocationType type, int baseEnergyAmount = 0)
    {
        this.type = type;
        this.baseEnergyAmount = baseEnergyAmount;
        currentDayEnergyAmount = baseEnergyAmount;
    }
    
    public void ApplyEnergyModifier(int energyModifier)
    {
        currentDayEnergyAmount += energyModifier;
    }
    
    public void ResetDayEnergy()
    {
        currentDayEnergyAmount = baseEnergyAmount;
    }
    
    public void EnableLocation()
    {
        enabled = true;
    }
}

public enum LocationType
{
    Hospital,
    School,
    Supermarket,
    Cinema,
    Club,
    Factory,
    University,
    Casino,
    Park,
    Farm,
}
