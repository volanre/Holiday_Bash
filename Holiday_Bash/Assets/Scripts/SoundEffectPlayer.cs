using UnityEngine;
using UnityEngine.Rendering;

public class SoundEffectPlayer : MonoBehaviour
{
    [SerializeField]
    public AudioSource source;

    [SerializeField]
    private AudioClip sound;

    private bool playingLong = false;

    public void PlayLoadedSound(float volume)
    {
        if (!Player.isAlive) return;
        if (source != null && sound != null)
        {
            source.PlayOneShot(sound, GameManager.SFXVolume * volume);
        }
    }

    /// <summary>
    /// This function is specificly meant for playing the player death noise
    /// </summary>
    /// <param name="noise"></param>
    /// <param name="volume"></param>
    public void PlayLongSound(AudioClip noise, float volume)
    {
        source.resource = noise;
        source.volume = GameManager.SFXVolume * volume;
        if (!playingLong)
        {
            playingLong = true;
            source.Play();
        }


    }

    public void PlaySpecificSound(AudioClip noise, float volume)
    {
        if (!Player.isAlive) return;
        if (source != null)
        {
            source.PlayOneShot(noise, GameManager.SFXVolume * volume);
        }
        else Debug.Log("Error: No AudioSource connected");
    }
}
