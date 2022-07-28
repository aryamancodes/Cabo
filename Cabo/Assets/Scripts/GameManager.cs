using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ START, PLAYER_READY, PLAYER_DRAW, PLAYER_TURN, PLAYER_PLACE,
                        ENEMY_READY, ENEMY_DRAW, ENEMY_TURN, ENEMY_PLACE, ALL_TURN, 
                        BLINDSWAP, SWAP, PEAK, PLAY, CABO, 
                        GAME_OVER, REPLAY_NORMAL, REPLAY_FALSE_CABO, PAUSE, QUIT, NONE }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void EventHandler();
    public static event EventHandler gameStateChanged;
    public GameState prevState;
    public GameState currState = GameState.NONE;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        setGameState(GameState.START);
    }
    public void setGameState(GameState newState)
    {
        Debug.Log("NEW GAME STATE " + newState);
        prevState = currState;
        currState = newState;
        //send event to subscribers, if any
        if(gameStateChanged != null)
        {
            gameStateChanged();
        }
    }
}
