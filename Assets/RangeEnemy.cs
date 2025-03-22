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
    [SerializeField] private float shootingRange = 5f;

    AudioManager audioManager;
    private Animator animator;

    // 🎥 Serialized animation names for customization
    [SerializeField] private string idleAnim = "Idle";
    [SerializeField] private string shootAnim = "Shoot";
    [SerializeField] private string dieAnim = "Die";

    // Shooting variables
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 4f;
    [SerializeField] float projectileSpeed = 5f;
    private float nextShotAt = 0f;
    private bool isShooting = false;
    [SerializeField] public int limit;
    [SerializeField] private bool isDestroyAfterFinish;
    [SerializeField] private bool isSpawnOnce;
    private bool isDead = false;
    [SerializeField] private float chargeTime = 2f;

    [SerializeField] private float deathDuration = 2f;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Get Animator component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerLevel = player?.GetComponent<PlayerLevel>();

        if (PlayerPrefs.HasKey("difficulty"))
        {
            switch (PlayerPrefs.GetString("difficulty"))
            {
                case "easy": hp = 2; break;
                case "medium": hp = 4; break;
                case "hard": hp = 8; break;
                default: hp = 4; break;
            }
        }
        else hp = 4;

        ScaleEnemyHP(playerLevel?.GetCurrentLevel() ?? 1);

        // 🎥 Start with idle animation
        animator.Play(idleAnim);
    }

    private void Update()
    {
        if (!isDead && targetDestination != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, targetDestination.position);

            if (distanceToTarget <= shootingRange && Time.time >= nextShotAt && !isShooting)
            {
                StartCoroutine(ShootRoutine());
                nextShotAt = Time.time + fireRate + chargeTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (targetDestination != null)
        {
            Vector3 direction = (targetDestination.position - transform.position).normalized;
            rgdbd2d.velocity = direction * speed;

            spriteRenderer.flipX = targetDestination.position.x > transform.position.x;
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

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        float originalSpeed = speed;
        speed = 0f; // Stop movement
        animator.Play(shootAnim);
        yield return new WaitForSeconds(chargeTime);
         // Adjust based on animation duration

        if (!isDead)
        {
            Shoot();
            animator.Play(idleAnim); // 🎥 Return to idle animation
            speed = originalSpeed;
        }
        isShooting = false;
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
        else
        {
            StartCoroutine(GlowRed());
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        /*if (playerLevel != null)
        {
            playerLevel.GainExperience(experiencePoints);
        }*/

        if (spawnOnDeath != null)
        {
            spawnOnDeath.ResetSpawn();
        }

        // Stop movement & attacks
        speed = 0f;
        rgdbd2d.velocity = Vector2.zero;
        rgdbd2d.isKinematic = true;
        isShooting = true;
        isAttacking = false;
        GetComponent<Collider2D>().enabled = false;

        // 🎥 Play death animation
        animator.Play(dieAnim);
        audioManager.PlaySFX(audioManager.rangeEnemyDeath);
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + deathDuration);

        for (int i = 0; i < limit; i++)
        {
            Spawn();
        }

        DestroySelf();
    }

    private IEnumerator GlowRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    public void Spawn()
    {
        GameObject reward = Instantiate(template, transform.position, Quaternion.identity);
        reward.transform.localScale = template.transform.localScale;
    }

    private void ScaleEnemyHP(int playerLevel)
    {
        int pLvl = playerLevel - 1;
        if (playerLevel <= 30)
        {
            hp += (pLvl / 10) * hp;
        }
        else
        {
            hp += (3 * hp) + (((pLvl - 30) / 5) * hp);
        }
    }

    private void DestroySelf()
    {
        if (isDestroyAfterFinish)
        {
            Destroy(gameObject);
        }
    }
}
