using UnityEngine;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] Transform targetDestination;
    private MC targetCharacter;
    private GameObject targetGameobject;
    [SerializeField] float speed;
    [SerializeField] string targetTag = "Target";
    [SerializeField] private bool isSpawnOnce;
    private Rigidbody2D rgdbd2d;
    private SpriteRenderer spriteRenderer; // For flipping & glow effect
    [SerializeField] int hp = 999;
    [SerializeField] int dmg = 1;
    private bool isAttacking = false;
    [SerializeField] public int limit;
    [SerializeField] private bool isDestroyAfterFinish;

    private AudioManager audioManager; // Reference to AudioManager
    private Animator animator;
    [SerializeField] GameObject winFlashScreen;

    [SerializeField] private string idleAnim = "Idle";
    [SerializeField] private string dieAnim = "Die";
    [SerializeField] private string shootAnim = "Shoot"; // New shooting animation

    // Shooting variables
    [SerializeField] private float shootingRange = 5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 4f; // Time between waves
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float chargeTime = 0.5f; // Faster charge time
    [SerializeField] private int projectilesPerWave = 5; // Number of projectiles per wave
    [SerializeField] private float delayBetweenProjectiles = 0.1f; // Delay between projectiles in a wave
    [SerializeField] private GameObject chargeEffectPrefab; // Visual effect during charge
    private GameObject chargeEffectInstance; // Instance of the charge effect

    private float nextShotAt = 0f;
    private bool isShooting = false;

    private bool isDead = false;

    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Find AudioManager safely
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<AudioManager>();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (PlayerPrefs.HasKey("difficulty"))
        {
            hp = PlayerPrefs.GetString("difficulty") switch
            {
                "easy" => 1000,
                "medium" => 2000,
                "hard" => 5000,
                _ => 2000,
            };
        }
        else
        {
            hp = 2000;
        }

        animator.Play(idleAnim);
    }

    private void Update()
    {
        if (!isDead && targetDestination != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetDestination.position);

            // Check if within shooting range and ready to shoot
            if (distanceToTarget <= shootingRange && Time.time >= nextShotAt && !isShooting)
            {
                StartCoroutine(ShootWaveRoutine());
                nextShotAt = Time.time + fireRate + chargeTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead && targetDestination != null)
        {
            Vector3 direction = (targetDestination.position - transform.position).normalized;
            rgdbd2d.velocity = direction * speed;
            spriteRenderer.flipX = targetDestination.position.x > transform.position.x;
        }
        else
        {
            rgdbd2d.velocity = Vector2.zero; // Stop movement if no target
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(targetTag) && !isDead)
        {
            targetCharacter = collision.gameObject.GetComponent<MC>();
            targetCharacter?.takeDMG(dmg);
            audioManager.PlaySFX(audioManager.hurt);
        }
    }

    public void takeDMG(int dmg)
    {
        if (isDead) return;

        hp -= dmg;
        if (hp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(GlowRed());
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        speed = 0f;
        rgdbd2d.velocity = Vector2.zero;
        rgdbd2d.isKinematic = true;
        isAttacking = false;
        GetComponent<Collider2D>().enabled = false;

        // Play death animation
        animator.Play(dieAnim);
        audioManager.PlaySFX(audioManager.bossDeath);
        StartCoroutine(WaitForDeathAnimation());

        StartCoroutine(FlashColorLoop()); // Corrected coroutine call

        if (winFlashScreen != null) // Prevent null error
        {
            winFlashScreen.SetActive(true);
        }
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        DestroySelf();
    }

    private IEnumerator GlowRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    private void DestroySelf()
    {
        if (isDestroyAfterFinish)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashColorLoop()
    {
        while (true)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // New shooting coroutine for straight-line shockwaves
    private IEnumerator ShootWaveRoutine()
    {
        isShooting = true;

        // Start charge effect
        if (chargeEffectPrefab != null)
        {
            chargeEffectInstance = Instantiate(chargeEffectPrefab, transform.position, Quaternion.identity, transform);
        }

        animator.Play(shootAnim); // Play shooting animation
        yield return new WaitForSeconds(chargeTime);

        if (!isDead)
        {
            // Shoot a straight-line wave of projectiles
            for (int i = 0; i < projectilesPerWave; i++)
            {
                ShootProjectile((targetDestination.position - firePoint.position).normalized);
                yield return new WaitForSeconds(delayBetweenProjectiles); // Delay between projectiles
            }

            animator.Play(idleAnim); // Return to idle animatio
        }

        // Clean up charge effect
        if (chargeEffectInstance != null)
        {
            Destroy(chargeEffectInstance);
        }

        isShooting = false;
    }

    // Shoot a single projectile in a straight line
    private void ShootProjectile(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
        if (rbProjectile != null)
        {
            rbProjectile.velocity = direction * projectileSpeed;
        }

        FlipProjectile(projectile, direction);
        Destroy(projectile, 10f); // Destroy projectile after 10 seconds
    }

    // Flip projectile based on direction
    private void FlipProjectile(GameObject projectile, Vector2 direction)
    {
        if (direction.x < 0)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            projectile.transform.localScale = scale;
        }
    }
}