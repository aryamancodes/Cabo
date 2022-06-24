using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject card1;
    public GameObject card2;
    public GameObject playerArea;
    public GameObject enemyArea;

    public void OnClick()
    {
        for(int i=0; i<4; ++i)
        {
            GameObject playerCard = Instantiate(card1, new Vector3(0,0,0), Quaternion.identity);
            playerCard.transform.SetParent(playerArea.transform, false);

            GameObject enemyCard = Instantiate(card2, new Vector3(0,0,0), Quaternion.identity);
            enemyCard.transform.SetParent(enemyArea.transform, false); 
        }

    }
}
