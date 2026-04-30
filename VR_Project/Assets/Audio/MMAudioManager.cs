using UnityEngine;

public class MainMenuAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip startShiftClickSound;
    [SerializeField] private AudioClip clockOutClickSound;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.08f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.8f;

    private void Start()
    {
        PlayMenuMusic();
    }

    private void PlayMenuMusic()
    {
        if (musicSource == null || menuMusic == null)
            return;

        musicSource.clip = menuMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayStartShiftClick()
    {
        if (sfxSource == null || startShiftClickSound == null)
            return;

        sfxSource.PlayOneShot(startShiftClickSound, sfxVolume);
    }

    public void PlayClockOutClick()
    {
        if (sfxSource == null || clockOutClickSound == null)
            return;

        sfxSource.PlayOneShot(clockOutClickSound, sfxVolume);
    }
}