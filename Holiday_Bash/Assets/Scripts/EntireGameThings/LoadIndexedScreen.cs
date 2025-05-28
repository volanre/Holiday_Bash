using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadIndexedScreen : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
