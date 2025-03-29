using UnityEngine;

public class CardHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    public float scaleAmount = 1.1f;     // How much the card scales up
    public float animationSpeed = 10f;   // Speed of scaling and moving animation

    private bool isHovered = false;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * scaleAmount;
    }

    private void OnMouseEnter()
    {
        isHovered = true;
    }

    private void OnMouseExit()
    {
        isHovered = false;
    }

    private void Update()
    {
        if (isHovered)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * animationSpeed);
        }
    }
}