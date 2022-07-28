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
    public bool isPlayerCard; 

    

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
        var currState = GameManager.Instance.currState;
        switch(currState)
        {
            case GameState.START:
            {
                flipCard();
                button.interactable = false;
                --CardHandler.playerFlipped;
                if(CardHandler.playerFlipped == 0)
                {
                    GameManager.Instance.setGameState(GameState.PLAYER_READY);
                }   
                break;
            }
            case GameState.ENEMY_READY:
            {
                flipCard();
                button.interactable = false;
                --CardHandler.playerFlipped;
                if(CardHandler.playerFlipped == 0)
                {
                    GameManager.Instance.setGameState(GameState.PLAYER_READY);
                }
                break;
            }
            case GameState.PLAYER_DRAW:
            {
                flipCard();
                button.interactable = false;
                --CardHandler.playerFlipped;
                if(CardHandler.playerFlipped == 0)
                {
                    GameManager.Instance.setGameState(GameState.PLAYER_TURN);
                }
                break;
            }
        }
    }
}
