using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayOptionsManager : MonoBehaviour
{
    //Display appropriate playing options - based on the currState of the GameManager
    //Play options are both text hints to the players and action buttons for special cards
    public List<PlayOption> options;


    //dictionary of hints of the form currState -> {active player hint, waiting player hint}
    Dictionary<GameState, List<string>> dict = new Dictionary<GameState, List<string>>()
    {
        { GameState.START, new List<string>{"Click on the flipped cards when you're ready!", "Click on the flipped cards when you're ready!"} },
        { GameState.PLAYER_READY, new List<string>{"Waiting for opponent!", "Click on the flipped cards when you're ready!"} },
        { GameState.PLAYER_DRAW, new List<string>{"Draw a card or drag a previously played card in your area. Click the flipped card when you're ready!"
                                                , "Waiting for opponent to draw card!"} },

        { GameState.PLAYER_TURN, new List<string>{"Place a card in the middle to play!", "Waiting for opponent to play a card!"} },
        { GameState.PLAY, new List<string>{"Don't forget to end turn!", "Waiting for opponent to end turn!"} },
        { GameState.SPECIAL_PLAY, new List<string>{"Don't forget to end turn!", "Waiting for opponent to end turn!"} },
    };

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
            player_hint.Text.text = dict[currState][0];
            enemy_hint.Text.text = dict[currState][0];
        }
        if(currState == GameState.PLAYER_READY)
        {
            player_hint.Text.text = dict[currState][0];
            if(prevState == GameState.ENEMY_READY)
            {
                GameManager.Instance.setGameState(GameState.PLAYER_DRAW);
            }
        }
        if(currState == GameState.ENEMY_READY)
        {
            enemy_hint.Text.text = dict[GameState.PLAYER_READY][0];
            if(prevState == GameState.PLAYER_READY)
            {
                GameManager.Instance.setGameState(GameState.PLAYER_DRAW);
            }
        }

         if(currState == GameState.PLAYER_DRAW)
        {
            player_hint.Text.text = dict[currState][0];
            enemy_hint.Text.text = dict[currState][1];
        }
        if(currState == GameState.ENEMY_DRAW)
        {
            player_hint.Text.text = dict[GameState.PLAYER_DRAW][1];
            enemy_hint.Text.text = dict[GameState.PLAYER_DRAW][0];
        }


        if(currState == GameState.PLAYER_TURN)
        {
            player_hint.Text.text = dict[currState][0];
            enemy_hint.Text.text = dict[currState][1];
        }

        if(currState == GameState.ENEMY_TURN)
        {
            player_hint.Text.text = dict[GameState.PLAYER_TURN][1];
            enemy_hint.Text.text = dict[GameState.PLAYER_TURN][0];
        }


        if(currState == GameState.PLAY || currState == GameState.SPECIAL_PLAY)
        {
            if(prevState == GameState.PLAYER_TURN)
            {
                player_hint.Text.text = dict[currState][0];
                enemy_hint.Text.text = dict[currState][1];
            }
            else if(prevState == GameState.ENEMY_TURN)
            {
                player_hint.Text.text = dict[currState][1];
                enemy_hint.Text.text = dict[currState][0]; 
            }
            showOption("end_turn");
        }
    }

}
