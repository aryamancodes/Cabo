using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    //handle the distributing of cards - based on currGameState of the GameManager
    public static CardHandler Instance;
    public Card emptyCard;
    public Sprite playerBack;
    public Sprite enemyBack;

    public GameObject playerArea;
    public GameObject enemyArea;
    public GameObject placeArea;

    public Button deck; 
    public static int playerFlipped = 0;
    public static int enemyFlipped = 0;

    void Awake()
    {
        Debug.Log("Enabled");
        GameManager.gameStateChanged += OnGameStateChanged;
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //GameManager.Instance.setGameState(GameState.START);

    }

    void OnDisable()
    {
        GameManager.gameStateChanged -= OnGameStateChanged;
    }


    public void OnGameStateChanged()
    {
        var currState = GameManager.Instance.currState;
        var prevState = GameManager.Instance.prevState;
        if(currState == GameState.START)
        {
            firstDistribute();
            setDrawCards(false);
        }
 
        if(currState == GameState.PLAYER_DRAW || currState == GameState.ENEMY_DRAW)
        {
            setDrawCards(true);
        }

        if(currState == GameState.PLAYER_TURN)
        {
            setPlayerDrag(true);
        }
        if(currState == GameState.PLAYER_DRAW)
        {
           setEnemyDrag(false); 
        }
        if(currState == GameState.ENEMY_DRAW)
        {
           setPlayerDrag(false); 
        }

        if(currState == GameState.ENEMY_TURN)
        {
            setEnemyDrag(true);
        }

    }

    public void firstDistribute()
    {
        for(int i=0; i<4; ++i)
        {
            Card playerCard = Instantiate(emptyCard, new Vector2(0,0), Quaternion.identity);
            GameObject playerSlot = Instantiate(playerCard.slot, new Vector2(0,0), Quaternion.identity);
            playerCard.card = DeckGenerator.getCard();
            playerCard.back = playerBack;
            playerSlot.transform.SetParent(playerArea.transform, false);
            playerCard.transform.SetParent(playerSlot.transform, false);
            playerCard.gameObject.layer = playerArea.layer;
            if(i%2 == 1)
            {
                playerCard.card.isSpecialCard  = false; //first peaked cards are not special
                playerCard.flipCard();
            }
            else
            {
                playerCard.button.interactable = false;
            }
            playerFlipped = 2;
            
            
            Card enemyCard = Instantiate(emptyCard, new Vector2(0,0), Quaternion.identity);
            GameObject enemySlot = Instantiate(enemyCard.slot, new Vector2(0,0), Quaternion.identity);
            enemyCard.card = DeckGenerator.getCard();
            enemyCard.back = enemyBack;
            enemySlot.transform.SetParent(enemyArea.transform, false);
            enemyCard.transform.SetParent(enemySlot.transform, false);
            enemyCard.gameObject.layer = enemyArea.layer;
            if(i%2 == 0)
            {
               enemyCard.card.isSpecialCard  = false; //first peaked cards are not special
                enemyCard.flipCard();
            }
            else
            {
                enemyCard.button.interactable = false;
            }
            enemyFlipped = 2;
        }
    }

    public void Button_onDrawCard()
    {
        Card drawnCard = Instantiate(emptyCard, new Vector2(0,0), Quaternion.identity);
        GameObject slot = Instantiate(emptyCard.slot, new Vector2(0,0), Quaternion.identity); 
        drawnCard.card = DeckGenerator.getCard();
        if(GameManager.Instance.currState == GameState.PLAYER_DRAW)
        {
            drawnCard.back = playerBack;
            insertDrawnCard(playerArea, slot, drawnCard);
            playerFlipped++;
        }
        else if(GameManager.Instance.currState == GameState.ENEMY_DRAW)
        {
            drawnCard.back = enemyBack;
            enemyFlipped++;
            insertDrawnCard(enemyArea, slot, null,drawnCard);
        }
        setDrawCards(false);
    }

    public void insertDrawnCard(GameObject area, GameObject slot, Card playerCard=null, Card enemyCard=null)
    {
        Transform card;
        if(playerCard != null) 
        {
            card = playerCard.transform; 
            playerCard.flipCard();

        }
        else 
        { 
            card = enemyCard.transform;
            enemyCard.flipCard();
        }
        //insert into existing slot
        foreach(Transform child in area.transform)
        {
            if(child.childCount == 0)
            {
                card.SetParent(child, true);
                card.gameObject.layer = area.layer;
                card.rotation = Quaternion.identity;
                //destroy unnecessary slot creation and fix weird scale increase
                card.localScale = new Vector3(1,1,1);
                Destroy(slot);
                return;
            }
        }
        //insert as a new slot 
        slot.transform.SetParent(area.transform, false);
        card.SetParent(slot.transform, false);
        card.gameObject.layer = area.layer;
    }

     void setDrawCards(bool val)
    {
        deck.interactable = val;
        foreach(Transform child in placeArea.transform)
        {
            child.GetComponent<Card>().button.interactable = val;
        }
    }

    public void setPlayerDrag(bool val)
    {
        foreach(Transform child in playerArea.transform)
        {
            if(child.childCount == 0)
            {
                continue;
            }
            else
            {
                child.GetChild(0).GetComponent<Card>().button.interactable = val;
            }
        }
    }

    public void setEnemyDrag(bool val)
    {
        foreach(Transform child in enemyArea.transform)
        {
            if(child.childCount == 0)
            {
                continue;
            }
            else
            {
                child.GetChild(0).GetComponent<Card>().button.interactable = val;
            }
        }
    }

    public void cardPlayed(Card card)
    {
        if(card.isSpecialCard)
        {
            GameManager.Instance.setGameState(GameState.SPECIAL_PLAY);
        }
        else
        {
            GameManager.Instance.setGameState(GameState.PLAY);
        }   
    }

}
