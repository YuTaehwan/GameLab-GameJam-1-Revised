using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPlatform : MonoBehaviour
{

    private float fallDelay = .2f;
    private float destroyDelay = 2f;
    private float startX;
    private float startY;

    [SerializeField] private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.position.y > transform.position.y)
            {
                StartCoroutine(Fall());
            }
        }
    }
    IEnumerator Fall()
    {
        Vector3 startPos = GetComponent<BlockBase>().initialPos;
        startX = startPos.x;
        startY = startPos.y;
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(destroyDelay);
        rb.position = new Vector2(-1000, -1000);
        yield return new WaitForSeconds(5.0f);
        ReturnToOriginalPlace();
        //Destroy(gameObject, destroyDelay);
    }

    void ReturnToOriginalPlace()
    {
        rb.velocity = Vector2.zero;
        rb.position = new Vector2(startX, startY);
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
