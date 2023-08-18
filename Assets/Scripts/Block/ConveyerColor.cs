using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerColor : MonoBehaviour
{
    SpriteRenderer objectColor;

    // Start is called before the first frame update
    void Start()
    {
        objectColor = GetComponent<SpriteRenderer>();
        InvokeRepeating("ChangeColor", 0.28f, 0.28f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeColor()
    {
        if (objectColor.color == new Color(0.2264151f, 0.1313635f, 0.1313635f, 1f))
        {
            objectColor.color = new Color(0.8516554f, 0.8867924f, 0.8491456f, 1f);
        }
        else if (objectColor.color == new Color(0.8516554f, 0.8867924f, 0.8491456f, 1f))
        {
            objectColor.color = new Color(0.2264151f, 0.1313635f, 0.1313635f, 1f);
        }
    }
}
