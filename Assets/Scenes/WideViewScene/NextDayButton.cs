using UnityEngine;
using UnityEngine.UI;

public class NextDayButton : MonoBehaviour
{
    public Button button;


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
        CityManager.Instance.FinishDay();
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
