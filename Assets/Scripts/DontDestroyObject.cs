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

    private static GameManager _gameManager = new GameManager();
    public static GameManager gameManager
    {
        get
        {
            return _gameManager;
        }
    }

    private static BlockManager _blockManager = new BlockManager();
    public static BlockManager blockManager
    {
        get
        {
            return _blockManager;
        }
    }

    private static GameFileManager _gameFileManager = new GameFileManager();
    public static GameFileManager gameFileManager
    {
        get
        {
            return _gameFileManager;
        }
    }

    private bool shoudSceneLoad;
    private int targetStage;
    public bool gameInitialized = false;

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

        gameInitialized = false;
        InitOtherManagers();
        shoudSceneLoad = false;
    }

    public void Update()
    {
        if (shoudSceneLoad)
        {
            shoudSceneLoad = false;
            StartCoroutine(_gameManager.LoadScene(targetStage));
        }

        if (_gameManager.playMode == PlayMode.STAGE_ENTER) {
            StartCoroutine(_gameManager.ShowStage());
        }
    }

    private static void SetupInstance()
    {
        instance = FindObjectOfType<DontDestroyObject>();
    }

    private void InitOtherManagers()
    {
        _gameManager.Init();
        _blockManager.Init();

        gameInitialized = true;
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

    public void Sandbox() {
        _gameManager.Sandbox();
    }

    public void LoadScene(int stageNum)
    {
        targetStage = stageNum;
        shoudSceneLoad = true;
    }
}