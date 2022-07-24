using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    public PlayerCard emptyPlayerCard;
    public EnemyCard emptyEnemyCard; 

    public GameObject playerArea;
    public GameObject enemyArea;

    public GridLayoutGroup grid;

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

    public void OnGameStateChanged()
    {
        if(GameManager.Instance.currState == GameState.START)
        {
            firstDistribute();
            //FirstPeakBoth();
        }

        if(GameManager.Instance.currState == GameState.PLAYER_READY)
        {
            flipStartingCards("player");
        }
        if(GameManager.Instance.currState == GameState.ENEMY_READY)
        {
            flipStartingCards("enemy");
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
            //flip 2 player cards
            if(i%2 == 1)
            {
                playerCard.image.sprite = playerCard.card.face;
                playerCard.faceUp = true;
            }
            else
            {
                playerCard.button.interactable = false;
            }
            
            
            EnemyCard enemyCard = Instantiate(emptyEnemyCard, new Vector2(0,0), Quaternion.identity);
            GameObject enemySlot = Instantiate(enemyCard.slot, new Vector2(0,0), Quaternion.identity);
            enemyCard.card = DeckGenerator.getCard();
            enemySlot.transform.SetParent(enemyArea.transform, false);
            enemyCard.transform.SetParent(enemySlot.transform, false);
            //flip 2 enemy cards
            if(i%2 == 0)
            {
                enemyCard.image.sprite = enemyCard.card.face;
                enemyCard.faceUp = true;
            }
            else
            {
                enemyCard.button.interactable = false;
            }
        }
    }

    public void flipStartingCards(string who)
    {
        if(who == "player")
        {
            PlayerCard toFlip = playerArea.transform.GetChild(1).GetChild(0).gameObject.GetComponent<PlayerCard>();
            toFlip.flipCard();
            toFlip = playerArea.transform.GetChild(3).GetChild(0).gameObject.GetComponent<PlayerCard>();
            toFlip.flipCard();

        }
        if(who == "enemy")
        {
            EnemyCard toFlip = enemyArea.transform.GetChild(0).GetChild(0).gameObject.GetComponent<EnemyCard>();
            toFlip.flipCard();
            toFlip = enemyArea.transform.GetChild(2).GetChild(0).gameObject.GetComponent<EnemyCard>();
            toFlip.flipCard();
        }
    }
}
