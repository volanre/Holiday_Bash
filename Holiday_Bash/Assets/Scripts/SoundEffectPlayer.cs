using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip sound;

    [SerializeField]
    private float volume = 0.7f;

    public void playSound()
    {
        if (source != null && sound != null)
        {
            source.PlayOneShot(sound, volume);
        }
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
