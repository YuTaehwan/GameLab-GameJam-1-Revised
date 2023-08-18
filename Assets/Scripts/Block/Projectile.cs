using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float moveSpeed = 1f;
    private float destroyDelay = 5f;

    private float leastDestroyTime = .1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        leastDestroyTime -= Time.deltaTime;
        transform.position += moveSpeed * transform.up * Time.deltaTime;
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (leastDestroyTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
