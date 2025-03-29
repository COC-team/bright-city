using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
{
    public GameObject dialogPanel;
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    private void Start()
    {
        dialogPanel.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    public void Show(string message)
    {
        Debug.Log("Show popup!!!!!!!!!");
        messageText.text = message;
        CityManager.Instance.popupOpened = true;
        dialogPanel.SetActive(true);
    }

    private void OnConfirmClicked()
    {
        CityManager.Instance.FinishDay();
        dialogPanel.SetActive(false);
    }

    private void OnCancelClicked()
    {
        dialogPanel.SetActive(false);
        CityManager.Instance.popupOpened = false;
    }
}

