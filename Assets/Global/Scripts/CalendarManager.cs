using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
    public Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        Debug.Log("Button clicked!");
        if (CityManager.Instance.popupOpened)
        {
            // Если попап открыт, не обновляй эффект
            return;
        }

        CityManager.Instance.DisplayLastNewsMessage();
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
