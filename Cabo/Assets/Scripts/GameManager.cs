using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


// All the possible game states, seralized as byte to send over the network
public enum GameState: byte
{ 
    START=0, PLAYER_READY, PLAYER_DRAW, PLAYER_TURN, ENEMY_READY, ENEMY_DRAW, ENEMY_TURN, SNAP_SELF, SNAP_OTHER, SNAP_FAIL, 
    BLIND_SWAP1, BLIND_SWAP2, SWAP1, SWAP2, PEAK_PLAYER, PEAK_ENEMY, PLAY, SPECIAL_PLAY, CABO, 
    GAME_OVER, REPLAY_PLAYER, REPLAY_ENEMY, REPLAY, PAUSE, QUIT, NONE 
}


/*
    Static class that represents the state of the game as a finite state machine.
    Each time the current state is changed, the gameStateChanged event is sent to
    all subscribers. Additionally, the layers used to identify player and enemy clicks
    are defined in this class.
*/
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public delegate void EventHandler();
    public static event EventHandler gameStateChanged;
    public GameState prevState;
    public GameState currState = GameState.NONE;

    public int playerLayer;
    public int enemyLayer;
    public int UILayer;
    public int IgnoreLayer;
    public bool canSnap = false;
    public PhotonView view;

    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        UILayer = LayerMask.NameToLayer("UI");
        IgnoreLayer = LayerMask.NameToLayer("Default");
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Set the current state of the FSM. Optionally set the prev state without sending event to subscriber
    // This is useful to identify the current players during special states such as swapping.
    public void localSetGameState(GameState newState, GameState prev=GameState.NONE)
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
    //Static wrapper and RPC to communicate setGameState() over the locally and over the network
    public void Network_setGameState(GameState newState, GameState prev=GameState.NONE)
    {
        localSetGameState(newState, prev);
        view.RPC(nameof(RPC_setGameState), RpcTarget.Others, (byte)newState, (byte) prev);
    }
    [PunRPC]
    public void RPC_setGameState(byte newState, byte prev)
    {
        localSetGameState( (GameState)newState, (GameState) prev);
    }
}
