using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    [SerializeField] private Transform rotationCenter;

    [SerializeField] private float rotationRadius = 4f, angularSpeed = 2f;
    public float startX, startY, angle;

    private float posX, posY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        posX = startX + rotationCenter.position.x + Mathf.Cos(angle) * rotationRadius;
        posY = startY + rotationCenter.position.y + Mathf.Sin(angle) * rotationRadius;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(posX, posY), angularSpeed * Time.deltaTime);
        angle = angle + Time.deltaTime * angularSpeed;

        if (angle >= 360f) angle = 0f;
    }
}
