using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfoPanel : MonoBehaviour
{
    bool hasInit = false;
    public GameObject blockInfoButtonPrefab;
    public RectTransform BtnPanelRect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasInit && DontDestroyObject.Instance.gameInitialized) {
            hasInit = true;
            foreach (BlockData bd in DontDestroyObject.blockManager.allBlockData) {
                if (bd.prefab.GetComponent<IInstallable>() != null && bd.prefab.GetComponent<IDeletable>() != null) {
                    GameObject btn = Instantiate(blockInfoButtonPrefab, BtnPanelRect);
                    btn.GetComponent<BlockInfoButton>().Init(bd);
                }
            } 
        }    
    }
}
