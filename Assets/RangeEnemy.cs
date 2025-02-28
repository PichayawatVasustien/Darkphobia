using UnityEngine;
using System.Collections;

public class RangeEnemy : MonoBehaviour
{
    [SerializeField] Transform targetDestination;
    MC targetCharacter;
    GameObject targetGameobject;
    [SerializeField] float speed = 2f;
    [SerializeField] string targetTag = "Target";

    private Rigidbody2D rgdbd2d;
    [SerializeField] int experiencePoints = 10;
    [SerializeField] int hp = 999;
    [SerializeField] int dmg = 1;
    private bool isAttacking = false;
    private PlayerLevel playerLevel;
    [SerializeField] public GameObject template;

    [SerializeField] private Spawnfrom spawnOnDeath;

    AudioManager audioManager;

    // Shooting variables
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float projectileSpeed = 5f;
    private float nextShotAt = 0f;
    private bool isShooting = false;
    [SerializeField] public int limit;
    [SerializeField] private bool isDestroyAfterFinish;
    [SerializeField] private bool isSpawnOnce;

    private SpriteRenderer spriteRenderer; // Renderer reference

    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the Renderer component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerLevel = player.GetComponent<PlayerLevel>();

        if (PlayerPrefs.HasKey("difficulty"))
        {
            if (PlayerPrefs.GetString("difficulty") == "easy")
            {
                hp = 2;
            }
            else if (PlayerPrefs.GetString("difficulty") == "medium")
            {
                hp = 4;
            }
            else if (PlayerPrefs.GetString("difficulty") == "hard")
            {
                hp = 8;
            }
            else
            {
                hp = 4;
            }
        }
        else
        {
            hp = 4;
        }
        ScaleEnemyHP(playerLevel.GetCurrentLevel());
    }

    private void Update()
    {
        if (Time.time >= nextShotAt && !isShooting)
        {
            StartCoroutine(ShootRoutine());
            nextShotAt = Time.time + fireRate + 2f; // Add 2 sec charge time to fireRate
        }
    }

    private void FixedUpdate()
    {
        if (targetDestination != null)
        {
            Vector3 direction = (targetDestination.position - transform.position).normalized;
            rgdbd2d.velocity = direction * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(targetTag))
        {
            targetGameobject = collision.gameObject;
            targetCharacter = targetGameobject.GetComponent<MC>();

            if (targetCharacter != null && !isAttacking)
            {
                Attack();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(targetTag) && !isAttacking)
        {
            Attack();
        }
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        float originalSpeed = speed;

        speed = 0f; // Stop movement
        yield return new WaitForSeconds(2f); // Wait for 2 seconds

        Shoot();

        speed = originalSpeed; // Resume movement
        isShooting = false;
    }

    private void Attack()
    {
        if (targetCharacter != null)
        {
            isAttacking = true;
            targetCharacter.takeDMG(dmg);
            audioManager?.PlaySFX(audioManager.hurt);
            Invoke(nameof(ResetAttack), 1f);
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector3 direction = (targetDestination.position - firePoint.position).normalized;

        Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
        if (rbProjectile != null)
        {
            rbProjectile.velocity = direction * projectileSpeed;
        }

        FlipProjectile(projectile, direction);
        Destroy(projectile, 10f);
    }

    private void FlipProjectile(GameObject projectile, Vector3 direction)
    {
        if (direction.x < 0)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            projectile.transform.localScale = scale;
        }
    }

    public void takeDMG(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (playerLevel == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerLevel = player.GetComponent<PlayerLevel>();
            }
        }

        if (playerLevel != null)
        {
            playerLevel.GainExperience(experiencePoints);
        }
        else
        {
            Debug.LogWarning("PlayerLevel not found when enemy died!");
        }

        // ✅ Trigger the assigned Spawnfrom script when enemy dies
        if (spawnOnDeath != null)
        {
            spawnOnDeath.ResetSpawn();
        }
        else
        {
            Debug.LogWarning("Spawnfrom reference is missing in Enemy!");
        }
        if (isSpawnOnce)
        {
            for (int i = 0; i < limit; i++)
            {
                Spawn();
            }
            DestorySelf();
        }
    }

    private System.Collections.IEnumerator GlowRed()
    {
        spriteRenderer.color = Color.red; // Change color to red
        yield return new WaitForSeconds(0.2f); // Stay red for 0.2 seconds
        spriteRenderer.color = Color.white; // Reset color
    }
    public void Spawn()
    {
        Instantiate(template, transform.position, Quaternion.identity);
    }
    private void ScaleEnemyHP(int playerLevel)
    {
        if (playerLevel <= 30)
        {
            hp += (playerLevel / 10) * hp;
        }
        else
        {
            hp += (3 * hp) + (((playerLevel - 30) / 5) * hp);
        }
    }
    private void DestorySelf()
    {
        if (isDestroyAfterFinish)
        {
            Destroy(this.gameObject);
        }
    }
}
