using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    // Static reference to the instance of our SceneManager
    //public static ScreenManager instance;

    // private void Awake()
    // {
    //     // Check if instance already exists
    //     if (instance == null)
    //     {
    //         // If not, set instance to this
    //         instance = this;
    //     }
    //     else if (instance != this)
    //     {
    //         // If instance already exists and it's not this, then destroy this to enforce the singleton.
    //         Destroy(gameObject);
    //     }

    //     // Set this to not be destroyed when reloading scene
    //     DontDestroyOnLoad(gameObject);
    // }

    // private void Update()
    // {
    //     // Check if the user is on a non-main scene and presses the Escape key
    //     if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         // Load the main scene (assuming the main scene is at build index 0)
    //         LoadScene(0);
    //     }
    // }


    // General method to load scenes based on build index
    public static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public static void ResetCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
