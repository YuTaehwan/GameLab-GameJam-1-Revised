using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
private Vector3 offset = new Vector3(0f, 0f, -10f);
private float smoothTime = .25f;
private Vector3 velocity = Vector3.zero;

private bool isShowStage = false;
private float timeStamp = 0f;
private Vector3 startShowPos;

[SerializeField] private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null) {
            Vector3 targetPosition = target.position + offset;
            if (DontDestroyObject.gameManager.playMode == PlayMode.PLAY)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
        
    }

    void FixedUpdate() {
        if (isShowStage && target != null) {
            timeStamp += Time.deltaTime;
            Vector3 targetPosition = Vector3.Lerp(startShowPos, target.position + offset, timeStamp / GameManager.STAGE_SHOW_TIME);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + offset, ref velocity, smoothTime);
        }
    }

    public void SetShowStage (Vector3 startPos) {
        isShowStage = true;
        timeStamp = 0f;
        startShowPos = startPos;
    }

    public void FinishShowStage () {
        isShowStage = false;
        transform.position = target.position + offset;
    }
	
}
