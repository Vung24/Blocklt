using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseEnemy : MonoBehaviour
{
    public Tilemap boxTilemap;
    public int speed = 2;
    public Sprite[] moveUpAnim, moveDownAnim, moveLeftAnim, moveRightAnim, idleAnim;
    protected static HashSet<Vector3Int> blockedPositions;
    protected Vector3Int currentPos, currDirect = Vector3Int.zero;
    protected Vector3 targetPos;
    protected SpriteRenderer enemyRenderer;
    protected bool isMoving = false, canMove = true;
    protected LayerMask obstacleLayer;
    protected Coroutine animCoroutine;
    private float duration;
    protected virtual void Start()
    {
        boxTilemap = LevelManager.instance.boxTilemap;
        blockedPositions = LevelManager.instance.blockedPositions;
        obstacleLayer = LevelManager.instance.obstacleLayer;

        enemyRenderer = GetComponent<SpriteRenderer>();
        currentPos = boxTilemap.WorldToCell(transform.position);
        targetPos = boxTilemap.GetCellCenterWorld(currentPos);
        transform.position = targetPos;
        duration = 10f / speed;
    }
    protected IEnumerator MoveToPosition(Vector3Int newCell, Vector3Int dirInt, bool changeDir)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = boxTilemap.GetCellCenterWorld(newCell);
        float elapsedTime = 0f;

        if (changeDir)
        {
            StopAnim();
            animCoroutine = StartCoroutine(PlayAnimation(dirInt));
        }
        while (elapsedTime < duration)
        {
            if (blockedPositions.Contains(newCell))
            {
                isMoving = false;
                yield break;
            }
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        currentPos = newCell;
        isMoving = false;
        yield return new WaitForSeconds(0.05f);
    }

    protected bool CheckObstacle(Vector3Int nextpos)
    {
        Vector3 worldCheckPos = boxTilemap.GetCellCenterWorld(nextpos);
        Collider2D obstacleCheck = Physics2D.OverlapBox(worldCheckPos, Vector2.one * 0.3f, 0f, obstacleLayer);
        if (obstacleCheck != null && obstacleCheck.gameObject != this.gameObject && !obstacleCheck.gameObject.CompareTag("Player"))
        {
            return true;
        }
        return false;
    }
    protected IEnumerator PlayAnimation(Vector3Int direction)
    {
        Debug.Log("change dirrect");
        Sprite[] animationFrames;
        enemyRenderer.flipX = false;
        if (direction == Vector3Int.up) animationFrames = moveUpAnim;
        else if (direction == Vector3Int.down) animationFrames = moveDownAnim;
        else if (direction == Vector3Int.left)
        {
            enemyRenderer.flipX = true;
            animationFrames = moveLeftAnim;
        }
        else if (direction == Vector3Int.right) animationFrames = moveRightAnim;
        else animationFrames = idleAnim;
        int frameIndex = 0;
        while (true)
        {
            enemyRenderer.sprite = animationFrames[frameIndex];
            frameIndex = (frameIndex + 1) % animationFrames.Length;
            yield return new WaitForSeconds(1.2f / animationFrames.Length);
        }
    }
    protected void StopAnim()
    {
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
    }
}
