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
    [SerializeField] TextMeshProUGUI option1Text, option2Text, option3Text, option4Text; // Text fields for each option

    private int currentLevel = 1;
    private int currentExperience = 0;
    private int experienceToLevelUp = 100;
    private MC player;
    private Moving playerMovement;
    private WeaponUIManager weaponManager;
    private Button[] options;

    private int maxHPLevel = 1; // Track MaxHP upgrade level
    private int speedLevel = 1; // Track Speed upgrade level

    // Track weapon upgrades
    private int meleeWeaponLevel = 0;
    private int fireballWeaponLevel = 0;
    private int orbitFireballLevel = 0;

    private int newWeaponChosenCount = 0; // Track how many times "New Weapon" was chosen
    private int currentWeaponUpgradeIndex = 0; // Track which weapon upgrade is currently displayed

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

        option1.onClick.AddListener(OnOption1Clicked);
        option2.onClick.AddListener(OnOption2Clicked);
        option3.onClick.AddListener(OnOption3Clicked);
        option4.onClick.AddListener(OnOption4Clicked);
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
        experienceToLevelUp = Mathf.FloorToInt(experienceToLevelUp * 1.2f);

        foreach (Spawner spawner in FindObjectsOfType<Spawner>())
        {
            spawner.IncreaseSpawnRate(currentLevel);
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

        // Create a list of all options
        List<string> availableOptions = new List<string> { "MaxHP", "Speed", "FullHeal", "NewWeapon" };

        // If "New Weapon" has been chosen twice, replace it with weapon upgrades
        if (newWeaponChosenCount >= 2)
        {
            availableOptions.Remove("NewWeapon");
            availableOptions.Add("WeaponUpgrade");
        }

        // Shuffle the options
        for (int i = 0; i < availableOptions.Count; i++)
        {
            int randomIndex = Random.Range(i, availableOptions.Count);
            string temp = availableOptions[i];
            availableOptions[i] = availableOptions[randomIndex];
            availableOptions[randomIndex] = temp;
        }

        // Assign the first 3 options to the buttons
        option1Text.text = GetOptionText(availableOptions[0]);
        option2Text.text = GetOptionText(availableOptions[1]);
        option3Text.text = GetOptionText(availableOptions[2]);

        // Activate the first 3 buttons
        option1.gameObject.SetActive(true);
        option2.gameObject.SetActive(true);
        option3.gameObject.SetActive(true);
    }

    private string GetOptionText(string option)
    {
        switch (option)
        {
            case "MaxHP": return "MaxHP LV " + maxHPLevel;
            case "Speed": return "Speed LV " + speedLevel;
            case "FullHeal": return "Full Heal";
            case "NewWeapon": return "New Weapon";
            case "WeaponUpgrade": return GetWeaponUpgradeText();
            default: return "";
        }
    }

    private string GetWeaponUpgradeText()
    {
        string[] weaponUpgrades = { "Melee Weapon LV " + (meleeWeaponLevel + 1),
                                   "Fireball LV " + (fireballWeaponLevel + 1),
                                   "Orbit Fireball LV " + (orbitFireballLevel + 1) };
        return weaponUpgrades[currentWeaponUpgradeIndex];
    }

    private void OnOption1Clicked()
    {
        HandleOption(option1Text.text);
    }

    private void OnOption2Clicked()
    {
        HandleOption(option2Text.text);
    }

    private void OnOption3Clicked()
    {
        HandleOption(option3Text.text);
    }

    private void OnOption4Clicked()
    {
        HandleOption(option4Text.text);
    }

    private void HandleOption(string optionText)
    {
        if (optionText.StartsWith("MaxHP"))
        {
            IncreaseMaxHP();
        }
        else if (optionText.StartsWith("Speed"))
        {
            IncreaseSpeed();
        }
        else if (optionText.StartsWith("Full Heal"))
        {
            FullHeal();
        }
        else if (optionText.StartsWith("New Weapon"))
        {
            AddWeapon();
        }
        else if (optionText.StartsWith("Melee Weapon") ||
                 optionText.StartsWith("Fireball") ||
                 optionText.StartsWith("Orbit Fireball"))
        {
            UpgradeWeapon();
        }

        ResumeGame();
    }

    private void IncreaseMaxHP()
    {
        player?.IncreaseMaxHP(20);
        maxHPLevel++; // Increase MaxHP level
    }

    private void FullHeal()
    {
        player?.FullHeal();
    }

    private void IncreaseSpeed()
    {
        playerMovement?.IncreaseSpeed(0.2f);
        speedLevel++; // Increase Speed level
    }

    private void AddWeapon()
    {
        if (newWeaponChosenCount < 2)
        {
            weaponManager?.UnlockNewWeapon();
            newWeaponChosenCount++; // Increment the count
        }
    }

    private void UpgradeWeapon()
    {
        switch (currentWeaponUpgradeIndex)
        {
            case 0:
                UpgradeMeleeWeapon();
                break;
            case 1:
                UpgradeFireballWeapon();
                break;
            case 2:
                UpgradeOrbitFireball();
                break;
        }

        // Cycle to the next weapon upgrade
        currentWeaponUpgradeIndex = (currentWeaponUpgradeIndex + 1) % 3;
    }

    private void UpgradeMeleeWeapon()
    {
        meleeWeaponLevel++;
        MeleeWeapon meleeWeapon = FindObjectOfType<MeleeWeapon>();
        if (meleeWeapon != null)
        {
            meleeWeapon.IncreaseDamage(0.2f); // Increase damage
            meleeWeapon.DecreaseTimeToAttack(0.1f); // Decrease time to attack
        }
    }

    private void UpgradeFireballWeapon()
    {
        fireballWeaponLevel++;
        RangeWeapon fireballWeapon = FindObjectOfType<RangeWeapon>();
        if (fireballWeapon != null)
        {
            fireballWeapon.IncreaseFireRate(0.1f); // Increase fire rate
            fireballWeapon.IncreaseFireballLevel(); // Increase fireball level
        }
    }

    private void UpgradeOrbitFireball()
    {
        orbitFireballLevel++;
        FireballOrbit orbitWeapon = FindObjectOfType<FireballOrbit>();
        if (orbitWeapon != null)
        {
            orbitWeapon.IncreaseNumberOfFireballs(1); // Increase number of fireballs
        }
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
            levelText.text = "LEVEL: " + currentLevel;
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