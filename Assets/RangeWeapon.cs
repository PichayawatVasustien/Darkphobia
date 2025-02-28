using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    [SerializeField] float timeToAttack = 1f; // Attack cooldown
    private float timer;

    [SerializeField] GameObject fireballPrefab; // Prefab for fireball

    private Moving playerMove;

    [SerializeField] private float lastHorizontalDirection = 1f; // Default right
    [SerializeField] private float lastVerticalDirection = 0f;   // Default no vertical movement

    private void Awake()
    {
        playerMove = GetComponentInParent<Moving>();
    }


    private void Update()
    {
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        float verticalDirection = Input.GetAxisRaw("Vertical");

        // Update last direction when movement is detected
        if (horizontalDirection != 0 || verticalDirection != 0)
        {
            lastHorizontalDirection = horizontalDirection;
            lastVerticalDirection = verticalDirection;
        }

        else if (horizontalDirection != 0)
        {
            lastHorizontalDirection = horizontalDirection;
        }

        else if (verticalDirection != 0)
        {
            lastVerticalDirection = verticalDirection;
        }

        if (timer < timeToAttack)
        {
            timer += Time.deltaTime;
            return;
        }

        timer = 0;
        SpawnFireball(lastHorizontalDirection, lastVerticalDirection);
    }

    private void SpawnFireball(float lastHorizontalDirection, float lastVerticalDirection)
    {
        if (fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            RangeProjectile projectile = fireball.GetComponent<RangeProjectile>();

            if (projectile != null)
            {
                

                // Fire in last direction if no movement
                projectile.SetDirection(lastHorizontalDirection, lastVerticalDirection);
            }
            else
            {
                Debug.LogError("RangeProjectile script is missing on Fireball prefab!");
            }
        }
    }
}

