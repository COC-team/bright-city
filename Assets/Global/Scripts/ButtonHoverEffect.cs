using System;
using UnityEngine;

public class ButtonHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private AudioClip click;

    public float scaleMultiplier = 1.1f;  // Scale increase on hover
    public float darkenAmount = 0.2f;     // How much the button darkens (0.2 = 20% darker)
    public float animationSpeed = 10f;    // Animation speed
    public bool isPopupButton = false;

    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        click = Resources.Load<AudioClip>("Audio/button_click");

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void OnMouseEnter()
    {
        isHovered = true;
    }

    void OnMouseExit()
    {
        isHovered = false;
    }

    private void OnMouseUp()
    {
        AudioSource.PlayClipAtPoint(click, Camera.main.transform.position);
    }

    void Update()
    {
        
        if (!isPopupButton && CityManager.Instance && CityManager.Instance.popupOpened)
        {
            // Если попап открыт, не обновляй эффект
            return;
        }
        
        if (isHovered)
        {
            // Smoothly scale up
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleMultiplier, Time.deltaTime * animationSpeed);

            // Smoothly darken the sprite
            if (spriteRenderer != null)
            {
                Color targetColor = originalColor * (1f - darkenAmount);
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * animationSpeed);
            }
        }
        else
        {
            // Smoothly reset scale
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * animationSpeed);

            // Smoothly reset color
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, Time.deltaTime * animationSpeed);
            }
        }
    }
}