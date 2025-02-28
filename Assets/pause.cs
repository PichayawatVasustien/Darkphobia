using Unity.VisualScripting;
using UnityEngine;

public class pause : MonoBehaviour
{
    private bool isPaused = false;
    private bool canPause = true;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject pauseButton;
    AudioManager audioManager;
    PlayerLevel PL;

    private void Start()
    {
        pauseScreen.SetActive(false);
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        PL = FindObjectOfType<PlayerLevel>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Press Esc to pause/unpause
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!canPause)
        {
            return;
        }

        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else if (!isPaused && !PL.getIsLvlUpPause())
        {
            Time.timeScale = 1;
        } 
        if (isPaused)
        {
            audioManager.StopMusic();
        }
        else
        {
            audioManager.ResumeMusic();
        }
        if (pauseScreen != null)
        {
            pauseScreen.SetActive(isPaused);
            pauseButton.SetActive(!isPaused);
        }
        else
        {
            Debug.LogWarning("Pause screen GameObject is not assigned in the Inspector.");
        }
    }

    public void DisablePausing()
    {
        canPause = false;
        pauseScreen.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }
}
