using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
public class GoToLobby : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void goLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
