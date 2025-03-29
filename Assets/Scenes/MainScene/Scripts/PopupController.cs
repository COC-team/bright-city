using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public GameObject popupPanel; // Drag & Drop в инспекторе
    public Button closeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("hui  wqewqeq");
        ClosePopup();
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup(string text)
    {
        popupPanel.SetActive(true);
        SetText(text);
    }

    public void ClosePopup()
    {
        print("hui");
        popupPanel.SetActive(false);
        SetText(string.Empty);
    }

    private void SetText(string text)
    {
        popupPanel.GetComponentInChildren<TMP_Text>().text = text;
    }
}
