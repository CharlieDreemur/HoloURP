
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public abstract class PlayerBase : MonoBehaviour
{
    [SerializeField]
    public int initCardCount = 5;
    [SerializeField]
    [SerializeReference]
    public List<CardBase> HandCards = new List<CardBase>();
    [SerializeField]
    private int _maxHealth = 3;
    [Header("Transforms Settings")]
    [SerializeField]
    public Transform _handTransform;
    [SerializeField]
    public CardDeck cardDeck;
    [SerializeField]
    public PlayZone playZone;
    public int Health
    {
        get => _currHealth;
        set
        {
            _currHealth = Mathf.Clamp(value, 0, _maxHealth);
            HealthEvent?.Invoke();
        }
    }
    private int _currHealth = 3;
    //HealthEvent
    public UnityEvent HealthEvent = new UnityEvent();
    public CardEvent AddCardEvent = new CardEvent();
    public UnityEvent<List<int>> RemoveCardEvent = new UnityEvent<List<int>>();
    public UnityEvent<List<int>> PlayCardAnimationEvent = new UnityEvent<List<int>>();
    public UnityEvent<CardBase> PlayCardEvent = new UnityEvent<CardBase>();
    void Awake()
    {

        Health = _maxHealth;
        HealthEvent.AddListener(() => Debug.Log("Health: " + Health));
    }
    void Start()
    {
    }
    public virtual void DrawCards(int n = 1)
    {
        cardDeck.DrawCards(this, n);
    }
    public void AddCard(CardBase card)
    {
        HandCards.Add(card);
        AddCardEvent?.Invoke(new List<CardBase> { card });
    }
    public void AddCards(List<CardBase> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            HandCards.Add(cards[i]);
        }
        AddCardEvent?.Invoke(cards);
    }

    public void RemoveCard(CardBase card)
    {
        int index = HandCards.IndexOf(card);
        HandCards.Remove(card);
        RemoveCardEvent?.Invoke(new List<int> {index});
    }
    public void RemoveCards(List<CardBase> cards)
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            indexes.Add(HandCards.IndexOf(cards[i]));
            HandCards.Remove(cards[i]);
        }
        RemoveCardEvent?.Invoke(indexes);
    }
    public bool PlayCard(CardBase card)
    {
        return PlayCardAtIndex(HandCards.IndexOf(card));
    }
    public bool PlayCardAtIndex(int index)
    {
        //Debug.Log("Play card at index: " + index);
        if (index < 0 || index >= HandCards.Count || HandCards.Count == 0)
        {
            return false;
        }
        CardBase card = HandCards[index];
        if (card is BombCard)
        {
            Debug.Log("You can't play a bomb card");
            return false;
        }
        NumberCard numberCard = card as NumberCard;
        if(!playZone.TryAddCardToPlayZone(this, numberCard)){
            return false;
        }
        HandCards.Remove(HandCards[index]);
        List<int> indexes = new List<int> { index };
        PlayCardAnimationEvent?.Invoke(indexes);
        PlayCardEvent?.Invoke(card);
        return true;
    }

    public void PlayCards(List<CardBase> cards)
    {
        if (cards.Count == 0 || HandCards.Count == 0)
        {
            return;
        }
        //determine the indexes of the cards
        List<int> indexes = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            indexes.Add(HandCards.IndexOf(cards[i]));
        }
        RemoveCards(cards);
        PlayCardAnimationEvent?.Invoke(indexes);
        //PlayCardEvent?.Invoke(card);
    }

    public virtual void PunishOpponent(PlayerBase opponent)
    {

    }

    public bool HasLargerCard(NumberCard card){
        foreach(var handCard in HandCards){
            if(handCard is NumberCard){
                NumberCard numberCard = handCard as NumberCard;
                if(numberCard.LargerThan(card)){
                    return true;
                }
            }
        }
        return false;
    }
}
