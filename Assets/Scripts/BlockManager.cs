using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BlockManager
{
    private List<BlockData> _allBlockData = new List<BlockData>();
    public List<BlockData> allBlockData => _allBlockData;

    public BlockData GetBlockData(BlockType type)
    {
        foreach (var blockData in _allBlockData)
        {
            if (blockData.type == type)
            {
                return blockData;
            }
        }

        return new BlockData();
    }

    public void Init()
    {
        _allBlockData = DontDestroyObject.gameFileManager.LoadBlockData();
    }
}

public struct BlockData {
    public BlockType type;
    public string name;
    public string korean_name;
    public int price;
    public int needStar;
    public Vector2Int size;
    public GameObject prefab;
    public GameObject btnImgPrefab;
    public Sprite btnImg;
}
