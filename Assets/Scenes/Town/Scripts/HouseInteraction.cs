using UnityEngine;

public class HouseInteraction : MonoBehaviour
{
    public PopupController popupController;
    public Location location;
    public string popupText = "This building will be enabled soon.";

    public void OnMouseDown()
    {
        if (popupController != null)
        {
            if (location.IsLocationEnabled())
            {
                popupController.ShowPopup(location.getLocationString());
            }
        }
    }
}