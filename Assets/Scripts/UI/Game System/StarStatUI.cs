using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StarStatUI : MonoBehaviour
{
    public TextMeshProUGUI starStatText;

    private bool isShowStar = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        DontDestroyObject.gameManager.OnGiveStarCallbacks += ChangeStarNum;
    }

    void OnDisable() {
        DontDestroyObject.gameManager.OnGiveStarCallbacks -= ChangeStarNum;
    }

    // Update is called once per frame
    void Update()
    {
        if (DontDestroyObject.Instance.gameInitialized && !isShowStar) {
            starStatText.text = DontDestroyObject.gameManager.allStarCount.ToString();
        }
    }

    void ChangeStarNum(int originalStar, int newStar) {
        isShowStar = true;
        Debug.Log("ChangeStarNum");
        StartCoroutine(ShowStarCoroutine());
    }

    IEnumerator ShowStarCoroutine() {
        yield return new WaitForSeconds(1.3f);
        
        isShowStar = false;
    }
}
