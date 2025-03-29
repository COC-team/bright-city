using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public GameObject popupPanel; // Drag & Drop в инспекторе
    public Button closeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        ClosePopup();
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup(string text)
    {
        popupPanel.SetActive(true);
        CityManager.Instance.popupOpened = true;
        SetText(text);
    }

    public void ClosePopup()
    {
        Debug.LogWarning("huila");
        popupPanel.SetActive(false);
        CityManager.Instance.popupOpened = false;
        SetText(string.Empty);
    }

    private void SetText(string text)
    {
        popupPanel.GetComponentInChildren<TMP_Text>().text = text;
    }
}
