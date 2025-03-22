using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the shockwave after a certain time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MC player = collision.GetComponent<MC>();
            if (player != null)
            {
                player.takeDMG(damage);
            }
        }
    }
}