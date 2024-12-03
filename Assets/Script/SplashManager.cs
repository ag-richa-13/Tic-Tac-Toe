using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float splashDuration = 3f; // Duration of the splash scene
    void Start()
    {
        // Automatically load the next scene after the specified duration
        Invoke("LoadNextScene", splashDuration);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("RandomRoomScene"); // Replace with your scene's name
    }
}
