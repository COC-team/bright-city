using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private Dictionary<string, AsyncOperation> sceneLoaders = new Dictionary<string, AsyncOperation>();
    private string currentActiveScene = "MainScene"; // Set your main scene name here

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

    public void PreloadScene(string sceneName)
    {
        if (!sceneLoaders.ContainsKey(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;
            asyncLoad.completed += (operation) =>
            {
                DeleteEventSystemsInScene(sceneName); // Delete EventSystem in the newly loaded scene
                DisableAudioListenersInScene(sceneName);
                SetSceneActive(sceneName, false); // Keep the scene inactive until it's activated explicitly
            };
            sceneLoaders.Add(sceneName, asyncLoad);
        }
    }

    public void ActivateScene(string sceneName)
    {
        if (sceneLoaders.ContainsKey(sceneName))
        {
            // Deactivate current active scene before switching
            SetSceneActive(currentActiveScene, false);

            // Activate new scene
            sceneLoaders[sceneName].allowSceneActivation = true;
            SetSceneActive(sceneName, true);

            // Update the current active scene
            currentActiveScene = sceneName;
        }
    }

    private void SetSceneActive(string sceneName, bool isActive)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid())
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                obj.SetActive(isActive);
            }
        }
    }

    private void DeleteEventSystemsInScene(string sceneName)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);

        if (loadedScene.IsValid())
        {
            GameObject[] rootObjects = loadedScene.GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {
                // Find all EventSystems in the loaded scene
                EventSystem[] eventSystems = obj.GetComponentsInChildren<EventSystem>(true); // Including inactive ones

                foreach (EventSystem eventSystem in eventSystems)
                {
                    // Destroy the GameObject that holds the EventSystem (delete the entire GameObject)
                    if (eventSystem != null)
                    {
                        Destroy(eventSystem.gameObject); // Delete the GameObject
                    }
                }
            }
        }
    }

    private void DisableAudioListenersInScene(string sceneName)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);

        if (loadedScene.IsValid())
        {
            GameObject[] rootObjects = loadedScene.GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {
                AudioListener[] audioListeners = obj.GetComponentsInChildren<AudioListener>(true); // Search all AudioListeners
                
                foreach (AudioListener audioListener in audioListeners)
                {
                    audioListener.enabled = false; // Disable all found AudioListeners
                }
            }
        }
    }
}
