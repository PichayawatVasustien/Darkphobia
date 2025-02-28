using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject soundMenu;
    [SerializeField] GameObject soundButton;
    [SerializeField] GameObject graphicMenu;
    [SerializeField] GameObject graphicButton;
    [SerializeField] GameObject languageMenu;
    [SerializeField] GameObject languageButton;

    private void Start()
    {
        soundMenu.SetActive(true);
        graphicMenu.SetActive(false);
        languageMenu.SetActive(false);
    }

    public void OpenSoundMenu ()
    {
        soundMenu.SetActive(true);
        graphicMenu.SetActive(false);
        languageMenu.SetActive(false);
    }

    public void OpenGraphicMenu ()
    {
        soundMenu.SetActive(false);
        graphicMenu.SetActive(true);
        languageMenu.SetActive(false);
    }

    public void OpenLanguageMenu()
    {
        soundMenu.SetActive(false);
        graphicMenu.SetActive(false);
        languageMenu.SetActive(true);
    }
}
