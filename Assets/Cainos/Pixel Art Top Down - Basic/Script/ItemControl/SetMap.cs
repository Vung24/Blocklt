using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class SetupWave
{
    public GameObject item;
    public string[] posItem;
}
public class SetMap : MonoBehaviour
{
    public static SetMap instance;
    public SetupWave[] maps;
    public Sprite[] emojiBlock;
    public GameObject floatText;
    public Collider2D cameraBound;
    private Tilemap boxTilemap;
    private HashSet<Vector3Int> blockedPositions;
    private GameObject currentWave;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        boxTilemap = LevelManager.instance.boxTilemap;
        blockedPositions = LevelManager.instance.blockedPositions;
        for(int i = 0; i < maps.Length; i++){
            GameObject wave = new GameObject("Wave"+i);
            wave.transform.SetParent(this.transform);
            wave.SetActive(false);
            SpriteRenderer spr = maps[i].item.GetComponent<SpriteRenderer>();
            BarControl.instance.addItemToBar(spr.sprite);

            for(int j = 0; j < maps[i].posItem.Length; j++){
                string[] pos = maps[i].posItem[j].Split(',');
                int x = int.Parse(pos[0]);
                int y = int.Parse(pos[1]);
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                Vector3 worldPos = boxTilemap.GetCellCenterWorld(tilePos);
                worldPos.y += 0.15f;
                GameObject item = Instantiate(maps[i].item, worldPos, Quaternion.identity);
                Item itemScript = item.GetComponent<Item>();
                itemScript.name = maps[i].item.name;
                // Debug.Log("Item name: "+itemScript.name);

                item.transform.SetParent(wave.transform);
            }
        }
        checkWave();
    }
    public void checkWave(){
        bool check = false;
        for(int i = 0; i < maps.Length; i++){
            Transform wave = this.transform.GetChild(i);
            // Debug.Log(wave.childCount);
            if (wave.childCount <= 1) continue;
            else
            {
                check = true;
                if (wave.gameObject.activeSelf == false)
                {
                    wave.gameObject.SetActive(true);
                    BarControl.instance.updateBar(i);
                    currentWave = wave.gameObject;
                    foreach (Transform child in wave)
                    {
                        Item item = child.gameObject.GetComponent<Item>();
                        item.AppearEffect();
                    }
                }
                break;
            }
        }
        if (check == false)
        {
            BarControl.instance.updateBar(maps.Length);
            Debug.Log("All waves are done");
            GameManager.instance.winGame = true;
            UIManager.instance.OpenWinLosePanel();
        }
    }
    
    public void checkBlockPosition(){
        for (int i = 0; i < currentWave.transform.childCount; i++)
        {
            Transform item = currentWave.transform.GetChild(i);
            Vector3Int pos = boxTilemap.WorldToCell(item.position);
            Item itemScript = item.GetComponent<Item>();
            if (blockedPositions.Contains(pos))
            {
                itemScript.handleBlock();
            }
            else
            {
                itemScript.handleUnblock();
            }
        }
    }
}
