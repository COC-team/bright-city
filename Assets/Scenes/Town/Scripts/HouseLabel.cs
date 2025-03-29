using UnityEngine;
using TMPro;

public class HouseLabel : MonoBehaviour
{
    public TextMeshProUGUI label; // Ссылка на TextMeshPro
    private Location location;

    private void Start()
    {
        if (label != null)
        {
            label.gameObject.SetActive(false); // Изначально лейбл скрыт
        }

        location = GetComponent<Location>();

        if (location == null)
        {
            Debug.LogWarning("Location компонент не найден на объекте " + gameObject.name);
        }
    }

    private void OnMouseEnter()
    {
        if (label != null && location != null && location.IsLocationEnabled())
        {
            label.gameObject.SetActive(true); // Показать лейбл при наведении
        }
    }

    private void OnMouseExit()
    {
        if (label != null)
        {
            label.gameObject.SetActive(false); // Скрыть лейбл при уходе
        }
    }
}