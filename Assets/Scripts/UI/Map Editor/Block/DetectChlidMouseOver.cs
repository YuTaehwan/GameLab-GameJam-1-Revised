using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectChlidMouseOver : MonoBehaviour
{
    public GameObject parent;
    public IDeletable parentDeletable;

    private void Start()
    {
        parentDeletable = parent.GetComponent<IDeletable>();
    }

    public void OnMouseDown()
    {
        Debug.Log("click");
        parentDeletable.onClicked();
    }

    public void OnMouseOver()
    {
        Debug.Log("hover");
        parentDeletable.onHovered();
    }

    public void OnMouseExit()
    {
        Debug.Log("exit");
        parentDeletable.onExited();
    }
}
