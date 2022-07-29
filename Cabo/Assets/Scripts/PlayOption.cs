using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayOption : MonoBehaviour
{
    public string optionName;
    public Button button;
    public TMP_Text Text;
    public void open()
    {
        gameObject.SetActive(true);
    }

    public void close()
    {
        gameObject.SetActive(false);
    }

    public void Awake()
    {
        if(button != null)
        {
            button.onClick.AddListener(Button_click); //subscribe to the onClick event
        }
    }
 
     //Handle the onClick event
    public void Button_click()
    {
        if(optionName == "end_turn")
        {
            if(GameManager.Instance.prevState == GameState.PLAYER_TURN)
            {
                GameManager.Instance.setGameState(GameState.ENEMY_DRAW);
            }
            else
            {
                GameManager.Instance.setGameState(GameState.PLAYER_DRAW);
            }
        }
    }
}
