using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridCordLogger : MonoBehaviour
{
    public Tilemap selectionTilemap;
    private Vector3Int hoverPos;

    void Update()
    {
        hoverPos = selectionTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Debug.Log("Mouse Grid Pos: " + hoverPos);
    }
}
