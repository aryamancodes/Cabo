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
    public int startIndex;


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
        startIndex = card.getIndex();
    }
    public void startDrag()
    {
        if(card.canDrag)
        {
            isDragging = true;
            CardHandler.Instance.Network_setLockCards(false);
        }
    }

    public void stopDrag()
    {
        CardHandler.Instance.Network_setLockCards(true);
        if(card.canDrag)
        {
            isDragging = false;

            if(dropZone == null || dropZone.layer == startParent.layer || dropZone.layer == GameManager.Instance.IgnoreLayer) { returnToStart(); }

            else if(dropZone.layer == GameManager.Instance.UILayer) //drop card into center
            {
                transform.position = placeArea.transform.position;
                transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(-30f, 30f)));
                gameObject.layer = placeArea.layer;
                transform.SetParent(placeArea.transform);
                card.flipCard("up", false);

                // A lil cheat that I always do irl - and will do online ;), no snap fail punishment 
                // after Cabo is called 
                if(GameManager.Instance.currState == GameState.CABO)
                {
                    int length = placeArea.transform.childCount;
                    int lastPlayedValue = placeArea.transform.GetChild(length-2).GetComponent<Card>().value;
                    bool syncMove = card.value == lastPlayedValue || (card.value == 13 && lastPlayedValue == -1) || (card.value == -1 && lastPlayedValue == -1) ;
                    if(syncMove){ CardHandler.Instance.Network_playCard(startIndex, startParent.layer); } 
                    else { returnToStart(); }
                    GameManager.Instance.Network_setGameState(GameState.GAME_OVER);
                    return;                 
                }
                else{ CardHandler.Instance.Network_playCard(startIndex, startParent.layer); } 
                if(!GameManager.Instance.canSnap)
                {
                    CardHandler.Instance.cardPlayed(card);
                }
                else if(GameManager.Instance.canSnap)
                {
                    GameState whoseCardSnapped= GameState.NONE; 
                    if(startParent.layer == GameManager.Instance.playerLayer){ whoseCardSnapped = GameState.PLAYER_TURN; }
                    else if(startParent.layer == GameManager.Instance.enemyLayer) { whoseCardSnapped = GameState.ENEMY_TURN; }
                    CardHandler.Instance.checkSnapped(whoseCardSnapped); 
                }
            }

            else if(startParent.layer == GameManager.Instance.UILayer)
            {
                drawFromPlaceArea();
                CardHandler.Instance.Network_drawFromPlaceArea();
            }

            else if(GameManager.Instance.currState == GameState.SNAP_OTHER)
            {
                insertIntoArea();
                CardHandler.Instance.Network_giveOpponentCard(startIndex, startParent.layer);
                if(startParent.layer == GameManager.Instance.playerLayer){ GameManager.Instance.Network_setGameState(GameState.PLAYER_DRAW); }   
                else { GameManager.Instance.Network_setGameState(GameState.ENEMY_DRAW); }   
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
        //set selected card locally, since index is out of sync between clients
        if(GameManager.Instance.currState == GameState.PLAYER_DRAW)
        { 
            CardHandler.Instance.playerSelectedCard = card;
            card.back = CardHandler.Instance.playerBack;
        }
        else if(GameManager.Instance.currState == GameState.ENEMY_DRAW)
        {
            CardHandler.Instance.enemySelectedCard = card;
            card.back = CardHandler.Instance.enemyBack;
        }
        CardHandler.Instance.setDrawCardsAndArea(false, false);
        CardHandler.Instance.setPlayerClickDragAndArea(false, false, false); 
        CardHandler.Instance.setPlayerClickDragAndArea(false, false, false); 
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
