using UnityEngine;
using UnityEngine.Rendering;

public class SoundEffectPlayer : MonoBehaviour
{
    [SerializeField]
    public AudioSource source;

    [SerializeField]
    private AudioClip sound;

    public void PlayLoadedSound(float volume)
    {
        if (!Player.isAlive) return;
        if (source != null && sound != null)
        {
            source.PlayOneShot(sound, GameManager.SFXVolume * volume);
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
