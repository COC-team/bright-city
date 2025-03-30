using System.Collections;
using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red; // Color when highlighted
    public string sceneToLoad = "NextScene1"; // Scene to load when clicked
    private AudioClip click;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Save original color
        click = Resources.Load<AudioClip>("Audio/button_click");

    }

    void OnMouseEnter()
    {
        spriteRenderer.color = highlightColor; // Change color on hover
    }

    void OnMouseExit()
    {
        spriteRenderer.color = originalColor; // Revert to original color
    }

    private void OnMouseUp()
    {
        AudioSource.PlayClipAtPoint(click, Camera.main.transform.position);
        StartCoroutine(LoadSceneWithDelay());
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Задержка в 0.5 секунды
        
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