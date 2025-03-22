using UnityEngine;

public class MeleeHit : MonoBehaviour
{
    [SerializeField] int dmg = 1;
    [SerializeField] float meleeDuration = 0.8f; // How long melee stays
    float timer;

    void Update()
    {

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in hit)
        {
            if (c.CompareTag("Enemy"))
            {
                Enemy enemy = c.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.takeDMG(dmg);
                }

                slowenemy slow = c.GetComponent<slowenemy>();
                if (slow != null)
                {
                    slow.takeDMG(dmg);
                }

                RangeEnemy range = c.GetComponent<RangeEnemy>();
                if (range != null)
                {
                    range.takeDMG(dmg);
                }

                BossEnemy boss = c.GetComponent<BossEnemy>();
                if (boss != null)
                {
                    boss.takeDMG(dmg);
                }

            }
        }
    }

    private void OnEnable()
    {
        timer = meleeDuration;
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Destroy(gameObject);
        }
    }
}
