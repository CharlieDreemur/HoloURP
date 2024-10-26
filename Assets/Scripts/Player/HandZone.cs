
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class CardEvent : UnityEvent<List<CardBase>> { }
public class HandZone : MonoBehaviour
{
    [SerializeField]
    public int initCardCount = 5;
    [SerializeField]
    [SerializeReference]
    public List<CardBase> HandCards = new List<CardBase>();
    [SerializeField]
    private int _maxHealth = 3;
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
    public CardEvent RemoveCardEvent = new CardEvent();
    public UnityEvent<List<int>> PlayCardAnimationEvent = new UnityEvent<List<int>>();
    public CardEvent PlayCardEvent = new CardEvent();
    void Awake()
    {

        Health = _maxHealth;
        HealthEvent.AddListener(() => Debug.Log("Health: " + Health));
    }
    void Start()
    {
        List<CardBase> cards = new List<CardBase>();
        for (int i = 1; i <= initCardCount; i++)
        {
            cards.Add(new NumberCard(i));
        }
        AddCards(cards);
    }
    public void AddCards(List<CardBase> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            HandCards.Add(cards[i]);
        }
        AddCardEvent?.Invoke(cards);
    }


    public void RemoveCards(List<CardBase> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            HandCards.Remove(cards[i]);
        }
        RemoveCardEvent?.Invoke(cards);
    }

    public void PlayCard(int index)
    {
        Debug.Log("Play card at index: " + index);
        if (index < 0 || index >= HandCards.Count || HandCards.Count == 0)
        {
            return;
        }
        CardBase card = HandCards[index];
        List<int> indexes = new List<int> { index };
        HandCards.Remove(HandCards[index]);
        PlayCardAnimationEvent?.Invoke(indexes);
        PlayCardEvent?.Invoke(new List<CardBase> {card});
    }

    public void PlayCards(List<CardBase> cards)
    {
        if(cards.Count == 0 || HandCards.Count == 0)
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
        PlayCardEvent?.Invoke(cards);
    }

}
