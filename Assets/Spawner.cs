using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] public GameObject template;
    [SerializeField] public int limit = 5;
    [SerializeField] private bool isSpawnOnce = false;
    [SerializeField] private bool isDestroyAfterFinish = false;
    [SerializeField] private float delay = 10f;

    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;
    [SerializeField] private GameObject topBorder;
    [SerializeField] private GameObject bottomBorder;

    private float nextSpawnAt = 0;
    private int currentSpawned = 0;

    private float minX, maxX, minY, maxY;
    private PlayerLevel playerLevel; // ✅ Reference to track player level

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

        SpawnAll();
    }

    private void Update()
    {
        if (currentSpawned >= limit)
        {
            DestroySelf();
            return;
        }

        if (nextSpawnAt <= 0)
        {
            Spawn();
            nextSpawnAt = delay;
            currentSpawned += 1;
        }

        nextSpawnAt -= Time.deltaTime;
    }

    public void SpawnAll()
    {
        if (isSpawnOnce)
        {
            for (int i = 0; i < limit; i++)
            {
                Spawn();
            }
            DestroySelf();
        }
    }

    public void Spawn()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(template, spawnPosition, Quaternion.identity);
    }

    public void ResetSpawn()
    {
        nextSpawnAt = 0;
        currentSpawned = 0;
        SpawnAll();
    }

    private void DestroySelf()
    {
        if (isDestroyAfterFinish)
        {
            Destroy(this.gameObject);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float spawnX = Random.Range(minX, maxX);
        float spawnY = Random.Range(minY, maxY);

        return new Vector3(spawnX, spawnY, 0);
    }

    public void IncreaseSpawnRate()
    {
        limit += 2;  
        delay = Mathf.Max(1f, delay * 0.9f); 
        Debug.Log("Spawner updated! New limit: " + limit + " | New delay: " + delay);
    }
}

