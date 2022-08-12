using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{        
    public Card card;
    public GameObject canvas;
    public GameObject placeArea;
    public GameObject startParent = null;
    public bool isDragging = false; 
    public GameObject dropZone = null;
    public int index;


    void Start()
    {        
        canvas = GameObject.Find("Canvas");
        placeArea = GameObject.Find("Place Card");

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

    //called by Begin Drag event trigger
    public void setStartParent()
    {
        startParent = transform.parent.gameObject;
    }

    //called by Begin Drag event trigger
    public void setIndex()
    {
        index = card.getIndex();
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
                    transform.position = placeArea.transform.position;
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(-30f, 30f)));
                    gameObject.layer = placeArea.layer;
                    transform.SetParent(placeArea.transform);
                    card.flipCard("up", false);
                    CardHandler.Instance.Network_playCard(index, startParent.layer);
                    
                    //FIXME: Correctly detect who snaps the card
                    // if(GameManager.Instance.currState == GameState.PLAY)
                    // {
                    //     GameState whoSnapped = GameState.NONE; 
                    //     if(startParent.layer == GameManager.Instance.playerLayer){ whoSnapped = GameState.PLAYER_TURN; }
                    //     else if(startParent.layer == GameManager.Instance.enemyLayer) { whoSnapped = GameState.ENEMY_TURN; }
                    //     CardHandler.Instance.checkSnapped(whoSnapped); 
                    // }
                    // else { 
                    CardHandler.Instance.cardPlayed(card); 
                }

                else if(startParent.layer == GameManager.Instance.UILayer)
                {
                    drawFromPlaceArea();
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

    void drawFromPlaceArea()
    {
        insertIntoArea();
        CardHandler.Instance.setDrawCardsAndArea(false, false);
        if(GameManager.Instance.currState == GameState.PLAYER_DRAW)
        { 
            CardHandler.Instance.playerSelectedCard =  GetComponent<Card>();
            CardHandler.Instance.setPlayerClickDragAndArea(false, false, false); 
            CardHandler.Instance.setPlayerClickDragAndArea(false, false, false);
        }
        else if(GameManager.Instance.currState == GameState.ENEMY_DRAW)
        {
            CardHandler.Instance.enemySelectedCard = GetComponent<Card>();
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
        GameObject newSlot = Instantiate(card.slot, new Vector2(0,0), Quaternion.identity);
        newSlot.transform.SetParent(dropZone.transform, false);
        newSlot.gameObject.layer = dropZone.layer;
        transform.SetParent(newSlot.transform, false);
        transform.gameObject.layer = dropZone.layer;
        transform.rotation = Quaternion.identity;
    }
}
