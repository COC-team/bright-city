using System.Collections;
using UnityEngine;

public class FirstTown : MonoBehaviour
{
    public PopupController popupController;

    void Start()
    {
        if (!CityManager.Instance.firstTown)
        {
            StartCoroutine(WaitAndPrint());
        }
    }
    
    IEnumerator WaitAndPrint()
    {
        yield return new WaitForSeconds(1f);
        CityManager.Instance.firstTown = true;
        popupController.ShowPopup("Welcome to the town! Here you can see all your buildings. At first, all locations are locked. You will unlock them in upcoming days.");
    }
}
