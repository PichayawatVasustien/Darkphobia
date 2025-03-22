using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject defaultEnemySpawner;
    [SerializeField] public GameObject rangeEnemySpawner;
    [SerializeField] public GameObject slowEnemySpawner;
    [SerializeField] public GameObject bossEnemySpawner;

    private PlayerLevel playerLevel; // ? Make sure this is assigned properly

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLevel = player.GetComponent<PlayerLevel>();
        }
        else
        {
            Debug.LogError("Player not found! Ensure the Player has the correct tag.");
        }

        if (defaultEnemySpawner != null)
        {
            defaultEnemySpawner.SetActive(true);
        }
        if (bossEnemySpawner != null)
        {
            bossEnemySpawner.SetActive(false);
        }
    }

    void Update()
    {
        if (playerLevel == null) return; // ? Prevents errors if PlayerLevel is missing

        if (playerLevel.GetCurrentLevel() >= 5 && rangeEnemySpawner != null && !rangeEnemySpawner.activeSelf)
        {
            rangeEnemySpawner.SetActive(true);
        }
        if (playerLevel.GetCurrentLevel() >= 10 && slowEnemySpawner != null && !slowEnemySpawner.activeSelf)
        {
            slowEnemySpawner.SetActive(true);
        }
    }
}

