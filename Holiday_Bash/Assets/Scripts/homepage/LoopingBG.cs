using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public class LoopingBG : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float x, y;


    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + new Vector2(x, y) * Time.deltaTime, rawImage.uvRect.size);
    }


    // void Start()
    // {
    //     PlayerBackground();
    // }

    // public void PlayerBackground()
    // {
    //     StartCoroutine(PreloadAndPlay(videoPlayer));
    // }


    // IEnumerator PreloadAndPlay(VideoPlayer vp)
    // {
    //     vp.Prepare();
    //     while (!vp.isPrepared)
    //         yield return null;

    //     vp.Play();
    // }
}
