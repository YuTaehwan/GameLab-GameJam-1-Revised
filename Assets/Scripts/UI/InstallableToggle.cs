using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstallableToggle : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI text;
    public BlockData blockData;
    public BlockInfo blockInfo;

    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(BlockData blockData, BlockInfo blockInfo) {
        this.blockInfo = blockInfo;
        this.blockData = blockData;
        text.text = blockData.korean_name;
    }
}
