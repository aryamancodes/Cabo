using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum GameState: byte
{ 
    START=0, PLAYER_READY, PLAYER_DRAW, PLAYER_TURN, ENEMY_READY, ENEMY_DRAW, ENEMY_TURN, SNAP_PASS, SNAP_FAIL, 
    BLIND_SWAP1, BLIND_SWAP2, SWAP1, SWAP2, PEAK_PLAYER, PEAK_ENEMY, PLAY, SPECIAL_PLAY, CABO, 
    GAME_OVER, REPLAY_NORMAL, REPLAY_FALSE_CABO, PAUSE, QUIT, NONE 
}


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

    [PunRPC]
    public void RPC_setGameState(byte newState, byte prev)
    {
        GameManager.Instance.setGameState( (GameState)newState, (GameState) prev);
    }

    public void Network_setGameState(GameState newState, GameState prev=GameState.NONE)
    {
        view.RPC(nameof(RPC_setGameState), RpcTarget.Others, (byte)newState, (byte) prev);
    }

    // Set the current state of the FSM. Optionally set the prev state without sending event to subsrcibers
    // This is useful to determine which player placed a card, for example during the ALL_TURN stage
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
