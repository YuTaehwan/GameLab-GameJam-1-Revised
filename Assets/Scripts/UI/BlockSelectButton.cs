using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

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

    public void Init(BlockInfo blockInfo, UnityAction onClick, GameObject deleteButtonImagePrefab)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(onClick);

        if (blockInfo.type != BlockType.DELETE)
        {
            blockData = DontDestroyObject.blockManager.GetBlockData(blockInfo.type);
            transform.Find("BlockName").GetComponent<TextMeshProUGUI>().text = blockData.korean_name;

            GameObject bimg = Instantiate(blockData.btnImgPrefab, transform);
            bimg.GetComponent<RectTransform>().SetAsFirstSibling();
            bimg.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, blockInfo.rotation));
        } else
        {
            transform.Find("BlockName").GetComponent<TextMeshProUGUI>().text = "블록 삭제";

            GameObject bimg = Instantiate(deleteButtonImagePrefab, transform);
            bimg.GetComponent<RectTransform>().SetAsFirstSibling();
        }

        hasInit = true;
    }
}
