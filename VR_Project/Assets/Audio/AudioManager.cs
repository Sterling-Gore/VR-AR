using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource grillSizzleSource;
    [SerializeField] AudioSource fryerSizzleSource;
    [SerializeField] AudioSource fridgeHumSource;

    [Header("Background")]
    public AudioClip background;

    [Header("Fridge")]
    public AudioClip[] fridgeOpenCreakClips;
    public AudioClip fridgeHum;
    public AudioClip[] fridgeCloseClips;

    [Header("Cooking Loops")]
    public AudioClip[] pattySizzleClips;
    public AudioClip[] fryCookClips;

    [Header("Prep Station / SFX")]
    public AudioClip[] bottleSqueezeClips;
    public AudioClip[] stackBurgerClips;

    private int fridgeOpenCreakIndex = 0;
    private int fridgeCloseIndex = 0;
    private int pattySizzleIndex = 0;
    private int fryCookIndex = 0;
    private int bottleSqueezeIndex = 0;
    private int stackBurgerIndex = 0;

    private void Start() 
    {
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private AudioClip GetNextClip(AudioClip[] clips, ref int index)
    {
        if (clips == null || clips.Length == 0)
            return null;

        AudioClip clip = clips[index];

        index++;

        if (index >= clips.Length)
            index = 0;

        return clip;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    public void PlayFridgeOpenCreak()
    {
        PlaySFX(GetNextClip(fridgeOpenCreakClips, ref fridgeOpenCreakIndex));
    }

    public void PlayFridgeClose()
    {
        PlaySFX(GetNextClip(fridgeCloseClips, ref fridgeCloseIndex));
    }

    public void PlayBottleSqueeze()
    {
        PlaySFX(GetNextClip(bottleSqueezeClips, ref bottleSqueezeIndex));
    }

    public void PlayStackConnect()
    {
        PlaySFX(GetNextClip(stackBurgerClips, ref stackBurgerIndex));
    }

    public void PlayFridgeHumLoop()
    {
        if (fridgeHumSource == null || fridgeHum == null) return;

        if (fridgeHumSource.isPlaying)
            return;

        fridgeHumSource.clip = fridgeHum;
        fridgeHumSource.loop = true;
        fridgeHumSource.Play();
    }

    public void StopFridgeHumLoop()
    {
        if (fridgeHumSource == null) return;

        fridgeHumSource.loop = false;
        fridgeHumSource.Stop();
        fridgeHumSource.clip = null;
    }

    public void PlayGrillLoop()
    {
        if (grillSizzleSource == null) return;

        if (grillSizzleSource.isPlaying)
            return;

        AudioClip clip = GetNextClip(pattySizzleClips, ref pattySizzleIndex);
        if (clip == null) return;

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

    public void PlayFryerLoop()
    {
        if (fryerSizzleSource == null) return;

        if (fryerSizzleSource.isPlaying)
            return;

        AudioClip clip = GetNextClip(fryCookClips, ref fryCookIndex);
        if (clip == null) return;

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