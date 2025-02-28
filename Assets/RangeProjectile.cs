using UnityEngine;

public class RangeProjectile : MonoBehaviour
{
    Vector3 direction;
    [SerializeField] float speed;
    [SerializeField] int dmg = 5;

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y, 0).normalized;

        // Flip the projectile based on direction
        Vector3 scale = transform.localScale;
        scale.x = dir_x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    bool hitDetected = false;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in hit)
        {
            if (c.CompareTag("Enemy") && !hitDetected) // ✅ Works for both Enemy & slowenemy
            {
                // ✅ Damage regular Enemy
                Enemy enemy = c.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.takeDMG(dmg);
                }

                // ✅ Damage slowenemy
                slowenemy slow = c.GetComponent<slowenemy>();
                if (slow != null)
                {
                    slow.takeDMG(dmg);
                }

                RangeEnemy range = c.GetComponent<RangeEnemy>();
                if (range != null)
                {
                    range.takeDMG(dmg);
                }

                hitDetected = true;
                break; // Stop checking after hitting an enemy
            }

            if (c.CompareTag("Wall")) // ✅ Destroy on wall impact
            {
                hitDetected = true;
                break;
            }
        }

        // Destroy projectile if it hits an enemy or wall
        if (hitDetected)
        {
            Destroy(gameObject);
        }
    }
}
