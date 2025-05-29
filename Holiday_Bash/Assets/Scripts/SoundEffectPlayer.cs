using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    [SerializeField]
    public AudioSource source;

    [SerializeField]
    private AudioClip sound;

    [SerializeField]
    private float volume = 0.7f;

    public void PlayLoadedSound()
    {
        if (source != null && sound != null)
        {
            source.PlayOneShot(sound, volume);
        }
    }
    public void PlaySpecificSound(AudioClip noise)
    {
        if (source != null)
        {
            source.PlayOneShot(noise, volume);
        }
        else Debug.Log("Error: No AudioSource connected");
    }

    /// <summary>
    ///Plays at a specifc time
    /// </summary>
    /// <param name="timeWanted">Time in seconds</param>
    public void playSoundAtTime(float timeWanted)
    {
        if (timeWanted > sound.length)
        {
            return;
        }
        else
        {
            source.time = timeWanted;
            source.PlayOneShot(sound, volume);
        }
    }
    
}
