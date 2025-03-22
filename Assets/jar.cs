using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class Jar : MonoBehaviour
{
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float respawnDelay = 60f;
    [SerializeField] private float addedTime = 80f;
    private Vector2 hiddenPosition = new Vector2(9999, 9999);
    private bool isTeleporting = false;
    private BombTimer bombTimer;

    void Awake()
    {
        bombTimer = FindObjectOfType<BombTimer>();
    }

    private IEnumerator TeleportWithDelay()
    {
        isTeleporting = true;
        if (arrow) arrow.SetActive(false);
        transform.position = hiddenPosition;

        yield return new WaitForSeconds(respawnDelay);

        float newX = Random.Range(minPosition.x, maxPosition.x);
        float newY = Random.Range(minPosition.y, maxPosition.y);
        transform.position = new Vector2(newX, newY);

        if (arrow) arrow.SetActive(true);
        isTeleporting = false;
    }

    void Update()
    {
        if (isTeleporting) return; // Prevent multiple coroutines

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in hit)
        {
            if (c.CompareTag("Player"))
            {
                StartCoroutine(TeleportWithDelay());
                bombTimer?.ExtendTimer(addedTime); // Extend bomb timer when collected
            }
            break;
        }
    }
}
