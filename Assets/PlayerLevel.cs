using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider xpBar;
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] Button option1, option2, option3, option4;

    private int currentLevel = 1;
    private int currentExperience = 0;
    private int experienceToLevelUp = 10;
    private MC player;
    private Moving playerMovement;
    private WeaponUIManager weaponManager;
    private Button[] options;

    public bool isLvlUpPause = false;

    void Start()
    {
        player = FindObjectOfType<MC>();
        playerMovement = FindObjectOfType<Moving>();
        weaponManager = FindObjectOfType<WeaponUIManager>();
        options = new Button[] { option1, option2, option3, option4 };
        isLvlUpPause = false;

        UpdateUI();
        levelUpPanel.SetActive(false);

        option1.onClick.AddListener(IncreaseMaxHP);
        option2.onClick.AddListener(FullHeal);
        option3.onClick.AddListener(IncreaseSpeed);
        option4.onClick.AddListener(AddWeapon);
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        if (currentExperience >= experienceToLevelUp)
        {
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToLevelUp;
        experienceToLevelUp = Mathf.FloorToInt(experienceToLevelUp * 1.5f);

        foreach (Spawner spawner in FindObjectsOfType<Spawner>())
        {
            spawner.IncreaseSpawnRate();
        }

        ShowLevelUpChoices();
        UpdateUI();
    }

    private void ShowLevelUpChoices()
    {
        Time.timeScale = 0;
        isLvlUpPause = true;
        levelUpPanel.SetActive(true);

        // Hide all options initially
        foreach (Button option in options)
        {
            option.gameObject.SetActive(false);
        }

        // Randomly select 3 out of the 4 options
        List<Button> shuffledOptions = new List<Button>(options);
        for (int i = 0; i < shuffledOptions.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledOptions.Count);
            Button temp = shuffledOptions[i];
            shuffledOptions[i] = shuffledOptions[randomIndex];
            shuffledOptions[randomIndex] = temp;
        }

        for (int i = 0; i < 3; i++)
        {
            shuffledOptions[i].gameObject.SetActive(true);
        }
    }

    private void IncreaseMaxHP()
    {
        player?.IncreaseMaxHP(20);
        ResumeGame();
    }

    private void FullHeal()
    {
        player?.FullHeal();
        ResumeGame();
    }

    private void IncreaseSpeed()
    {
        playerMovement?.IncreaseSpeed(1f);
        ResumeGame();
    }

    private void AddWeapon()
    {
        weaponManager?.UnlockNewWeapon();
        ResumeGame();
    }

    private void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        isLvlUpPause = false;
        Time.timeScale = 1;
    }

    private void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }

        if (xpBar != null)
        {
            xpBar.maxValue = experienceToLevelUp;
            xpBar.value = currentExperience;
        }
    }

    public bool getIsLvlUpPause()
    {
        return isLvlUpPause;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}