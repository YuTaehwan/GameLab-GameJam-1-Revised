using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    GameObject player;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag =="Player")
        {
            player = GameObject.Find("Player");
            player.GetComponent<PlayerMovement>().maxSpeed /= 2;
            player.GetComponent<PlayerMovement>().jumpSpeed = 0;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {

        player.GetComponent<PlayerMovement>().maxSpeed *= 2;
        player.GetComponent<PlayerMovement>().jumpSpeed = 8;
    }

}
