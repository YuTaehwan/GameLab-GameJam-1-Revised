using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleFlagInLobby : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rb;
    [SerializeField] private GameObject fireWorkParticle = default;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine("Particle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Particle()
    {
        yield return new WaitForSeconds(3.0f);
        createFireWork();
        StartCoroutine("Particle");
    }

    void createFireWork()
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(-1, 1);
        float size = 5.0f;
        GameObject particle = Instantiate(fireWorkParticle,  new Vector3(randomX * size, randomY * size, 0f), transform.rotation);
        ParticleSystem particlesys = particle.GetComponent<ParticleSystem>();
        particlesys.Play();
    }
}
