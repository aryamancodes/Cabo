using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
public class CardBase : ScriptableObject
{
    public Sprite shownFace;
    public Sprite cardFace;
    public Sprite hiddenFace;
    public int value;

    public enum Suit {Heart, Diamond, Spade, Club, Joker}
    public Suit suit;

    public bool isSpecialCard;

    public void Init(Sprite hidden, Sprite face, int value, Suit suit, bool isSpecialCard)
    {
        this.hiddenFace = hidden;
        this.cardFace = face;
        this.shownFace = face;
        this.value = value;
        this.suit = suit;
        this.isSpecialCard = isSpecialCard;
    }

    public static CardBase CreateInstance(Sprite hidden, Sprite face, int value, Suit suit,  bool isSpecialCard)
    {
        var data = ScriptableObject.CreateInstance<CardBase>();
       data.Init(hidden, face, value, suit, isSpecialCard);
       return data;
    }
}
