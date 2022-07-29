using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{ START, PLAYER_READY, PLAYER_DRAW, PLAYER_TURN,
                        ENEMY_READY, ENEMY_DRAW, ENEMY_TURN, ALL_TURN, 
                        BLIND_SWAP1, BLIND_SWAP2, SWAP1, SWAP2, PEAK_PLAYER, PEAK_ENEMY, PLAY, SPECIAL_PLAY, CABO, 
                        GAME_OVER, REPLAY_NORMAL, REPLAY_FALSE_CABO, PAUSE, QUIT, NONE }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void EventHandler();
    public static event EventHandler gameStateChanged;
    public GameState prevState;
    public GameState currState = GameState.NONE;

    public int playerLayer;
    public int enemyLayer;
    public int UILayer;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        UILayer = LayerMask.NameToLayer("UI");
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        setGameState(GameState.START);
    }

    //set the current state of the FSM. Optionally set the prev state without sending event to subsrcibers
    //This is useful to determine which player placed a card, for example during the ALL_TURN stage
    public void setGameState(GameState newState, GameState prev=GameState.NONE)
    {
        Debug.Log("NEW GAME STATE " + newState);
        if(prev != GameState.NONE){ prevState = prev; }
        else{ prevState = currState; }
        currState = newState;
        //send event to subscribers, if any
        if(gameStateChanged != null)
        {
            gameStateChanged();
        }
    }
}
