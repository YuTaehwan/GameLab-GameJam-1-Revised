using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlingLine : MonoBehaviour
{
    private LineRenderer lineRend;

    private Vector2 startPos;

    private Vector2 destPos;
    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        startPos = gameObject.transform.parent.position;
        destPos = gameObject.transform.parent.transform.parent.position;
        lineRend.SetPosition(0, startPos);
        lineRend.SetPosition(1, destPos);
    }
}
