using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{

    public GameObject projectile;

    public Transform spawnLocation;

    public Quaternion spawnRotation;

    public float spawnTime = 2.3f;

    private float timeSinceSpawned = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //spawnLocation = new Vector3() 
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSpawned += Time.deltaTime;
        if (timeSinceSpawned >= spawnTime)
        {
            if (transform.parent != null)
            {
                //float rotationZ = transform.parent.transform.rotation.z;
                //Debug.Log("z:" + rotationZ);
                //Debug.Log(Quaternion.Euler(new Vector3(0, 0, rotationZ)));
                Instantiate(projectile, spawnLocation.position, transform.parent.transform.rotation);
            }
            else if (this.transform.eulerAngles.z == 90 || this.transform.eulerAngles.z == 270)
            {
                Debug.Log("2");
                Instantiate(projectile, spawnLocation.position, Quaternion.Inverse(transform.rotation));
            }
            else if(this.transform.eulerAngles.z == 0)
            {
                Instantiate(projectile, spawnLocation.position, Quaternion.Euler(0, 0, 180));
            }else
            {
                Instantiate(projectile, spawnLocation.position, Quaternion.Euler(0, 0, 0));
            }
            timeSinceSpawned = 0;
        }
    }
}
