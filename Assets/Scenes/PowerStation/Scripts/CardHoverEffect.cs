using UnityEngine;

public class CardHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    public float scaleAmount = 1.1f;     // How much the card scales up
    public float animationSpeed = 10f;   // Speed of scaling and moving animation

    private bool isHovered = false;
    private AudioClip hoverSound;

    private void Start()
    {
        hoverSound = Resources.Load<AudioClip>("Audio/card_choosing");
        originalScale = transform.localScale;
        targetScale = originalScale * scaleAmount;
    }

    private void OnMouseEnter()
    {
        AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position, 2f);
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