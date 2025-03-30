using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.red; // Цвет выделения
    public string sceneToLoad = "NextScene"; // Название сцены для загрузки
    public bool shouldShutdown = false; // Флаг для проверки, нужно ли завершить игру
    private AudioClip click;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Запоминаем оригинальный цвет
        click = Resources.Load<AudioClip>("Audio/button_click");
    }

    void OnMouseEnter()
    {
        spriteRenderer.color = highlightColor; // Меняем цвет при наведении
    }

    void OnMouseExit()
    {
        spriteRenderer.color = originalColor; // Возвращаем цвет
    }

    private void OnMouseUp()
    {
        AudioSource.PlayClipAtPoint(click, Camera.main.transform.position);
        StartCoroutine(LoadSceneWithDelay());
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Задержка в 0.5 секунды

        if (shouldShutdown)
        {
            QuitGame(); // Если установлен флаг shouldShutdown, вызываем метод завершения игры
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad); // Загружаем сцену после задержки
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