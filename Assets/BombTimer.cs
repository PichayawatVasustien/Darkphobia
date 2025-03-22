using UnityEngine;
using TMPro;
using System.Collections;

public class BombTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bombStatusText;
    [SerializeField] private float initialTime = 60f;
    [SerializeField] private float damagePercentPerSecond = 5f;
    private float timeLeft;
    private bool bombActive = false;
    private bool isPlayerSafe = false;
    private Coroutine damageCoroutine;
    [SerializeField] private MC Player;

    void Start()
    {
        timeLeft = initialTime;
        StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        while (true)
        {
            while (timeLeft > 0)
            {
                bombStatusText.text = "Time Left: " + timeLeft.ToString("0") + " s";
                yield return new WaitForSeconds(1f);
                timeLeft--;
            }
            while (timeLeft <= 0)
            {
                int damage = Mathf.CeilToInt((damagePercentPerSecond / 100f) * Player.getMaxHp());
                Player.takeDMG(damage);
                bombStatusText.text = "Bomb Active! Taking Damage!";
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void ExtendTimer(float extraTime)
    {
        timeLeft += extraTime;
    }
}

