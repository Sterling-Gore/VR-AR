using UnityEngine;

[System.Serializable]
public class AudioClipWithVolume
{
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource grillSizzleSource;
    [SerializeField] AudioSource fryerSizzleSource;
    [SerializeField] AudioSource fridgeHumSource;

    [Header("Global Volume")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.061f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float grillVolume = 0.148f;
    [Range(0f, 1f)] [SerializeField] private float fryerVolume = 0.25f;
    [Range(0f, 1f)] [SerializeField] private float fridgeHumVolume = 0.094f;

    [Header("Background")]
    public AudioClip background;

    [Header("Fridge")]
    public AudioClipWithVolume[] fridgeOpenCreakClips;
    public AudioClip fridgeHum;
    public AudioClipWithVolume[] fridgeCloseClips;

    [Header("Cooking Loops")]
    public AudioClipWithVolume[] pattySizzleClips;
    public AudioClipWithVolume[] fryCookClips;

    [Header("Prep Station / SFX")]
    public AudioClipWithVolume[] bottleSqueezeClips;
    public AudioClipWithVolume[] stackBurgerClips;

    [Header("Trash")]
    public AudioClipWithVolume[] trashClips;

    private int fridgeOpenCreakIndex = 0;
    private int fridgeCloseIndex = 0;
    private int pattySizzleIndex = 0;
    private int fryCookIndex = 0;
    private int bottleSqueezeIndex = 0;
    private int stackBurgerIndex = 0;
    private int trashIndex = 0;
    private void Start() 
    {
        ApplyVolumes();

        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;

        // Keep SFX source at 1 because each PlayOneShot gets its own volume scale.
        if (SFXSource != null)
            SFXSource.volume = 1f;

        if (fridgeHumSource != null)
            fridgeHumSource.volume = fridgeHumVolume;
    }

    private AudioClipWithVolume GetNextSound(AudioClipWithVolume[] sounds, ref int index)
    {
        if (sounds == null || sounds.Length == 0)
            return null;

        int attempts = 0;

        while (attempts < sounds.Length)
        {
            if (index >= sounds.Length)
                index = 0;

            AudioClipWithVolume sound = sounds[index];
            index++;
            attempts++;

            if (sound != null && sound.clip != null)
                return sound;
        }

        return null;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (SFXSource != null && clip != null)
        {
            SFXSource.PlayOneShot(clip, sfxVolume);
        }
    }

    private void PlaySFX(AudioClipWithVolume sound)
    {
        if (SFXSource != null && sound != null && sound.clip != null)
        {
            SFXSource.PlayOneShot(sound.clip, sfxVolume * sound.volume);
        }
    }

    public void PlayFridgeOpenCreak()
    {
        PlaySFX(GetNextSound(fridgeOpenCreakClips, ref fridgeOpenCreakIndex));
    }

    public void PlayFridgeClose()
    {
        PlaySFX(GetNextSound(fridgeCloseClips, ref fridgeCloseIndex));
    }

    public void PlayBottleSqueeze()
    {
        PlaySFX(GetNextSound(bottleSqueezeClips, ref bottleSqueezeIndex));
    }

    public void PlayStackConnect()
    {
        PlaySFX(GetNextSound(stackBurgerClips, ref stackBurgerIndex));
    }

    public void PlayFridgeHumLoop()
    {
        if (fridgeHumSource == null || fridgeHum == null) return;

        if (fridgeHumSource.isPlaying)
            return;

        fridgeHumSource.volume = fridgeHumVolume;
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

        AudioClipWithVolume sound = GetNextSound(pattySizzleClips, ref pattySizzleIndex);
        if (sound == null) return;

        grillSizzleSource.clip = sound.clip;
        grillSizzleSource.volume = grillVolume * sound.volume;
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

        AudioClipWithVolume sound = GetNextSound(fryCookClips, ref fryCookIndex);
        if (sound == null) return;

        fryerSizzleSource.clip = sound.clip;
        fryerSizzleSource.volume = fryerVolume * sound.volume;
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

    public void PlayTrashSound()
    {
        PlaySFX(GetNextSound(trashClips, ref trashIndex));
    }
}