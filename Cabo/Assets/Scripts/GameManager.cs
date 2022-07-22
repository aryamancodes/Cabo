using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ START, PLAYER_TURN, ENEMY_TURN, ALL_TURN, BLINDSWAP, SWAP, PEAK, PLAY, CABO, GAME_OVER, REPLAY_NORMAL, REPLAY_FALSE_CABO, PAUSE, QUIT, NONE }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void EventHandler();
    public static event EventHandler gameStateChanged;
    public GameState prevState;
    public GameState currState;// = GameState.NONE;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        //setGameState(GameState.START);
    }
    public void setGameState(GameState newState)
    {
        Debug.Log("NEW GAME STATE " + newState);
        prevState = currState;
        currState = newState;
        // switch(newState)
        // {
        //     // case GameState.START:
        //     //     DrawCards.FirstDistribute();
        //     //     break;
        // }

        //send event to subscribers, if any
        if(gameStateChanged != null)
        {
            gameStateChanged();
        }
    }


    void Update()
    {
        
    }
}
