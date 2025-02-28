using UnityEngine;

public class LoopAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private string animationName = "Idle"; // Default animation name

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component missing on " + gameObject.name);
            return;
        }

        PlayLoopingAnimation();
    }

    private void PlayLoopingAnimation()
    {
        animator.Play(animationName);
    }
}
