using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image image;
    public CardBase card = null;
    public Sprite back;
    public GameObject slot;    
    public bool faceUp = false;

    public Button button;
    public Sprite face;
    public CardBase.Suit suit;
    public int value;
    public bool isSpecialCard;
    public bool canDrag = false;


    void Start()
    {
        if(this.card != null)
        {
            this.face = card.face;
            this.suit = card.suit;
            this.value = card.value;
            this.isSpecialCard = card.isSpecialCard;
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

    public void flipCard()
    {
        if(faceUp)
        {
            image.sprite = back;
            
        }
        else
        {
            image.sprite = face;
        }
        faceUp = !faceUp;
    }

    public void flipCard(string direction)
    {
        if(direction == "down")
        {
            image.sprite = back;
            faceUp = false;
        }
        else if(direction == "up")
        {
            image.sprite = face;
            faceUp = true;
        }
    }

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
                flipCard();
                // button.interactable = false;
                // --CardHandler.Instance.playerFlipped;
                // if(CardHandler.Instance.playerFlipped == 0)
                // {
                //     GameManager.Instance.setGameState(GameState.PLAYER_READY);
                // }   
                break;
            }
            case GameState.ENEMY_READY:
            {
                flipCard();
                // button.interactable = false;
                // --CardHandler.Instance.playerFlipped;
                // if(CardHandler.Instance.playerFlipped == 0)
                // {
                //     GameManager.Instance.setGameState(GameState.PLAYER_READY);
                // }
                break;
            }
            case GameState.PLAYER_DRAW:
            {
                Debug.Log("reaching hrere");
                if(this.faceUp == true)
                {
                    flipCard();
                    button.interactable = false;
                    GameManager.Instance.setGameState(GameState.PLAYER_TURN);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                flipCard();
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.setGameState(GameState.PLAY, GameState.PLAYER_TURN);
                break;
            }
            case GameState.PEAK_ENEMY:
            {
                flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.setGameState(GameState.PLAY, prevState);
                break;
            }
            case GameState.BLIND_SWAP1:
            {
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.setGameState(GameState.BLIND_SWAP2, prevState);
                break; 
            }
            case GameState.BLIND_SWAP2:
            {
                CardHandler.Instance.playerSelectedCard = this;
                CardHandler.Instance.swapCards();
                GameManager.Instance.setGameState(GameState.PLAY, prevState);
                break; 
            }
            case GameState.SWAP1:
            {
                flipCard();
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                flipCard();
                CardHandler.Instance.playerSelectedCard = this;
                CardHandler.Instance.cardPlayed(null);                
                GameManager.Instance.setGameState(GameState.SPECIAL_PLAY, prevState);
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
                flipCard();
                //button.interactable = false;
                // --CardHandler.Instance.enemyFlipped;
                // if(CardHandler.Instance.enemyFlipped == 0)
                // {
                //     GameManager.Instance.setGameState(GameState.ENEMY_READY);
                // }   
                break;
            }
            case GameState.PLAYER_READY:
            {
                flipCard();
                // button.interactable = false;
                // --CardHandler.Instance.enemyFlipped;
                // if(CardHandler.Instance.enemyFlipped == 0)
                // {
                //     GameManager.Instance.setGameState(GameState.ENEMY_READY);
                // }
                break;
            }
            case GameState.ENEMY_DRAW:
            {
                if(this.faceUp == true)
                {
                    flipCard();
                    button.interactable = false;
                    GameManager.Instance.setGameState(GameState.ENEMY_TURN);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                flipCard();                
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.setGameState(GameState.PLAY, prevState);
                break;

            }
            case GameState.PEAK_ENEMY:
            {
                flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.setGameState(GameState.PLAY, prevState);
                break;

            }
            case GameState.BLIND_SWAP1:
            {
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.setGameState(GameState.BLIND_SWAP2, prevState);
                break; 
            }
            case GameState.BLIND_SWAP2:
            {
                CardHandler.Instance.enemySelectedCard = this;
                CardHandler.Instance.swapCards();
                GameManager.Instance.setGameState(GameState.PLAY, prevState);
                break; 
            }
            case GameState.SWAP1:
            {
                flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                CardHandler.Instance.cardPlayed(null);                
                GameManager.Instance.setGameState(GameState.SPECIAL_PLAY, prevState);
                break;
            }
        }
    }
}
