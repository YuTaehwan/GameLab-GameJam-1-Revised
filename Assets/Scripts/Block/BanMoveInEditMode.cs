using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanMoveInEditMode : MonoBehaviour
{
    [SerializeField] private GameManager gameManager = default;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = DontDestroyObject.gameManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (DontDestroyObject.Instance.IsEditMode())
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
