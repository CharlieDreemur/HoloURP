
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
    public CardEvent PlayCardEvent = new CardEvent();
    void Awake()
    {

        Health = _maxHealth;
        HealthEvent.AddListener(() => Debug.Log("Health: " + Health));
    }
    void Start()
    {
        List<CardBase> cards = new List<CardBase>();
        for(int i = 1; i <= initCardCount; i++)
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
        List<CardBase> cards = new List<CardBase> { HandCards[index] };
        RemoveCards(cards);
        PlayCardEvent?.Invoke(cards);
    }

    public void PlayCards(List<CardBase> cards)
    {
        RemoveCards(cards);
        PlayCardEvent?.Invoke(cards);
    }

}
