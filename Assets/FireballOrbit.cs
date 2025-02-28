using UnityEngine;

public class FireballOrbit : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private int numberOfFireballs = 4;
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float orbitSpeed = 2f;
    [SerializeField] private Vector2 fireballSize = new Vector2(2f, 2f); // Ensure it's declared

    private Transform player;
    private GameObject[] fireballs;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        fireballs = new GameObject[numberOfFireballs];
        for (int i = 0; i < numberOfFireballs; i++)
        {
            float angle = i * (360f / numberOfFireballs);
            Vector3 spawnPosition = GetPosition(angle);
            fireballs[i] = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity, transform);

            // ✅ Apply the size change correctly
            fireballs[i].transform.localScale = new Vector3(fireballSize.x, fireballSize.y, 1);
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
}
