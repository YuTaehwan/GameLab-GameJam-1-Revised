using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePlayerInLobby : MonoBehaviour
{
    Rigidbody2D rb;
    public float jumpTime;
    private float jumpSpeed = 8.0f;
    public float moveSpeed = 6.0f;
    public float jumpDelay;
    public Animator squashStretchAnimator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpTime = 1.5f;
        jumpDelay = jumpTime;
    }

    // Update is called once per frame
    void Update()
    {
        jumpDelay -= Time.deltaTime;
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        if (jumpDelay <= 0f)
        {
            jumpDelay = jumpTime;
            rb.velocity = Vector2.up * jumpSpeed;
            squashStretchAnimator.SetTrigger("Jump");
        }

        if (rb.transform.position.x > 9.5f)
        {
            rb.transform.position = new Vector3(-9.5f, rb.transform.position.y, 0);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            squashStretchAnimator.SetTrigger("Landing");
        }
    }
}
