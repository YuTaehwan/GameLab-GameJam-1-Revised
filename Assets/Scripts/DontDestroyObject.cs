using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    private static DontDestroyObject instance = null;
    public static DontDestroyObject Instance
    {
        get
        {
            if(instance == null)
            {
                SetupInstance();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitOtherManagers();
        shoudSceneLoad = false;
    }

    private void InitOtherManagers()
    {
        _gameManager.Init();
    }

    private static void SetupInstance()
    {
        instance = FindObjectOfType<DontDestroyObject>();
    }

    private static GameManager _gameManager = new GameManager();
    public static GameManager gameManager
    {
        get
        {
            return _gameManager;
        }
    }

    public bool IsEditMode()
    {
        return _gameManager.playMode == PlayMode.EDIT;
    }

    public bool IsPlayMode()
    {
        return _gameManager.playMode == PlayMode.PLAY;
    }

    public void GoLobby()
    {
        _gameManager.GoLobby();
    }

    public void LoadScene(int stageNum)
    {
        targetStage = stageNum;
        shoudSceneLoad = true;
    }

    private bool shoudSceneLoad;
    private int targetStage;

    public void Update()
    {
        if (shoudSceneLoad)
        {
            shoudSceneLoad = false;
            StartCoroutine(_gameManager.LoadScene(targetStage));
        }

        if (_gameManager.playMode == PlayMode.STAGE_ENTER) {
            _gameManager.playMode = PlayMode.STAGE_SHOW;
            StartCoroutine(_gameManager.ShowStage());
        }
    }
}