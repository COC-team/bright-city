using UnityEngine;

public class BuildingBounceEffect : MonoBehaviour
{
    private Location location; // Убедись, что Location инициализирован!
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovered = false;
    public float bounceAmount = 0.1f;  // Насколько увеличивается
    public float bounceSpeed = 5f;     // Скорость анимации

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * (1 + bounceAmount);

        // Найди компонент Location на этом объекте или где-то еще
        location = GetComponent<Location>();

        if (location == null)
        {
            Debug.LogWarning("Location компонент не найден на объекте " + gameObject.name);
        }
    }

    void OnMouseEnter()
    {
        if (location == null || location.IsLocationEnabled()) // Проверка перед включением эффекта
        {
            isHovered = true;
        }
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    void Update()
    {
        if (location == null || location.IsLocationEnabled()) // Проверка перед обновлением эффекта
        {
            if (isHovered)
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * bounceSpeed);
            else
                transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * bounceSpeed);
        }
        else
        {
            // Верни масштаб в исходное состояние, если эффект должен быть отключен
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * bounceSpeed);
        }
    }
}