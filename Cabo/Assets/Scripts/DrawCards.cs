using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCards : MonoBehaviour
{
    public static DrawCards Instance;
    public PlayerCard emptyPlayerCard;
    public EnemyCard emptyEnemyCard; 

    public GameObject playerArea;
    public GameObject enemyArea;

    public GridLayoutGroup grid;

    void Awake()
    {
        Debug.Log("Enabled");
        GameManager.gameStateChanged += OnGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.gameStateChanged -= OnGameStateChanged;
    }

    public void OnGameStateChanged()
    {
        if(GameManager.Instance.currState == GameState.START)
        {
            FirstDistribute();
        }
    }

    public void FirstDistribute()
    {
        for(int i=0; i<4; ++i)
        {
            PlayerCard playerCard = Instantiate(emptyPlayerCard, new Vector2(0,0), Quaternion.identity);
            GameObject playerSlot = Instantiate(playerCard.slot, new Vector2(0,0), Quaternion.identity);
            playerCard.card = DeckHandler.getCard();
            playerSlot.transform.SetParent(playerArea.transform, false);
            playerCard.transform.SetParent(playerSlot.transform, false);
            
            EnemyCard enemyCard = Instantiate(emptyEnemyCard, new Vector2(0,0), Quaternion.identity);
            GameObject enemySlot = Instantiate(enemyCard.slot, new Vector2(0,0), Quaternion.identity);
            enemyCard.card = DeckHandler.getCard();
            enemySlot.transform.SetParent(enemyArea.transform, false);
            enemyCard.transform.SetParent(enemySlot.transform, false);
        }
    }
}
