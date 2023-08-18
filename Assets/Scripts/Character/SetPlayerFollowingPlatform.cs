using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerFollowingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.position.y > transform.position.y)
        {
            collision.transform.SetParent(transform);
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
    }
}
