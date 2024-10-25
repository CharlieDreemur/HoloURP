
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{

    [SerializeField]
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
    public UnityEvent HealthEvent;
    public UnityEvent AddCardEvent;
    public UnityEvent RemoveCardEvent;
    public UnityEvent PlayCardEvent;
    void Awake()
    {
        HealthEvent.AddListener(() => Debug.Log("Health: " + Health));
    }

    public void AddCard(CardBase card)
    {
        HandCards.Add(card);
    }

    public void RemoveCard(CardBase card)
    {
        HandCards.Remove(card);
    }


    public void PlayCard(int index)
    {
        if (HandCards.Count > 0)
        {
            
            RemoveCard(HandCards[index]);
        }
    }
}
