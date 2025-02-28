using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    Vector3 direction;
    [SerializeField] float speed = 5f;
    [SerializeField] int damage = 5;
    bool hitDetected = false;

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y, 0).normalized;

        // Flip the projectile based on direction
        Vector3 scale = transform.localScale;
        scale.x = dir_x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Continuous hit detection
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in hit)
        {
            if (c.CompareTag("Player") && !hitDetected)
            {
                MC player = c.GetComponent<MC>();
                if (player != null)
                {
                    player.takeDMG(damage);
                }

                hitDetected = true;
                break;
            }

            if (c.CompareTag("Wall"))
            {
                hitDetected = true;
                break;
            }
        }

        // Destroy projectile if it hits the player or a wall
        if (hitDetected)
        {
            Destroy(gameObject);
        }
    }
}
