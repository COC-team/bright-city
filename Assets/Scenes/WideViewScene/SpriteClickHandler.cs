using UnityEngine;

public class SpriteClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red;
    public string sceneToLoad; // Scene to load when clicked

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Save original color
    }

    void OnMouseEnter()
    {
        if (CityManager.Instance.popupOpened)
        {
            // Если попап открыт, не обновляй эффект
            return;
        }
        spriteRenderer.color = highlightColor; // Change color on hover
    }

    void OnMouseExit()
    {
        spriteRenderer.color = originalColor; // Revert to original color
    }

    void OnMouseDown()
    {
        // Activate the preloaded scene using the global SceneLoader
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadNewScene(sceneToLoad); // Activate the preloaded scene
        }
        else
        {
            Debug.LogError("SceneLoader instance not found. Make sure it's in the scene.");
        }
    }
}