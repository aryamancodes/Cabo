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
    public GameObject startParent = null;
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

    public void setStartParent()
    {
        startParent = this.transform.parent.gameObject;
    }
    public void startDrag()
    {
        if(card.canDrag)
        {
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
                if(dropZone.layer == GameManager.Instance.IgnoreLayer)
                {
                    returnToStart();
                }
                if(dropZone.layer == GameManager.Instance.UILayer && startParent.layer != GameManager.Instance.UILayer) //drop card into center
                {
                    transform.position = placeCard.transform.position;
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(-30f, 30f)));
                    transform.gameObject.layer = dropZone.layer;
                    transform.SetParent(placeCard.transform);
                    Card played = transform.gameObject.GetComponent<Card>();
                    played.flipCard("up");
                    //FIXME: Correctly detect who snaps the card
                    if(GameManager.Instance.currState == GameState.PLAYER_DRAW || GameManager.Instance.currState == GameState.ENEMY_DRAW)
                    {
                        GameState whoSnapped = GameState.NONE; 
                        if(startParent.layer == GameManager.Instance.playerLayer){ whoSnapped = GameState.PLAYER_TURN; }
                        else if(startParent.layer == GameManager.Instance.enemyLayer) { whoSnapped = GameState.ENEMY_TURN; }
                        CardHandler.Instance.checkSnapped(whoSnapped); 
                    }
                    else { CardHandler.Instance.cardPlayed(played); }
                }

                else if(startParent.layer == GameManager.Instance.UILayer)
                {
                    drawFromPlaceCard();
                }

                else
                {
                    insertIntoArea();
                }
            }

            //return back to starting position
            else
            {
                returnToStart();

            }
            dropZone = null;
        }
    }

    void returnToStart()
    {
        transform.position = startParent.transform.position;
        transform.SetParent(startParent.transform, true);
        transform.rotation = Quaternion.identity;
    }

    void drawFromPlaceCard()
    {
        insertIntoArea();
        CardHandler.Instance.setDrawCardsAndArea(false, false);
        if(GameManager.Instance.currState == GameState.PLAYER_DRAW)
        { 
            CardHandler.Instance.playerSelectedCard = this.gameObject.GetComponent<Card>();
            CardHandler.Instance.setPlayerClickDragAndArea(false, false, false); 
            CardHandler.Instance.setPlayerClickDragAndArea(false, false, false);
        }
        else if(GameManager.Instance.currState == GameState.ENEMY_DRAW)
        {
            CardHandler.Instance.enemySelectedCard = this.gameObject.GetComponent<Card>();
            CardHandler.Instance.setEnemyClickDragAndArea(false, false, false); 
            CardHandler.Instance.setPlayerClickDragAndArea(false, false, false);
        }
        
    }

    //dropping in the player or enemy grid layout group
    void insertIntoArea()
    {
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
        newSlot.gameObject.layer = dropZone.layer;
        transform.SetParent(newSlot.transform, false);
        transform.gameObject.layer = dropZone.layer;
        transform.rotation = Quaternion.identity;
    }
}
