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
    public Sprite face;
    public Sprite face_hidden;
    
    public CardBase.Suit suit;
    public int value;
    public bool isSpecialCard;
    public bool canDrag = false;

    void Start()
    {
        if(card != null)
        {
            face = card.face;
            suit = card.suit;
            value = card.value;
            isSpecialCard = card.isSpecialCard;
        } 
        if(faceUp)
        {
            image.sprite = face;
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
        if(faceUp) { image.sprite = back; }

        // Modify the base card face directly, otherwise changes don't sync
        if(!faceUp && hidden) { card.face = face_hidden; }

        if(!faceUp && !hidden) {  image.sprite = face; }

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
            faceUp = false;

        }

        // Modify the base card face directly, otherwise changes don't sync
        else if(direction == "up" && hidden)
        {
            card.face = face_hidden;
            faceUp = true;
        }
        
        else if(direction == "up")
        {
            image.sprite = face;
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
                    //flipCard();
                    button.interactable = false;
                    GameManager.Instance.Network_setGameState(GameState.PLAYER_TURN);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                //flipCard();
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.PLAY, GameState.PLAYER_TURN);
                break;
            }
            case GameState.PEAK_ENEMY:
            {
                //flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break;
            }
            case GameState.BLIND_SWAP1:
            {
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.BLIND_SWAP2, prevState);
                break; 
            }
            case GameState.BLIND_SWAP2:
            {
                CardHandler.Instance.playerSelectedCard = this;
                CardHandler.Instance.swapCards();
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break; 
            }
            case GameState.SWAP1:
            {
                //flipCard();
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                //flipCard();
                CardHandler.Instance.playerSelectedCard = this;
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
                //CardHandler.Instance.Network_enemyCardFlipped(index, true);
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
                //CardHandler.Instance.Network_enemyCardFlipped(index, true);
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
                    //flipCard();
                    button.interactable = false;
                    GameManager.Instance.Network_setGameState(GameState.ENEMY_TURN);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                //flipCard();                
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break;

            }
            case GameState.PEAK_ENEMY:
            {
                //flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break;

            }
            case GameState.BLIND_SWAP1:
            {
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.BLIND_SWAP2, prevState);
                break; 
            }
            case GameState.BLIND_SWAP2:
            {
                CardHandler.Instance.enemySelectedCard = this;
                CardHandler.Instance.swapCards();
                GameManager.Instance.Network_setGameState(GameState.PLAY, prevState);
                break; 
            }
            case GameState.SWAP1:
            {
                //flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.Network_setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                //flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                CardHandler.Instance.cardPlayed(null);                
                GameManager.Instance.Network_setGameState(GameState.SPECIAL_PLAY, prevState);
                break;
            }
        }
    }
}
