using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleFlag : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rb;
    [SerializeField] private GameObject fireWorkParticle = default;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetTrigger("Flag");
        DontDestroyObject.gameManager.GiveStar();
        StartCoroutine("Particle");
    }
    
    IEnumerator Particle()
    {
        yield return new WaitForSeconds(1.0f);
        createFireWork();
        yield return new WaitForSeconds(1.0f);
        createFireWork();
        yield return new WaitForSeconds(1.0f);
        createFireWork();
        yield return new WaitForSeconds(1.0f);
        DontDestroyObject.gameManager.GoLobby();
    }

    void createFireWork()
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(0, 1);
        float size = 5.0f;
        GameObject particle = Instantiate(fireWorkParticle, rb.transform.position + new Vector3(randomX * size, randomY * size, 0f), transform.rotation);
        ParticleSystem particlesys = particle.GetComponent<ParticleSystem>();
        particlesys.Play();
    }
}
