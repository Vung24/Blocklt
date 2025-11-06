using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoppyManager : MonoBehaviour
{
    public static LoppyManager instance;
    public Button[] buttonsLv;
    void Awake()
    {
        LevelButton();
    }
    void LevelButton()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < buttonsLv.Length; i++)
        {
            buttonsLv[i].interactable = false;
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            buttonsLv[i].interactable = true;
        }
    }
    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        SceneManager.LoadScene(levelName);
    }
}
