using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red; // Color when highlighted
    public string sceneToLoad = "NextScene1"; // Scene to load when clicked

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Save original color
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
            SceneLoader.Instance.LoadNewScene(sceneToLoad); // Activate the preloaded scene
        }
        else
        {
            Debug.LogError("SceneLoader instance not found. Make sure it's in the scene.");
        }
    }
}