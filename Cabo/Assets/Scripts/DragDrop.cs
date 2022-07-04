using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{    
    private GameObject canvas;
    private GameObject placeCard;
    private GameObject slot;
    private GameObject startParent = null;
    private bool isDragging = false; 
    private GameObject dropZone = null;
    private int UILayer = 5;


    void Start()
    {
        canvas = GameObject.Find("Canvas");
        placeCard = GameObject.Find("Place Card");
        slot = GameObject.Find("Slot");
    }
     // Update is called once per frame
    void Update()
    {
        if(isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if(canvas != null)
            {
                transform.SetParent(canvas.transform, false);    
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        dropZone = collision.gameObject;
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        dropZone = null;
    }

    public void startDrag()
    {
        if(startParent == null)
        {
            startParent = transform.parent.gameObject;
        }
        isDragging = true;
    }

    public void stopDrag()
    {
        isDragging = false;

        if(dropZone != null)
        {
            if(dropZone.layer == UILayer) //drop in place card
            {
                transform.position = placeCard.transform.position;
            }

            else
            {
                insertIntoArea();
            }
        }

        //return back to starting position
        else
        {
            transform.SetParent(startParent.transform, true);
            //transform.gameObject.layer = startParent.layer;
        }
        dropZone = null;
    }


    //dropping in enemy's grid layout group
    void insertIntoArea()
    {
        //replace a previously moved card
        foreach(Transform child in dropZone.transform)
            {
                if(child.childCount == 0)
                {
                    transform.SetParent(child, true);
                    transform.gameObject.layer = dropZone.layer;
                    return;
                }
            }
        //insert as a new card 
        GameObject newSlot = Instantiate(slot, new Vector2(0,0), Quaternion.identity);
        newSlot.transform.SetParent(dropZone.transform, false);
        transform.SetParent(newSlot.transform, false);
        transform.gameObject.layer = dropZone.layer;
    }
}