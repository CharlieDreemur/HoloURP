using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Draw Setting")]
    private float drawDuration = 0.5f;
    [SerializeField]
    private Transform playerHandTransform;
    [SerializeField]
    private HandZone handZone;
    private float deckBodySpacing = 0.002f;
    [Header("Debug")]
    [SerializeField]
    private List<GameObject> deckVisuals = new List<GameObject>();
    [SerializeField]
    [SerializeReference]
    private List<CardBase> deck;
    private const int Seed = 0;  // Seed for the random number generator
    private System.Random rng = new System.Random(Seed);
    private InputControls _controls;
    public const int MAX_CARD_NUMBER = 4;
    void Awake()
    {
        _controls = new InputControls();
        _controls.Enable();
        _controls.Player.DrawCard.performed += _ => DrawCards(1);
    }
    void Start()
    {
        deck = GenerateStartDeck();
        InitVisual();
    }
    private List<CardBase> GenerateStartDeck()
    {
        if (maxSize <= 0)
        {
            return null;
        }

        List<CardBase> cards = new List<CardBase>
        {
            new BombCard(rng)
        };

        int numberedCardCount = maxSize - 1; // Subtract 1 for the bomb card

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
    private void InitVisual()
    {
        for (int i = 0; i < currentSize; i++)
        {
            GameObject newDeckBody = Instantiate(deckBodyPrefab, transform);
            newDeckBody.transform.localPosition = new Vector3(0, 0, i * deckBodySpacing);
            deckVisuals.Add(newDeckBody);
        }
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
    public void DrawCards(int n)
    {
        List<CardBase> drawCards = TryDrawCards(n);
        if (drawCards == null)
        {
            return;
        }
        UnityAction callback = () =>
        {
            handZone.AddCards(drawCards);
        };
        PlayDrawCardAnimatin(n);
    }

    private void PlayDrawCardAnimatin(int n, UnityAction callback = null)
    {

        for (int i = 0; i < n; i++)
        {
            GameObject topCard = deckVisuals[deckVisuals.Count - 1];
            Debug.Log("Drawing card from deck:" + topCard);
            deckVisuals.RemoveAt(deckVisuals.Count - 1);
            topCard.transform.DOMove(playerHandTransform.position, drawDuration).SetEase(Ease.OutCubic);
            topCard.transform.DORotateQuaternion(playerHandTransform.rotation, drawDuration).SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    Debug.Log("Card drawn to hand.");
                    callback?.Invoke();
                });
        }
    }
    /// <summary>
    /// Draw n cards from the deck.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    private List<CardBase> TryDrawCards(int n)
    {
        List<CardBase> drawnCards = new List<CardBase>();

        // Ensure we don't draw more cards than available in the deck
        int drawCount = Mathf.Min(n, deck.Count);

        for (int i = 0; i < drawCount; i++)
        {
            CardBase drawnCard = deck[0];
            deck.RemoveAt(0);
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
