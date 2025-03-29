using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private string currentActiveScene;
    private string originalScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNewScene(string sceneName)
    {
        if (sceneName == currentActiveScene)
        {
            Debug.LogWarning($"Scene {sceneName} is already the active scene.");
            return;
        }

        Debug.Log($"Loading new scene: {sceneName}");

        // Start the scene loading process
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string newScene)
    {
        // Unload the current scene if it exists
        if (!string.IsNullOrEmpty(currentActiveScene))
        {
            Debug.Log($"Unloading current scene: {currentActiveScene}");
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentActiveScene);

            // Wait for the unloading to complete
            if (unloadOperation != null)
            {
                yield return new WaitUntil(() => unloadOperation.isDone);
                Debug.Log($"Scene {currentActiveScene} successfully unloaded.");
            }
        }

        // Begin loading the new scene
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);

        // Ensure the newly loaded scene becomes the active scene
        loadOperation.completed += (operation) =>
        {
            Debug.Log($"Scene {newScene} successfully loaded.");
            currentActiveScene = newScene;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene));
        };

        // Wait for the loading to complete
        yield return new WaitUntil(() => loadOperation.isDone);
    }
    
    public void LoadSceneAndCollectLocations(string sceneToLoad)
    {
        originalScene = SceneManager.GetActiveScene().name; // Save the current scene name
        StartCoroutine(LoadSceneAndCollectCoroutine(sceneToLoad));
    }

    private IEnumerator LoadSceneAndCollectCoroutine(string sceneToLoad)
    {
        // Load the new scene additively
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        yield return loadOperation;

        // Ensure the scene is fully loaded
        Scene loadedScene = SceneManager.GetSceneByName(sceneToLoad);
        while (!loadedScene.isLoaded)
        {
            yield return null;
        }

        // Find all Location objects in the loaded scene
        List<Location> locations = new List<Location>(FindObjectsByType<Location>(FindObjectsSortMode.None));
        Debug.Log($"Found {locations.Count} locations in {sceneToLoad}");

        // Now, you can do something with those locations, like storing them in your City object
        CityManager.Instance.city.locations = locations;

        // Unload the loaded scene after collecting data
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneToLoad);
        yield return unloadOperation;

        // Load the original scene back (if needed)
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(originalScene));
        Debug.Log("Returned to original scene: " + originalScene);
    }
}
