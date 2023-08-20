using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystemUI : MonoBehaviour
{
    [SerializeField] private GameObject goLobbyButtonObj;
    [SerializeField] private GameObject editButtonObj;
    [SerializeField] private GameObject statPanelObj;
    [SerializeField] private GameObject blockInfoButtonObj;

    [SerializeField] private GameObject blockInfoPanelObj;

    private PlayMode curPlayMode = PlayMode.LOBBY;

    // Start is called before the first frame update
    void Start()
    {
        goLobbyButtonObj.SetActive(false);
        editButtonObj.SetActive(false);
        statPanelObj.SetActive(true);
        blockInfoButtonObj.SetActive(true);
        blockInfoPanelObj.SetActive(false);

        curPlayMode = PlayMode.LOBBY;
    }

    // Update is called once per frame
    void Update()
    {
        if (curPlayMode != DontDestroyObject.gameManager.playMode) {
            curPlayMode = DontDestroyObject.gameManager.playMode;

            if (curPlayMode == PlayMode.LOBBY) {
                goLobbyButtonObj.SetActive(false);
                editButtonObj.SetActive(false);
                statPanelObj.SetActive(true);
                blockInfoButtonObj.SetActive(true);
            } else if (curPlayMode == PlayMode.EDIT) {
                goLobbyButtonObj.SetActive(true);
                editButtonObj.SetActive(false);
                statPanelObj.SetActive(true);
                blockInfoButtonObj.SetActive(true);
            } else if (curPlayMode == PlayMode.PLAY) {
                goLobbyButtonObj.SetActive(true);
                editButtonObj.SetActive(true);
                statPanelObj.SetActive(true);
                blockInfoButtonObj.SetActive(true);
            }
        }
    }

    public void GoLobby() {
        DontDestroyObject.gameManager.GoLobby();
    }

    public void EditMode() {
        GameObject.Find("Player").GetComponent<PlayerMovement>().Die();
    }

    public void ShowBlockInfo() {
        blockInfoPanelObj.SetActive(true);
    }

    public void HidBlockInfo() {
        blockInfoPanelObj.SetActive(false);
    }
}
