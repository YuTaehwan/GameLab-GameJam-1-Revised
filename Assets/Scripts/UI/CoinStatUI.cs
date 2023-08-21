using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinStatUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DontDestroyObject.Instance.gameInitialized && DontDestroyObject.gameManager.playMode == PlayMode.EDIT) {
            coinText.text = (DontDestroyObject.gameManager.curMaxCoin - DontDestroyObject.gameManager.curUsedCoin).ToString();
        }
    }
}
