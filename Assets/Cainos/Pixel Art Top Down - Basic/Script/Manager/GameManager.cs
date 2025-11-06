using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public int numPlayer = 1, level = 1;
    public bool winGame = false, canMove = true;
    public GameObject player1, player2;
    
    public Transform singleTransform, multiTransform;

    public CinemachineVirtualCamera virtualCamera;
    void Awake()
    {
        instance = this;
        numPlayer = PlayerPrefs.GetInt("numPlayer", 1);
    }
    void Start()
    {
        InitSound();
        ChangeFollowTarget();
    }
    void InitSound()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(() =>
            {
                if(AudioManager.instance){
                    AudioManager.instance.PlaySFX("Button");
                }    
            });
        }
        if(AudioManager.instance){
            AudioManager.instance.StopBGM();
            AudioManager.instance.PlayBGM("BgMusic");
        }     
    }
    void ChangeFollowTarget()
    {
        if (numPlayer == 1){
            virtualCamera.Follow = singleTransform;
            player2.SetActive(false);
        } else {
            virtualCamera.Follow = multiTransform;
            player2.SetActive(true);
        }
    }
    public void CheckLose()
    {
        if (!player1.activeSelf && !player2.activeSelf)
        {
            UIManager.instance.OpenWinLosePanel();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
