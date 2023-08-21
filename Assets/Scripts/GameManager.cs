using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    public const float STAGE_SHOW_TIME = 1f;
    public const float SHOW_FLAG_TIME = .8f;
    public const float SHOW_CHARACTER_TIME = .5f;

    private GridSelector gridSelector;
    public GridSelector gridSelectorObj => gridSelector;

    private PlayMode _playMode;
    public PlayMode playMode => _playMode;
    public SceneLoadState loadState;

    private CameraFollow playCamera;
    private UICameraControl editCamera;
    private GameObject selectionUI;

    private Action OnExitEditModeCallbacks;
    public Action<int, int> OnGiveStarCallbacks;

    private int deathCount;

    public int curStage;
    public int curMaxCoin;
    public int curUsedCoin;

    public List<StageFileData> stageDataList = new List<StageFileData>();
    public int allStarCount = 0;
    public int levelCleared = 0;

    public void Init()
    {
        _playMode = PlayMode.LOBBY;
        loadState = SceneLoadState.ENDLOAD;

        SaveFileData saveData = DontDestroyObject.gameFileManager.LoadSaveData();
        stageDataList = saveData.stageDatas;
        allStarCount = saveData.allStarCount;
        levelCleared = saveData.levelCleared;
    }

    public void SaveGame() {
        SaveFileData saveData = new SaveFileData
        {
            allStarCount = allStarCount,
            stageDatas = stageDataList,
            levelCleared = levelCleared
        };

        DontDestroyObject.gameFileManager.SaveGameData(saveData);
    }

    public StageFileData GetStageData(int stageNum)
    {
        foreach (var stageData in stageDataList)
        {
            if (stageData.stageNum == stageNum)
            {
                return stageData;
            }
        }

        return new StageFileData();
    }
    
    public void SetStageData(StageFileData stageData)
    {
        for (int i = 0; i < stageDataList.Count; i++)
        {
            if (stageDataList[i].stageNum == stageData.stageNum)
            {
                stageDataList[i] = stageData;
                return;
            }
        }
    }

    public void StartStage(int stageNum)
    {
        curStage = stageNum;
        curMaxCoin = GetStageData(stageNum).initCoin;
        curUsedCoin = 0;

        gridSelector = UnityEngine.Object.FindObjectOfType<GridSelector>();
        gridSelector.InitSelectionUI(stageNum);

        playCamera = UnityEngine.Object.FindObjectOfType<CameraFollow>();
        editCamera = UnityEngine.Object.FindObjectOfType<UICameraControl>();
        selectionUI = GameObject.Find("SelectionUI");

        deathCount = -1;

        selectionUI.SetActive(false);
        _playMode = PlayMode.STAGE_ENTER;
    }

    public void EditMode()
    {
        _playMode = PlayMode.EDIT;

        playCamera.enabled = false;
        editCamera.enabled = true;
        editCamera.Init();
        selectionUI.SetActive(true);

        deathCount += 1;

        if (deathCount > 0)
        {
            selectionUI.GetComponentInChildren<ChangeScore>().PlayScore(deathCount - 1, deathCount);
        }
    }
    
    public void ExitEditMode()
    {
        _playMode = PlayMode.PLAY;

        playCamera.enabled = true;
        editCamera.Init();
        editCamera.enabled = false;
        selectionUI.SetActive(false);

        OnExitEditModeCallbacks?.Invoke();
    }

    public void GoLobby()
    {
        _playMode = PlayMode.LOBBY;
        SceneManager.LoadScene("Lobby");
    }

    public IEnumerator LoadScene(int stageNum)
    {
        loadState = SceneLoadState.LOAD;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Stage" + stageNum, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("asdf");
        yield return new WaitForEndOfFrame();
        loadState = SceneLoadState.ENDLOAD;
        StartStage(stageNum);
    }

    public IEnumerator ShowStage() {
        _playMode = PlayMode.STAGE_SHOW;

        playCamera.transform.position = GameObject.Find("Flag").transform.position;
        yield return new WaitForSeconds(SHOW_FLAG_TIME);
        Vector3 flagPos = GameObject.Find("Flag").transform.position;
        Vector3 playerPos = GameObject.Find("Player").transform.position;
        playCamera.SetShowStage(flagPos);
        yield return new WaitUntil(() => {
            return 
                playCamera.transform.position.x <= playerPos.x + 0.01f &&
                playCamera.transform.position.x >= playerPos.x - 0.01f &&
                playCamera.transform.position.y <= playerPos.y + 0.01f &&
                playCamera.transform.position.y >= playerPos.y - 0.01f;
        });
        playCamera.FinishShowStage();
        yield return new WaitForSeconds(SHOW_CHARACTER_TIME);
        
        EditMode();
        if (curStage == 1) {
            ExitEditMode();
        }
    }

    public void RegisterExitEditorCallback(Action callback)
    {
        OnExitEditModeCallbacks += callback;
    }

    public void GiveStar() {
        StageFileData stageData = GetStageData(curStage);

        int giveStar;
        if (curMaxCoin - curUsedCoin < stageData.starBasis[0]) {
            giveStar = 1;
        } else if (curMaxCoin - curUsedCoin < stageData.starBasis[1]) {
            giveStar = 2;
        } else {
            giveStar = 3;
        }

        int originalStar = stageData.stageStars;
        if (originalStar == 0)
            levelCleared = curStage;

        if (originalStar < giveStar)
            stageData.stageStars = giveStar;

        OnGiveStarCallbacks?.Invoke(originalStar, giveStar);
        
        SetStageData(stageData);
        if (giveStar > originalStar)
            allStarCount += giveStar - originalStar;
        
        SaveGame();
    }

    public void ResetCoin() {
        curUsedCoin = 0;
    }

    public void UseCoin(int coin) {
        curUsedCoin += coin;
    }

    public void RefundCoin(int coin) {
        curUsedCoin -= coin;
    }
}

public enum PlayMode
{
    LOBBY,
    PLAY,
    EDIT,
    STAGE_ENTER,
    STAGE_SHOW
}

public enum SceneLoadState
{
    LOAD,
    ENDLOAD
}