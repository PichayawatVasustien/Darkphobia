using UnityEngine;
using System.Collections;

public class spinningfireball : MonoBehaviour
{
    Vector3 direction;
    [SerializeField] float speed;
    [SerializeField] int dmg = 5;

    [SerializeField] private float fadeDuration = 0.5f; // Duration of fade-in/out
    [SerializeField] private float visibilityDuration = 1.5f; // Time between fade cycles

    private SpriteRenderer spriteRenderer; // To control transparency
    private bool isFading = false;
    private bool isVisible = true; // Track visibility state

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on the spinning fireball!");
        }
        else
        {
            StartCoroutine(FadeFireball());
        }
    }

    public void SetDirection(float dir_x, float dir_y)
    {
        direction = new Vector3(dir_x, dir_y, 0).normalized;

        // Flip the projectile based on direction
        Vector3 scale = transform.localScale;
        scale.x = dir_x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void Update()
    {
        if (!isFading)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        // Only deal damage if the fireball is visible
        if (isVisible)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
            for (int i = 0; i < colliders.Length; i++)
            {
                // ✅ Damage regular Enemy
                Enemy e = colliders[i].GetComponent<Enemy>();
                if (e != null)
                {
                    e.takeDMG(dmg);
                }

                // ✅ Damage slowenemy
                slowenemy slow = colliders[i].GetComponent<slowenemy>();
                if (slow != null)
                {
                    slow.takeDMG(dmg);
                }

                RangeEnemy range = colliders[i].GetComponent<RangeEnemy>();
                if (range != null)
                {
                    range.takeDMG(dmg);
                }

                BossEnemy boss = colliders[i].GetComponent<BossEnemy>();
                if (boss != null)
                {
                    boss.takeDMG(dmg);
                }
            }
        }
    }

    private IEnumerator FadeFireball()
    {
        while (true)
        {
            // Fade out
            yield return StartCoroutine(Fade(0f, fadeDuration));
            isVisible = false; // Fireball is now invisible

            // Wait while invisible
            yield return new WaitForSeconds(visibilityDuration);

            // Fade in
            yield return StartCoroutine(Fade(1f, fadeDuration));
            isVisible = true; // Fireball is now visible

            // Wait while visible
            yield return new WaitForSeconds(visibilityDuration);
        }
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        isFading = true;
        float startAlpha = spriteRenderer.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            // Apply the new alpha to the fireball
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;

            yield return null;
        }

        isFading = false;
    }
}