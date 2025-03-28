using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red; // Цвет выделения
    public string sceneToLoad = "NextScene"; // Название сцены для загрузки

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Запоминаем оригинальный цвет
    }

    void OnMouseEnter()
    {
        spriteRenderer.color = highlightColor; // Меняем цвет при наведении
    }

    void OnMouseExit()
    {
        spriteRenderer.color = originalColor; // Возвращаем цвет
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene(sceneToLoad); // Загружаем сцену при клике
    }
}