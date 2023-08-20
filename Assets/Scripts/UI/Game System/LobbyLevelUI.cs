using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyLevelUI : MonoBehaviour
{
    public GameObject levelButtonPanel;
    public GameObject levelButtonPrefab;

    private bool hasInit = false;

    // Start is called before the first frame update
    void Start()
    {
        hasInit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasInit && DontDestroyObject.Instance.gameInitialized)
        {
            hasInit = true;
            
            foreach (var level in DontDestroyObject.gameManager.stageDataList)
            {
                GameObject levelButton = Instantiate(levelButtonPrefab, levelButtonPanel.transform);
                levelButton.GetComponent<GetButtonTextAndOpenScene>().SetLevel(level.stageNum);
            }
        }
    }
}
