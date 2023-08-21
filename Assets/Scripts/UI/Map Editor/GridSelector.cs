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
    public GameObject selectGridPrefab;
    public GameObject unableGridPrefab;

    public GameObject selectButtonPrefab;
    public GameObject deleteButtonImagePrefab;

    public Tilemap selectionTilemap;
    public RectTransform selectionButtonPanel;

    public GameObject coinUsePanel;
    public RectTransform canvasRect;

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

    private MapLoader mapLoader;
    private MapData curMapData;

    private bool hasInitialized = false;
    private BlockInfo deleteButtonInfo;
    private BlockInfo nonBlockInfo;

    private List<Button> _blockButtons = new List<Button>();
    public List<Button> blockButtons { get => _blockButtons; }

    // Start is called before the first frame update
    void Start()
    {
        hasInitialized = false;
        coinUsePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasInitialized && DontDestroyObject.Instance.IsEditMode()) {
            if (Input.GetMouseButtonDown(1) && selectedBlockInfo.type != BlockType.DELETE)
            {
                selectedBlockInfo = nonBlockInfo;
            }

            if (selectedBlockInfo.type == BlockType.NONE || EventSystem.current.IsPointerOverGameObject()) {
                coinUsePanel.SetActive(false);
            }

            if (selectedBlockInfo.type != BlockType.NONE && selectedBlockInfo.type != BlockType.DELETE && !EventSystem.current.IsPointerOverGameObject())
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
                    BlockData blockData = DontDestroyObject.blockManager.GetBlockData(selectedBlockInfo.type);
                    Vector2Int blockSize = 
                        selectedBlockInfo.isHalfRotated ? 
                        new Vector2Int(blockData.size.y, blockData.size.x) : 
                        blockData.size;
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

                            // SHOW COIN USE PANEL
                            if (i == blockSize.x - 1 && j == 0)
                            {
                                ShowCoinUsePanel(g.GetComponent<Transform>().position, -blockData.price);
                            }
                        }
                    }

                    prevHoverPos = hoverPos;
                }

                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && isValidToPlace)
                {
                    Vector3 targetPos = selectionTilemap.GetCellCenterWorld(hoverPos);
                    BlockData blockData = DontDestroyObject.blockManager.GetBlockData(selectedBlockInfo.type);
                    if (DontDestroyObject.gameManager.curMaxCoin - DontDestroyObject.gameManager.curUsedCoin >= blockData.price) {
                        Vector2Int blockSize = 
                        selectedBlockInfo.isHalfRotated ? 
                            new Vector2Int(blockData.size.y, blockData.size.x) : 
                            blockData.size;
                        selectedBlockInfo.startGridPos = selectionTilemap.WorldToCell(GetStartGridPos(targetPos, blockSize));
                        PlaceBlockObj(selectedBlockInfo, false);
                        DontDestroyObject.gameManager.UseCoin(blockData.price);
                        //FinishEditMode();
                    }
                }
            }
            else
            {
                InitSelectionUIObjs();
            }
        } else {
            coinUsePanel.SetActive(false);
        }
    }
    
    public void InitSelectionUI(int level)
    {
        mapLoader = GetComponent<MapLoader>();
        curMapData = mapLoader.LoadMap(level - 1);

        foreach (BlockInfo b in curMapData.installableBlocks)
        {
            AddButton(b);
        }

        deleteButtonInfo = new BlockInfo
        {
            type = BlockType.DELETE
        };
        AddButton(deleteButtonInfo);

        // Initialize private values
        instantiatedSelectGrids = new List<GameObject>();
        instantiatedUnableGrids = new List<GameObject>();
        instantiatedBlockObjs = new List<GameObject>();

        ResetEditor();

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
        if (blockInfo.type == BlockType.DELETE)
        {
            coinUsePanel.SetActive(false);
        }

        SetDeletableFlag(blockInfo.type == BlockType.DELETE);
    }

    private void AddButton(BlockInfo blockInfo)
    {
        GameObject b = Instantiate(selectButtonPrefab, selectionButtonPanel);
        
        BlockInfo blockInfo1_ = blockInfo;

        b.GetComponent<BlockSelectButton>().Init(blockInfo, () => {
            SetBlock(blockInfo1_);
        }, deleteButtonImagePrefab);

        blockButtons.Add(b.GetComponent<Button>());
    }

    private void PlaceBlockObj(BlockInfo blockInfo, bool isDefault)
    {
        BlockData blockData = DontDestroyObject.blockManager.GetBlockData(blockInfo.type);
        GameObject g = Instantiate(blockData.prefab);
        g.GetComponent<BlockBase>().SetBlockRotation(blockInfo.isHalfRotated, blockInfo.rotation);
        g.GetComponent<BlockBase>().SetStartGridPos(blockInfo.startGridPos);
        g.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, blockInfo.rotation));

        Vector2Int blockSize = 
            blockInfo.isHalfRotated ? 
            new Vector2Int(blockData.size.y, blockData.size.x) : 
            blockData.size; 
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
        g.GetComponent<BlockBase>().InitializeBlock(blockInfo.startGridPos, blockInfo.isHalfRotated, blockInfo.rotation, position, isDefault);

        IDeletable deletable = g.GetComponent<IDeletable>();
        if (deletable != null)
        {
            deletable.InitDeletable(blockData.size);
            deletable.SetOnDeleteCallback((target) => {
                if (selectedBlockInfo.type != BlockType.DELETE) {
                    return;
                }

                if (isDefault && DontDestroyObject.gameManager.curMaxCoin - DontDestroyObject.gameManager.curUsedCoin < blockData.price)
                {
                    return;
                }

                coinUsePanel.SetActive(false);

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

                if (isDefault) {
                    DontDestroyObject.gameManager.UseCoin(blockData.price);
                } else {
                    DontDestroyObject.gameManager.RefundCoin(blockData.price);
                }
                //FinishEditMode();
            });

            deletable.SetOnHoverCallback((target) => {
                if (selectedBlockInfo.type != BlockType.DELETE) {
                    return;
                }

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

                            if (i == blockSize.x - 1 && j == 0)
                            {
                                ShowCoinUsePanel(g.GetComponent<Transform>().position, isDefault ? -blockData.price : blockData.price);
                            }
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

                if (selectedBlockInfo.type == BlockType.DELETE) {
                    coinUsePanel.SetActive(false);
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
        BlockData blockData = DontDestroyObject.blockManager.GetBlockData(selectedBlockInfo.type);
        Vector2Int blockSize =
            selectedBlockInfo.isHalfRotated ?
            new Vector2Int(blockData.size.y, blockData.size.x) :
            blockData.size;

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

    public void ResetEditor() {
        foreach (var block in instantiatedBlockObjs) {
            Destroy(block);
        }
        instantiatedBlockObjs = new List<GameObject>();

        SearchPreventedBlocks();
        foreach (BlockInfo b in curMapData.defaultBlocks)
        {
            PlaceBlockObj(b, true);
        }

        DontDestroyObject.gameManager.ResetCoin();
    }

    private void ShowCoinUsePanel(Vector3 targetPos, int price) {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(targetPos);
        Vector2 screenPos = new Vector2(
            viewportPos.x * canvasRect.sizeDelta.x - (canvasRect.sizeDelta.x * 0.5f) + 10,
            viewportPos.y * canvasRect.sizeDelta.y - (canvasRect.sizeDelta.y * 0.5f) - 70
        );

        coinUsePanel.SetActive(true);
        coinUsePanel.GetComponent<RectTransform>().anchoredPosition = screenPos;
        coinUsePanel.GetComponentInChildren<TextMeshProUGUI>().text = (price >= 0 ? "+" : "") + price.ToString();

        if (price < 0 && DontDestroyObject.gameManager.curMaxCoin - DontDestroyObject.gameManager.curUsedCoin < -price)
        {
            coinUsePanel.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1, 0.3f, 0.3f, 1);
        }
        else
        {
            coinUsePanel.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
        }
    }
}

public enum BlockType
{
    NONE = 0,
    DELETE,
    JUMP,
    MOVE_H,
    MOVE_V,
    FALL,
    NORMAL,
    ROTATE,
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