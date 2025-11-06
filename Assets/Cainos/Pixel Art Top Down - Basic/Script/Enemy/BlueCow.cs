using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueCow : BaseEnemy
{
    
    private bool moved = false;
    private Vector3Int[] directions = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FollowClosestPlayerLoop());
    }

    IEnumerator FollowClosestPlayerLoop()
    {
        while (true)
        {
            if (!isMoving && GameManager.instance.canMove)
            {
                Transform targetPlayer = GetClosestPlayer();
                if (targetPlayer == null)
                {
                    yield return null;
                    continue;
                }

                Vector3Int bestDir = Vector3Int.zero;
                float bestDist = float.MaxValue;
                Vector3Int playerCell = boxTilemap.WorldToCell(targetPlayer.position);

                foreach (Vector3Int dir in directions)
                {
                    Vector3Int nextPos = currentPos + dir;

                    if (blockedPositions.Contains(nextPos) || CheckObstacle(nextPos)) continue;

                    float dist = Vector3Int.Distance(nextPos, playerCell);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestDir = dir;
                    }
                }
                if (bestDir != Vector3Int.zero)
                {
                    moved = true;
                    bool changeDir = bestDir != currDirect;
                    currDirect = bestDir;
                    StartCoroutine(MoveToPosition(currentPos + bestDir, bestDir, changeDir));
                }
                else
                {
                    if (moved)
                    {
                        StopAnim();
                        animCoroutine = StartCoroutine(PlayAnimation(Vector3Int.zero));
                        moved = false;
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    Transform GetClosestPlayer()
    {
        GameObject p1 = GameManager.instance.player1;
        GameObject p2 = GameManager.instance.player2;

        Vector3 bluePos = transform.position;
        float d1 = Vector3.Distance(bluePos, p1.transform.position);

        if (!p2.activeSelf) return p1.transform;
        else if (!p1.activeSelf) return p2.transform;

        float d2 = Vector3.Distance(bluePos, p2.transform.position);
        return (d1 <= d2) ? p1.transform : p2.transform;
    }
}
