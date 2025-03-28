using System;
using UnityEngine;

public class StationManager  : MonoBehaviour
{
    public static StationManager Instance { get; private set; }  // Singleton Instance

    public int energyAmount = 0;
    
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
    }

    public void AddEnergy(int amount)
    {
        energyAmount += amount;
        Debug.Log($"Current station energy {energyAmount}!");
    }
    
    public void RemoveEnergy(int amount)
    {
        energyAmount -= amount;
        if (energyAmount < 0)
        {
            energyAmount = 0;
        }
        Debug.Log($"Current station energy {energyAmount}!");
    }
    
    public void ResetEnergy()
    {
        energyAmount = 0;
    }
}
