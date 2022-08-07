using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
public class CardBase : ScriptableObject
{
    public Sprite face;
    public int value;

    public enum Suit {Heart, Diamond, Spade, Club, Joker}
    public Suit suit;

    public bool isSpecialCard;

    public void Init(Sprite face, int value, Suit suit, bool isSpecialCard)
    {
        this.face = face;
        this.value = value;
        this.suit = suit;
        this.isSpecialCard = isSpecialCard;
    }

    public static CardBase CreateInstance(Sprite face, int value, Suit suit,  bool isSpecialCard)
    {
        var data = ScriptableObject.CreateInstance<CardBase>();
       data.Init(face, value, suit, isSpecialCard);
       return data;
    }
}
