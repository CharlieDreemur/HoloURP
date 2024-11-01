using System.Collections.Generic;
using UnityEngine;

public static class CardsUtils
{

    // Method to shuffle the deck
    public static void Shuffle(ref List<CardBase> cards)
    {
        int n = cards.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = CardGameManager.Instance.RNG.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]); // Swap the cards at indices i and j
        }
        Debug.Log("Deck shuffled");
    }
}
