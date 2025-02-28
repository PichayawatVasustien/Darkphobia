using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponUIManager : MonoBehaviour
{
    [SerializeField] private List<Image> weaponDisplays; // Use a list for flexibility
    [SerializeField] private List<Sprite> weaponSprites;
    [SerializeField] private List<GameObject> allWeapons;

    private List<int> unlockedWeapons = new List<int>();

    private void Start()
    {
        if (allWeapons.Count < 1 || weaponSprites.Count < 1 || weaponDisplays.Count < 1)
        {
            Debug.LogError("Ensure all weapon lists and UI elements are properly assigned!");
            return;
        }

        unlockedWeapons.Clear();
        unlockedWeapons.Add(0); // Start with the first weapon
        UpdateWeaponUI();
    }

    public void UnlockNewWeapon()
    {
        if (unlockedWeapons.Count < allWeapons.Count)
        {
            int newWeaponIndex = unlockedWeapons.Count;
            unlockedWeapons.Add(newWeaponIndex);
            Debug.Log("New weapon unlocked: " + allWeapons[newWeaponIndex].name);
        }
        else
        {
            Debug.Log("All weapons already unlocked!");
        }
        UpdateWeaponUI();
    }

    private void UpdateWeaponUI()
    {
        // Hide all weapon UI slots initially
        foreach (var display in weaponDisplays)
        {
            display.gameObject.SetActive(false);
        }

        // Assign sprites and enable the correct UI elements
        for (int i = 0; i < unlockedWeapons.Count && i < weaponDisplays.Count; i++)
        {
            int weaponIndex = unlockedWeapons[i];
            if (weaponIndex < weaponSprites.Count)
            {
                weaponDisplays[i].sprite = weaponSprites[weaponIndex];
                weaponDisplays[i].gameObject.SetActive(true);
            }
        }

        // Activate only the unlocked weapons
        for (int i = 0; i < allWeapons.Count; i++)
        {
            allWeapons[i].SetActive(unlockedWeapons.Contains(i));
        }
    }
}
