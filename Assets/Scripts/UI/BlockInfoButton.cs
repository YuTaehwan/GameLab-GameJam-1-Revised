using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockInfoButton : MonoBehaviour
{
    bool hasInit = false;
    public BlockData blockData;
    public TextMeshProUGUI blockPriceText;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hasInit && DontDestroyObject.Instance.gameInitialized) {
            if (DontDestroyObject.gameManager.allStarCount >= blockData.needStar)
            {
                blockPriceText.text = "해금!";
                button.interactable = true;
            }
            else
            {
                blockPriceText.text = "필요한 별: " + blockData.needStar.ToString();
                button.interactable = false;
            }
        }
    }

    public void Init(BlockData blockData)
    {
        hasInit = true;
        this.blockData = blockData;
        transform.Find("BlockName").GetComponent<TMPro.TextMeshProUGUI>().text = blockData.korean_name;
        Image image = GetComponent<Image>();
        image.sprite = blockData.btnImg;
    }
}
