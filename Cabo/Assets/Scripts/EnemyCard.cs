using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCard : MonoBehaviour
{
    public Image image;
    public Card card = null;
    public Sprite back;
    public GameObject slot;

    public bool faceUp = false;
    public Button button;


    private Sprite face;
    private Card.Suit suit;
    private int value;
    private bool isSpecialCard;
    private bool hasCard;


     void Start()
    {
        if(this.card != null && !hasCard)
        {
            this.face = card.face;
            this.suit = card.suit;
            this.value = card.value;
            this.isSpecialCard = card.isSpecialCard;
        }
        
    }

     public void flipCard()
    {
        if(faceUp)
        {
            image.sprite = back;
            
        }
        else
        {
            image.sprite = face;
        }
        faceUp = !faceUp;
    }

    public void flipCard(string direction)
    {
        if(direction == "down")
        {
            image.sprite = back;
            faceUp = false;
        }
        else if(direction == "up")
        {
            image.sprite = face;
            faceUp = true;
        }
    }
}
