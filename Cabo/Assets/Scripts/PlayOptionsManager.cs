using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayOptionsManager : MonoBehaviour
{
    //Display appropriate playing options - based on the currState of the GameManager
    public List<PlayOption> options;

    void Awake()
    {
        GameManager.gameStateChanged += OnGameStateChanged;
        hideAllOptions();

    }
    void OnDisable()
    {
        GameManager.gameStateChanged -= OnGameStateChanged;
    }

    public void showOption(string name)
    {
        foreach(PlayOption option in options)
        {
            if (option.optionName == name)
            {
                option.open();
            }
        }
    }

    public PlayOption getOption(string name)
    {
        PlayOption ret = null;
        foreach(PlayOption option in options)
        {
            if (option.optionName == name)
            {
                ret = option;
            }
        }
        return ret;
    }
    public void hideAllOptions ()
    {
        foreach(PlayOption option in options)
        {
            option.close();
        }
    }

    public void OnGameStateChanged()
    {
        if(GameManager.Instance.currState == GameState.START)
        {
            showOption("player_ready");
            showOption("enemy_ready");
        }
    }

    public void button_playerReadyClicked()
    {
        getOption("player_ready").button.interactable = false;
        GameManager.Instance.setGameState(GameState.PLAYER_READY);
        checkBothReady();
    }

     public void button_enemyReadyClicked()
    {
        getOption("enemy_ready").button.interactable = false;
        GameManager.Instance.setGameState(GameState.ENEMY_READY);
        checkBothReady();
    }

    public void checkBothReady()
    {
        if(!getOption("player_ready").button.interactable && !getOption("enemy_ready").button.interactable)
        {
            GameManager.Instance.setGameState(GameState.PLAYER_TURN);
            hideAllOptions();
        }
    }


}
