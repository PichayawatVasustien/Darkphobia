using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] float timeToAttack = 4f;
    [SerializeField] GameObject meleePrefab; // Prefab instead of fixed object
    [SerializeField] Vector2 attackSize = new Vector2(2f, 1f);
    [SerializeField] float dmg = 1f; // Changed to float
    [SerializeField] float meleeDuration = 0.8f; // How long melee stays

    private float timer;
    private Moving playerMove;

    private void Awake()
    {
        playerMove = GetComponentInParent<Moving>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            Attack();
        }
    }

    private void Attack()
    {
        timer = timeToAttack;

        // Get player's facing direction
        float direction = Mathf.Sign(playerMove.transform.localScale.x);
        float offsetX = 2f; // Adjust if needed

        // Create melee object in front of player
        GameObject melee = Instantiate(meleePrefab, transform.position, Quaternion.identity);

        // Set position to be in front of the player
        melee.transform.position = transform.position + new Vector3(offsetX * direction, 0, 0);

        // Flip sprite properly (without affecting its size)
        melee.transform.localScale = new Vector3(Mathf.Abs(melee.transform.localScale.x) * direction,
                                                 melee.transform.localScale.y,
                                                 melee.transform.localScale.z);

        // Detect enemies
        Collider2D[] colliders = Physics2D.OverlapBoxAll(melee.transform.position, attackSize, 0f);
        ApplyDamage(colliders);

        // Destroy melee after duration
        Destroy(melee, meleeDuration);
    }

    public void IncreaseDamage(float amount)
    {
        dmg += amount; // Increase damage by the specified amount
        Debug.Log($"Melee Weapon Damage: {dmg}");
    }

    public void DecreaseTimeToAttack(float amount)
    {
        timeToAttack -= amount;
        if (timeToAttack < 0.1f) timeToAttack = 0.1f; // Prevent negative values
    }

    private void ApplyDamage(Collider2D[] colliders)
    {
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy e))
            {
                Debug.Log($"Hit Enemy: {e.name}");
                e.takeDMG(Mathf.RoundToInt(dmg)); // Convert float to int for damage
            }
            else if (collider.TryGetComponent(out slowenemy slow))
            {
                Debug.Log($"Hit SlowEnemy: {slow.name}");
                slow.takeDMG(Mathf.RoundToInt(dmg)); // Convert float to int for damage
            }
            else if (collider.TryGetComponent(out RangeEnemy range))
            {
                Debug.Log($"Hit RangeEnemy: {range.name}");
                range.takeDMG(Mathf.RoundToInt(dmg)); // Convert float to int for damage
            }
            else if (collider.TryGetComponent(out BossEnemy boss))
            {
                Debug.Log($"Hit BossEnemy: {boss.name}");
                boss.takeDMG(Mathf.RoundToInt(dmg)); // Convert float to int for damage
            }
        }
    }
}