using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] int experiencePoints;
    private PlayerLevel playerLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerLevel = player.GetComponent<PlayerLevel>();
    }

    bool hitDetected = false;
    // Update is called once per frame
    void Update()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in hit)
        {
            if (c.CompareTag("Player"))
            {
                hitDetected = true;
                if (playerLevel != null)
                {
                    playerLevel.GainExperience(experiencePoints);

                }
            }

            // Destroy the projectile if it hits an enemy
            if (hitDetected)
            {
                Destroy(gameObject);  // Destroy the projectile after it hits an enemy
            }
        }
    }
}
