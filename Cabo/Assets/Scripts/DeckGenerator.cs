using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class DeckGenerator: MonoBehaviour
{
    public List<Sprite> heartCards = new List<Sprite>();
    public List<Sprite> diamondCards = new List<Sprite>();
    public List<Sprite> spadeCards = new List<Sprite>();
    public List<Sprite> clubCards = new List<Sprite>();
    public List<Sprite> jokers = new List<Sprite>(); 
    
    public static List<CardBase> deck = new List<CardBase>();

    void Awake()
    {
        Dictionary< CardBase.Suit, List<Sprite> > dict = new Dictionary< CardBase.Suit, List<Sprite> >();
        dict.Add(CardBase.Suit.Heart, heartCards);
        dict.Add(CardBase.Suit.Diamond, diamondCards);
        dict.Add(CardBase.Suit.Spade, spadeCards);
        dict.Add(CardBase.Suit.Club, clubCards);
        dict.Add(CardBase.Suit.Joker, jokers);
        

        foreach(CardBase.Suit suit in Enum.GetValues(typeof(CardBase.Suit)))
        {
            
            if(suit == CardBase.Suit.Joker)
            {
                deck.Add(CardBase.CreateInstance(dict[CardBase.Suit.Joker][0], -1, CardBase.Suit.Joker, false));
                deck.Add(CardBase.CreateInstance(dict[CardBase.Suit.Joker][1], -1, CardBase.Suit.Joker, false));
                continue;
            }

            for(int i=0; i<13; ++i)
            {
                bool special = i>=6; //special cases for all cards greater than 7 except red kings

                //red kings have a special value but aren't special card when played
                if(i == 12 && (suit == CardBase.Suit.Heart || suit == CardBase.Suit.Diamond))
                {
                    deck.Add(CardBase.CreateInstance(dict[suit][i], 0, suit, false));
                    continue;
                }

                deck.Add(CardBase.CreateInstance(dict[suit][i], i+1, suit, special));
            }
        }
        System.Random rng = new System.Random();
        Shuffle(deck);
    }

    //Fisher-Yates shuffle in-place
    void Shuffle(List<CardBase> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static CardBase getCard()
    {
        var card = deck[0];
        deck.RemoveAt(0);
        return card;
    }
}
