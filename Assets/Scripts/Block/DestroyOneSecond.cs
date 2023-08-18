using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOneSecond : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Die");
    }
    
    IEnumerator Die() {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
    
}
