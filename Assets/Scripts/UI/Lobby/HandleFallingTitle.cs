using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleFallingTitle : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.transform.position.x > 12f)
        {
            rb.transform.position = new Vector3(-7f, rb.transform.position.y, 0);
        }
    }
}
