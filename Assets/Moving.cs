using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]

public class Moving : MonoBehaviour
{
    Rigidbody2D rgbd2d;
    Animator animator;

    [HideInInspector]
    public Vector3 movementVector;
    float lastVerticalVector;

    [SerializeField] float speed = 3f;

    public float lastHorizontalVector { get; private set; } = 1f; // Default facing right

    void Start()
    {
        rgbd2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movementVector = new Vector3();
    }

    void Update()
    {
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.y = Input.GetAxisRaw("Vertical");

        // Update animation parameter
        animator.SetFloat("Speed", movementVector.magnitude);

        // Update the last horizontal movement direction if the player moves
        if (movementVector.x != 0)
        {
            lastHorizontalVector = Mathf.Sign(movementVector.x); // -1 for left, 1 for right
        }

        rgbd2d.velocity = movementVector * speed;

        // Flip character sprite if moving left
        if (movementVector.x != 0)
        {
            transform.localScale = new Vector3(lastHorizontalVector, 1, 1);
        }
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
    }
}
