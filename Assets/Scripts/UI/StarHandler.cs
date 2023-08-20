using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class StarHandler : MonoBehaviour
{
    public Sprite onStar;
    public Sprite offStar;

    public void SetOn() {
        GetComponent<Image>().sprite = onStar;
    }

    public void SetOff() {
        GetComponent<Image>().sprite = offStar;
    }
}
