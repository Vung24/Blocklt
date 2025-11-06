using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLosePanel : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public GameObject countWin, desImage;
    public TextMeshProUGUI countWinP1, countWinP2, functionBtn, titleWLPanel;
    public Sprite winEmoji, loseEmoji;
    private int totalCount;
    public void AdjustWinLosePanel()
    {
        int scoreP1 = int.Parse(UIManager.instance.textScore1.text);
        int scoreP2 = int.Parse(UIManager.instance.textScore2.text);
        totalCount = scoreP1 + scoreP2;
        Invoke("PlaySound", 0.3f);
        if (GameManager.instance.winGame && GameManager.instance.numPlayer == 2)
        {
            desImage.SetActive(false);
            countWin.SetActive(true);
            
            int countP1, countP2;
            countP1 = PlayerPrefs.GetInt("countP1", 0);
            countP2 = PlayerPrefs.GetInt("countP2", 0);
            if (scoreP1 > scoreP2)
            {
                countP1++;
                PlayerPrefs.SetInt("countP1", countP1);
                countWin.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

            }
            else if (scoreP1 < scoreP2)
            {
                countP2++;
                PlayerPrefs.SetInt("countP2", countP2);
                countWin.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            }
            countWinP1.text = countP1.ToString();
            countWinP2.text = countP2.ToString();
        }
        else
        {
            countWin.SetActive(false);
            desImage.SetActive(true);
            Image desImg = desImage.GetComponent<Image>();
            if (GameManager.instance.winGame)
            {
                desImg.sprite = winEmoji;
            }
            else
            {
                BarControl.instance.updateBar(5);
                titleWLPanel.text = "Level Failed";
                functionBtn.text = "Restart";
                desImg.sprite = loseEmoji;
            }
        }
    }
    void PlaySound()
    {
        if (GameManager.instance.winGame)
        {
            if(AudioManager.instance){
                AudioManager.instance.PlaySFX("Win");
            }
            StartCoroutine(ScoreCountUp(totalCount));
        }
        else
        {
            if(AudioManager.instance){
                AudioManager.instance.PlaySFX("Lose");
            }
        }
    }
    IEnumerator ScoreCountUp(int targetScore)
    {
        int currentScore = 0;
        while (currentScore < targetScore)
        {
            currentScore += 10;
            if (currentScore > targetScore)
                currentScore = targetScore;

            scoreText.text = currentScore.ToString("D7");
            yield return new WaitForSeconds(0.03f); 
        }
    }
    public void ContinueScene()
    {
        int currLv = GameManager.instance.level;
        if (GameManager.instance.winGame)
        {
            SceneManager.LoadScene(currLv + 1);
        }
        else
        {
            string levelName = "Level" + currLv;
            SceneManager.LoadScene(levelName);
        }
    }
}
