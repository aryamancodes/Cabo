using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;

public class DeckHandler: MonoBehaviour
{
    public List<Sprite> heartCards = new List<Sprite>();
    public List<Sprite> diamondCards = new List<Sprite>();
    public List<Sprite> spadeCards = new List<Sprite>();
    public List<Sprite> clubCards = new List<Sprite>();
    public List<Sprite> jokers = new List<Sprite>(); 
    
    public static List<Card> deck = new List<Card>();

    void Awake()
    {
        Dictionary< Card.Suit, List<Sprite> > dict = new Dictionary< Card.Suit, List<Sprite> >();
        dict.Add(Card.Suit.Heart, heartCards);
        dict.Add(Card.Suit.Diamond, diamondCards);
        dict.Add(Card.Suit.Spade, spadeCards);
        dict.Add(Card.Suit.Club, clubCards);
        dict.Add(Card.Suit.Joker, jokers);
        

        foreach(Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
        {
            
            if(suit == Card.Suit.Joker)
            {
                deck.Add(Card.CreateInstance(dict[Card.Suit.Joker][0], -1, Card.Suit.Joker, true));
                deck.Add(Card.CreateInstance(dict[Card.Suit.Joker][1], -1, Card.Suit.Joker, true));
                continue;
            }

            for(int i=0; i<13; ++i)
            {
                bool special = i>6 || i==0; //special cases for all cards greater than 7 and aces

                //red kings have a special value
                if(i == 12 && (suit == Card.Suit.Heart || suit == Card.Suit.Diamond))
                {
                    deck.Add(Card.CreateInstance(dict[suit][i], 0, suit, true));
                    continue;
                }

                deck.Add(Card.CreateInstance(dict[suit][i], i+1, suit, special));
            }
        }
        System.Random rng = new System.Random();
        Shuffle(deck);
        GameManager.Instance.setGameState(GameState.START);

    }

    //Fisher-Yates shuffle in-place
    void Shuffle(List<Card> list)
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

    public static Card getCard()
    {
        var card = deck[0];
        deck.RemoveAt(0);
        return card;
    }
}
