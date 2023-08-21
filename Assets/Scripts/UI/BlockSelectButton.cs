using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class BlockSelectButton : MonoBehaviour
{
    bool hasInit = false;
    BlockData blockData;

    public GameObject lockPanel;
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        lockPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   
        if (DontDestroyObject.gameManager.playMode == PlayMode.SANDBOX) {
            lockPanel.SetActive(false);
            button.interactable = true;
        } else {
            if (hasInit && blockData.type != BlockType.NONE && blockData.type != BlockType.DELETE) {
                if (blockData.needStar <= DontDestroyObject.gameManager.allStarCount)
                {
                    lockPanel.SetActive(false);
                    button.interactable = true;
                }
                else
                {
                    lockPanel.SetActive(true);
                    button.interactable = false;
                }
            }
        }
    }

    public void Init(BlockInfo blockInfo, UnityAction onClick, GameObject[] otherBtnImgPrefabs)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(onClick);

        if (blockInfo.type != BlockType.DELETE && blockInfo.type != BlockType.TILE_A && blockInfo.type != BlockType.TILE_B && blockInfo.type != BlockType.PLAYER && blockInfo.type != BlockType.FLAG)
        {
            blockData = DontDestroyObject.blockManager.GetBlockData(blockInfo.type);
            transform.Find("BlockName").GetComponent<TextMeshProUGUI>().text = blockData.korean_name;

            GameObject bimg = Instantiate(blockData.btnImgPrefab, transform);
            bimg.GetComponent<RectTransform>().SetAsFirstSibling();
            bimg.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, blockInfo.rotation));
        } else if (blockInfo.type == BlockType.DELETE) {
            SetButton("블록 삭제", otherBtnImgPrefabs[0]);
        } else if (blockInfo.type == BlockType.TILE_A) {
            SetButton("타일 A", otherBtnImgPrefabs[1]);
        } else if (blockInfo.type == BlockType.TILE_B) {
            SetButton("타일 B", otherBtnImgPrefabs[2]);
        } else if (blockInfo.type == BlockType.PLAYER) {
            SetButton("플레이어", otherBtnImgPrefabs[3]);
        } else if (blockInfo.type == BlockType.FLAG) {
            SetButton("깃발", otherBtnImgPrefabs[4]);
        }

        hasInit = true;
    }

    private void SetButton(string name, GameObject img) {
        transform.Find("BlockName").GetComponent<TextMeshProUGUI>().text = name;

        GameObject bimg = Instantiate(img, transform);
        bimg.GetComponent<RectTransform>().SetAsFirstSibling();
    }
}
