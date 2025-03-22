using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] public GameObject template;
    [SerializeField] private bool isBossSpawner = false;
    [SerializeField] private float delay = 10f;

    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;
    [SerializeField] private GameObject topBorder;
    [SerializeField] private GameObject bottomBorder;

    private float nextSpawnAt = 0;
    private int currentSpawned = 0;

    private float minX, maxX, minY, maxY;
    private PlayerLevel playerLevel; // ✅ Reference to track player level
    private float originalDelay;

    private void Start()
    {
        if (leftBorder == null || rightBorder == null || topBorder == null || bottomBorder == null)
        {
            Debug.LogError("Assign all four border GameObjects in the Inspector!");
            return;
        }

        // ✅ Find the player level system
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerLevel = player.GetComponent<PlayerLevel>();
        }

        // ✅ Get boundary positions from the GameObjects
        minX = leftBorder.transform.position.x;
        maxX = rightBorder.transform.position.x;
        minY = bottomBorder.transform.position.y;
        maxY = topBorder.transform.position.y;

        originalDelay = delay;

        SpawnAll();
    }

    private void Update()
    {
       if (nextSpawnAt <= 0)
       {
           Spawn();
           nextSpawnAt = delay;
       }

        nextSpawnAt -= Time.deltaTime;
    }

    public void SpawnAll()
    {
        if (isBossSpawner)
        {
            for (int i = 0; i < 1; i++)
            {
                Spawn();
            }
            DestroySelf();
        }
        else
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (template != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(template, spawnPosition, Quaternion.identity);
        }
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float spawnX = Random.Range(minX, maxX);
        float spawnY = Random.Range(minY, maxY);

        return new Vector3(spawnX, spawnY, 0);
    }

    public void IncreaseSpawnRate(int currentLevel)
    {
        float increaseFactor;
        if (currentLevel <= 30)
        {
            int cycleLevel = (currentLevel - 1) % 10;
            increaseFactor = (cycleLevel / 9f) * 0.5f;
        }
        else
        {
            int cycleLevel = (currentLevel - 31) % 5;
            increaseFactor = (cycleLevel / 4f) * 0.5f;
        }
        delay = originalDelay * (1 - increaseFactor);
    }

}

