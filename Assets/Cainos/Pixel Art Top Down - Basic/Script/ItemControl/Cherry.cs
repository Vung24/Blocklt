using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cherry : Item
{
    private Bounds bound;
    private Coroutine respawnRoutine;
    private static HashSet<Vector3Int> occupiedCherryPositions = new HashSet<Vector3Int>();
    private Vector3Int currentCellPos;
    private Vector3 lscale;
    protected override void Start()
    {
        base.Start();
        bound = SetMap.instance.cameraBound.bounds;
        respawnRoutine = StartCoroutine(RespawnRoutine());
        lscale = transform.localScale;
    }

    IEnumerator RespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Transform tf = transform;
            GetComponent<Collider2D>().enabled = false;
            yield return DOTween.Sequence()
                .Join(sr.DOFade(0f, 1f))
                .Join(tf.DOScale(new Vector3(0, 2f, 0), 1f))
                .WaitForCompletion();
            occupiedCherryPositions.Remove(currentCellPos);

            Vector3Int newCellPos;
            int tries = 100;
            do
            {
                float x = Random.Range(bound.min.x, bound.max.x);
                float y = Random.Range(bound.min.y, bound.max.y);
                newCellPos = boxTilemap.WorldToCell(new Vector3(x, y, 0));
                tries--;
            } while ((LevelManager.instance.blockedPositions.Contains(newCellPos) ||
                    occupiedCherryPositions.Contains(newCellPos)) && tries > 0);

            if (tries > 0 && !isBlock)
            {
                currentCellPos = newCellPos;
                occupiedCherryPositions.Add(currentCellPos);
                transform.position = boxTilemap.GetCellCenterWorld(currentCellPos) + new Vector3(0f, 0.1f, 0f);
            }

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            tf.localScale = lscale;
            yield return new WaitForSeconds(1f);
            yield return DOTween.Sequence()
                .Join(sr.DOFade(1f, 1f))
                .Join(tf.DORotate(new Vector3(0f, 360 * 7f, 0f), 1.5f, RotateMode.FastBeyond360))
                .WaitForCompletion();

            GetComponent<Collider2D>().enabled = true;
        }
    }

    protected override void OnDestroy()
    {
        occupiedCherryPositions.Remove(currentCellPos);
    }
}
