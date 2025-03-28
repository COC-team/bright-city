using UnityEngine;

public class SpriteClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red;
    public string sceneToLoad = "NextScene";

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Preload the scene in the background
        SceneLoader.Instance.PreloadScene(sceneToLoad);
    }

    void OnMouseEnter()
    {
        spriteRenderer.color = highlightColor;
    }

    void OnMouseExit()
    {
        spriteRenderer.color = originalColor;
    }

    void OnMouseDown()
    {
        SceneLoader.Instance.ActivateScene(sceneToLoad); // Activate the preloaded scene
    }
}