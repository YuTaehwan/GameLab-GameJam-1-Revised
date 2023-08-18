using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraControl : MonoBehaviour
{
    public Camera UICamera;

    private float mouseMoveSenstivity = -0.01f;
    private float scrollSensitivity = -1f;
    private float minSize = 2.0f;
    private float maxSize = 10.0f;

    private float initialSize = 5.0f;
    private Vector3 initialPos = new Vector3(0, 0, -10f);

    private Vector3 curMousePos = Vector3.zero;
    private Vector3 prevMousePos = Vector3.zero;

    private Vector3 minXYPos = new Vector3(-15, -15, -10);

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // Scroll Input
        float scroll = Input.mouseScrollDelta.y * scrollSensitivity;

        if (UICamera.orthographicSize <= minSize && scroll < 0)
        {
            UICamera.orthographicSize = minSize;
        }
        else if (UICamera.orthographicSize >= maxSize && scroll > 0)
        {
            UICamera.orthographicSize = maxSize;
        }
        else
        {
            UICamera.orthographicSize += scroll;
        }

        // Click Input
        if (Input.GetMouseButtonDown(1))
        {
            curMousePos = Input.mousePosition;
            prevMousePos = Input.mousePosition;
            Debug.Log(curMousePos + " " + prevMousePos);
        }

        if (Input.GetMouseButton(1))
        {
            curMousePos = Input.mousePosition;
            Vector3 d = curMousePos - prevMousePos;
            transform.position += d * mouseMoveSenstivity;

            prevMousePos = curMousePos;
        }

        if (Input.GetMouseButtonUp(1))
        {
            curMousePos = Vector3.zero;
            prevMousePos = Vector3.zero;
        }
    }

    public void Init()
    {
        UICamera.orthographicSize = initialSize;
        Vector3 pos = GameObject.Find("Player").transform.position;
        pos.z = -10f;
        transform.position = pos;
    }
}
