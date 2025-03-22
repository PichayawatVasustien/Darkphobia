using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("----------Audio Source----------")]
    [SerializeField] AudioSource musicSource; 
    [SerializeField] AudioSource SFXSource;
    [Header("----------Audio Clip----------")]
    public AudioClip background;
    public AudioClip hurt;
    public AudioClip death;
    public AudioClip defaultEnemyDeath;
    public AudioClip rangeEnemyDeath;
    public AudioClip slowEnemyDeath;
    public AudioClip bossDeath;
    public AudioClip Win;
    public AudioClip start;
    //??????????????????? public AudioClip ????;

    private void Start()
    {
        musicSource.clip = background; 
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void PlayOtherMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
        musicSource.UnPause();
    }
}
