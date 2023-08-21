using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class MapLoader : MonoBehaviour
{
    public TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public MapData LoadMap(int level)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Map" + level);

        string jsonText = File.ReadAllText(path);
        MapData mapData = JsonUtility.FromJson<MapData>(jsonText);

        return mapData;
    }

    public MapData LoadMap()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Map/" + inputField.text);

        string jsonText = File.ReadAllText(path);
        MapData mapData = JsonUtility.FromJson<MapData>(jsonText);

        return mapData;
    }

    public void SaveMap(MapData mapData) {
        string path = Path.Combine(Application.streamingAssetsPath, "Map/" + inputField.text);

        string jsonText = JsonUtility.ToJson(mapData);
        File.WriteAllText(path, jsonText);
    }
}

[Serializable]
public struct MapData
{
    public List<BlockInfo> defaultBlocks;
    public List<BlockInfo> installableBlocks;
    public List<Vector3Int> mainMaps;
    public Vector2Int playerPos;
    public Vector2Int flagPos;
}