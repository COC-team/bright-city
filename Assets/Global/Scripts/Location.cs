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
    public string description;
    [FormerlySerializedAs("enabled")] public bool enabledLocation = false;
    
    public Location(LocationType type, int baseEnergyAmount = 0)
    {
        this.type = type;
        this.baseEnergyAmount = baseEnergyAmount;
        currentDayEnergyAmount = baseEnergyAmount;
    }
    
    public void ApplyEnergyModifier(int energyModifier)
    {
        Debug.Log($"Applying energy modifier: {energyModifier} to location: {type}");
        currentDayEnergyAmount += energyModifier;
        currentDayEnergyAmount = Math.Max(currentDayEnergyAmount, 0);
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
        var enabledLocations = CityManager.Instance.city.getEnabledLocations();
        foreach (var location in enabledLocations)
        {
            if (location.type == type)
            {
                return true;
            }
        }
        
        return false;
    }

    public String getLocationString()
    {
        return "Building name: " + type +
               "\nBase Energy Amount: " + baseEnergyAmount
               + "\nDescription: " + description;
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
