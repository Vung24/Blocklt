using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public int playerIndex;
    public float moveSpeed = 5f;
    private Tilemap boxTilemap, wallTilemap1, wallTilemap2, stoneTilemap;
    private TileBase[] boxTilesCreate, boxTilesBreak;
    private LayerMask obstacleLayer;

    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode placeKey = KeyCode.Space;

    private HashSet<Vector3Int> blockedPositions;
    private Vector3Int currentPos, dirInt = Vector3Int.down;
    private Vector3 targetPos;
    private bool isMoving = false, isPlacing = false, isHolding = false, playerDie = false;
    private Animator animator;
    private Coroutine lastTouch;

    void Start()
    {
        InitVariables();

        animator = GetComponent<Animator>();
        currentPos = boxTilemap.WorldToCell(transform.position);
        targetPos = boxTilemap.GetCellCenterWorld(currentPos);
        transform.position = targetPos;
    }
    void InitVariables()
    {
        blockedPositions = LevelManager.instance.blockedPositions;
        boxTilemap = LevelManager.instance.boxTilemap;
        wallTilemap1 = LevelManager.instance.wallTilemap1;
        wallTilemap2 = LevelManager.instance.wallTilemap2;
        stoneTilemap = LevelManager.instance.stoneTilemap;
        boxTilesCreate = LevelManager.instance.boxTilesCreate;
        boxTilesBreak = LevelManager.instance.boxTilesBreak;
        obstacleLayer = LevelManager.instance.obstacleLayer;
    }

    void Update()
    {
        if (isPlacing || playerDie || !GameManager.instance.canMove) return;

        if (Input.GetKeyDown(placeKey))
        {
            StartCoroutine(PlaceBox(dirInt));
        }
        else if (!isMoving)
        {
            Vector3Int moveDir = GetInputDirection();
            if (moveDir != Vector3Int.zero)
            {
                dirInt = moveDir;
                StartCoroutine(CheckHolding());
                animator.SetBool("IsMoving", true);

                if (isHolding) TryMove(moveDir);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }

    Vector3Int GetInputDirection()
    {
        if (Input.GetKey(downKey)) { animator.SetInteger("Direction", 0); return Vector3Int.down; }
        if (Input.GetKey(upKey)) { animator.SetInteger("Direction", 1); return Vector3Int.up; }
        if (Input.GetKey(rightKey)) { animator.SetInteger("Direction", 2); return Vector3Int.right; }
        if (Input.GetKey(leftKey)) { animator.SetInteger("Direction", 3); return Vector3Int.left; }
        return Vector3Int.zero;
    }

    void TryMove(Vector3Int moveDir)
    {
        Vector3Int nextPos = currentPos + moveDir;
        Vector3 worldCheckPos = boxTilemap.GetCellCenterWorld(nextPos);
        bool canMove = true;
        Collider2D obstacleCheck = Physics2D.OverlapBox(worldCheckPos, Vector2.one * 0.5f, 0f, obstacleLayer);

        if (obstacleCheck != null && obstacleCheck.gameObject != this.gameObject && !obstacleCheck.gameObject.CompareTag("Enemy"))
        {
            canMove = false;
        }
        
        if (!blockedPositions.Contains(nextPos) && canMove)
        {
            StartCoroutine(MoveToPosition(nextPos));
        }

        if (lastTouch != null) StopCoroutine(lastTouch);
        lastTouch = StartCoroutine(CountLastTouch());
    }

    IEnumerator MoveToPosition(Vector3Int newCell)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = boxTilemap.GetCellCenterWorld(newCell);

        float elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        currentPos = newCell;
        isMoving = false;

        yield return new WaitForSeconds(0.05f);
    }

    IEnumerator PlaceBox(Vector3Int dir)
    {
        isPlacing = true;
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(DisablePlacing());

        Vector3Int nextPos = currentPos + dir;

        if (boxTilemap.GetTile(nextPos) == null)
        {
            Debug.Log(stoneTilemap);
            while (!blockedPositions.Contains(nextPos) && stoneTilemap.GetTile(nextPos) == null)
            {
                Vector3 worldCheckPos = boxTilemap.GetCellCenterWorld(nextPos);

                Collider2D obstacleCheck = Physics2D.OverlapBox(worldCheckPos, Vector2.one * 0.5f, 0f, obstacleLayer);
                if (obstacleCheck != null)
                {
                    break;
                }
                blockedPositions.Add(nextPos);
                StartCoroutine(AnimateTileCreate(nextPos));
                nextPos += dir;
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            while (wallTilemap1.GetTile(nextPos) == null && wallTilemap2.GetTile(nextPos) == null && boxTilemap.GetTile(nextPos) != null)
            {
                blockedPositions.Remove(nextPos);
                StartCoroutine(AnimateTileBreak(nextPos));
                nextPos += dir;
                yield return new WaitForSeconds(0.1f);
            }
        }
        SetMap.instance.checkBlockPosition();
    }

    IEnumerator AnimateTileCreate(Vector3Int nextPos)
    {
        if(AudioManager.instance){
            AudioManager.instance.PlaySFX("Create");
        }
        foreach (TileBase tile in boxTilesCreate)
        {
            boxTilemap.SetTile(nextPos, tile);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator AnimateTileBreak(Vector3Int nextPos)
    {
        if(AudioManager.instance){
            AudioManager.instance.PlaySFX("Break");
        }
        foreach (TileBase tile in boxTilesBreak)
        {
            boxTilemap.SetTile(nextPos, tile);
            yield return new WaitForSeconds(0.05f);
        }
        boxTilemap.SetTile(nextPos, null);
    }


    IEnumerator CheckHolding()
    {
        yield return new WaitForSeconds(0.2f);
        isHolding = true;
    }
    IEnumerator CountLastTouch()
    {
        yield return new WaitForSeconds(0.4f);
        isHolding = false;
    }

    IEnumerator DisablePlacing()
    {
        yield return new WaitForSeconds(0.05f);
        isPlacing = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(AudioManager.instance){
                AudioManager.instance.PlaySFX("Die");
            }
            playerDie = true;
            transform.DOScaleY(0f, 0.5f).OnComplete(() =>
            {
                UIManager.instance.SetAvatarDie(playerIndex);
                Debug.Log("Player " + playerIndex + " die");
                gameObject.SetActive(false);
                GameManager.instance.CheckLose();
            });
        }
    }
}
