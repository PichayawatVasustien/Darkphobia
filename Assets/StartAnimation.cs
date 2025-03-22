using UnityEngine;
using DG.Tweening;

public class StartAnimation : MonoBehaviour
{
    //[SerializeField] GameObject ;
    [SerializeField] RectTransform mainMenuPanelRect, creditButtonRect;
    [SerializeField] float mainMenuPosY, creditButtonPosX;
    [SerializeField] float tweenDuration;
    AudioManager audioManager; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        Time.timeScale = 1f;
        mainMenuPanelRect.DOLocalMoveY(mainMenuPosY, tweenDuration);
        creditButtonRect.DOLocalMoveX(creditButtonPosX, tweenDuration);
        audioManager.PlayOtherMusic(audioManager.start);
    }
}
