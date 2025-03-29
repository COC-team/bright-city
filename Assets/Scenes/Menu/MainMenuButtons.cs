using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red; // Цвет выделения
    public string sceneToLoad = "NextScene"; // Название сцены для загрузки
    public bool shouldShutdown = false; // Флаг для проверки, нужно ли завершить игру

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
        if (shouldShutdown)
        {
            QuitGame(); // Если установлен флаг shouldShutdown, вызываем метод завершения игры
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad); // Загружаем сцену при клике
        }
    }

    // Метод для завершения игры
    private void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Останавливает игру в редакторе
#else
        Application.Quit(); // Завершаем игру, если в сборке
#endif
    }
}