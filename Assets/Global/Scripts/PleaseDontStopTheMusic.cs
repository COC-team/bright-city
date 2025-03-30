using UnityEngine;

public class PleaseDontStopTheMusic : MonoBehaviour
{
    private static PleaseDontStopTheMusic instance;
    void Awake()
    {
        // Check if there is already an instance of the music
        if (instance == null)
        {
            // If there isn't, set this as the instance
            instance = this;
            DontDestroyOnLoad(this.gameObject); // Prevents destruction on scene change
        }
        else
        {
            // If there is, destroy this GameObject to avoid duplicates
            Destroy(this.gameObject);
        }
    }
}
