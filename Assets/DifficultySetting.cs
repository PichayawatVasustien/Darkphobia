using UnityEngine;

public class DifficultySetting : MonoBehaviour
{
    public void SetEasy()
    {
        PlayerPrefs.SetString("difficulty", "easy");
    }

    public void SetMedium()
    {
        PlayerPrefs.SetString("difficulty", "medium");
    }

    public void SetHard()
    {
        PlayerPrefs.SetString("difficulty", "hard");
    }
}
