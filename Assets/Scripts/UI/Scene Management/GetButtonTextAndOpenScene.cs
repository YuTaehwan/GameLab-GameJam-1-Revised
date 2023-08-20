using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GetButtonTextAndOpenScene : MonoBehaviour
{
    public int level;
    public TextMeshProUGUI buttonText;
    public Button levelButton;

    public StarHandler[] starHandlers = new StarHandler[3];

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetLevel(int level) {
        this.level = level;
        buttonText.text = level.ToString();
    }
    
    public void OpenScene()
    {
        DontDestroyObject.Instance.LoadScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        if (DontDestroyObject.Instance.gameInitialized) {
            if (DontDestroyObject.gameManager.GetStageData(level).stageStars == 0) {
                foreach(var starHandler in starHandlers) {
                    starHandler.SetOff();
                }
            } else {
                for (int i = 0; i < DontDestroyObject.gameManager.GetStageData(level).stageStars; i++) {
                    starHandlers[i].SetOn();
                }
                for (int i = DontDestroyObject.gameManager.GetStageData(level).stageStars; i < 3; i++) {
                    starHandlers[i].SetOff();
                }
            }

            if (level != 1 && DontDestroyObject.gameManager.levelCleared + 1 < level) {
                levelButton.interactable = false;
            } else {
                levelButton.interactable = true;
            }
        }
    }
}
