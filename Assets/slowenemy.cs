using System.Collections;
using UnityEngine;

public class slowenemy : MonoBehaviour
{
    [SerializeField] Transform targetDestination;
    private MC targetCharacter;

    [SerializeField] private float speed = 0.7f;
    [SerializeField] private string targetTag = "Target";

    private Rigidbody2D rgdbd2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private int experiencePoints = 30;
    [SerializeField] private int hp = 3000;
    [SerializeField] private int dmg = 3;
    private bool isDying = false;

    [SerializeField] private Spawnfrom spawnOnDeath;
    private AudioManager audioManager;

    [SerializeField] private GameObject template;
    [SerializeField] private bool isDestroyAfterFinish;
    [SerializeField] private int limit;

    [SerializeField] private string movementAnimation = "IdleMove";
    [SerializeField] private string deathAnimation = "Die";
    [SerializeField] private float deathDuration = 1.5f; // Manually set death animation time

    private void Awake()
    {
        rgdbd2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (PlayerPrefs.HasKey("difficulty"))
        {
            switch (PlayerPrefs.GetString("difficulty"))
            {
                case "easy": hp = 6; break;
                case "medium": hp = 12; break;
                case "hard": hp = 40; break;
                default: hp = 12; break;
            }
        }
        else hp = 12;

        if (player != null)
        {
            PlayerLevel playerLevel = player.GetComponent<PlayerLevel>();
            ScaleEnemyHP(playerLevel.GetCurrentLevel());
        }

    }

    private void Start()
    {
        if (animator) animator.Play(movementAnimation); // Constant movement animation
    }

    private void FixedUpdate()
    {
        if (isDying || targetDestination == null) return;

        Vector3 direction = (targetDestination.position - transform.position).normalized;
        rgdbd2d.velocity = direction * speed;

        spriteRenderer.flipX = targetDestination.position.x > transform.position.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(targetTag) && !isDying)
        {
            targetCharacter = collision.gameObject.GetComponent<MC>();
            targetCharacter?.takeDMG(dmg);
            audioManager.PlaySFX(audioManager.hurt);
        }
    }

    public void takeDMG(int damage)
    {
        if (isDying) return; // Ignore damage if already dying

        hp -= damage;
        if (hp > 0)
        {
            StartCoroutine(GlowRed()); // Flash red for damage effect
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDying) return;
        isDying = true;

        // Stop all movement & physics
        rgdbd2d.velocity = Vector2.zero;
        rgdbd2d.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        audioManager.PlaySFX(audioManager.slowEnemyDeath);
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (animator)
        {
            animator.Play(deathAnimation); // Play death animation
        }

        yield return new WaitForSeconds(deathDuration); // Manually set duration

        for (int i = 0; i < limit; i++)
        {
            GameObject reward = Instantiate(template, transform.position, Quaternion.identity);
            reward.transform.localScale = template.transform.localScale;
        }

        if (isDestroyAfterFinish) Destroy(gameObject);
    }

    private IEnumerator GlowRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
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
}
