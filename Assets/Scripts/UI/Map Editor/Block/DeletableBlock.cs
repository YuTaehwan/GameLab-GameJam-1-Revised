using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeletableBlock : BlockBase, IDeletable
{
    private SpriteRenderer sr;

    private bool deletable;
    private Action<GameObject> onDeleteCallback;
    private Action<GameObject> onHoverCallback;
    private Action<GameObject> onExitHoverCallback;

    private BoxCollider2D[] colliders;
    private BoxCollider2D clickCollider;
    private bool originalTriggerFlag;
    private Vector2 originalColliderSize;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        deletable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitDeletable(Vector2Int blockSize)
    {
        colliders = GetComponentsInChildren<BoxCollider2D>();

        clickCollider = GetComponent<BoxCollider2D>();
        if (clickCollider == null)
        {
            clickCollider = gameObject.AddComponent<BoxCollider2D>();
            clickCollider.isTrigger = true;
            clickCollider.size = blockSize;
        }

        originalTriggerFlag = clickCollider.isTrigger;
        originalColliderSize = clickCollider.size;
    }

    public void SetDeletable(bool deletable)
    {
        this.deletable = deletable;

        foreach (Collider2D c in colliders)
        {
            c.enabled = !deletable;
        }

        clickCollider.enabled = true;
        clickCollider.isTrigger = deletable;

        if (!deletable)
        {
            clickCollider.isTrigger = originalTriggerFlag;
            clickCollider.size = originalColliderSize;
        }
    }

    public void SetOnDeleteCallback(Action<GameObject> callback)
    {
        onDeleteCallback = callback;
    }

    public void SetOnHoverCallback(Action<GameObject> callback)
    {
        onHoverCallback = callback;
    }

    public void SetOnExitHoverCallback(Action<GameObject> callback)
    {
        onExitHoverCallback = callback;
    }

    public void onClicked()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && deletable)
        {
            onDeleteCallback(this.gameObject);
        }
    }

    public void onHovered()
    {
        if (deletable)
            onHoverCallback(this.gameObject);
    }

    public void onExited()
    {
        onExitHoverCallback(this.gameObject);
    }

    public void OnMouseDown()
    {
        onClicked();
    }

    public void OnMouseOver()
    {
        onHovered();
    }

    public void OnMouseExit()
    {
        onExited();
    }
}
