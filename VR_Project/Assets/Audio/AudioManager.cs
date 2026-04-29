using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource grillSizzleSource;
    [SerializeField] AudioSource fryerSizzleSource;
    


    [Header("Audio Clip")]
    public AudioClip background;
    public AudioClip fridgeOpen;
    public AudioClip fridgeClose;
    public AudioClip pattySizzle;
    public AudioClip fryCook;
    public AudioClip bottleSqueeze;
    public AudioClip nurgerStack;

    private void Start() 
    {
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    public void PlayGrillLoop(AudioClip clip)
    {
        if (grillSizzleSource == null || clip == null) return;
        if (grillSizzleSource.isPlaying && grillSizzleSource.clip == clip) return;
        grillSizzleSource.clip = clip;
        grillSizzleSource.loop = true;
        grillSizzleSource.Play();
    }

    public void StopGrillLoop()
    {
        if (grillSizzleSource == null) return;
        grillSizzleSource.loop = false;
        grillSizzleSource.Stop();
        grillSizzleSource.clip = null;
    }

    public void PlayFryerLoop(AudioClip clip)
    {
        if (fryerSizzleSource == null || clip == null) return;
        if (fryerSizzleSource.isPlaying && fryerSizzleSource.clip == clip) return;
        fryerSizzleSource.clip = clip;
        fryerSizzleSource.loop = true;
        fryerSizzleSource.Play();
    }

    public void StopFryerLoop()
    {
        if (fryerSizzleSource == null) return;
        fryerSizzleSource.loop = false;
        fryerSizzleSource.Stop();
        fryerSizzleSource.clip = null;
    }
}
