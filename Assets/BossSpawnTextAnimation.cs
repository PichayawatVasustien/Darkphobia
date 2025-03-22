using DG.Tweening;
using UnityEngine;

public class BossSpawnTextAnimation : MonoBehaviour  
{

    [SerializeField] public CanvasGroup canvasGroup;
    private float duration = 10f; // Total time the object should appear

    private void Start()
    {
        StartFlickerEffect();
    }

    private void StartFlickerEffect()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0; // Start invisible

        Sequence flickerSequence = DOTween.Sequence();

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            flickerSequence.Append(canvasGroup.DOFade(1, 1))  // Fade in for 1 second
                           .Append(canvasGroup.DOFade(0, 1)); // Fade out for 1 second
            elapsedTime += 2f; // Each cycle (fade in + fade out) takes 2 seconds
        }

        flickerSequence.OnComplete(() => Destroy(gameObject)); // Destroy after 10 seconds
    }
}

