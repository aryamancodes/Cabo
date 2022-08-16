using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*

    Class attached to each card object, determining the Sprite shown
    along with how to handle clicks

*/
public class Card : MonoBehaviour
{
    public Image image;
    public CardBase card = null;
    public Sprite back;
    public GameObject slot;    
    public bool faceUp = false;

    public Button button;    
    public CardBase.Suit suit;
    public int value;
    public bool isSpecialCard;
    public bool canDrag = false;

    void Start()
    {
        if(card != null)
        {
            suit = card.suit;
            value = card.value;
            isSpecialCard = card.isSpecialCard;
        } 
        if(faceUp)
        {
            image.sprite = card.shownFace;
        }
        else
        {
            image.sprite = back;
        }
    }

    // Find the index of this card. This is used to communicate which 
    // card has been interacted with, over the network.
    public int getIndex()
    {
        int layer = gameObject.layer;
        if(layer == GameManager.Instance.playerLayer || layer == GameManager.Instance.enemyLayer)
        {
            return transform.parent.GetSiblingIndex();
        }
        return transform.GetSiblingIndex();
    }

    // Flips card, hiding the actual face from a client is necessary.
    // Returns the index of the card flipped.
    public int flipCard(bool hidden)
    {        
        if(faceUp)
        {
            image.sprite = back; 
            card.shownFace = back;
        }

        // Modify the base card face directly, otherwise changes don't sync
        if(!faceUp && hidden) 
        { 
            image.sprite = card.hiddenFace; 
            card.shownFace = card.hiddenFace;
        }

        if(!faceUp && !hidden) 
        {  
            image.sprite = card.cardFace; 
            card.shownFace = card.cardFace;
        }

        faceUp = !faceUp;
        return getIndex();
    }

    // Flips a card, asserting its direction. Returns the index of the card 
    // flipped.
    public int flipCard(string direction, bool hidden)
    {
        if(direction == "down")
        {
            image.sprite = back;
            card.shownFace = back;
            faceUp = false;
        }

        // Modify the base card face directly, otherwise changes don't sync
        else if(direction == "up" && hidden)
        {
            image.sprite = card.hiddenFace;
            card.shownFace = card.hiddenFace;
            faceUp = true;
        }
        
        else if(direction == "up")
        {
            image.sprite = card.cardFace;
            card.shownFace = card.cardFace;
            faceUp = true;
        }

        return getIndex();
    }


    // Handles the clicking of cards during the various game states
    public void cardClicked()
    {
        if(gameObject.layer == GameManager.Instance.playerLayer)
        {
            playerCardClicked();
        }
        if(gameObject.layer == GameManager.Instance.enemyLayer)
        {
            enemyCardClicked();
        }
    }
    public void playerCardClicked()
    {
        var currState = GameManager.Instance.currState;
        var prevState=  GameManager.Instance.prevState;
        switch(currState)
        {
            case GameState.START:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_playerCardFlipped(index, true);
                button.interactable = false;
               --CardHandler.Instance.playerFlipped;
                if(CardHandler.Instance.playerFlipped == 0)
                {
                    GameManager.Instance.Network_setGameState(GameState.PLAYER_READY);
                }   
                break;
            }
            case GameState.ENEMY_READY:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_playerCardFlipped(index, true);
                button.interactable = false;
                --CardHandler.Instance.playerFlipped;
                if(CardHandler.Instance.playerFlipped == 0)
                {
                    GameManager.Instance.Network_setGameState(GameState.PLAYER_READY);
                }
                break;
            }
            case GameState.PLAYER_DRAW:
            {
                if(faceUp == true)
                {
                    button.interactable = false;
                    GameManager.Instance.Network_setGameState(GameState.PLAYER_TURN, prevState);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_playerCardFlipped(index, true);
                CardHandler.Instance.Network_setPlayerSelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.PLAY, GameState.PLAYER_TURN);
                break;
            }
            case GameState.PEAK_ENEMY:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_playerCardFlipped(index, true);
                CardHandler.Instance.Network_setPlayerSelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break;
            }
            case GameState.BLIND_SWAP1:
            {
                CardHandler.Instance.Network_setPlayerSelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.BLIND_SWAP2, prevState);
                break; 
            }
            case GameState.BLIND_SWAP2:
            {
                CardHandler.Instance.Network_setPlayerSelectedCard(this);
                CardHandler.Instance.Network_swapCards();
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break; 
            }
            case GameState.SWAP1:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_playerCardFlipped(index, true);
                CardHandler.Instance.Network_setPlayerSelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_playerCardFlipped(index, true);
                CardHandler.Instance.Network_setPlayerSelectedCard(this);
                CardHandler.Instance.cardPlayed(null);                
                GameManager.Instance.Network_setGameState(GameState.SPECIAL_PLAY, prevState);
                break;
            }

        }
    }

    public void enemyCardClicked()
    {
        var currState = GameManager.Instance.currState;
        var prevState = GameManager.Instance.prevState;
        switch(currState)
        {
            case GameState.START:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_enemyCardFlipped(index, true);
                button.interactable = false;
                --CardHandler.Instance.enemyFlipped;
                if(CardHandler.Instance.enemyFlipped == 0)
                {
                    GameManager.Instance.Network_setGameState(GameState.ENEMY_READY);
                    GameManager.Instance.Network_setGameState(GameState.ENEMY_READY);
                }   
                break;
            }
            case GameState.PLAYER_READY:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_enemyCardFlipped(index, true);
                button.interactable = false;
                --CardHandler.Instance.enemyFlipped;
                if(CardHandler.Instance.enemyFlipped == 0)
                {
                    GameManager.Instance.Network_setGameState(GameState.ENEMY_READY);
                    GameManager.Instance.Network_setGameState(GameState.PLAYER_DRAW);
                }
                break;
            }
            case GameState.ENEMY_DRAW:
            {
                if(faceUp == true)
                {
                    int index = flipCard(false);
                    CardHandler.Instance.Network_enemyCardFlipped(index, true);
                    button.interactable = false;
                    GameManager.Instance.Network_setGameState(GameState.ENEMY_TURN, prevState);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_enemyCardFlipped(index, true);
                CardHandler.Instance.Network_setEnemySelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break;

            }
            case GameState.PEAK_ENEMY:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_enemyCardFlipped(index, true);
                CardHandler.Instance.Network_setEnemySelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break;

            }
            case GameState.BLIND_SWAP1:
            {
                CardHandler.Instance.Network_setEnemySelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.BLIND_SWAP2, prevState);
                break; 
            }
            case GameState.BLIND_SWAP2:
            {
                CardHandler.Instance.Network_setEnemySelectedCard(this);
                CardHandler.Instance.Network_swapCards();
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break; 
            }
            case GameState.SWAP1:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_enemyCardFlipped(index, true);
                CardHandler.Instance.Network_setEnemySelectedCard(this);
                GameManager.Instance.Network_setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                int index = flipCard(false);
                CardHandler.Instance.Network_enemyCardFlipped(index, true);
                CardHandler.Instance.Network_setEnemySelectedCard(this);
                CardHandler.Instance.cardPlayed(null);                
                GameManager.Instance.Network_setGameState(GameState.SPECIAL_PLAY, prevState);
                break;
            }
        }
    }
}
