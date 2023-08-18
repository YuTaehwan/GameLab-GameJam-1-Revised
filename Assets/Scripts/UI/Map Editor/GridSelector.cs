using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using Unity.VisualScripting;

public class GridSelector : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> btnImgPrefabs;

    [SerializeField]
    public List<GameObject> allObjPrefabs;

    [SerializeField]
    public List<BlockType> blockTypes;

    [SerializeField]
    public List<string> blockString;

    public GameObject selectGridPrefab;
    public GameObject unableGridPrefab;

    public GameObject selectButtonPrefab;
    public GameObject deleteButtonImagePrefab;

    public Tilemap selectionTilemap;
    public RectTransform selectionButtonPanel;

    private Vector3Int hoverPos;
    private Vector3Int prevHoverPos;
    private BlockInfo selectedBlockInfo;

    private List<GameObject> instantiatedSelectGrids;
    private List<GameObject> instantiatedUnableGrids;
    private List<GameObject> instantiatedBlockObjs;

    private Tilemap mainTilemap;
    private HashSet<Vector2Int> preventedBlocksSet;
    private Vector2Int minTileBound = new Vector2Int(-30, -30);
    private Vector2Int maxTileBound = new Vector2Int(120, 60);

    public GameObject clickedParticle;

    [SerializeField]
    public readonly Dictionary<BlockType, Vector2Int> blockSizeData = new Dictionary<BlockType, Vector2Int>()
    {
        { BlockType.JUMP, new Vector2Int(1, 1) },
        { BlockType.MOVE_HORIZONTAL, new Vector2Int(6, 1) },
        { BlockType.MOVE_VERTICAL, new Vector2Int(2, 5) },
        { BlockType.FALL, new Vector2Int(2, 1) },
        { BlockType.JUMP_LAUNCHER, new Vector2Int(3, 1) },
        { BlockType.NORMAL, new Vector2Int(3, 1) },
        { BlockType.ROTATION, new Vector2Int(1, 1) },
        { BlockType.FERRIS, new Vector2Int(7, 6) },
        { BlockType.CONVEYOR, new Vector2Int(4, 1) },
        { BlockType.SPIKE_SMALL, new Vector2Int(1, 1) },
        { BlockType.SPIKE_BIG, new Vector2Int(3, 1) },
        { BlockType.OIL_PRESS, new Vector2Int(1, 1) },
        { BlockType.STICKY, new Vector2Int(3, 1) },
        { BlockType.BOW, new Vector2Int(1, 1) },
        { BlockType.NONE, new Vector2Int(0, 0) },
        { BlockType.DELETE, new Vector2Int(0, 0) },
    };

    private Dictionary<BlockType, GameObject> blockObjectData;
    private Dictionary<BlockType, GameObject> blockBtnImgData;
    private Dictionary<BlockType, string> blockBtnNameData;

    private MapLoader mapLoader;
    private MapData curMapData;

    private bool hasInitialized = false;
    private BlockInfo deleteButtonInfo;
    private BlockInfo nonBlockInfo;

    // Start is called before the first frame update
    void Start()
    {
        hasInitialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasInitialized && DontDestroyObject.Instance.IsEditMode()) {
            if (selectedBlockInfo.type != BlockType.NONE && selectedBlockInfo.type != BlockType.DELETE)
            {
                if (instantiatedUnableGrids.Count != 0)
                {
                    foreach (GameObject g in instantiatedUnableGrids)
                    {
                        Destroy(g);
                    }

                    instantiatedUnableGrids = new List<GameObject>();
                }

                hoverPos = selectionTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                bool isValidToPlace = IsValidPositionToPlace(new Vector2Int(hoverPos.x, hoverPos.y));

                if (hoverPos != prevHoverPos)
                {
                    foreach (GameObject g in instantiatedSelectGrids)
                    {
                        Destroy(g);
                    }

                    instantiatedSelectGrids = new List<GameObject>();

                    Vector3 targetPos = selectionTilemap.GetCellCenterWorld(hoverPos);
                    Vector2Int blockSize = 
                        selectedBlockInfo.isHalfRotated ? 
                        new Vector2Int(blockSizeData[selectedBlockInfo.type].y, blockSizeData[selectedBlockInfo.type].x) : 
                        blockSizeData[selectedBlockInfo.type];
                    Vector3 startPos = GetStartGridPos(targetPos, blockSize);

                    for (int i = 0; i < blockSize.x; i++)
                    {
                        for (int j = 0; j < blockSize.y; j++)
                        {
                            GameObject g = Instantiate(selectGridPrefab);

                            if (!isValidToPlace)
                            {
                                g.GetComponent<SpriteRenderer>().color = new Color(.9f, .3f, .3f, .4f);
                            }

                            instantiatedSelectGrids.Add(g);
                            g.GetComponent<Transform>().position =
                                new Vector3(
                                    startPos.x + selectionTilemap.cellSize.x * i,
                                    startPos.y + selectionTilemap.cellSize.y * j,
                                    startPos.z
                                );
                        }
                    }

                    prevHoverPos = hoverPos;
                }

                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && isValidToPlace)
                {
                    Vector3 targetPos = selectionTilemap.GetCellCenterWorld(hoverPos);
                    Vector2Int blockSize = 
                        selectedBlockInfo.isHalfRotated ? 
                        new Vector2Int(blockSizeData[selectedBlockInfo.type].y, blockSizeData[selectedBlockInfo.type].x) : 
                        blockSizeData[selectedBlockInfo.type];
                    selectedBlockInfo.startGridPos = selectionTilemap.WorldToCell(GetStartGridPos(targetPos, blockSize));
                    PlaceBlockObj(selectedBlockInfo);
                    FinishEditMode();
                }
            }
            else
            {
                InitSelectionUIObjs();
            }
        }
    }
    
    public void InitSelectionUI(int level)
    {
        blockObjectData = new Dictionary<BlockType, GameObject>();
        blockBtnImgData = new Dictionary<BlockType, GameObject>();
        blockBtnNameData = new Dictionary<BlockType, string>();

        // Initialize all blocks info
        if (allObjPrefabs.Count != blockTypes.Count || allObjPrefabs.Count != btnImgPrefabs.Count)
        {
            Debug.LogError("Number of block object prefabs(" + allObjPrefabs.Count + ") is not match with block types(" + blockTypes.Count + ")");
        }

        for (int i = 0; i < allObjPrefabs.Count; i++)
        {
            blockObjectData.Add(blockTypes[i], allObjPrefabs[i]);
            blockBtnImgData.Add(blockTypes[i], btnImgPrefabs[i]);
            blockBtnNameData.Add(blockTypes[i], blockString[i]);
        }

        mapLoader = GetComponent<MapLoader>();
        curMapData = mapLoader.LoadMap(level - 1);

        foreach (BlockInfo b in curMapData.installableBlocks)
        {
            AddButton(b);
        }

        deleteButtonInfo = new BlockInfo();
        deleteButtonInfo.type = BlockType.DELETE;
        AddButton(deleteButtonInfo);

        // Initialize private values
        instantiatedSelectGrids = new List<GameObject>();
        instantiatedUnableGrids = new List<GameObject>();
        instantiatedBlockObjs = new List<GameObject>();

        SearchPreventedBlocks();
        foreach (BlockInfo b in curMapData.defaultBlocks)
        {
            PlaceBlockObj(b);
        }

        nonBlockInfo = new BlockInfo { type = BlockType.NONE };
        selectedBlockInfo = nonBlockInfo;

        hasInitialized = true;
    }

    public void SetBlock(BlockInfo blockInfo)
    {
        if (selectedBlockInfo.type == BlockType.DELETE)
        {
            SetDeletableFlag(false);
        }

        selectedBlockInfo = blockInfo;

        SetDeletableFlag(blockInfo.type == BlockType.DELETE);
    }

    private void AddButton(BlockInfo blockInfo)
    {
        GameObject b = Instantiate(selectButtonPrefab);
        b.GetComponent<RectTransform>().SetParent(selectionButtonPanel);
        BlockInfo blockInfo1_ = blockInfo;
        b.GetComponent<Button>().onClick.AddListener(() => {
            SetBlock(blockInfo1_);
        });

        if (blockInfo.type != BlockType.DELETE)
        {
            b.transform.Find("BlockName").GetComponent<TextMeshProUGUI>().text = blockBtnNameData[blockInfo.type];

            GameObject bimg = Instantiate(blockBtnImgData[blockInfo.type]);
            bimg.GetComponent<RectTransform>().SetParent(b.GetComponent<RectTransform>());
            bimg.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, blockInfo.rotation));
        } else
        {
            b.transform.Find("BlockName").GetComponent<TextMeshProUGUI>().text = "삭제하기";

            GameObject bimg = Instantiate(deleteButtonImagePrefab);
            bimg.GetComponent<RectTransform>().SetParent(b.GetComponent<RectTransform>());
        }
    }

    private void PlaceBlockObj(BlockInfo blockInfo)
    {
        GameObject g = Instantiate(blockObjectData[blockInfo.type]);
        g.GetComponent<BlockBase>().SetBlockRotation(blockInfo.isHalfRotated, blockInfo.rotation);
        g.GetComponent<BlockBase>().SetStartGridPos(blockInfo.startGridPos);
        g.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, blockInfo.rotation));

        Vector2Int blockSize = 
            blockInfo.isHalfRotated ? 
            new Vector2Int(blockSizeData[blockInfo.type].y, blockSizeData[blockInfo.type].x) : 
            blockSizeData[blockInfo.type]; 
        Vector3 startPos = selectionTilemap.GetCellCenterWorld(blockInfo.startGridPos);
        Vector3 endPos = GetEndGridPos(startPos, blockSize);
        Vector3 position = (startPos + endPos) / 2;

        List<Vector2Int> preventTilesT = new List<Vector2Int>(); 
        for (int i = 0 ; i < blockSize.x ; i++)
        {
            for (int j = 0 ; j < blockSize.y ; j++)
            {
                preventTilesT.Add(new Vector2Int(blockInfo.startGridPos.x + i, blockInfo.startGridPos.y + j));
            }
        }

        foreach (Vector2Int t in preventTilesT)
        {
            preventedBlocksSet.Add(t);
        }

        g.GetComponent<Transform>().position = position;
        g.GetComponent<BlockBase>().SetInitialPos(position);

        IDeletable deletable = g.GetComponent<IDeletable>();
        if (deletable != null)
        {
            deletable.InitDeletable(blockSizeData[blockInfo.type]);
            deletable.SetOnDeleteCallback((target) => {
                instantiatedBlockObjs.Remove(target);
                Destroy(target);

                if (instantiatedUnableGrids.Count > 0)
                {
                    foreach (GameObject g in instantiatedUnableGrids)
                    {
                        Destroy(g);
                    }

                    instantiatedUnableGrids = new List<GameObject>();
                }

                foreach (Vector2Int t in preventTilesT)
                {
                    preventedBlocksSet.Remove(t);
                }

                FinishEditMode();
            });

            deletable.SetOnHoverCallback((target) => {
                if (instantiatedBlockObjs.Contains(target))
                {
                    if (instantiatedUnableGrids.Count > 0)
                    {
                        foreach (GameObject g in instantiatedUnableGrids)
                        {
                            Destroy(g);
                        }

                        instantiatedUnableGrids = new List<GameObject>();
                    }

                    for (int i = 0; i < blockSize.x; i++)
                    {
                        for (int j = 0; j < blockSize.y; j++)
                        {
                            GameObject g = Instantiate(unableGridPrefab);

                            instantiatedUnableGrids.Add(g);
                            g.GetComponent<Transform>().position =
                                new Vector3(
                                    startPos.x + selectionTilemap.cellSize.x * i,
                                    startPos.y + selectionTilemap.cellSize.y * j,
                                    startPos.z
                                );
                        }
                    }
                }
            });

            deletable.SetOnExitHoverCallback((target) =>
            {
                if (instantiatedUnableGrids.Count > 0)
                {
                    foreach (GameObject g in instantiatedUnableGrids)
                    {
                        Destroy(g);
                    }

                    instantiatedUnableGrids = new List<GameObject>();
                }
            });
        }

        instantiatedBlockObjs.Add(g);
    }

    public void FinishEditMode()
    {
        if (DontDestroyObject.gameManager.playMode == PlayMode.EDIT)
        {
            selectedBlockInfo = nonBlockInfo;

            InitSelectionUIObjs();
            InitDeletionUIObjs();
            SetDeletableFlag(false);

            GameObject c = Instantiate(clickedParticle);
            c.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            c.GetComponent<ParticleSystem>().Play();
            DontDestroyObject.gameManager.ExitEditMode();
        }
    }

    private Vector3 GetStartGridPos(Vector3 targetPos, Vector2Int blockSize)
    {
        Vector3 startPos = new Vector3
            (targetPos.x - (selectionTilemap.cellSize.x * (blockSize.x / 2)),
            targetPos.y - (selectionTilemap.cellSize.y * (blockSize.y / 2)),
            0);

        return startPos;
    }

    private Vector3 GetEndGridPos(Vector3 startPos, Vector2Int blockSize)
    {
        Vector3 endPos = new Vector3
            (startPos.x + (selectionTilemap.cellSize.x * (blockSize.x - 1)),
            startPos.y + (selectionTilemap.cellSize.y * (blockSize.y - 1)),
            0);
        return endPos;
    }

    private void SetDeletableFlag(bool isBlockDeletable)
    {
        foreach (GameObject g in instantiatedBlockObjs)
        {
            IDeletable deletable = g.GetComponent<IDeletable>();
            if (deletable != null)
            {
                deletable.SetDeletable(isBlockDeletable);
            }
        }
    }

    private void InitSelectionUIObjs()
    {
        if (instantiatedSelectGrids == null)
        {
            instantiatedSelectGrids = new List<GameObject>();
        }

        if (instantiatedSelectGrids.Count != 0)
        {
            foreach (GameObject g in instantiatedSelectGrids)
            {
                Destroy(g);
            }

            instantiatedSelectGrids = new List<GameObject>();
        }
    }

    private void InitDeletionUIObjs()
    {
        if (instantiatedUnableGrids == null)
        {
            instantiatedUnableGrids = new List<GameObject>();
        }

        if (instantiatedUnableGrids.Count != 0)
        {
            foreach (GameObject g in instantiatedUnableGrids)
            {
                Destroy(g);
            }

            instantiatedUnableGrids = new List<GameObject>();
        }
    }

    private void SearchPreventedBlocks()
    {
        preventedBlocksSet = new HashSet<Vector2Int>();

        mainTilemap = GameObject.Find("GroundTile").GetComponentInChildren<Tilemap>();

        for (int i = minTileBound.x; i < maxTileBound.x; i++)
        {
            for (int j = minTileBound.y; j < maxTileBound.y; j++)
            {
                if (mainTilemap.HasTile(new Vector3Int(i, j, 0)))
                {
                    preventedBlocksSet.Add(new Vector2Int(i, j));
                }
            }
        }
        
        Vector3Int t = mainTilemap.WorldToCell(GameObject.Find("Player").transform.position);
        preventedBlocksSet.Add(new Vector2Int(t.x, t.y));
    }

    private bool IsValidPositionToPlace(Vector2Int hoverPosition)
    {
        Vector2Int blockSize =
            selectedBlockInfo.isHalfRotated ?
            new Vector2Int(blockSizeData[selectedBlockInfo.type].y, blockSizeData[selectedBlockInfo.type].x) :
            blockSizeData[selectedBlockInfo.type];

        Vector2Int startBlockPos = hoverPosition - (blockSize / 2);

        for (int i = startBlockPos.x; i < startBlockPos.x + blockSize.x; i++)
        {
            for (int j = startBlockPos.y; j < startBlockPos.y + blockSize.y; j++)
            {
                if (preventedBlocksSet.Contains(new Vector2Int(i, j)))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public enum BlockType
{
    NONE = 0,
    DELETE,
    JUMP,
    MOVE_HORIZONTAL,
    MOVE_VERTICAL,
    FALL,
    JUMP_LAUNCHER,
    NORMAL,
    ROTATION,
    FERRIS,
    CONVEYOR,
    SPIKE_SMALL,
    SPIKE_BIG,
    OIL_PRESS,
    STICKY,
    BOW
}

[Serializable]
public struct BlockInfo
{
    public BlockType type;
    public bool isHalfRotated;
    public int rotation;
    public Vector3Int startGridPos;
}