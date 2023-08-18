using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    [SerializeField]
    public BlockType type;
    public bool isHalfRotated;
    public int rotation;
    public Vector3Int startGridPos;
    public Vector3 initialPos;

    public void SetStartGridPos(Vector3Int startGridPos)
    {
        this.startGridPos = startGridPos;
    }

    public void SetBlockRotation(bool isHalfRotated, int rotation)
    {
        this.isHalfRotated = isHalfRotated;
        this.rotation = rotation;
    }

    public void SetInitialPos(Vector3 pos)
    {
        initialPos = pos;
    }
}
