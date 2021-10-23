using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    /* Global Data */
    Vector3 worldMouse;
    RaycastHit2D hitInfo;

    /* Better Viewing Items */
    GameObject copy;
    Transform SelectedCard = null;

    /* Dragging Items */
    bool isDragging = false;
    Transform draggedObject;
    Vector3 resetPosition;

    // Update is called once per frame
    void Update()
    {
        /* Get the mouse position for use elsewhere */
        worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hitInfo = Physics2D.Raycast(worldMouse, Vector2.zero);

        /* Handle zooming in on deck items */
        HandleZooming();

        /* Handle Dragging */
        HandleDragging();
    }

    public void HandleZooming()
    {
        /* If we are dragging, don't do any zooming */
        if (isDragging)
        {
            if(copy != null)
            {
                Destroy(copy);
                copy = null;
            }
            return;
        }


        /* Raycast the mouse position and check for an interactable element */
        if (hitInfo.collider != null && hitInfo.collider.GetComponent<CardGraphic>() != null)
        {
            if (hitInfo.collider.transform != SelectedCard)
            {
                /* Destroy the old card */
                if (copy != null) Destroy(copy);

                /* Generate a card copy for better viewing */
                copy = Instantiate(hitInfo.collider.gameObject, hitInfo.collider.transform.parent);
                Destroy(copy.GetComponent<Collider2D>());
                Destroy(copy.GetComponent<CardGraphic>());

                /* Move the new card */
                SelectedCard = hitInfo.collider.transform;
                copy.transform.position = new Vector3(SelectedCard.position.x, SelectedCard.position.y + 1.0f, -1.1f);
                copy.transform.localScale = 2 * SelectedCard.transform.localScale;
            }
        }
        else
        {
            /* Remove the old card */
            if(copy != null)
            {
                Destroy(copy);
                copy = null;
            }
        }
    }

    public void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /* Potentially start a drag */
            if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Draggable"))
            {
                /* We can start a drag with this object */
                draggedObject = hitInfo.collider.transform;
                draggedObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                resetPosition = draggedObject.position;
                isDragging = true;
            }
        }
        else if(Input.GetMouseButton(0) && isDragging)
        {
            /* Continue a drag */
            draggedObject.position = new Vector3(worldMouse.x, worldMouse.y, -1.1f);
        }
        else if(Input.GetMouseButtonUp(0) && isDragging)
        {
            /* End a drag */

            /* If we landed on a valid target, complete that drag. Otherwise reset. */
            DragTarget target = null;
            if (hitInfo.collider != null) target = hitInfo.collider.GetComponent<DragTarget>();

            if(target != null && target.OnDragInto(draggedObject.gameObject))
            {
                draggedObject.position = resetPosition;
                draggedObject.gameObject.layer = LayerMask.NameToLayer("Default");
                isDragging = false;
            }
            else
            {
                draggedObject.position = resetPosition;
                draggedObject.gameObject.layer = LayerMask.NameToLayer("Default");
                isDragging = false;
            }
        }
    }
}
