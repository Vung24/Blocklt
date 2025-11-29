
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject scoreP1, scoreP2, pausePanel;
    public Image avatarP1, avatarP2;
    public WinLosePanel winLosePanel;
    public TextMeshProUGUI textScore1, textScore2;
    public Sprite[] p1, p2;
    public Sprite pLose, pWin;
    Coroutine avatarCoroutine1, avatarCoroutine2;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (GameManager.instance.numPlayer == 1)
        {
            scoreP2.SetActive(false);
        }
    }

    public void LoadMenuScene()
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.StopBGM();
        }
        SceneManager.LoadScene(0);
    }
    public void IncreaseScore(int indexPlayer, int bonusScore)
    {
        int score;
        if (indexPlayer == 1)
        {
            score = int.Parse(textScore1.text);
            textScore1.text = (score + bonusScore).ToString();
            animCollect(avatarP1, p1, ref avatarCoroutine1);
        }
        else
        {
            score = int.Parse(textScore2.text);
            textScore2.text = (score + bonusScore).ToString();
            animCollect(avatarP2, p2, ref avatarCoroutine2);
        }
    }
    void animCollect(Image avatar, Sprite[] pImages, ref Coroutine avatarCoroutine)
    {
        avatar.sprite = pImages[1];
        if (avatarCoroutine != null) StopCoroutine(avatarCoroutine);
        avatarCoroutine = StartCoroutine(ChangeSpriteAfterDelay(avatar, pImages[0], 0.5f));
    }

    IEnumerator ChangeSpriteAfterDelay(Image avatar, Sprite newSprite, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(GameManager.instance.canMove) avatar.sprite = newSprite;
    }
    public void OpenWinLosePanel()
    {
        winLosePanel.AdjustWinLosePanel();
        animAppear(winLosePanel.gameObject, 1);
        GameManager.instance.canMove = false;
        if (GameManager.instance.winGame)
        {
            showWinAvatar(avatarP1);
            if (scoreP2.activeSelf)
            {
                showWinAvatar(avatarP2);
            }
            int maxLevel = PlayerPrefs.GetInt("UnlockedLevel",1);
            if(GameManager.instance.level >= maxLevel)
                PlayerPrefs.SetInt("UnlockedLevel", GameManager.instance.level + 1);
        }
        else
        {
            SetAvatarDie(1);
            if (scoreP2.activeSelf) SetAvatarDie(2);
        }
    }
    public void OpenPausedPanel()
    {
        animAppear(pausePanel, 1);
        GameManager.instance.canMove = false;
    }
    public void ClosedPausedPanel()
    {
        animDisappear(pausePanel);
        GameManager.instance.canMove = true;
    }
    public void SetAvatarDie(int playerIndex)
    {
        if (playerIndex == 1)
        {
            avatarP1.sprite = pLose;
        }
        else
        {
            avatarP2.sprite = pLose;
        }
    }
    void showWinAvatar(Image avatar)
    {
        avatar.sprite = pWin;
        avatar.rectTransform.rotation = Quaternion.Euler(0f, 0f, -7f);
        avatar.rectTransform
            .DORotate(new Vector3(0, 0, 7f), 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        Sequence scaleSequence1 = DOTween.Sequence();
        scaleSequence1.Append(avatar.rectTransform.DOScale(new Vector3(1.2f, 1.1f, 1f), 0.2f))
                     .Append(avatar.rectTransform.DOScale(new Vector3(1.1f, 1.2f, 1f), 0.2f))
                     .Append(avatar.rectTransform.DOScale(Vector3.one, 0.2f))
                     .SetLoops(-1)
                     .SetEase(Ease.InOutSine);
    }
    void animAppear(GameObject obj, float scale)
    {
        obj.transform.DOScale(Vector3.one * scale, 0.3f).SetEase(Ease.OutBack);
    }
    void animDisappear(GameObject obj)
    {
        obj.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
    }
}
