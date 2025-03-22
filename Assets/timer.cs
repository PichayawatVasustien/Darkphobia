using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Assign in Inspector
    public GameObject spawner; // Assign the Spawner GameObject in the Inspector
    [SerializeField] private float timeRemaining = 150f; // 10 minutes in seconds
    private bool isTimerRunning = true;
    private bool hasActivatedSpawner = false; // Prevent multiple activations
    public GameObject spawnText;
    [SerializeField] GameObject winFlashScreen;

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining < 0) timeRemaining = 0; // Ensure it doesn't go negative
                UpdateTimerUI();

                // Check if time is exactly 2 minutes left
                if (timeRemaining <= 120f && !hasActivatedSpawner)
                {
                    ActivateSpawner();
                    hasActivatedSpawner = true; // Ensure it only activates once
                }
            }
            else
            {
                isTimerRunning = false;
                timeRemaining = 0; // Ensure it is exactly zero
                UpdateTimerUI(); // Force UI to update with 00:00
                winFlashScreen.SetActive(true);
                Debug.Log("Time is up!");
            }
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Ensure no negative values are displayed
        minutes = Mathf.Max(0, minutes);
        seconds = Mathf.Max(0, seconds);

        timerText.text = $"{minutes:00}:{seconds:00}"; // Format as MM:SS
    }

    void ActivateSpawner()
    {
        if (spawner != null)
        {
            spawner.SetActive(true);
            Debug.Log("Spawner Activated at 2 minutes left!");
            spawnText.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Spawner GameObject is not assigned!");
        }
    }
}
