using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform player; // Player the arrow follows
    public Transform target; // Target item the arrow points to

    private void Update()
    {
        if (target != null)
        {
            // Follow player with offset
            transform.position = player.position + new Vector3(0, 2, 0);

            // Point toward target
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
