using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public PlayerCard emptyPlayerCard;
    public EnemyCard emptyEnemyCard; 

    public GameObject playerArea;
    public GameObject enemyArea;

    public void OnClick()
    {
        for(int i=0; i<4; ++i)
        {
            PlayerCard playerCard = Instantiate(emptyPlayerCard, new Vector2(0,0), Quaternion.identity);
            playerCard.card = DeckHandler.getCard();
            playerCard.transform.SetParent(playerArea.transform, false);

            EnemyCard enemyCard = Instantiate(emptyEnemyCard, new Vector2(0,0), Quaternion.identity);
            enemyCard.card = DeckHandler.getCard();
            enemyCard.transform.SetParent(enemyArea.transform, false); 
        }

    }
}
