using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// PlayZone represents the area on the table where cards are placed after being played.
/// </summary>
public class PlayZone : MonoBehaviour
{
    [SerializeField]
    private HandZone _playerStats;
    [SerializeField]
    private CardDeck _cardDeck;
    [Header("Debug")]
    [SerializeField]
    [SerializeReference]
    private List<CardBase> cards = new List<CardBase>();
    public CardEvent AddCardToPlayZoneEvent = new CardEvent();
    public UnityEvent AddCardsIntoDeckEvent = new UnityEvent();
    void Awake(){
        _playerStats.PlayCardEvent.AddListener(AddCardToTable);
    }

    // Add a card to the table
    public void AddCardToTable(List<CardBase> cards)
    {
        cards.AddRange(cards);
        AddCardToPlayZoneEvent?.Invoke(cards);
    }

    public void AddCardsIntoDeck(List<CardBase> cards)
    {
        _cardDeck.AddCards(cards);
        //clear the cards
        cards.Clear();
        AddCardsIntoDeckEvent?.Invoke();

    }

}
