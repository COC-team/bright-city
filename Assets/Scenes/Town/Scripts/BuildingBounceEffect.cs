using UnityEngine;

public class BuildingBounceEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovered = false;
    public float bounceAmount = 0.1f;  // Насколько увеличивается
    public float bounceSpeed = 5f;     // Скорость анимации

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * (1 + bounceAmount);
    }

    void OnMouseEnter()
    {
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    void Update()
    {
        if (isHovered)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * bounceSpeed);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * bounceSpeed);
    }
}