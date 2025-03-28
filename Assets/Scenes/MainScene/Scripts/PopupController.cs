using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public GameObject popupPanel; // Drag & Drop в инспекторе
    public Button closeButton;

    public string text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SetText(text);
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
    }

    private void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    private void SetText(string text)
    {
        popupPanel.GetComponentInChildren<TMP_Text>().text = text;
    }
}
