using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{        
    private Card card;
    private GameObject canvas;
    private GameObject placeCard;
    private GameObject slot;
    private GameObject startParent = null;
    private bool isDragging = false; 
    private GameObject dropZone = null;
    private int UILayer;


    void Start()
    {
        card = gameObject.GetComponent<Card>();
        canvas = GameObject.Find("Canvas");
        placeCard = GameObject.Find("Place Card");
        slot = GameObject.Find("Slot");
    }
     // Update is called once per frame
    void Update()
    {
        if(isDragging && card.canDrag)
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
        var currState = GameManager.Instance.currState;
        if(card.canDrag)
        {
            if(startParent == null)
            {
                startParent = transform.parent.gameObject;
            }
            isDragging = true;
        }
    }

    public void stopDrag()
    {
        if(card.canDrag)
        {
            isDragging = false;

            if(dropZone != null)
            {
                if(dropZone.layer == GameManager.Instance.UILayer) //drop card into center
                {
                    transform.position = placeCard.transform.position;
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(-30f, 30f)));
                    transform.gameObject.layer = dropZone.layer;
                    transform.SetParent(placeCard.transform);
                    Card played = transform.gameObject.GetComponent<Card>();
                    played.flipCard("up");
                    CardHandler.Instance.cardPlayed(played);
                    CardHandler.Instance.setDrawCards(false);
                    if(GameManager.Instance.prevState == GameState.PLAYER_TURN)
                    {
                        CardHandler.Instance.setPlayerClickAndDrag(false, false);
                    }
                    else if(GameManager.Instance.prevState == GameState.ENEMY_TURN)
                    {
                        CardHandler.Instance.setEnemyClickAndDrag(false, false);

                    }
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
                transform.rotation = Quaternion.identity;

            }
            dropZone = null;
        }
    }


    //dropping in enemy's grid layout group
    void insertIntoArea()
    {
        if(startParent.layer == GameManager.Instance.UILayer)
        {
            CardHandler.Instance.setPlayerClickAndDrag(false, false);
        }
        //replace a previously moved card
        foreach(Transform child in dropZone.transform)
            {
                if(child.childCount == 0)
                {
                    transform.SetParent(child, true);
                    transform.gameObject.layer = dropZone.layer;
                    transform.rotation = Quaternion.identity;
                    return;
                }
            }
        //insert as a new card 
        GameObject newSlot = Instantiate(slot, new Vector2(0,0), Quaternion.identity);
        newSlot.transform.SetParent(dropZone.transform, false);
        transform.SetParent(newSlot.transform, false);
        transform.gameObject.layer = dropZone.layer;
        transform.rotation = Quaternion.identity;
    }
}
