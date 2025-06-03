using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartupRunner : MonoBehaviour
{
    public float waitTime = 5.3f;
    public VideoClip introClip;
    public VideoPlayer vp;
    void Start()
    {
        StartCoroutine(PreloadAndPlay(vp));
    }
    IEnumerator WaitForIntro()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(1);
    }
    IEnumerator PreloadAndPlay(VideoPlayer vp)
    {
        vp.Prepare();
        while (!vp.isPrepared)
            yield return null;

        yield return new WaitForSeconds(0.5f);
        vp.Play();
        StartCoroutine(WaitForIntro());
    }
}
