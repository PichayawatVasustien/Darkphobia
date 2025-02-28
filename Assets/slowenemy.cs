using UnityEngine;

public class slowenemy : MonoBehaviour
{
    [SerializeField] Transform targetDestination;
    private MC targetCharacter;
    private GameObject targetGameobject;

    [SerializeField] float speed = 0.7f; // Slow movement speed
    [SerializeField] string targetTag = "Target";

    private Rigidbody2D rgdbd2d;
    private SpriteRenderer spriteRenderer; // For glowing effect
    [SerializeField] int experiencePoints = 30;
    [SerializeField] int hp = 3000;
    [SerializeField] int dmg = 3;
    private bool isAttacking = false;
    private PlayerLevel playerLevel;

    [SerializeField] private Spawnfrom spawnOnDeath;

    private AudioManager audioManager;
    private bool facingRight = true; // Track the sprite's facing direction
    [SerializeField] private bool isDestroyAfterFinish;
    [SerializeField] private bool isSpawnOnce;
    [SerializeField] public int limit;
    [SerializeField] public GameObject template;

    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get SpriteRenderer for glowing
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerLevel = player.GetComponent<PlayerLevel>();

        if (PlayerPrefs.HasKey("difficulty"))
        {
            if (PlayerPrefs.GetString("difficulty") == "easy")
            {
                hp = 8;
            }
            else if (PlayerPrefs.GetString("difficulty") == "medium")
            {
                hp = 12;
            }
            else if (PlayerPrefs.GetString("difficulty") == "hard")
            {
                hp = 40;
            }
            else
            {
                hp = 12;
            }
        }
        else
        {
            hp = 12;
        }
        ScaleEnemyHP(playerLevel.GetCurrentLevel());
    }

    private void FixedUpdate()
    {
        if (targetDestination != null)
        {
            Vector3 direction = (targetDestination.position - transform.position).normalized;
            rgdbd2d.velocity = direction * speed; // Slow movement

            // Flip the sprite based on movement direction
            if (direction.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip the x-axis
        transform.localScale = scale;
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

    private void Attack()
    {
        if (targetCharacter != null)
        {
            isAttacking = true;
            targetCharacter.takeDMG(dmg);
            audioManager.PlaySFX(audioManager.hurt);
            Invoke(nameof(ResetAttack), 1.8f); // Slower attack speed
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

        Debug.Log("SlowEnemy took damage: " + dmg + " | Remaining HP: " + hp);

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
