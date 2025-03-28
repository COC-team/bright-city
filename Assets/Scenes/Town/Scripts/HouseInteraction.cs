using UnityEngine;

public class HouseInteraction : MonoBehaviour
{
    public PopupController popupController;
    public string popupText = "nothing";

    public void OnMouseDown()
    {
        if (popupController != null)
        {
            popupController.ShowPopup(popupText);
        }
    }
}