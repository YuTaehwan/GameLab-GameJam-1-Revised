using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using Unity.VisualScripting;

public class Sandbox : MonoBehaviour
{
    public GameObject selectGridPrefab;
    public GameObject unableGridPrefab;

    public GameObject selectButtonPrefab;
    public GameObject deleteButtonImagePrefab;
    public GameObject tileAButtonImagePrefab;
    public GameObject tileBButtonImagePrefab;

    public Tile tileA;
    public Tile tileB;

    public Tilemap selectionTilemap;
    public RectTransform selectionButtonPanel;

    private Vector3Int hoverPos;
    private Vector3Int prevHoverPos;
    private BlockInfo selectedBlockInfo;

    private List<GameObject> instantiatedSelectGrids;
    private List<GameObject> instantiatedUnableGrids;
    private List<GameObject> instantiatedBlockObjs;
    private GameObject instantiatedDeleteGrid;

    private Tilemap mainTilemap;
    private HashSet<Vector2Int> preventedBlocksSet;
    private Vector2Int minTileBound = new Vector2Int(-30, -30);
    private Vector2Int maxTileBound = new Vector2Int(120, 60);

    private MapLoader mapLoader;
    private MapData curMapData;

    private bool hasInitialized = false;
    private BlockInfo deleteButtonInfo;
    private BlockInfo nonBlockInfo;
    private BlockInfo tileABlockInfo;
    private BlockInfo tileBBlockInfo;
    private BlockInfo playerBlockInfo;
    private BlockInfo flagBlockInfo;

    public GameObject playerPrefab;
    public GameObject flagPrefab;
    public GameObject playerImgPrefab;
    public GameObject flagImgPrefab;

    private List<Button> _blockButtons = new List<Button>();
    public List<Button> blockButtons { get => _blockButtons; }

    private bool isFlagInstalled;
    private bool isPlayerInstalled;

    private float[] rotation = new float[] { 0, 90, 180, 270 };
    private int rotateIdx = 0;

    public GameObject installbleTogglePanel;
    public GameObject installableTogglePrefab;
    public List<InstallableToggle> installableToggles = new List<InstallableToggle>();

