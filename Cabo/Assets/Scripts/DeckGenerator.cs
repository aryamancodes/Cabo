using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

/*
    Static class to generate and uniqely distribute a deck of CardBase objects
    based on a seed.
*/
public class DeckGenerator: MonoBehaviourPunCallbacks
{
    public static DeckGenerator Instance;

    public Sprite hidden;
    
    public List<Sprite> heartCards = new List<Sprite>();
    public List<Sprite> diamondCards = new List<Sprite>();
    public List<Sprite> spadeCards = new List<Sprite>();
    public List<Sprite> clubCards = new List<Sprite>();
    public List<Sprite> jokers = new List<Sprite>(); 
    
    public List<CardBase> deck = new List<CardBase>();

    void Awake()
    {
        Instance = this;
    }
    public void generateDeck(int seed)
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
                deck.Add(CardBase.CreateInstance(hidden, dict[CardBase.Suit.Joker][0], -2, CardBase.Suit.Joker, false));
                deck.Add(CardBase.CreateInstance(hidden, dict[CardBase.Suit.Joker][1], -2, CardBase.Suit.Joker, false));
                continue;
            }

            for(int i=0; i<13; ++i)
            {
                bool special = i>=6; //special cases for all cards greater than 7 except red kings

                //red kings have a special value but aren't special card when played
                if(i == 12 && (suit == CardBase.Suit.Heart || suit == CardBase.Suit.Diamond))
                {
                    deck.Add(CardBase.CreateInstance(hidden, dict[suit][i], -1, suit, false));
                    continue;
                }
                //aces have a value of 0
                if(i==0) { deck.Add(CardBase.CreateInstance(hidden, dict[suit][i], 0, suit, special)); }
                else{ deck.Add(CardBase.CreateInstance(hidden, dict[suit][i], i+1, suit, special)); } 

            }
        }
        Shuffle(deck, seed);
    }

    //Fisher-Yates shuffle in-place
    void Shuffle(List<CardBase> list, int seed)
    {
        var rng = new System.Random(seed);
        int n = list.Count;
        while (n > 1)
        {
             n--;
            int k = rng.Next(n + 1);
            CardBase value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public CardBase getCard()
    {
        {
            var card = deck[0];
            deck.RemoveAt(0);

            //if all the cards have been drawn, re-generate the cards in the placePile
            if(deck.Count == 0)
            {
                foreach(Transform child in CardHandler.Instance.placeArea.transform)
                {
                    CardBase insertBack = child.GetComponent<Card>().card;
                    bool special = insertBack.value > 6 && !(insertBack.value == 12 && (insertBack.suit == CardBase.Suit.Heart || insertBack.suit == CardBase.Suit.Diamond));
                    deck.Add(CardBase.CreateInstance(hidden, insertBack.shownFace, insertBack.value, insertBack.suit, insertBack.isSpecialCard));
                }
            }
            return card;
        }
    }

    public void clearDeck()
    {
        deck.Clear();
    }
}
