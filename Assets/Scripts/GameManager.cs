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

    public PlayMode playMode;
    public SceneLoadState loadState;

    private CameraFollow playCamera;
    private UICameraControl editCamera;
    private GameObject selectionUI;

    private Action OnExitEditModeCallbacks;

    private int deathCount;

    public int curStage;

    public void StartStage(int stageNum)
    {
        curStage = stageNum;

        gridSelector = UnityEngine.Object.FindObjectOfType<GridSelector>();
        gridSelector.InitSelectionUI(stageNum);

        playCamera = UnityEngine.Object.FindObjectOfType<CameraFollow>();
        editCamera = UnityEngine.Object.FindObjectOfType<UICameraControl>();
        selectionUI = GameObject.Find("SelectionUI");

        deathCount = -1;

        selectionUI.SetActive(false);
        playMode = PlayMode.STAGE_ENTER;
    }

    public void EditMode()
    {
        playMode = PlayMode.EDIT;

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
        playMode = PlayMode.PLAY;

        playCamera.enabled = true;
        editCamera.Init();
        editCamera.enabled = false;
        selectionUI.SetActive(false);

        OnExitEditModeCallbacks?.Invoke();
    }

    public void Init()
    {
        playMode = PlayMode.LOBBY;
        loadState = SceneLoadState.ENDLOAD;
    }

    public void GoLobby()
    {
        playMode = PlayMode.LOBBY;
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
        GameObject lobbyButton = GameObject.Find("GoToLobby");
        lobbyButton.SetActive(false);

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

        lobbyButton.SetActive(true);
        EditMode();
        if (curStage == 0) {
            ExitEditMode();
        }
    }

    public void RegisterExitEditorCallback(Action callback)
    {
        OnExitEditModeCallbacks += callback;
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