    private void toggleRotateIdx () {
        rotateIdx = (rotateIdx + 1) % 4;
        prevHoverPos = new Vector3Int(-10000, -100, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        hasInitialized = false;
        instantiatedDeleteGrid = Instantiate(unableGridPrefab);
        instantiatedDeleteGrid.SetActive(false);

        isPlayerInstalled = false;
        isFlagInstalled = false;

        InitSelectionUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasInitialized) {
            if (Input.GetKeyDown(KeyCode.O)) {
                toggleRotateIdx();
            }

            if (selectedBlockInfo.type != BlockType.DELETE) {
                instantiatedDeleteGrid.SetActive(false);
            }

            if (Input.GetMouseButtonDown(1) && selectedBlockInfo.type != BlockType.DELETE)
            {
                selectedBlockInfo = nonBlockInfo;
            }

            hoverPos = selectionTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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

                bool isValidToPlace = IsValidPositionToPlace(new Vector2Int(hoverPos.x, hoverPos.y));

                if (hoverPos != prevHoverPos)
                {
                    foreach (GameObject g in instantiatedSelectGrids)
                    {
                        Destroy(g);
                    }

                    instantiatedSelectGrids = new List<GameObject>();

                    Vector3 targetPos = selectionTilemap.GetCellCenterWorld(hoverPos);
                    Vector2Int blockSize;
                    if (selectedBlockInfo.type != BlockType.TILE_A && selectedBlockInfo.type != BlockType.TILE_B)
                    {
                        BlockData blockData;
                        if (selectedBlockInfo.type == BlockType.PLAYER || selectedBlockInfo.type == BlockType.FLAG) {
                            blockData = new BlockData {
                                type = selectedBlockInfo.type,
                                size = new Vector2Int(1, 1),
                                prefab = selectedBlockInfo.type == BlockType.PLAYER ? playerPrefab : flagPrefab,
                            };
                        } else {
                            blockData = DontDestroyObject.blockManager.GetBlockData(selectedBlockInfo.type);
                        }
                        blockSize = 
                            (rotateIdx == 1 || rotateIdx == 3) ? 
                            new Vector2Int(blockData.size.y, blockData.size.x) : 
                            blockData.size;
                    } else {
                        blockSize = new Vector2Int(1, 1);
                    }
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
                }

                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && isValidToPlace)
                {
                    if (selectedBlockInfo.type != BlockType.TILE_A && selectedBlockInfo.type != BlockType.TILE_B) {
                        Vector3 targetPos = selectionTilemap.GetCellCenterWorld(hoverPos);
                    
                        Vector2Int blockSize;
                        BlockData blockData;

                        if (selectedBlockInfo.type == BlockType.PLAYER || selectedBlockInfo.type == BlockType.FLAG) {
                            blockData = new BlockData {
                                type = selectedBlockInfo.type,
                                size = new Vector2Int(1, 1),
                                prefab = selectedBlockInfo.type == BlockType.PLAYER ? playerPrefab : flagPrefab,
                            };
                            
                            if (selectedBlockInfo.type == BlockType.PLAYER) {
                                if (isPlayerInstalled) {
                                    return;
                                } else {
                                    isPlayerInstalled = true;
                                }
                                isPlayerInstalled = true;
                            } else if (selectedBlockInfo.type == BlockType.FLAG) {
                                if (isFlagInstalled) {
                                    return;
                                } else {
                                    isFlagInstalled = true;
                                }
                            }
                        } else {
                            blockData = DontDestroyObject.blockManager.GetBlockData(selectedBlockInfo.type);
                        }

                        blockSize = 
                            (rotateIdx == 1 || rotateIdx == 3) ? 
                            new Vector2Int(blockData.size.y, blockData.size.x) : 
                            blockData.size;
                        
                        selectedBlockInfo.startGridPos = selectionTilemap.WorldToCell(GetStartGridPos(targetPos, blockSize));
                        PlaceBlockObj(selectedBlockInfo, false);
                    } else {
                        mainTilemap.SetTile(hoverPos, selectedBlockInfo.type == BlockType.TILE_A ? tileA : tileB);
                        preventedBlocksSet.Add(new Vector2Int(hoverPos.x, hoverPos.y));
                    }
                }
            }
            else
            {
                InitSelectionUIObjs();
                
                if (IsInMainMap(hoverPos)) {
                    instantiatedDeleteGrid.SetActive(true);
                    instantiatedDeleteGrid.GetComponent<Transform>().position = mainTilemap.GetCellCenterWorld(hoverPos);

                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        mainTilemap.SetTile(hoverPos, null);
                        preventedBlocksSet.Remove(new Vector2Int(hoverPos.x, hoverPos.y));
                    }
                } else {
                    instantiatedDeleteGrid.SetActive(false);
                }
            }
            prevHoverPos = hoverPos;
        }
    }

    public bool IsInMainMap(Vector3Int pos) {
        return mainTilemap.HasTile(pos);
    }
    
    public void InitSelectionUI()
    {
        mapLoader = GetComponent<MapLoader>();
        //curMapData = mapLoader.LoadMap(level - 1);

        foreach (BlockData bd in DontDestroyObject.blockManager.allBlockData)
        {
            BlockInfo b = new BlockInfo
            {
                type = bd.type,
                isHalfRotated = false,
                rotation = 0
            };

            AddButton(b);

            GameObject toggle = Instantiate(installableTogglePrefab, installbleTogglePanel.transform);
            toggle.GetComponent<InstallableToggle>().Init(bd, b);
            installableToggles.Add(toggle.GetComponent<InstallableToggle>());
        }

        tileABlockInfo = new BlockInfo {
            type = BlockType.TILE_A,
        };
        AddButton(tileABlockInfo);

        tileBBlockInfo = new BlockInfo {
            type = BlockType.TILE_B
        };
        AddButton(tileBBlockInfo);

        playerBlockInfo = new BlockInfo
        {
            type = BlockType.PLAYER
        };
        AddButton(playerBlockInfo);

        flagBlockInfo = new BlockInfo
        {
            type = BlockType.FLAG
        };
        AddButton(flagBlockInfo);

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

    public void LoadMap() {
        curMapData = mapLoader.LoadMap();

        mainTilemap = GameObject.Find("GroundTile").GetComponentInChildren<Tilemap>();
        for (int i = minTileBound.x; i < maxTileBound.x; i++)
        {
            for (int j = minTileBound.y; j < maxTileBound.y; j++)
            {
                if (curMapData.mainMaps.Contains(new Vector3Int(i, j, 0))) {
                    mainTilemap.SetTile(new Vector3Int(i, j, 0), tileA);
                } else if (curMapData.mainMaps.Contains(new Vector3Int(i, j, 1))) {
                    mainTilemap.SetTile(new Vector3Int(i, j, 0), tileB);
                } else {
                    mainTilemap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }
        }

        instantiatedSelectGrids = new List<GameObject>();
        instantiatedUnableGrids = new List<GameObject>();
        instantiatedBlockObjs = new List<GameObject>();

        playerBlockInfo.startGridPos = new Vector3Int(curMapData.playerPos.x, curMapData.playerPos.y, 0);
        PlaceBlockObj(playerBlockInfo, true);
        flagBlockInfo.startGridPos = new Vector3Int(curMapData.flagPos.x, curMapData.flagPos.y, 0);
        PlaceBlockObj(flagBlockInfo, true);

        ResetEditor();

        foreach (BlockInfo b in curMapData.defaultBlocks)
        {
            PlaceBlockObj(b, true);
        }
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
        GameObject b = Instantiate(selectButtonPrefab, selectionButtonPanel);
        
        BlockInfo blockInfo1_ = blockInfo;

        b.GetComponent<BlockSelectButton>().Init(blockInfo, () => {
            SetBlock(blockInfo1_);
        }, new GameObject[] {deleteButtonImagePrefab, tileAButtonImagePrefab, tileBButtonImagePrefab, playerImgPrefab, flagImgPrefab});

        blockButtons.Add(b.GetComponent<Button>());
    }

    private void PlaceBlockObj(BlockInfo blockInfo, bool isDefault)
    {
        BlockData blockData;
        if (blockInfo.type == BlockType.PLAYER || blockInfo.type == BlockType.FLAG) {
            blockData = new BlockData {
                type = blockInfo.type,
                size = new Vector2Int(1, 1),
                prefab = blockInfo.type == BlockType.PLAYER ? playerPrefab : flagPrefab,
            };
        } else {
            blockData = DontDestroyObject.blockManager.GetBlockData(blockInfo.type);
        }
        
        GameObject g = Instantiate(blockData.prefab);
        g.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, isDefault ? blockInfo.rotation : rotation[rotateIdx]));

        Vector2Int blockSize = 
            (isDefault ? blockInfo.isHalfRotated : (rotateIdx == 1 || rotateIdx == 3)) ? 
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
        g.GetComponent<BlockBase>().InitializeBlock(blockInfo.startGridPos, isDefault ? blockInfo.isHalfRotated : rotateIdx is 1 or 3, isDefault ? blockInfo.rotation : (int)rotation[rotateIdx], position, isDefault);
        g.GetComponent<BlockBase>().type = blockInfo.type;

        IDeletable deletable = g.GetComponent<IDeletable>();
        if (deletable != null)
        {
            deletable.InitDeletable(blockData.size);
            deletable.SetOnDeleteCallback((target) => {
                if (selectedBlockInfo.type != BlockType.DELETE) {
                    return;
                }

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

                if (blockData.type == BlockType.PLAYER) {
                    isPlayerInstalled = false;
                } else if (blockData.type == BlockType.FLAG) {
                    isFlagInstalled = false;
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
    }

    private bool IsValidPositionToPlace(Vector2Int hoverPosition)
    {
        Vector2Int blockSize;
        if (selectedBlockInfo.type != BlockType.TILE_A && selectedBlockInfo.type != BlockType.TILE_B
            && selectedBlockInfo.type != BlockType.PLAYER && selectedBlockInfo.type != BlockType.FLAG)
        {
            BlockData blockData = DontDestroyObject.blockManager.GetBlockData(selectedBlockInfo.type);
            blockSize = 
                selectedBlockInfo.isHalfRotated ? 
                new Vector2Int(blockData.size.y, blockData.size.x) : 
                blockData.size;
        } else {
            blockSize = new Vector2Int(1, 1);
        }

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

        DontDestroyObject.gameManager.ResetCoin();
    }

    public void ExitSandbox() {
        DontDestroyObject.gameManager.GoLobby();
    }

    public void SaveMap() {
        MapData saveMapData = new MapData();
        saveMapData.defaultBlocks = new List<BlockInfo>();
        saveMapData.installableBlocks = new List<BlockInfo>();
        saveMapData.mainMaps = new List<Vector3Int>();
        
        foreach (GameObject bgo in instantiatedBlockObjs) {
            BlockBase bb = bgo.GetComponent<BlockBase>();
            if (bb.type != BlockType.PLAYER && bb.type != BlockType.FLAG) {
                BlockInfo blockInfo = new BlockInfo {
                    type = bb.type,
                    isHalfRotated = bb.isHalfRotated,
                    rotation = bb.rotation,
                    startGridPos = bb.startGridPos
                };

                saveMapData.defaultBlocks.Add(blockInfo);
            } else {
                if (bb.type == BlockType.PLAYER) {
                    saveMapData.playerPos = new Vector2Int(bb.startGridPos.x, bb.startGridPos.y);
                } else if (bb.type == BlockType.FLAG) {
                    saveMapData.flagPos = new Vector2Int(bb.startGridPos.x, bb.startGridPos.y);
                }
            }
        }

        foreach (InstallableToggle toggle in installableToggles) {
            if (toggle.toggle.isOn) {
                saveMapData.installableBlocks.Add(toggle.blockInfo);
            }
        }

        for (int i = minTileBound.x; i < maxTileBound.x; i++)
        {
            for (int j = minTileBound.y; j < maxTileBound.y; j++)
            {
                if (mainTilemap.HasTile(new Vector3Int(i, j, 0))) {
                    if (mainTilemap.GetTile(new Vector3Int(i, j, 0)) == tileA)
                    {
                        saveMapData.mainMaps.Add(new Vector3Int(i, j, 0));
                    } else {
                        saveMapData.mainMaps.Add(new Vector3Int(i, j, 1));
                    }
                }
            }
        }

        mapLoader.SaveMap(saveMapData);
    }
}