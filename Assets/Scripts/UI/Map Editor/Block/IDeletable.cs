using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDeletable
{
    void SetDeletable(bool deletable);
    void SetOnDeleteCallback(Action<GameObject> callback);
    void SetOnHoverCallback(Action<GameObject> callback);
    void SetOnExitHoverCallback(Action<GameObject> callback);

    public void onClicked();

    public void onHovered();

    public void onExited();

    public void InitDeletable(Vector2Int blockSize);
}
