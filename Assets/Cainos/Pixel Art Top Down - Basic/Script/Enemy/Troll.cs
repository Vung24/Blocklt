using System.Collections;
using UnityEngine;

public class Troll : BaseEnemy
{
    public Vector3Int dirInt;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(MoveLoop());
    }

    IEnumerator MoveLoop(){
        while(true){
            if(!isMoving && GameManager.instance.canMove){
                Vector3Int nextPos = currentPos + dirInt;
                int turnCount = 0;
                while(blockedPositions.Contains(nextPos) && turnCount < 4)
                {
                    dirInt = Rotate90Degrees(dirInt);
                    nextPos = currentPos + dirInt;
                    turnCount += 1;
                }
                if(turnCount >= 4){
                    Debug.Log(turnCount);
                    StopAnim();
                    animCoroutine = StartCoroutine(PlayAnimation(Vector3Int.zero));
                    yield return new WaitForSeconds(0.3f);
                } else{
                    bool changeDir = dirInt != currDirect;
                    currDirect = dirInt;
                    StartCoroutine(MoveToPosition(nextPos, dirInt, changeDir));
                }
            }
            yield return null;
        }
    }
    Vector3Int Rotate90Degrees(Vector3Int dirInt){
        if(dirInt == Vector3Int.left) return Vector3Int.up;
        else if(dirInt == Vector3Int.right) return Vector3Int.down;
        else if(dirInt == Vector3Int.up) return Vector3Int.right;
        else if(dirInt == Vector3Int.down) return Vector3Int.left;
        else return Vector3Int.zero;
    }
}

