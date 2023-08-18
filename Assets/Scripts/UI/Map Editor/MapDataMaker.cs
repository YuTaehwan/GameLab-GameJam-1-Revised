using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MapDataMaker : MonoBehaviour
{
    public List<BlockInfo> defaultBlocks;
    public List<BlockInfo> installableBlocks;

    // Start is called before the first frame update
    void Start()
    {
        //SaveMap(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveMap(int level)
    {
        MapData mapData = new MapData();
        mapData.defaultBlocks = defaultBlocks;
        mapData.installableBlocks = installableBlocks;

        string json = JsonUtility.ToJson(mapData, true);
        string path = Path.Combine(Application.dataPath, "Map" + level);

        File.WriteAllText(path, json);
    }
}
