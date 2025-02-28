using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            }
        }

        // Destroy the projectile if it hits an enemy
        if (hitDetected)
        {
            Destroy(gameObject);  // Destroy the projectile after it hits an enemy
        }
    }
}
