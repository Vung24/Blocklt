using System.Collections;
using UnityEngine;

public class Cow : BaseEnemy
{
    public Vector3Int dirInt;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop(){
        int step = 0, maxStep = 5;
        while (true)
        {
            if (!isMoving && GameManager.instance.canMove)
            {
                Vector3Int nextPos = currentPos + dirInt;
                int turnCount = 0;
                int randDir = Random.value < 0.5f ? -1 : 1;
                while ((blockedPositions.Contains(nextPos) || CheckObstacle(nextPos) || step >= maxStep) && turnCount <= 4)
                {
                    step = 0;
                    maxStep = Random.Range(4, 10);
                    dirInt = Rotate90Degrees(dirInt, randDir);
                    nextPos = currentPos + dirInt;
                    turnCount += 1;
                }
                Debug.Log(dirInt);
                if (turnCount > 4)
                {
                    StopAnim();
                    animCoroutine = StartCoroutine(PlayAnimation(Vector3Int.zero));
                    yield return new WaitForSeconds(0.3f);
                }
                else
                {
                    step++;
                    bool changeDir = dirInt != currDirect;
                    currDirect = dirInt;
                    StartCoroutine(MoveToPosition(nextPos, dirInt, changeDir));
                }
            }
            yield return null;
        }
    }
    Vector3Int Rotate90Degrees(Vector3Int dirInt, int randDir){
        if(dirInt == Vector3Int.left) return randDir*Vector3Int.up;
        else if(dirInt == Vector3Int.right) return randDir*Vector3Int.down;
        else if(dirInt == Vector3Int.up) return randDir*Vector3Int.right;
        else if(dirInt == Vector3Int.down) return randDir*Vector3Int.left;
        else return Vector3Int.zero;
    }
}

