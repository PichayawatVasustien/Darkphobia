using System.Collections;
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
    private SpriteRenderer spriteRenderer; // For flipping & glow effect
    [SerializeField] int experiencePoints = 10;
    [SerializeField] int hp = 999;
    [SerializeField] int dmg = 1;
    private bool isAttacking = false;
    private PlayerLevel playerLevel;
    [SerializeField] public int limit;
    [SerializeField] private bool isDestroyAfterFinish;
    [SerializeField] private Spawnfrom spawnOnDeath;

    private AudioManager audioManager; // Reference to AudioManager
    private Animator animator; // 🎥 Animator component

    // 🎥 Serialized animation names
    [SerializeField] private string idleAnim = "Idle";
    [SerializeField] private string dieAnim = "Die";

    private bool isDead = false;

    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Get Animator component

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerLevel = player.GetComponent<PlayerLevel>();

        if (PlayerPrefs.HasKey("difficulty"))
        {
            hp = PlayerPrefs.GetString("difficulty") switch
            {
                "easy" => 2,
                "medium" => 4,
                "hard" => 8,
                _ => 4,
            };
        }
        else
        {
            hp = 4;
        }

        ScaleEnemyHP(playerLevel.GetCurrentLevel());

        // 🎥 Start with idle animation
        animator.Play(idleAnim);
    }

    private void FixedUpdate()
    {
        if (!isDead && targetDestination != null)
        {
            Vector3 direction = (targetDestination.position - transform.position).normalized;
            rgdbd2d.velocity = direction * speed;

            // Flip enemy based on movement direction
            spriteRenderer.flipX = targetDestination.position.x < transform.position.x;
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

        /*if (playerLevel != null)
        {
            playerLevel.GainExperience(experiencePoints);
        }*/

        if (spawnOnDeath != null)
        {
            spawnOnDeath.ResetSpawn();
        }

        speed = 0f;
        rgdbd2d.velocity = Vector2.zero;
        rgdbd2d.isKinematic = true;
        isAttacking = false;
        GetComponent<Collider2D>().enabled = false;

        // 🎥 Play death animation
        animator.Play(dieAnim);
        audioManager.PlaySFX(audioManager.defaultEnemyDeath);
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        for (int i = 0; i < limit; i++)
        {
            Spawn();
        }

        DestorySelf();
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

    private void DestorySelf()
    {
        if (isDestroyAfterFinish)
        {
            Destroy(gameObject);
        }
    }
}
