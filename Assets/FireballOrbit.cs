using UnityEngine;
using System.Collections;

public class FireballOrbit : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private int numberOfFireballs = 4;
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float orbitSpeed = 2f;
    [SerializeField] private Vector2 fireballSize = new Vector2(2f, 2f);
    [SerializeField] private float fadeDuration = 0.5f; // Duration of fade-in/out
    [SerializeField] private float visibilityDuration = 1.5f; // Time between fade cycles

    private Transform player;
    private GameObject[] fireballs;
    private SpriteRenderer[] fireballRenderers; // To control transparency

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        InitializeFireballs();
        StartCoroutine(FadeFireballs());
    }

    private void InitializeFireballs()
    {
        // Destroy existing fireballs if any
        if (fireballs != null)
        {
            foreach (GameObject fireball in fireballs)
            {
                if (fireball != null)
                {
                    Destroy(fireball);
                }
            }
        }

        // Create new fireballs
        fireballs = new GameObject[numberOfFireballs];
        fireballRenderers = new SpriteRenderer[numberOfFireballs];
        for (int i = 0; i < numberOfFireballs; i++)
        {
            float angle = i * (360f / numberOfFireballs);
            Vector3 spawnPosition = GetPosition(angle);
            fireballs[i] = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity, transform);

            // Apply the size change correctly
            fireballs[i].transform.localScale = new Vector3(fireballSize.x, fireballSize.y, 1);

            // Get the SpriteRenderer component
            fireballRenderers[i] = fireballs[i].GetComponent<SpriteRenderer>();
            if (fireballRenderers[i] == null)
            {
                Debug.LogError("Fireball prefab is missing a SpriteRenderer component!");
            }
            else
            {
                Debug.Log($"Fireball {i} SpriteRenderer assigned successfully.");
            }
        }
    }

    private void Update()
    {
        if (player == null) return;

        transform.position = player.position;

        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] == null) continue;

            float angle = (Time.time * orbitSpeed * 100f + i * (360f / numberOfFireballs)) % 360f;
            fireballs[i].transform.position = GetPosition(angle);
        }
    }

    private Vector3 GetPosition(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector3(
            player.position.x + Mathf.Cos(radians) * orbitRadius,
            player.position.y + Mathf.Sin(radians) * orbitRadius,
            0f
        );
    }

    public void IncreaseNumberOfFireballs(int amount)
    {
        numberOfFireballs += amount;
        InitializeFireballs(); // Reinitialize fireballs with the new count
    }

    private IEnumerator FadeFireballs()
    {
        while (true)
        {
            // Fade out
            Debug.Log("Fading out...");
            yield return StartCoroutine(Fade(0f, fadeDuration));

            // Wait while invisible
            Debug.Log("Waiting while invisible...");
            yield return new WaitForSeconds(visibilityDuration);

            // Fade in
            Debug.Log("Fading in...");
            yield return StartCoroutine(Fade(1f, fadeDuration));

            // Wait while visible
            Debug.Log("Waiting while visible...");
            yield return new WaitForSeconds(visibilityDuration);
        }
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fireballRenderers[0].color.a; // Assume all fireballs have the same alpha
        float time = 0f;

        Debug.Log($"Fading to alpha: {targetAlpha} over {duration} seconds.");

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            // Apply the new alpha to all fireballs
            foreach (SpriteRenderer renderer in fireballRenderers)
            {
                if (renderer != null)
                {
                    Color color = renderer.color;
                    color.a = alpha;
                    renderer.color = color;
                }
            }

            yield return null;
        }

        Debug.Log($"Fade complete. Current alpha: {fireballRenderers[0].color.a}");
    }
}