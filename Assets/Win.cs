using DG.Tweening;
using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] GameObject WinMenu;
    AudioManager audioManager;
    pause p;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        p = GameObject.FindObjectOfType<pause>();
    }

    private void Start()
    {
        StartFadeEffect();
    }

    private void StartFadeEffect()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0; // Start fully transparent

        Sequence fadeSequence = DOTween.Sequence();

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject obj in objects)
        {
            if (obj.CompareTag("Enemy"))
            {
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                }

                slowenemy slow = obj.GetComponent<slowenemy>();
                if (slow != null)
                {
                    slow.Die();
                }

                RangeEnemy range = obj.GetComponent<RangeEnemy>();
                if (range != null)
                {
                    range.Die();
                }

                BossEnemy boss = obj.GetComponent<BossEnemy>();
                if (boss != null)
                {
                    boss.Die();
                }

            }
        }

        fadeSequence.Append(canvasGroup.DOFade(1, 5).SetUpdate(true).OnComplete(() =>
        {
            WinMenu.SetActive(true);
            audioManager.StopMusic();
            audioManager.PlayOtherMusic(audioManager.Win);
            p.DisablePausing();
            GameObject[] spawner = GameObject.FindGameObjectsWithTag("Spawner");
            foreach (GameObject s in spawner)
            {
                Destroy(s);
            }
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
            GameObject[] collectable = GameObject.FindGameObjectsWithTag("Collectable");
            foreach (GameObject c in collectable)
            {
                Destroy(c);
            }
            Time.timeScale = 0f; // Pause game AFTER fade-in completes
            
        }))
        .AppendInterval(1).SetUpdate(true) // Ensure interval works in unscaled time
        .Append(canvasGroup.DOFade(0, 5).SetUpdate(true)) // Ensure fade-out works in unscaled time
        .OnComplete(() =>
        {
            gameObject.SetActive(false); // Deactivate the GameObject after fading out
        });
    }
}

