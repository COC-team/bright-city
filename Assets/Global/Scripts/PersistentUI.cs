using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Persistent UI Object created and will not be destroyed.");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("A duplicate UI object was destroyed.");
        }
    }

    void Update()
    {
        Debug.Log("Persistent UI is active.");
    }

}