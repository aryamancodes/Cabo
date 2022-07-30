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
    public int playerFlipped = 0;
    public int enemyFlipped = 0;

    public Card playerSelectedCard = null;
    public Card enemySelectedCard = null;

    public Card played;

    void Awake()
    {
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
            FlipDownAllCards();

        }

        if(currState == GameState.PLAYER_TURN)
        {
            setPlayerClickAndDrag(true, true);
        }
        if(currState == GameState.PLAYER_DRAW)
        {
           setEnemyClickAndDrag(false, false);
           overrideSpecialCard(currState); 
        }
        if(currState == GameState.ENEMY_DRAW)
        {
           setPlayerClickAndDrag(false, false); 
           overrideSpecialCard(currState);
        }

        if(currState == GameState.ENEMY_TURN)
        {
            setEnemyClickAndDrag(true, true);
            FlipDownAllCards();

        }

        if(currState == GameState.SWAP1 || currState == GameState.BLIND_SWAP1 || currState == GameState.PEAK_PLAYER)
        {
            if(prevState == GameState.PLAYER_TURN)
            {
                setPlayerClickAndDrag(true, false);
                setEnemyClickAndDrag(false, false);
            }
            else if(prevState == GameState.ENEMY_TURN)
            {
                setPlayerClickAndDrag(false, false);
                setEnemyClickAndDrag(true, false);
            }
        }

         if(currState == GameState.SWAP2 || currState == GameState.BLIND_SWAP2 || currState == GameState.PEAK_ENEMY)
        {
            if(prevState == GameState.PLAYER_TURN)
            {
                setPlayerClickAndDrag(false, false);
                setEnemyClickAndDrag(true, false);
            }
            else if(prevState == GameState.ENEMY_TURN)
            {
                setPlayerClickAndDrag(true, false);
                setEnemyClickAndDrag(false, false);
            }
        }

    }

    public void firstDistribute()
    {
        for(int i=0; i<4; ++i)
        {
            Card playerCard = Instantiate(emptyCard, new Vector2(0,0), Quaternion.identity);
            GameObject playerSlot = Instantiate(playerCard.slot, new Vector2(0,0), Quaternion.identity);
            playerCard.card = DeckGenerator.getCard();
            playerCard.card.value = 13;
            playerCard.card.isSpecialCard = true;
            playerCard.back = playerBack;
            //playerCard.card.isSpecialCard = false;
            playerSlot.transform.SetParent(playerArea.transform, false);
            playerCard.transform.SetParent(playerSlot.transform, false);
            playerCard.gameObject.layer = playerArea.layer;
            if(i%2 == 1)
            {
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
            //enemyCard.card.isSpecialCard = false;
            enemySlot.transform.SetParent(enemyArea.transform, false);
            enemyCard.transform.SetParent(enemySlot.transform, false);
            enemyCard.gameObject.layer = enemyArea.layer;
            if(i%2 == 0)
            {
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

    //if a card is drawn and not played immediately, it is no longer special
    public void overrideSpecialCard(GameState curr)
    {
        // if(curr == GameState.PLAYER_DRAW)
        // {
        //     foreach(Transform child in enemyArea.transform)
        //     {
        //         if(child.childCount != 0)
        //         {
        //             child.GetChild(0).GetComponent<Card>().card.isSpecialCard = false;
        //         }
        //     }

        // }
        // if(curr == GameState.ENEMY_DRAW)
        // {
        //     foreach(Transform child in playerArea.transform)
        //     {
        //         if(child.childCount != 0)
        //         {
        //             child.GetChild(0).GetComponent<Card>().card.isSpecialCard = false;
        //         }
        //     }

        // foreach(Transform child in placeArea.transform)
        // {
        //     child.GetComponent<Card>().card.isSpecialCard = false;
        // }
    }

    public void FlipDownAllCards()
    {
        if(playerSelectedCard != null)
        {
            playerSelectedCard.flipCard("down");
            playerSelectedCard = null;
        }

        if(enemySelectedCard != null)
        {
            enemySelectedCard.flipCard("down");
            enemySelectedCard = null;
        }

    }

    public void setDrawCards(bool val)
    {
        deck.interactable = val;
        foreach(Transform child in placeArea.transform)
        {
            var card = child.GetComponent<Card>();
            card.button.interactable = val;
            card.canDrag = val;
        }
    }

    public void setPlayerClickAndDrag(bool clickVal, bool dragVal)
    {
        foreach(Transform child in playerArea.transform)
        {
            if(child.childCount != 0)
            {
                Card card = child.GetChild(0).GetComponent<Card>();
                card.canDrag = dragVal;
                card.button.interactable = clickVal;
            }
        }
    }

    public void setEnemyClickAndDrag(bool clickVal, bool dragVal)
    {
        foreach(Transform child in enemyArea.transform)
        {
            if(child.childCount == 0)
            {
                continue;
            }
            else
            {
                Card card = child.GetChild(0).GetComponent<Card>();
                card.canDrag = dragVal;
                card.button.interactable = clickVal;
            }
        }
    }

    public void cardPlayed(Card card)
    {
        played = card;
        if(played != null)
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

    public void swapCards()
    {
        
    }

}
