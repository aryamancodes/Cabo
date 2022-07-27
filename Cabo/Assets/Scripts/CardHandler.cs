using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    //handle the distributing of cards - based on currGameState of the GameManager
    public PlayerCard emptyPlayerCard;
    public EnemyCard emptyEnemyCard; 

    public GameObject playerArea;
    public GameObject enemyArea;

    public Button deck; 

    public static int playerFlipped = 0;
    public static int enemyFlipped = 0;




    void Awake()
    {
        Debug.Log("Enabled");
        GameManager.gameStateChanged += OnGameStateChanged;

        //GameManager.Instance.setGameState(GameState.START);

    }

    void OnDisable()
    {
        GameManager.gameStateChanged -= OnGameStateChanged;
    }

    void setDeck(bool val)
    {
        deck.interactable = val;
    }

    public void OnGameStateChanged()
    {
        if(GameManager.Instance.currState == GameState.START)
        {
            firstDistribute();
            setDeck(false);
        }
 
        if(GameManager.Instance.currState == GameState.PLAYER_TURN)
        {
            setDeck(true);
        }
    }

    public void firstDistribute()
    {
        for(int i=0; i<4; ++i)
        {
            PlayerCard playerCard = Instantiate(emptyPlayerCard, new Vector2(0,0), Quaternion.identity);
            GameObject playerSlot = Instantiate(playerCard.slot, new Vector2(0,0), Quaternion.identity);
            playerCard.card = DeckGenerator.getCard();
            playerSlot.transform.SetParent(playerArea.transform, false);
            playerCard.transform.SetParent(playerSlot.transform, false);
            if(i%2 == 1)
            {
                playerCard.flipCard();
            }
            else
            {
                playerCard.button.interactable = false;
            }
            playerFlipped = 2;
            
            
            EnemyCard enemyCard = Instantiate(emptyEnemyCard, new Vector2(0,0), Quaternion.identity);
            GameObject enemySlot = Instantiate(enemyCard.slot, new Vector2(0,0), Quaternion.identity);
            enemyCard.card = DeckGenerator.getCard();
            enemySlot.transform.SetParent(enemyArea.transform, false);
            enemyCard.transform.SetParent(enemySlot.transform, false);
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
        if(GameManager.Instance.currState == GameState.PLAYER_TURN)
        {
            PlayerCard drawnCard = Instantiate(emptyPlayerCard, new Vector2(0,0), Quaternion.identity);
            GameObject slot = Instantiate(emptyPlayerCard.slot, new Vector2(0,0), Quaternion.identity); 
            drawnCard.card = DeckGenerator.getCard();
            insertDrawnCard(playerArea, slot, drawnCard);
            GameManager.Instance.setGameState(GameState.ENEMY_TURN);
        }
        else if(GameManager.Instance.currState == GameState.ENEMY_TURN)
        {
            EnemyCard drawnCard = Instantiate(emptyEnemyCard, new Vector2(0,0), Quaternion.identity);
            GameObject slot = Instantiate(emptyEnemyCard.slot, new Vector2(0,0), Quaternion.identity);
            drawnCard.card = DeckGenerator.getCard();
            insertDrawnCard(enemyArea, slot, null,drawnCard);
        }
    }

    public void insertDrawnCard(GameObject area, GameObject slot, PlayerCard playerCard=null, EnemyCard enemyCard=null)
    {
        Transform card;
        if(playerCard != null) 
        {
            card = playerCard.transform; 
            playerCard.flipCard();

        }
        else { card = enemyCard.transform; }
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
}
