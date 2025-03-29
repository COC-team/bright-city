using System;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Rendering;

public class Location : MonoBehaviour
{
    public LocationType type;
    public int baseEnergyAmount;
    public int currentDayEnergyAmount;
    [FormerlySerializedAs("enabled")] public bool enabledLocation = false;
    
    public Location(LocationType type, int baseEnergyAmount = 0)
    {
        this.type = type;
        this.baseEnergyAmount = baseEnergyAmount;
        currentDayEnergyAmount = baseEnergyAmount;
    }
    
    public void ApplyEnergyModifier(int energyModifier)
    {
        currentDayEnergyAmount += energyModifier;
        currentDayEnergyAmount = Math.Min(currentDayEnergyAmount, 0);
    }
    
    public void ResetDayEnergy()
    {
        currentDayEnergyAmount = baseEnergyAmount;
    }
    
    public void EnableLocation()
    {
        enabledLocation = true;
    }

    public bool IsLocationEnabled()
    {
        return enabledLocation;
    }

    public String getLocationString()
    {
        return "Building name: " + type + 
               "\nBase Energy Amount: " + baseEnergyAmount;
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
    CommonHouse,
}
