using System.Collections.Generic;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public List<AudioClip> audioList;
    private AudioSource source;

    private AudioClip currentMusic;
    public AudioClip clickSFX;
    private bool isPlaying;
    private bool isPaused = false;


    void Start()
    {
        source = GetComponent<AudioSource>();
        currentMusic = audioList[Random.Range(0, audioList.Count)];
        isPlaying = source.isPlaying;
    }

    void Update()
    {
        isPlaying = source.isPlaying;
        float curVol = 0.5f;
        if (!Player.isAlive) curVol = 0.3f;
        if (!isPlaying)
        {
            if (isPaused) return;
            currentMusic = audioList[Random.Range(0, audioList.Count)];
            source.clip = currentMusic;
            source.volume = GameManager.MusicVolume;
            source.Play();
            source.PlayOneShot(currentMusic, curVol * GameManager.MusicVolume);
        }
    }
    public void PauseBGMusic()
    {
        isPaused = true;
        source.Stop();
    }
    public void UnpauseBGMusic()
    {
        isPaused = false;
    }
    public void playClick()
    {
        source.PlayOneShot(clickSFX, 4.5f * GameManager.SFXVolume);
    }
}
