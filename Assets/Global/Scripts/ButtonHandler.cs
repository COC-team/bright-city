using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red; // Color when highlighted
    public string sceneToLoad = "NextScene"; // Scene to load

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Save original color

        // Preload the scene using SceneLoader
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.PreloadScene(sceneToLoad);
        }
    }

    void OnMouseEnter()
    {
        spriteRenderer.color = highlightColor; // Change color on hover
    }

    void OnMouseExit()
    {
        spriteRenderer.color = originalColor; // Revert to original color
    }

    void OnMouseDown()
    {
        // Use the global SceneLoader to activate the scene
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.ActivateScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("SceneLoader instance not found. Make sure it's in the scene.");
        }
    }
}