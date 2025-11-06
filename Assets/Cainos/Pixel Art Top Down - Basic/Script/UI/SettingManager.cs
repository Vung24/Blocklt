using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Image bgmIcon;
    [SerializeField] private Image sfxIcon;
    [SerializeField] private Sprite bgmOnSprite;
    [SerializeField] private Sprite bgmOffSprite;
    [SerializeField] private Sprite sfxOnSprite;
    [SerializeField] private Sprite sfxOffSprite;

    void Start()
    {
        bgmButton.onClick.AddListener(() =>
        {
            if(AudioManager.instance){
                AudioManager.instance.ToggleBGM();
            }
            UpdateIcons();
        });

        sfxButton.onClick.AddListener(() =>
        {
            if(AudioManager.instance){
                AudioManager.instance.ToggleSFX();
            }
            UpdateIcons();
        });

        UpdateIcons();
    }

    void UpdateIcons()
    {
        if (AudioManager.instance)
        {
            bgmIcon.sprite = AudioManager.instance.IsBGMOn() ? bgmOnSprite : bgmOffSprite;
            sfxIcon.sprite = AudioManager.instance.IsSFXOn() ? sfxOnSprite : sfxOffSprite;
        }
    }
}