using UnityEngine;

public class spinningfireball : MonoBehaviour
{
    Vector3 direction;
    [SerializeField] float speed;
    [SerializeField] int dmg = 5;

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y, 0).normalized;

        // Flip the projectile based on direction
        Vector3 scale = transform.localScale;
        scale.x = dir_x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    bool hitDetected = false;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        for (int i = 0; i < colliders.Length; i++)
        {

            // ✅ Damage regular Enemy
            Enemy e = colliders[i].GetComponent<Enemy>();
            if (e != null)
            {
                colliders[i].GetComponent<Enemy>().takeDMG(dmg);
            }

            // ✅ Damage slowenemy
            slowenemy slow = colliders[i].GetComponent<slowenemy>();
            if (slow != null)
            {
                colliders[i].GetComponent<slowenemy>().takeDMG(dmg);
            }

            RangeEnemy range = colliders[i].GetComponent<RangeEnemy>();
            if (range != null)
            {
                colliders[i].GetComponent<RangeEnemy>().takeDMG(dmg);
            }
        }
    }
}

