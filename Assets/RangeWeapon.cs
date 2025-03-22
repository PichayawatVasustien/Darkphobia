using UnityEngine;
using System.Collections;

public class RangeWeapon : MonoBehaviour
{
    [SerializeField] private float timeToAttack = 1f; // Attack cooldown
    private float timer;

    [SerializeField] private GameObject fireballPrefab; // Prefab for fireball
    [SerializeField] private Transform firePoint; // Where the fireball spawns
    [SerializeField] private float fireballSpeed = 5f; // Speed of the fireball
    [SerializeField] private float fireballSpread = 15f; // Spread angle for multiple fireballs
    [SerializeField] private float fireballDelay = 0.1f; // Delay between fireballs

    [SerializeField] private bool enableDoubleFireball = false; // Toggle for double fireball

    private Moving playerMove;
    private float lastHorizontalDirection = 1f; // Default right
    private float lastVerticalDirection = 0f;   // Default no vertical movement

    private int fireballLevel = 1; // Track fireball level

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

        if (timer < timeToAttack)
        {
            timer += Time.deltaTime;
            return;
        }

        timer = 0;
        StartCoroutine(SpawnFireballsWithDelay(lastHorizontalDirection, lastVerticalDirection));
    }

    private IEnumerator SpawnFireballsWithDelay(float dirX, float dirY)
    {
        if (fireballPrefab != null)
        {
            Vector2 direction = new Vector2(dirX, dirY).normalized;

            if (fireballLevel >= 5 || enableDoubleFireball) // Check for double fireball condition
            {
                Debug.Log("Shooting 2 fireballs with delay!");
                float angle = fireballSpread * Mathf.Deg2Rad;
                Vector2 direction1 = RotateDirection(direction, angle);
                Vector2 direction2 = RotateDirection(direction, -angle);

                CreateFireball(direction1); // First fireball
                yield return new WaitForSeconds(fireballDelay); // Delay
                CreateFireball(direction2); // Second fireball
            }
            else
            {
                Debug.Log("Shooting 1 fireball!");
                CreateFireball(direction); // Single fireball
            }
        }
    }

    private void CreateFireball(Vector2 direction)
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        RangeProjectile projectile = fireball.GetComponent<RangeProjectile>();

        if (projectile != null)
        {
            projectile.SetDirection(direction.x, direction.y);
        }
        else
        {
            Debug.LogError("RangeProjectile script is missing on Fireball prefab!");
        }
    }

    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        return new Vector2(
            direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle),
            direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle)
        );
    }

    public void IncreaseFireRate(float amount)
    {
        timeToAttack -= amount;
        if (timeToAttack < 0.1f) timeToAttack = 0.1f; // Prevent negative values
    }

    public void IncreaseFireballLevel()
    {
        fireballLevel++;
        Debug.Log($"Fireball Level: {fireballLevel}");
    }
}