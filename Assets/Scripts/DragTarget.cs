using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTarget : MonoBehaviour
{
    /// <summary>
    /// An event to be called when this object is dragged into.
    /// Returns success or failure.
    /// </summary>
    /// <param name="dragObject"></param>
    /// <returns></returns>
    virtual public bool OnDragInto(GameObject dragObject)
    {
        Debug.Log("Dragged into. Default Implementation.");
        return false;
    }
}
