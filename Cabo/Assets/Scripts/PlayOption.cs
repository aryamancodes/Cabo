using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayOption : MonoBehaviourPunCallbacks
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
        GameState curr = GameManager.Instance.currState;
        GameState prev = GameManager.Instance.prevState;
        button.interactable = false;
        if(optionName == "end_turn")
        {
            if(curr == GameState.CABO)
            {
                GameManager.Instance.Network_setGameState(GameState.GAME_OVER);
            }   
            else if(prev == GameState.PLAYER_TURN)
            {
                GameManager.Instance.Network_setGameState(GameState.ENEMY_DRAW);
            }
            else if(prev == GameState.ENEMY_TURN)
            {
                GameManager.Instance.Network_setGameState(GameState.PLAYER_DRAW);
            }
        }

        if(optionName == "cabo")
        {
            if(prev == GameState.PLAYER_TURN)
            {
                GameManager.Instance.Network_setGameState(GameState.ENEMY_DRAW, GameState.CABO);
            }
            else if(prev == GameState.ENEMY_TURN)
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
        if(optionName == "quit")
        {

        }
        if(optionName == "play_again")
        {
            if(curr == GameState.GAME_OVER)
            {
                if(PhotonNetwork.IsMasterClient) { GameManager.Instance.Network_setGameState(GameState.REPLAY_PLAYER); }
                else { GameManager.Instance.Network_setGameState(GameState.REPLAY_ENEMY); }
            }

            else if(curr == GameState.REPLAY_PLAYER || curr == GameState.REPLAY_ENEMY)
            {
                GameManager.Instance.Network_setGameState(GameState.REPLAY);
            }
        }

        if(optionName == "player_pause")
        {
            if(PhotonNetwork.IsMasterClient) 
            { 
                PlayOptionsManager.Instance.showOption("pause_menu", false); 
                PlayOptionsManager.Instance.showOption("pause_resume", false); 
                PlayOptionsManager.Instance.showOption("pause_leave", false); 
            }
        }
        if(optionName == "enemy_pause")
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                PlayOptionsManager.Instance.showOption("pause_menu", false); 
                PlayOptionsManager.Instance.showOption("pause_resume", false); 
                PlayOptionsManager.Instance.showOption("pause_leave", false); 
            }
        }      

        if(optionName == "pause_resume")
        {
            foreach(PlayOption option in PlayOptionsManager.Instance.options)
            {
                //close the pause menu
                if(option.optionName == "pause_menu")
                {
                    option.close();
                }
                //allow the pause button to be reclicked
                if(option.optionName == "player_pause" || option.optionName == "enemy_pause")
                {
                    option.button.interactable = true;
                }
            }
        }
    }
}
