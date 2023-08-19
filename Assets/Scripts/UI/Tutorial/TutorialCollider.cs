using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private Tutorial.TutorialType tutorialType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            GetComponent<BoxCollider2D>().enabled = false;
            DontDestroyObject.gameManager.EditMode();
            tutorial.SetTutorialType(tutorialType);
        }
    }
}
