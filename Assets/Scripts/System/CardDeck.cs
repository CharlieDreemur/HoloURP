using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    [Header("Deck Settings")]
    [SerializeField]
    private int maxSize = 10;
    [SerializeField]
    private int currentSize = 10;
    [Header("Deck Visuals")]
    [SerializeField]
    private GameObject deckBodyPrefab;
    [SerializeField]
    private float deckBodySpacing = 0.002f;
    [Header("Debug")]
    [SerializeField]
    private List<GameObject> deckBodies = new List<GameObject>();
    [SerializeField]
    private int previousSize;
    public int CurrentSize{
        get { return currentSize; }
        set { 
            if(value > maxSize){
                currentSize = maxSize;
            }
            if(value < 0)
            {
                currentSize = 0;
            }
            UpdateVisuals();
        }
    }
    private List<ICard> deck; 
    private const int Seed = 0;  // Seed for the random number generator
    private System.Random rng = new System.Random(Seed);

    public CardDeck(List<ICard> initialCards)
    {
        deck = new List<ICard>(initialCards);
        ShuffleDeck(); 
    }
    [ContextMenu("UpdateVisuals")]
    private void UpdateVisuals()
    {
        if(currentSize == previousSize)
        {
            return;
        }

        if(currentSize > previousSize)
        {
            for(int i = previousSize; i < currentSize; i++)
            {
                GameObject newDeckBody = Instantiate(deckBodyPrefab, transform);
                newDeckBody.transform.localPosition = new Vector3(0, 0, i * deckBodySpacing);
                deckBodies.Add(newDeckBody);
            }
        }
        else
        {
            for(int i = previousSize - 1; i >= currentSize; i--)
            {
                Destroy(deckBodies[i]);
                deckBodies.RemoveAt(i);
            }
        }
    }
    // Method to shuffle the deck
    public void ShuffleDeck()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            ICard value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
        Debug.Log("Deck shuffled");
    }

    /// <summary>
    /// Draw n cards from the deck.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public List<ICard> DrawCards(int n)
    {
        List<ICard> drawnCards = new List<ICard>();

        // Ensure we don't draw more cards than available in the deck
        int drawCount = Mathf.Min(n, deck.Count);

        for (int i = 0; i < drawCount; i++)
        {
            ICard drawnCard = deck[0];  // Get the top card
            deck.RemoveAt(0);           // Remove it from the deck
            drawnCards.Add(drawnCard);  // Add it to the drawn cards list
            drawnCard.OnDraw();         // Call the OnDraw method of the card
            Debug.Log($"Drew card: {drawnCard.GetType().Name}");
        }

        if (drawCount < n)
        {
            Debug.Log("Not enough cards left in the deck.");
        }

        return drawnCards;
    }
    /// <summary>
    /// Check if the deck is empty.
    /// </summary>
    /// <returns></returns>
    public bool IsDeckEmpty()
    {
        return deck.Count == 0;
    }

    /// <summary>
    /// Get the number of cards remaining in the deck.
    /// </summary>
    /// <returns></returns>
    public int RemainingCardCount()
    {
        return deck.Count;
    }
}
