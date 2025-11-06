using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public float timeLevel;
    public TextMeshProUGUI timerText;
    public static LevelManager instance;
    public HashSet<Vector3Int> blockedPositions = new HashSet<Vector3Int>();
    public Tilemap boxTilemap, wallTilemap1, wallTilemap2, stoneTilemap;
    public TileBase[] boxTilesCreate, boxTilesBreak;
    public LayerMask obstacleLayer;

    private float currentTime;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if(GameManager.instance.numPlayer == 1) currentTime = timeLevel + 60;
        else currentTime = timeLevel;
        if (blockedPositions.Count == 0)
            InitBlockedPositions();
    }
    void InitBlockedPositions()
    {
        AddTilemapToBlocked(boxTilemap);
        AddTilemapToBlocked(wallTilemap1);
        AddTilemapToBlocked(wallTilemap2);
    }

    void AddTilemapToBlocked(Tilemap tilemap)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.GetTile(pos) != null)
                blockedPositions.Add(pos);
        }
    }
    void Update()
    {
        if (!GameManager.instance.canMove) return;
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0)
            {
                currentTime = 0;
                if (AudioManager.instance)
                {
                    AudioManager.instance.PlaySFX("OutTime");
                }
                UIManager.instance.OpenWinLosePanel();
            }
        }
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
