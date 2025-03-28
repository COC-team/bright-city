using UnityEngine;

public class HouseInteraction : MonoBehaviour
{
    public PopupController popupController;
    public string popupText = "Это дом!";

    private void OnMouseDown()
    {
        if (popupController != null)
        {
            popupController.ShowPopup(popupText);
        }
        else
        {
            Debug.Log("hi");
        }
    }
}