using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public GameObject popupPanel; // Drag & Drop в инспекторе
    public Button closeButton;
    private AudioClip click;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        click = Resources.Load<AudioClip>("Audio/button_click");
        ClosePopup();
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void ShowPopup(string text)
    {
        AudioSource.PlayClipAtPoint(click, Camera.main.transform.position);
        popupPanel.SetActive(true);
        CityManager.Instance.popupOpened = true;
        SetText(text);
    }

    public void ClosePopup()
    {
        AudioSource.PlayClipAtPoint(click, Camera.main.transform.position);
        popupPanel.SetActive(false);
        CityManager.Instance.popupOpened = false;
        SetText(string.Empty);
    }

    private void SetText(string text)
    {
        popupPanel.GetComponentInChildren<TMP_Text>().text = text;
    }
}
