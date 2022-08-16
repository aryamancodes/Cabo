using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayOption : MonoBehaviour
{
    public string optionName;
    public Button button = null;
    public TMP_Text Text = null;
    public void open()
    {
        gameObject.SetActive(true);
        if(button != null)
        {
            button.interactable = true;
        }
    }

    public void close()
    {
        gameObject.SetActive(false);
    }

    public void Awake()
    {
        if(button != null)
        {
            button.onClick.AddListener(Button_click); //subscribe to the onClick event
        }
    }
 
     //Handle the onClick event
    public void Button_click()
    {
        GameState prev = GameManager.Instance.prevState;
        button.interactable = false;
        if(optionName == "end_turn")
        {
            if(GameManager.Instance.currState == GameState.CABO)
            {
                GameManager.Instance.Network_setGameState(GameState.GAME_OVER);
            }   
            else if(GameManager.Instance.prevState == GameState.PLAYER_TURN)
            {
                GameManager.Instance.Network_setGameState(GameState.ENEMY_DRAW);
            }
            else if(GameManager.Instance.prevState == GameState.ENEMY_TURN)
            {
                GameManager.Instance.Network_setGameState(GameState.PLAYER_DRAW);
            }
        }

        if(optionName == "cabo")
        {
            if(GameManager.Instance.prevState == GameState.PLAYER_TURN)
            {
                GameManager.Instance.Network_setGameState(GameState.ENEMY_DRAW, GameState.CABO);
            }
            else
            {
                GameManager.Instance.Network_setGameState(GameState.PLAYER_DRAW, GameState.CABO);
            }        
        }

        if(optionName == "blind_swap")
        {
            GameManager.Instance.Network_setGameState(GameState.BLIND_SWAP1, prev);
        }

        if(optionName == "peak_and_swap")
        {
            GameManager.Instance.Network_setGameState(GameState.SWAP1, prev);
        }
        if(optionName == "swap")
        {
            CardHandler.Instance.Network_swapCards();
            GameManager.Instance.Network_setGameState(GameState.PLAY, prev);
        }
        if(optionName == "peak_player")
        {
            GameManager.Instance.Network_setGameState(GameState.PEAK_PLAYER, prev);
        }
        if(optionName == "peak_enemy")
        {
            GameManager.Instance.Network_setGameState(GameState.PEAK_ENEMY, prev);
        }
    }
}
