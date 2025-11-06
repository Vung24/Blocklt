using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.IK;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int score;
    protected GameObject stuckImage, floatText;
    protected Sequence pulse;
    protected Tween tween;
    protected Sprite[] imagesBlock;
    protected HashSet<Vector3Int> blockedPositions;
    protected Tilemap boxTilemap;
    protected virtual void Start()
    {
        stuckImage = gameObject.transform.GetChild(0).gameObject;
        floatText = SetMap.instance.floatText;
        imagesBlock = SetMap.instance.emojiBlock;
        blockedPositions = LevelManager.instance.blockedPositions;
        boxTilemap = LevelManager.instance.boxTilemap;
    }
    
    public void AppearEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 0f;
        sr.color = color;
        sr.DOFade(1f, 0.7f);
        float originScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
        tween = transform.DOScale(originScale, 0.7f).OnComplete(() =>
        {
            pulse = DOTween.Sequence();
            pulse.Append(transform.DOScale(new Vector3(originScale * 1.1f, originScale, originScale), 0.3f));
            pulse.Append(transform.DOScale(new Vector3(originScale, originScale * 1.1f, originScale), 0.3f));
            pulse.SetLoops(-1);
        });
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            if (AudioManager.instance)
            {
                AudioManager.instance.PlaySFX("Item");
            }
            if (pulse != null && pulse.IsActive()) pulse.Kill();
            if (tween != null && tween.IsActive()) tween.Kill();
            Destroy(gameObject);
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            UIManager.instance.IncreaseScore(pc.playerIndex, score);
            SetMap.instance.checkWave();
            // Debug.Log(transform.position);
            GameObject floatClone = Instantiate(floatText, transform.position, Quaternion.identity, other.transform);
            floatClone.GetComponent<TextMeshPro>().text = "+" + score.ToString();
            if (score > 10)
                floatClone.GetComponent<TextMeshPro>().color = new Color(0.9137255f, 0.1051038f, 0.0235294f, 1f);
            Destroy(floatClone, 1f);
            OnDestroy();
        }
    }
    protected virtual void OnDestroy()
    {
        
    }
    protected bool isBlock = false;
    private Tween fadeTween;
    public void handleBlock()
    {
        if (isBlock) return;
        isBlock = true;
        if (fadeTween != null && fadeTween.IsActive()) fadeTween.Kill();
        int randomIndex = Random.Range(0, imagesBlock.Length);
        SpriteRenderer spr = stuckImage.GetComponent<SpriteRenderer>();
        spr.sprite = imagesBlock[randomIndex];
        fadeTween = spr.DOFade(1f, 0.5f);
    }
    public void handleUnblock(){
        if (!isBlock) return;
        isBlock = false;
        if (fadeTween != null && fadeTween.IsActive()) fadeTween.Kill();
        SpriteRenderer spr = stuckImage.GetComponent<SpriteRenderer>();
        fadeTween = spr.DOFade(0f, 0.5f);
    }
}
