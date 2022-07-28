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
        var currState = GameManager.Instance.currState;
        var prevState = GameManager.Instance.prevState;
        if(currState == GameState.START)
        {
            string hint = "Click on the flipped cards when you're ready!";
            player_hint.Text.text = hint;
            enemy_hint.Text.text = hint;
        }
        if(currState == GameState.PLAYER_READY)
        {
            player_hint.Text.text = "Waiting for opponent!";
            if(prevState == GameState.ENEMY_READY)
            {
                GameManager.Instance.setGameState(GameState.PLAYER_DRAW);
            }
        }
        if(currState == GameState.ENEMY_READY)
        {
            enemy_hint.Text.text = "Waiting for opponent!";
            if(prevState == GameState.PLAYER_READY)
            {
                GameManager.Instance.setGameState(GameState.PLAYER_DRAW);
            }
        }

        if(currState == GameState.PLAYER_DRAW)
        {
            player_hint.Text .text = "Draw a card or drag a previously played card in your area. Clik the flipped card when you're ready!";
        }

        if(currState == GameState.PLAYER_TURN)
        {
            player_hint.Text.text = "Place a card in the middle to play!";
        }
    }




}
