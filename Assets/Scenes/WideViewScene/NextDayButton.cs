using UnityEngine;
using UnityEngine.UI;

public class NextDayButton : MonoBehaviour
{
    public Button button;
    public ConfirmationDialog confirmationDialog;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    // This method is called when the button is clicked
    private void OnButtonClicked()
    {
        if (CityManager.Instance.popupOpened)
        {
            // Если попап открыт, не обновляй эффект
            return;
        }

        var message = "Do you really want to finish the day?\nYou selected energy for this day is "
                      + StationManager.Instance.GetEnergy() + "!";
        confirmationDialog.Show(message);
    }

    // Clean up when the object is destroyed
    private void OnDestroy()
    {
        // Remove listener when the button is destroyed
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
