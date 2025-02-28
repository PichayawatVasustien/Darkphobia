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
    //??????????????????? public AudioClip ????;

    private void Start()
    {
        musicSource.clip = background; 
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
}
