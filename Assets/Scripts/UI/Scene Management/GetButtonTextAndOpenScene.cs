using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
public class GetButtonTextAndOpenScene : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    private bool buttonClicked;
    // Start is called before the first frame update
    void Start()
    {
        buttonClicked = false;
    }
    
    public void OpenScene()
    {
        buttonClicked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonClicked)
        {
            buttonClicked = false;
            DontDestroyObject.Instance.LoadScene(Int32.Parse(buttonText.text));
        }
    }
}
