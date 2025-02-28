using UnityEngine;
using UnityEngine.UI; // Required for UI Slider

public class MC : MonoBehaviour
{
    [SerializeField] int hp = 100; // Player HP
    [SerializeField] int maxHP = 100; // Max HP
    [SerializeField] Slider hpBar; // UI Slider for HP bar
    [SerializeField] GameObject deathScreen; // UI Image for death screen
    AudioManager audioManager;
    pause p;

    private void Awake()
    {
        Time.timeScale = 1f;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        p = GameObject.FindObjectOfType<pause>();
    }

    private void Start()
    {
        // Set up HP Bar
        if (hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = hp;
        }

        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    public void takeDMG(int dmg)
    {
        hp -= dmg;
        hp = Mathf.Clamp(hp, 0, maxHP); // Prevent HP from going negative
        UpdateHPUI();

        if (hp < 1)
        {
            Die();
        }
    }

    private void UpdateHPUI()
    {
        if (hpBar != null)
        {
            hpBar.value = hp;
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");

        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(false); // Hide HP Bar
        }

        if (deathScreen != null)
        {
            deathScreen.SetActive(true);      
        }
        audioManager.PlaySFX(audioManager.death);
        audioManager.StopMusic();
        p.DisablePausing();
        Time.timeScale = 0f; // Freeze the game
    }
    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        hp += amount;

        if (hpBar != null)
        {
            hpBar.maxValue = maxHP; // ✅ Update max value of the slider
            hpBar.value = hp; // ✅ Update the current value
        }
    }


    public void FullHeal()
    {
        hp = maxHP;
        UpdateHPUI();
    }

}
