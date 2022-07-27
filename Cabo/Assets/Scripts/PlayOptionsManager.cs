using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayOptionsManager : MonoBehaviour
{
    //Display appropriate playing options - based on the currState of the GameManager
    //Play options are both text hints to the players and action buttons for special cards
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

    public PlayOption showOption(string name)
    {
        PlayOption ret = null;
        foreach(PlayOption option in options)
        {
            if (option.optionName == name)
            {
                option.open();
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
        PlayOption player_hint = showOption("player_text");
        PlayOption enemy_hint = showOption("enemy_text");
        if(GameManager.Instance.currState == GameState.START)
        {
            string hint = "Click on the flipped cards when you're ready!";
            player_hint.Text.text = hint;
            enemy_hint.Text.text = hint;
        }
        if(GameManager.Instance.currState == GameState.PLAYER_READY)
        {
            player_hint.Text.text = "Waiting for opponent to be ready!";
        }
        if(GameManager.Instance.currState == GameState.ENEMY_READY)
        {
            enemy_hint.Text.text = "Waiting for opponent to be ready!";
        }
    }

    // public void checkBothReady()
    // {
    //     if(!getOption("player_ready").button.interactable && !getOption("enemy_ready").button.interactable)
    //     {
    //         GameManager.Instance.setGameState(GameState.PLAYER_TURN);
    //         hideAllOptions();
    //     }
    // }


}
