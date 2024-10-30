using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class CardPlayInfo
{
    public NumberCard card;
    public PlayerBase owner;
}
/// <summary>
/// PlayZone represents the area on the table where cards are placed after being played.
/// </summary>
public class PlayZone : MonoBehaviour
{
    [SerializeField]
    private CardDeck _cardDeck;
    [Header("Debug")]
    [SerializeField]
    [SerializeReference]
    private List<CardBase> playZoneCards = new List<CardBase>();
    public CardEvent AddCardToPlayZoneEvent = new CardEvent();
    public UnityEvent HideLastCardEvent = new UnityEvent();
    public UnityEvent<NumberCard> ShowLastCardEvent = new UnityEvent<NumberCard>();
    public UnityEvent<UnityAction> AddCardsToDeckEvent = new UnityEvent<UnityAction>();
    private CardPlayInfo _lastCardInfo;
    public CardPlayInfo LastCardInfo{
        get{
            return _lastCardInfo;
        }
        set{
            _lastCardInfo = value;
            if(value == null) {
                HideLastCardEvent?.Invoke();
            }
            else{
                ShowLastCardEvent?.Invoke(value.card);
            }
        }
    }
    void Awake()
    {
    }

    public int GetLastCardNumber()
    {
        if (LastCardInfo == null)
        {
            return 0;
        }
        return LastCardInfo.card.Number;
    }
    public bool TryAddCardToPlayZone(PlayerBase player, NumberCard card)
    {
        if (LastCardInfo == null)
        {
            LastCardInfo = new CardPlayInfo
            {
                card = card,
                owner = player
            };
            AddCardToPlayZone(card);
            return true;
        }
        if (player == LastCardInfo.owner)
        {
            Debug.Log("You can't play card twice until the next player plays a card");
            return false;
        }
        if (!card.LargerThan(LastCardInfo.card))
        {
            Debug.Log("You must play a card larger than the last card");
            return false;
        }
        LastCardInfo = new CardPlayInfo
        {
            card = card,
            owner = player
        };
        AddCardToPlayZone(card);
        return true;
    }
    public void AddCardToPlayZone(CardBase card)
    {
        Debug.Log("AddCardToPlayZone" + card);
        playZoneCards.Add(card);
        AddCardToPlayZoneEvent?.Invoke(new List<CardBase> { card });
    }
    // Add a card to the play zone
    public void AddCardsToPlayZone(List<CardBase> cards)
    {
        Debug.Log("AddCardToPlayZone" + cards.Count);
        playZoneCards.AddRange(cards);
        AddCardToPlayZoneEvent?.Invoke(cards);
    }
    [ContextMenu("AddCardsIntoDeck")]
    public void AddCardsIntoDeck()
    {
        Debug.Log("AddCardsIntoDeck");
        // Clone the list to prevent the original list from being modified
        List<CardBase> copyCards = new List<CardBase>(playZoneCards);
        playZoneCards.Clear();
        LastCardInfo = null;
        AddCardsToDeckEvent?.Invoke(() => _cardDeck.AddCards(copyCards));
    }

}
