using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


/*
    Static class that handles the distributing, flipping and other
    player/enemy interactions with cards. 
*/
public class CardHandler : MonoBehaviourPunCallbacks
{
    public static CardHandler Instance;
    public Card emptyCard; 
    public Sprite playerBack;
    public Sprite enemyBack;

    public GameObject playerArea;
    public GameObject enemyArea;
    public GameObject placeArea;

    public Button deck; 
    public int playerFlipped = 0;
    public int enemyFlipped = 0;

    public Card playerSelectedCard = null;
    public Card enemySelectedCard = null;

    public Card played;
    public PhotonView view;

    void Awake()
    {
        GameManager.gameStateChanged += OnGameStateChanged;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDisable()
    {
        GameManager.gameStateChanged -= OnGameStateChanged;
    }

    void Start()
    {
        view = GetComponent<PhotonView>();
        if(PhotonNetwork.IsMasterClient)  
        {
            view.RPC("generateDeck", RpcTarget.All, Random.Range(1,300));
        }   
    }

    // Generate the same unique deck across all clients
    [PunRPC]
    public void generateDeck(int seed)
    {
        DeckGenerator.Instance.generateDeck(seed);
        GameManager.Instance.setGameState(GameState.START);
    }


    public void OnGameStateChanged()
    {
        var currState = GameManager.Instance.currState;
        var prevState = GameManager.Instance.prevState;
        if(currState == GameState.START)
        {
            setDrawCardsAndArea(false, false);
            firstDistribute();
        }
 
        if(currState == GameState.PLAYER_DRAW)
        {
            flipDownAllCards();
            overrideSpecialCard(); 
            if(PhotonNetwork.IsMasterClient)
            {
                setDrawCardsAndArea(true, true); 
                setPlayerClickDragAndArea(true, true, true);
                setEnemyClickDragAndArea(false, false, false);
            }
            else 
            { 
                setDrawCardsAndArea(false, false); 
                setPlayerClickDragAndArea(false, false, false);
                setEnemyClickDragAndArea(false, false, false);
            }
        }

        if(currState == GameState.ENEMY_DRAW)
        {
            flipDownAllCards();
            overrideSpecialCard(); 
            if(!PhotonNetwork.IsMasterClient)
            {
                setDrawCardsAndArea(true, true); 
                setPlayerClickDragAndArea(false, false, false);
                setEnemyClickDragAndArea(true, true, true);
            }
            else 
            { 
                setDrawCardsAndArea(false, false); 
                setPlayerClickDragAndArea(false, false, false);
                setEnemyClickDragAndArea(false, false, false);
            }
        }

        if(currState == GameState.PLAY || currState == GameState.SPECIAL_PLAY)
        {
            setDrawCardsAndArea(false, false);
            setPlayerClickDragAndArea(false, false, false);
            setEnemyClickDragAndArea(false, false, false);
        }

        if(currState == GameState.PLAYER_TURN)
        {
            flipDownAllCards();
            setDrawCardsAndArea(false, true);
            setPlayerClickDragAndArea(true, true, false);
            setEnemyClickDragAndArea(false, false, false);
        }

        if(currState == GameState.ENEMY_TURN)
        {
            flipDownAllCards();
            setDrawCardsAndArea(false, true);
            setPlayerClickDragAndArea(false, true, false);
            setEnemyClickDragAndArea(true, true, false);
        }

        if(currState == GameState.SWAP1 || currState == GameState.BLIND_SWAP1 || currState == GameState.PEAK_PLAYER)
        {
            if(prevState == GameState.PLAYER_TURN)
            {
                setPlayerClickDragAndArea(true, false, false);
                setEnemyClickDragAndArea(false, false, false);
            }
            else if(prevState == GameState.ENEMY_TURN)
            {
                setPlayerClickDragAndArea(false, false, false);
                setEnemyClickDragAndArea(true, false, false);
            }
        }

         if(currState == GameState.SWAP2 || currState == GameState.BLIND_SWAP2 || currState == GameState.PEAK_ENEMY)
        {
            if(prevState == GameState.PLAYER_TURN)
            {
                setPlayerClickDragAndArea(false, false, false);
                setEnemyClickDragAndArea(true, false, false);
            }
            else if(prevState == GameState.ENEMY_TURN)
            {
                setPlayerClickDragAndArea(true, false, false);
                setEnemyClickDragAndArea(false, false, false);
            }
        }

        if(currState == GameState.SNAP_PASS)
        {
            setDrawCardsAndArea(false, false);
            if(prevState == GameState.PLAYER_TURN)
            {
                setPlayerClickDragAndArea(true, true, false);
                setEnemyClickDragAndArea(false, false, true);
            }
            if(prevState == GameState.ENEMY_TURN)
            {
                setPlayerClickDragAndArea(false, true, true);
                setEnemyClickDragAndArea(true, true, false);
            }
        }
    }

    // Static wrappers and RPCs for flipping a card
    public void Network_playerCardFlipped(int index, bool hidden, string direction="")
    {
        view.RPC(nameof(RPC_playerCardFlipped), RpcTarget.Others, index, hidden, direction);
    }
    public void Network_enemyCardFlipped(int index, bool hidden, string direction="")
    {
        view.RPC(nameof(RPC_enemyCardFlipped), RpcTarget.Others, index, hidden, direction);
    }
    
    [PunRPC]
    public void RPC_playerCardFlipped(int index, bool hidden, string direction)
    {
        //if(PhotonNetwork.IsMasterClient){ return; }
        if(direction != "") { playerArea.transform.GetChild(index).GetChild(0).GetComponent<Card>().flipCard(direction, hidden); }
        else { playerArea.transform.GetChild(index).GetChild(0).GetComponent<Card>().flipCard(hidden); }   
    }

    [PunRPC]
    public void RPC_enemyCardFlipped(int index, bool hidden, string direction)
    {
        if (direction != "") { enemyArea.transform.GetChild(index).GetChild(0).GetComponent<Card>().flipCard(direction, hidden); }
        else { enemyArea.transform.GetChild(index).GetChild(0).GetComponent<Card>().flipCard(hidden); }
    }


     public void firstDistribute()
    {
        for(int i=0; i<4; ++i)
        {    
            Card playerCard = Instantiate(emptyCard, new Vector2(0,0), Quaternion.identity);
            GameObject playerSlot = Instantiate(playerCard.slot, new Vector2(0,0), Quaternion.identity); 
            playerCard.card = DeckGenerator.Instance.getCard();
            playerCard.back = playerBack;
            playerSlot.transform.SetParent(playerArea.transform, false);
            playerCard.transform.SetParent(playerSlot.transform, false);
            playerCard.gameObject.layer = playerArea.layer;
            playerSlot.gameObject.layer = playerArea.layer;
             if(i%2 == 1 && PhotonNetwork.IsMasterClient)
            {
                playerCard.flipCard("up", false);
                playerFlipped = 2;     

            }
            else { playerCard.button.interactable = false; }

                
            Card enemyCard  = Instantiate(emptyCard, new Vector2(0,0), Quaternion.identity);
            GameObject enemySlot = Instantiate(enemyCard.slot, new Vector2(0,0), Quaternion.identity);
            enemyCard.card = DeckGenerator.Instance.getCard();
            enemyCard.back = enemyBack;
            enemySlot.transform.SetParent(enemyArea.transform, false);
            enemyCard.transform.SetParent(enemySlot.transform, false);
            enemyCard.gameObject.layer = enemyArea.layer;
            enemySlot.gameObject.layer = enemyArea.layer;
            if(i%2 == 0 && !PhotonNetwork.IsMasterClient)
            {
                enemyCard.flipCard("up", false);
                enemyFlipped = 2;

            }
            else { enemyCard.button.interactable = false; }

        }
    }
   

    public void Button_onDrawCard()
    {
        Card drawnCard = Instantiate(emptyCard, new Vector2(0,0), Quaternion.identity);
        GameObject slot = Instantiate(emptyCard.slot, new Vector2(0,0), Quaternion.identity); 
        drawnCard.card = DeckGenerator.Instance.getCard();
        if(GameManager.Instance.currState == GameState.PLAYER_DRAW)
        {
            slot.layer = GameManager.Instance.playerLayer;
            drawnCard.back = playerBack;
            insertDrawnCard(playerArea, slot, drawnCard);
            playerSelectedCard = drawnCard;
            setPlayerClickDragAndArea(false, false, true);
            setEnemyClickDragAndArea(false, false, false);
        }
        else if(GameManager.Instance.currState == GameState.ENEMY_DRAW)
        {
            slot.layer = GameManager.Instance.enemyLayer;
            drawnCard.back = enemyBack;
            enemySelectedCard = drawnCard;
            insertDrawnCard(enemyArea, slot, null,drawnCard);
            setPlayerClickDragAndArea(false, false, false);
            setEnemyClickDragAndArea(false, false, false);

        }
        setDrawCardsAndArea(false, false);
    }

    public void insertDrawnCard(GameObject area, GameObject slot, Card playerCard=null, Card enemyCard=null)
    {
        Transform card;
        if(playerCard != null) 
        {
            card = playerCard.transform; 
            //playerCard.//flipCard();

        }
        else 
        { 
            card = enemyCard.transform;
            //enemyCard.//flipCard();
        }
        //insert into existing slot
        foreach(Transform child in area.transform)
        {
            if(child.childCount == 0)
            {
                card.SetParent(child, true);
                card.gameObject.layer = area.layer;
                card.rotation = Quaternion.identity;
                //destroy unnecessary slot creation and fix weird scale increase
                card.localScale = new Vector3(1,1,1);
                Destroy(slot);
                return;
            }
        }
        //insert as a new slot 
        slot.transform.SetParent(area.transform, false);
        card.SetParent(slot.transform, false);
        card.gameObject.layer = area.layer;
    }

    // If a card is drawn and not played immediately, it is no longer special.
    // This function disables the speciality of such cards.
    public void overrideSpecialCard()
    {

        foreach(Transform child in playerArea.transform)
        {
            if(child.childCount != 0)
            {
                child.GetChild(0).GetComponent<Card>().card.isSpecialCard = false;
            }
        }

        foreach(Transform child in enemyArea.transform)
        {
            if(child.childCount != 0)
            {
                child.GetChild(0).GetComponent<Card>().card.isSpecialCard = false;
            }
        }

        foreach(Transform child in placeArea.transform)
        {
            child.GetComponent<Card>().card.isSpecialCard = false;
        }
    }

    public void flipDownAllCards()
    {
        if(playerSelectedCard != null)
        {
            playerSelectedCard.flipCard("down", false);
            playerSelectedCard.button.interactable = false;
            playerSelectedCard.canDrag = false;
            playerSelectedCard = null;
        }

        if(enemySelectedCard != null)
        {
            enemySelectedCard.flipCard("down", false);
            enemySelectedCard.button.interactable = false;
            enemySelectedCard.canDrag = false;
            enemySelectedCard = null;
        }

    }

    public void setDrawCardsAndArea(bool drawVal, bool areaVal)
    {
        deck.interactable = drawVal;
        foreach(Transform child in placeArea.transform)
        {
            var card = child.GetComponent<Card>();
            card.button.interactable = drawVal;
            card.canDrag = drawVal;
        }
        if(areaVal) { placeArea.layer = GameManager.Instance.UILayer; }
        else { placeArea.layer =  GameManager.Instance.IgnoreLayer; }
    }

    public void setPlayerClickDragAndArea(bool clickVal, bool dragVal, bool areaVal)
    {
        foreach(Transform child in playerArea.transform)
        {
            if(child.childCount != 0)
            {
                Card card = child.GetChild(0).GetComponent<Card>();
                if(card == playerSelectedCard || card == enemySelectedCard){ continue; }
                card.canDrag = dragVal;
                card.button.interactable = clickVal;
            }
        }
       playerArea.GetComponent<Rigidbody2D>().simulated = areaVal;
    }

    public void setEnemyClickDragAndArea(bool clickVal, bool dragVal, bool areaVal)
    {
        foreach(Transform child in enemyArea.transform)
        {
            if(child.childCount != 0)
            {
                Card card = child.GetChild(0).GetComponent<Card>();
                if(card == playerSelectedCard || card == enemySelectedCard){ continue; }
                card.canDrag = dragVal;
                card.button.interactable = clickVal;
            }
        }
        enemyArea.GetComponent<Rigidbody2D>().simulated = areaVal;
    }

    public void cardPlayed(Card card)
    {
        played = card;
        if(played != null)
        {
            if(card.isSpecialCard)
            {
                GameManager.Instance.setGameState(GameState.SPECIAL_PLAY);
            }
            else
            {
                GameManager.Instance.setGameState(GameState.PLAY);
            }   
        }
    }

    public void swapCards()
    {
        var playerParent = playerSelectedCard.transform.parent;
        var enemyparent = enemySelectedCard.transform.parent;
        playerSelectedCard.transform.SetParent(enemyparent);
        enemySelectedCard.transform.SetParent(playerParent);
        playerSelectedCard.gameObject.layer = GameManager.Instance.enemyLayer;
        enemySelectedCard.gameObject.layer = GameManager.Instance.playerLayer;
    }

    public void checkSnapped(GameState who)
    {
        int length = placeArea.transform.childCount;
        if(length >=2 )
        {
            int lastPlayedCard = placeArea.transform.GetChild(length-2).GetComponent<Card>().value;
            int snappedCard =  placeArea.transform.GetChild(length-1).GetComponent<Card>().value;
            //check for same card values or the special case where red kings have a different value to black kings
            if(lastPlayedCard == snappedCard || lastPlayedCard == 13 && snappedCard == -1 || lastPlayedCard == 13 && snappedCard == -1)
            {
                GameManager.Instance.setGameState(GameState.SNAP_PASS, who);
                return;
            }
        }
        GameManager.Instance.setGameState(GameState.SNAP_FAIL, who);
    }
}
