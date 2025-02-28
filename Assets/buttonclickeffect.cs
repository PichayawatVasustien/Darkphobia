using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonClickEffect : MonoBehaviour, IPointerClickHandler
{
    private Animator animator;
    public string clickAnimationName = "ButtonClick"; // Make sure this matches the animation name
    public float animationDelay = 0.5f; // Adjust this based on your animation length

    private Button button;

    void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();

        if (animator == null)
        {
            Debug.LogError("No Animator found on " + gameObject.name);
        }

        if (button == null)
        {
            Debug.LogError("No Button component found on " + gameObject.name);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (animator != null)
        {
            StartCoroutine(PlayAnimationAndExecute());
        }
        else
        {
            ExecuteButtonFunction(); // If no animator, execute instantly
        }
    }

    private IEnumerator PlayAnimationAndExecute()
    {
        if (animator != null)
        {
            animator.SetTrigger("Click"); // Play the animation

            yield return new WaitForSeconds(animationDelay); // Wait for animation to finish
        }

        ExecuteButtonFunction(); // Now execute the button function
    }

    private void ExecuteButtonFunction()
    {
        if (button != null)
        {
            button.onClick.Invoke(); // Triggers the button's assigned function
        }
    }
}
