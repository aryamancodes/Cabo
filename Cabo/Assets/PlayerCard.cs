using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    public Card card;
    public Sprite back;

    private Sprite face;
    private Card.Suit suit;
    private int value;
    private bool isSpecialCard;
    // Start is called before the first frame update
    void Start()
    {
        this.face = card.face;
        this.suit = card.suit;
        this.value = card.value;
        this.isSpecialCard = card.isSpecialCard;
        
    }
}
