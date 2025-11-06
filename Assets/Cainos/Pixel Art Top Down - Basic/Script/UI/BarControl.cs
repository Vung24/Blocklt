using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BarControl : MonoBehaviour
{
    public static BarControl instance;
    public GameObject itemContain, content;
    public RectTransform rectArrow;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        rectArrow.DOAnchorPosY(rectArrow.anchoredPosition.y + 1f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    public void addItemToBar(Sprite itemSprite)
    {
        GameObject item = Instantiate(itemContain, content.transform);
        item.transform.GetChild(0).GetComponent<Image>().sprite = itemSprite;
        Image maskImage = item.transform.GetChild(1).GetComponent<Image>();
        maskImage.sprite = itemSprite;
    }

    public void updateBar(int index)
    {
        StartCoroutine(UpdateArrowPos(index));
    }

    IEnumerator UpdateArrowPos(int index)
    {
        yield return null;

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Transform mask = content.transform.GetChild(i).GetChild(1);
            if (i == index)
            {
                mask.gameObject.SetActive(false);

                RectTransform itemRect = content.transform.GetChild(i).GetComponent<RectTransform>();
                Vector2 arrowPos = rectArrow.anchoredPosition;

                rectArrow.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, arrowPos.y);
            }
            else
            {
                mask.gameObject.SetActive(true);
            }
        }
        if (index >= content.transform.childCount)
        {
            rectArrow.gameObject.SetActive(false);
        }
    }

}
