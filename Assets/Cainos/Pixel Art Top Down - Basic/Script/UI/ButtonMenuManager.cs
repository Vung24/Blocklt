using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenuManager : MonoBehaviour
{
    public GameObject playerPanel, levelPanel, settingPanel;
    // Start is called before the first frame update
    void Start()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(() =>
            {
                if (AudioManager.instance)
                {
                    AudioManager.instance.PlaySFX("Button");
                }
            });
        }
    }

    public void pressPlay()
    {
        animAppear(playerPanel, 0.65f);
    }
    public void pressSetting()
    {
        animAppear(settingPanel, 1f);
    }
    public void closeSettingPanel()
    {
        animDisappear(settingPanel);
    }

    public void onSingleMode()
    {
        PlayerPrefs.SetInt("numPlayer", 1);
        turnLevelPanel();
    }
    public void onMultiMode()
    {
        PlayerPrefs.SetInt("numPlayer", 2);
        turnLevelPanel();
    }
    void turnLevelPanel()
    {
        animDisappear(playerPanel);
        animAppear(levelPanel, 0.8f);
    }
    public void closeLevelPanel()
    {
        animDisappear(levelPanel);
        playerPanel.transform.localScale = Vector2.zero;
    }
     public void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    void animAppear(GameObject obj, float scale) {
        obj.transform.DOScale(Vector3.one * scale, 0.3f).SetEase(Ease.OutBack);
    }
    void animDisappear(GameObject obj) {
        obj.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
    }
}
