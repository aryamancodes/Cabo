using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Image image;
    public Card card = null;
    public Sprite back;
    public GameObject slot;    
    public bool faceUp = false;

    public Button button;



    public Sprite face;
    private Card.Suit suit;
    private int value;
    private bool isSpecialCard;
    private bool hasCard;

    private Color startColor;

    

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
}
