using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform targetDestination;
    private MC targetCharacter;
    private GameObject targetGameobject;
    [SerializeField] float speed;
    [SerializeField] string targetTag = "Target";
    [SerializeField] public GameObject template;
    [SerializeField] private bool isSpawnOnce;
    private Rigidbody2D rgdbd2d;
    private SpriteRenderer spriteRenderer; // Added for flipping & glow effect
    [SerializeField] int experiencePoints = 10;
    [SerializeField] int hp = 999;
    [SerializeField] int dmg = 1;
    private bool isAttacking = false;
    private PlayerLevel playerLevel;
    [SerializeField] public int limit;
    [SerializeField] private bool isDestroyAfterFinish;

    [SerializeField] private Spawnfrom spawnOnDeath; // Reference to Spawnfrom script

    private AudioManager audioManager; // Reference to AudioManager

    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get SpriteRenderer for flipping & glowing

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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

    private void FixedUpdate()
    {
        if (targetDestination != null)
        {
            Vector3 direction = (targetDestination.position - transform.position).normalized;
            rgdbd2d.velocity = direction * speed;

            // Flip enemy based on movement direction
            if (rgdbd2d.velocity.x > 0)
                spriteRenderer.flipX = false; // Facing right
            else if (rgdbd2d.velocity.x < 0)
                spriteRenderer.flipX = true;  // Facing left
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

    private void Attack()
    {
        if (targetCharacter != null)
        {
            isAttacking = true;
            targetCharacter.takeDMG(dmg);

            audioManager.PlaySFX(audioManager.hurt); // Play sound

            Invoke(nameof(ResetAttack), 1f);
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void takeDMG(int dmg)
    {
        hp -= dmg;

        // Start glowing red when taking damage
        StartCoroutine(GlowRed());

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
