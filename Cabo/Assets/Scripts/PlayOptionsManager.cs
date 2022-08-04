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
        { GameState.PLAYER_DRAW, new List<string>{"It's your turn! You can wraw a card or drag a a card from the discard pile in your area. Click the flipped card when you're ready!"
                                                , "Waiting for opponent to draw card!"} },
        { GameState.PLAYER_TURN, new List<string>{"Place a card in the middle to play!", "Waiting for opponent to play a card!"} },
        { GameState.PLAY, new List<string>{"Don't forget to end turn!", "Waiting for opponent to end turn!"} },
        { GameState.SPECIAL_PLAY, new List<string>{"Select a special option or end turn!", "Waiting for opponent to end turn!"} },
        { GameState.PEAK_PLAYER, new List<string>{"Select one of the available cards to peak!", "Waiting for opponent to peak!"} },
        { GameState.PEAK_ENEMY, new List<string>{"Select one of the available cards to peak!", "Waiting for opponent to peak!"} },
        { GameState.BLIND_SWAP1, new List<string>{"Select one of your cards to swap!", "Waiting for opponent to swap cards!"} },
        { GameState.BLIND_SWAP2, new List<string>{"Select one of the opponent's cards to swap!", "Waiting for opponent to swap cards!"} },
        { GameState.SWAP1, new List<string>{"Select one of your cards to peak and swap if you wish!", "Waiting for opponent to peak & swap cards!"} },
        { GameState.SWAP2, new List<string>{"Select one of the opponent's cards to peak and swap if you wish!", "Waiting for opponent to peak & swap cards!"} },
        { GameState.SNAP_PASS, new List<string>{"You've succesfully snapped a card :) Drag one of your cards into the opponent's area", "Your card got snapped!"} },
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
        hideAllOptions();
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

        if(currState == GameState.SPECIAL_PLAY)
        {
            var lastPlayed = CardHandler.Instance.played;

            if(lastPlayed == null)
            {
                showOption("swap");
                return;
            }

            int value = lastPlayed.card.value;

            if(value == 7 || value == 8)
            {
                showOption("peak_player");
            }
            if(value == 9 || value == 10)
            {
                showOption("peak_enemy");
            }
            if(value == 11 || value == 12)
            {
                showOption("blind_swap");
            }
            if(value == 13)
            {
                showOption("peak_and_swap");
            }
        }

        if(currState == GameState.PEAK_PLAYER || currState == GameState.PEAK_ENEMY || currState == GameState.SWAP1 || currState == GameState.SWAP2
        || currState == GameState.BLIND_SWAP1 || currState == GameState.BLIND_SWAP2)
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
        }

    }
}
