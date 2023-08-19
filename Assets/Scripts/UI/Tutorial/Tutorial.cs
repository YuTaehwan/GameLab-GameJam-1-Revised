using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject installTutorial;
    [SerializeField] private GameObject deleteTutorial;
    [SerializeField] private GameObject startTutorial;
    
    private Button installButton;
    private Button deleteButton;
    
    private TutorialType tutorialType = TutorialType.START;
    private bool isInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        tutorialType = TutorialType.START;
        isInitialized = false;

        startTutorial.SetActive(false);
        installTutorial.SetActive(false);
        deleteTutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized && DontDestroyObject.gameManager.playMode == PlayMode.STAGE_SHOW) {
            isInitialized = true;
            
            installButton = DontDestroyObject.gameManager.gridSelectorObj.blockButtons[0];
            deleteButton = DontDestroyObject.gameManager.gridSelectorObj.blockButtons[1];
        }

        if (isInitialized) {
            if (tutorialType != TutorialType.START && startTutorial.activeSelf == true) {
                startTutorial.SetActive(false);
            }

            if (DontDestroyObject.gameManager.playMode == PlayMode.EDIT) {
                if (tutorialType == TutorialType.INSTALL)
                {
                    if (installTutorial.activeSelf == false)
                    {
                        installTutorial.SetActive(true);
                        installButton.interactable = true;
                        deleteTutorial.SetActive(false);
                        deleteButton.interactable = false;
                    }
                }
                else if (tutorialType == TutorialType.DELETE)
                {
                    if (deleteTutorial.activeSelf == false)
                    {
                        installTutorial.SetActive(false);
                        installButton.interactable = false;
                        deleteTutorial.SetActive(true);
                        deleteButton.interactable = true;
                    }
                } 
            } else if (DontDestroyObject.gameManager.playMode == PlayMode.PLAY) {
                if (tutorialType == TutorialType.START) {
                    startTutorial.SetActive(true);
                }

                installTutorial.SetActive(false);
                installButton.interactable = false;
                deleteTutorial.SetActive(false);
                deleteButton.interactable = false;
            }
        }
    }

    public void SetTutorialType(TutorialType type)
    {
        tutorialType = type;

        if (type == TutorialType.INSTALL)
        {
            installTutorial.SetActive(true);
            installButton.interactable = true;
            deleteTutorial.SetActive(false);
            deleteButton.interactable = false;
        }
        else if (type == TutorialType.DELETE)
        {
            installTutorial.SetActive(false);
            installButton.interactable = false;
            deleteTutorial.SetActive(true);
            deleteButton.interactable = true;
        }
    }

    public enum TutorialType
    {
        START,
        INSTALL,
        DELETE
    }
}