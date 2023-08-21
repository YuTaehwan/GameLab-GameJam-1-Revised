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
    public bool isDefaultBlock;

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

    public void InitializeBlock(Vector3Int startGridPos, bool isHalfRotated, int rotation, Vector3 initialPos, bool isDefaultBlock)
    {
        SetStartGridPos(startGridPos);
        SetBlockRotation(isHalfRotated, rotation);
        SetInitialPos(initialPos);
        this.isDefaultBlock = isDefaultBlock;
    }
}
