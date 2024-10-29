using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// PlayZone represents the area on the table where cards are placed after being played.
/// </summary>
public class PlayZone : MonoBehaviour
{
    [SerializeField]
    private PlayerBase _player;
    [SerializeField]
    private CardDeck _cardDeck;
    [Header("Debug")]
    [SerializeField]
    [SerializeReference]
    private List<CardBase> playZoneCards = new List<CardBase>();
    public CardEvent AddCardToPlayZoneEvent = new CardEvent();
    public UnityEvent<UnityAction> AddCardsToDeckEvent = new UnityEvent<UnityAction>();
    void Awake()
    {
        _player.PlayCardEvent.AddListener(AddCardToPlayZone);
    }

    // Add a card to the play zone
    public void AddCardToPlayZone(List<CardBase> cards)
    {
        Debug.Log("AddCardToPlayZone" + cards.Count);
        playZoneCards.AddRange(cards);
        AddCardToPlayZoneEvent?.Invoke(cards);
    }

    public void AddCardsIntoDeck()
    {
        Debug.Log("AddCardsIntoDeck");
        // Clone the list to prevent the original list from being modified
        List<CardBase> copyCards = new List<CardBase>(playZoneCards);
        playZoneCards.Clear();
        AddCardsToDeckEvent?.Invoke(() => _cardDeck.AddCards(copyCards));

    }

}
