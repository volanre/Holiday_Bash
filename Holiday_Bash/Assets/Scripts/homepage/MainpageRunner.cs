using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Unity.VisualScripting;
using UnityEditor.Media;
public class MainpageRunner : MonoBehaviour
{
    [SerializeField] private VideoPlayer backgroundVideoPlayer;
    // [SerializeField] private VideoClip loopingBackgroundClip;
    // [SerializeField] private VideoClip startupClip;
    [SerializeField] private GameObject titleScreenCanvas;

    private bool backgroundInitiated = false;



    void Start()
    {
        titleScreenCanvas.SetActive(false);
        PlayClip(backgroundVideoPlayer);
    }

    void Update()
    {
        if (backgroundInitiated)
        {
            titleScreenCanvas.SetActive(true);
        }
    }

    public void PlayClip(VideoPlayer vp)
    {
        StartCoroutine(PreloadAndPlay(vp));
    }


    IEnumerator PreloadAndPlay(VideoPlayer vp)
    {
        vp.Prepare();
        while (!vp.isPrepared)
            yield return null;

        yield return new WaitForSeconds(0.2f);
        vp.Play();
        backgroundInitiated = true;
    }
    // void OnPrepareCompleted(VideoPlayer vp)
    // {
    //     Debug.Log("Preparation complete!");
    //     vp.Play();
    //     if (initialStarted)
    //     {
    //         backgroundInitiated = true;
    //     }
    //     initialStarted = true;
    // }
}
