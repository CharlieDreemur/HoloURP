using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
public class CardDeck : MonoBehaviour
{
    [Header("Deck Settings")]
    [SerializeField]
    public int MaxSize = 10;
    [SerializeField]
    private HandZone _playerStats;
    [Header("Debug")]
    [SerializeField]
    [SerializeReference]
    public List<CardBase> cardDecks;
    [SerializeField]
    private System.Random rng = new System.Random(0);
    public const int MAX_CARD_NUMBER = 4;
    public UnityEvent DrawCardEvent;
    private InputControls _controls;
    public UnityEvent<int, UnityAction> DrawCardAnimationEvent;
    public UnityEvent InitFinishEvent;
    [SerializeField]
    private int _drawCardCount = 1;
    void Awake()
    {
        _controls = new InputControls();
        _controls.Enable();
        _controls.Player.DrawCard.performed += _ => DrawCards(_drawCardCount);
    }
    void Start()
    {
        cardDecks = GenerateStartDeck();
        InitFinishEvent?.Invoke();
    }
    public void AddCards(List<CardBase> cards)
    {
        cardDecks.AddRange(cards);
    }
    public void DrawCards(int n)
    {
        List<CardBase> drawCards = TryDrawCards(n);
        if (drawCards == null)
        {
            return;
        }
        UnityAction callback = () =>
        {
            _playerStats.AddCards(drawCards);
        };
        DrawCardAnimationEvent?.Invoke(n, callback);
    }

    private List<CardBase> GenerateStartDeck()
    {
        if (MaxSize <= 0)
        {
            return null;
        }

        List<CardBase> cards = new List<CardBase>
        {
            new BombCard(rng)
        };

        int numberedCardCount = MaxSize - 1; // Subtract 1 for the bomb card

        // Distribute the numbered cards evenly between 1 and a
        for (int i = 0; i < numberedCardCount; i++)
        {
            int cardNumber = (i % MAX_CARD_NUMBER) + 1; // Cycles through numbers 1 to a
            NumberCard card = new NumberCard(cardNumber);
            cards.Add(card);
        }

        // Shuffle the deck randomly
        ShuffleDeck(cards);

        return cards;
    }

    // Method to shuffle the deck
    private void ShuffleDeck(List<CardBase> cards)
    {
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (cards[n], cards[k]) = (cards[k], cards[n]); // Swap the cards at indices n and k
        }
        Debug.Log("Deck shuffled");
    }
    /// <summary>
    /// Draw n cards from the deck.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public List<CardBase> TryDrawCards(int n)
    {
        List<CardBase> drawnCards = new List<CardBase>();

        // Ensure we don't draw more cards than available in the deck
        int drawCount = Mathf.Min(n, cardDecks.Count);

        for (int i = 0; i < drawCount; i++)
        {
            CardBase drawnCard = cardDecks[0];
            cardDecks.RemoveAt(0);
            drawnCards.Add(drawnCard);
            drawnCard.OnDraw();
            Debug.Log($"Drew card: {drawnCard.GetType().Name}");
        }

        if (drawCount < n)
        {
            Debug.Log("Not enough cards left in the deck.");
            return null;
        }

        return drawnCards;
    }
    /// <summary>
    /// Check if the deck is empty.
    /// </summary>
    /// <returns></returns>
    public bool IsDeckEmpty()
    {
        return cardDecks.Count == 0;
    }

    /// <summary>
    /// Get the number of cards remaining in the deck.
    /// </summary>
    /// <returns></returns>
    public int RemainingCardCount()
    {
        return cardDecks.Count;
    }
}
