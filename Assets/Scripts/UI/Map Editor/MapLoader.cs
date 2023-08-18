using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MapLoader : MonoBehaviour
{
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
}

[Serializable]
public struct MapData
{
    public List<BlockInfo> defaultBlocks;
    public List<BlockInfo> installableBlocks;
}