using TMPro;
using UnityEngine;

public class LabelShow : MonoBehaviour
{
    public TextMeshProUGUI label; // Ссылка на TextMeshPro

    private void Start()
    {
        if (label != null)
        {
            label.gameObject.SetActive(false); // Изначально лейбл скрыт
        }
    }

    private void OnMouseEnter()
    {
        if (label != null)
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
