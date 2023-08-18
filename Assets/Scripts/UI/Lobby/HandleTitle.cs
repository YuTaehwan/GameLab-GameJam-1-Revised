using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTitle : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine("FallTitle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator FallTitle() {
        yield return new WaitForSeconds(2.0f);
        rb.gravityScale = 5.0f;
    }
}
