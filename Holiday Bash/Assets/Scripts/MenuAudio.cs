using System.Collections.Generic;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public List<AudioClip> audioList;
    private AudioSource source;

    private AudioClip currentMusic;
    private bool isPlaying;


    void Start()
    {
        source = GetComponent<AudioSource>();
        currentMusic = audioList[Random.Range(0, audioList.Count)];
        isPlaying = source.isPlaying;

    }

    void Update()
    {
        isPlaying = source.isPlaying;
        if (!isPlaying)
        {
            currentMusic = audioList[Random.Range(0, audioList.Count)];
            source.PlayOneShot(currentMusic, 0.7f);            
        }
    }
}
