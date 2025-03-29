using System.Collections;
using UnityEngine;

public class FirstPowerStation : MonoBehaviour
{
    public PopupController popupController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("FirstPowerStation script started.");
        if (!CityManager.Instance.firstPowerStation)
        {
            StartCoroutine(WaitAndPrint());
        }
    }
    
    IEnumerator WaitAndPrint()
    {
        yield return new WaitForSeconds(1f);
        CityManager.Instance.firstPowerStation = true;
        popupController.ShowPopup("Welcome to the Power Station! This is where you will generate energy for your city. Drag the power cards to add energy for current day!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
