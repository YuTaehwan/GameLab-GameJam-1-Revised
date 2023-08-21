using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GameFileManager
{
    private readonly string SAVE_DATA_PATH = Application.streamingAssetsPath + "/SaveData";

    private readonly string BLOCK_DATA_PATH = Application.streamingAssetsPath + "/BlockData";
    private readonly string BLOCK_PREFAB_PATH = "Prefabs/Block/";
    private readonly string BLOCK_BTN_IMG_PREFAB_PATH = "Prefabs/Block Button Images/";
    private readonly string BLOCK_BTN_IMG_PATH = "Sprites/Block Button Images/";

    [Serializable]
    private struct BlockListFile {
        public List<BlockFileData> blockDataList;
    }

    public List<BlockData> LoadBlockData()
    {
        List<BlockData> blockDataList = new List<BlockData>();

        string path = BLOCK_DATA_PATH;
        string jsonText = System.IO.File.ReadAllText(path);
        BlockListFile blockListFile = JsonUtility.FromJson<BlockListFile>(jsonText);

        foreach (var blockFileData in blockListFile.blockDataList)
        {
            BlockData blockData = new BlockData
            {
                type = blockFileData.type,
                name = blockFileData.name,
                korean_name = blockFileData.korean_name,
                price = blockFileData.price,
                needStar = blockFileData.needStar,
                size = blockFileData.size,
                prefab = Resources.Load<GameObject>(BLOCK_PREFAB_PATH + blockFileData.type.ToString()),
                btnImgPrefab = Resources.Load<GameObject>(BLOCK_BTN_IMG_PREFAB_PATH + blockFileData.type.ToString()),
                btnImg = Resources.Load<Sprite>(BLOCK_BTN_IMG_PATH + blockFileData.type.ToString())
            };

            blockDataList.Add(blockData);
        }

        return blockDataList;
    }

    public SaveFileData LoadSaveData() {
        string path = SAVE_DATA_PATH;
        string jsonText = System.IO.File.ReadAllText(path);
        SaveFileData saveFileData = JsonUtility.FromJson<SaveFileData>(jsonText);

        return saveFileData;
    }

    public void SaveGameData(SaveFileData saveFileData) {
        string path = SAVE_DATA_PATH;
        string jsonText = JsonUtility.ToJson(saveFileData);
        System.IO.File.WriteAllText(path, jsonText);
    }
}

[Serializable]
public struct StageFileData {
    public int stageNum;
    public int stageStars;
    public int initCoin;
    public int[] starBasis;
}

[Serializable]
public struct SaveFileData {
    public int allStarCount;
    public int levelCleared;
    public List<StageFileData> stageDatas;
}

[Serializable]
public struct BlockFileData {
    public BlockType type;
    public string name;
    public string korean_name;
    public int price;
    public int needStar;
    public Vector2Int size;
}