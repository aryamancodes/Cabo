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
    public Sprite face_hidden;
    
    public CardBase.Suit suit;
    public int value;
    public bool isSpecialCard;
    public bool canDrag = false;
    public bool isHidden = false;

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

    public int getIndex()
    {
        int layer = gameObject.layer;
        if(layer == GameManager.Instance.playerLayer || layer == GameManager.Instance.enemyLayer)
        {
            return transform.parent.GetSiblingIndex();
        }
        return transform.GetSiblingIndex();
    }
    public int flipCard(bool hidden)
    {
        int index = getIndex();
        if(faceUp) { image.sprite = back; }
        if(!faceUp && hidden) 
        { 
            image.sprite = face_hidden; 
            isHidden =true;
        }
        if(!faceUp && !hidden) 
        { 
            image.sprite = face; 
            isHidden = false;
        }

        faceUp = !faceUp;
        return getIndex();
    }

    public int flipCard(string direction, bool hidden)
    {
        if(direction == "down")
        {
            image.sprite = back;
            faceUp = false;

        }
        else if(direction == "up" && hidden)
        {
            image.sprite = face_hidden;
            faceUp = true;
            Debug.Log("HIDDEN flipCard on " + getIndex() + " " + gameObject.layer);
            isHidden = true;

        }
        else if(direction == "up")
        {
            image.sprite = face;
            faceUp = true;
            Debug.Log("NORMAL flipCard on " + getIndex() + " " + gameObject.layer);
            isHidden = false;
        }
        return getIndex();
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
                int index = flipCard(false);
                CardHandler.Instance.Network_playerCardFlipped(index, true);
                button.interactable = false;
                --CardHandler.Instance.playerFlipped;
                if(CardHandler.Instance.playerFlipped == 0)
                {
                    GameManager.Instance.setGameState(GameState.PLAYER_READY);
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
                    GameManager.Instance.setGameState(GameState.PLAYER_READY);
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
                    GameManager.Instance.setGameState(GameState.PLAYER_TURN);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                //flipCard();
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.setGameState(GameState.PLAY, GameState.PLAYER_TURN);
                break;
            }
            case GameState.PEAK_ENEMY:
            {
                //flipCard();
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
                //flipCard();
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                //flipCard();
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
                int index = flipCard(false);
                CardHandler.Instance.Network_enemyCardFlipped(index, true);
                button.interactable = false;
                --CardHandler.Instance.enemyFlipped;
                if(CardHandler.Instance.enemyFlipped == 0)
                {
                    GameManager.Instance.setGameState(GameState.ENEMY_READY);
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
                    GameManager.Instance.setGameState(GameState.ENEMY_READY);
                    GameManager.Instance.Network_setGameState(GameState.ENEMY_READY);
                }
                break;
            }
            case GameState.ENEMY_DRAW:
            {
                if(faceUp == true)
                {
                    //flipCard();
                    button.interactable = false;
                    GameManager.Instance.setGameState(GameState.ENEMY_TURN);
                }
                break;
            }
            case GameState.PEAK_PLAYER:
            {
                //flipCard();                
                CardHandler.Instance.playerSelectedCard = this;
                GameManager.Instance.setGameState(GameState.PLAY, prevState);
                break;

            }
            case GameState.PEAK_ENEMY:
            {
                //flipCard();
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
                //flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                GameManager.Instance.setGameState(GameState.SWAP2, prevState);
                break;
            }
            case GameState.SWAP2:
            {
                //flipCard();
                CardHandler.Instance.enemySelectedCard = this;
                CardHandler.Instance.cardPlayed(null);                
                GameManager.Instance.setGameState(GameState.SPECIAL_PLAY, prevState);
                break;
            }
        }
    }
}
