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
    [Header("Debug")]
    [SerializeField]
    [SerializeReference]
    public List<CardBase> cardDecks;
    [SerializeField]
    public const int MAX_CARD_NUMBER = 4;
    public UnityEvent DrawCardEvent;
    public UnityEvent<PlayerBase, int, UnityAction> DrawCardAnimationEvent;
    public CardEvent AddCardsEvent = new CardEvent();
    [SerializeField]
    private int _drawCardCount = 1;
    void Awake(){
        
    }
    void Start()
    {
        cardDecks = GenerateStartDeck();
        AddCardsEvent?.Invoke(cardDecks);
    }
    public void AddCards(List<CardBase> cards)
    {
        Debug.Log("AddCards"+cards.Count);
        cardDecks.AddRange(cards);
        AddCardsEvent?.Invoke(cards);
    }
    public void DrawCards(PlayerBase player){
        DrawCards(player, _drawCardCount);
    }
    public void DrawCards(PlayerBase player, int n)
    {
        List<CardBase> drawCards = TryDrawCards(n);
        if (drawCards == null)
        {
            return;
        }
        UnityAction callback = () =>
        {
            player.AddCards(drawCards);
        };
        DrawCardAnimationEvent?.Invoke(player, n, callback);
    }

    private List<CardBase> GenerateStartDeck()
    {
        if (MaxSize <= 0)
        {
            return null;
        }

        List<CardBase> cards = new List<CardBase>();

        // Distribute the numbered cards evenly between 2 and a
        for (int i = 0; i < MaxSize; i++)
        {
            int cardNumber = (i % MAX_CARD_NUMBER) + 2; // Cycles through numbers 2 to a
            NumberCard card = new NumberCard(cardNumber);
            cards.Add(card);
        }

        // Shuffle the deck randomly
        CardsUtils.Shuffle(ref cards);

        return cards;
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
