using UnityEngine;

public class TutorialPage : MonoBehaviour
{
    [SerializeField] GameObject tutorialScreen;
    pause p;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f;
        p = GameObject.FindObjectOfType<pause>();
        p.DisablePausing();
        tutorialScreen.SetActive(true);
    }

    public void ResumeTimer()
    {
        Time.timeScale = 1f;
    }
}
