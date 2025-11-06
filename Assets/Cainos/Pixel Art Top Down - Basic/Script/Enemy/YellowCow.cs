using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class YellowCow : BaseEnemy
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop()
    {
        while (true)
        {
            if (!isMoving && GameManager.instance.canMove)
            {
                Vector3Int targetCell = GetNearestPlayerCell();
                List<Vector3Int> path = FindPath(currentPos, targetCell);

                if (path != null && path.Count > 1)
                {
                    Vector3Int nextCell = path[1]; 
                    Vector3Int moveDir = nextCell - currentPos;
                    moveDir = ClampToDirection(moveDir);

                    bool changeDir = moveDir != currDirect;
                    currDirect = moveDir;
                    StartCoroutine(MoveToPosition(nextCell, moveDir, changeDir));
                }
                else
                {
                    StopAnim();
                    animCoroutine = StartCoroutine(PlayAnimation(Vector3Int.zero));
                    yield return new WaitForSeconds(0.3f);
                }
            }

            yield return null;
        }
    }

    Vector3Int ClampToDirection(Vector3Int dir)
    {
        if (dir.x > 0) return Vector3Int.right;
        if (dir.x < 0) return Vector3Int.left;
        if (dir.y > 0) return Vector3Int.up;
        if (dir.y < 0) return Vector3Int.down;
        return Vector3Int.zero;
    }

    Vector3Int GetNearestPlayerCell()
    {
        GameObject p1 = GameManager.instance.player1;
        GameObject p2 = GameManager.instance.player2;
        Transform nearest = p1.transform;
        Vector3 bluePos = transform.position;
        float d1 = Vector3.Distance(bluePos, p1.transform.position);
        if (!p1.activeSelf) nearest = p2.transform;
        else if(p2.activeSelf) nearest = p1.transform;
        else if (GameManager.instance.numPlayer == 2)
        {
            float d2 = Vector3.Distance(bluePos, p2.transform.position);
            nearest = (d1 <= d2) ? p1.transform : p2.transform;
        }
        return boxTilemap.WorldToCell(nearest.position);
    }

    List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        queue.Enqueue(start);
        cameFrom[start] = start;

        Vector3Int[] directions = new Vector3Int[]
        {
            Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
        };

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbor = current + dir;

                if (cameFrom.ContainsKey(neighbor)) continue;
                if (blockedPositions.Contains(neighbor)) continue;
                if (CheckObstacle(neighbor)) continue;

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        if (!cameFrom.ContainsKey(goal)) return null;

        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int currentStep = goal;
        while (currentStep != start)
        {
            path.Add(currentStep);
            currentStep = cameFrom[currentStep];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}
