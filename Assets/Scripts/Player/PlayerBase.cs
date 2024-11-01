
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
public abstract class PlayerBase : MonoBehaviour
{
    [SerializeField]
    [SerializeReference]
    public List<CardBase> HandCards = new List<CardBase>();
    [SerializeField]
    public int MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
        }
    }
    public int CurrentHealth
    {
        get => _currHealth;
        set
        {
            _currHealth = value;
        }
    }
    [SerializeField]
    private int _maxHealth = 3;
    [SerializeField]
    private int _currHealth = 3;
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
        }
    }

    //HealthEvent
    public CardEvent AddCardEvent = new CardEvent();
    public UnityEvent<List<int>> RemoveCardEvent = new UnityEvent<List<int>>();
    public UnityEvent<List<int>> PlayCardAnimationEvent = new UnityEvent<List<int>>();
    public UnityEvent<CardBase> PlayCardEvent = new UnityEvent<CardBase>();
    public UnityEvent HurtEvent = new UnityEvent();
    public UnityEvent DeathEvent = new UnityEvent();
    public UnityEvent ClearEvent = new UnityEvent();
    void Awake()
    {
        Health = _maxHealth;
    }
    void Start()
    {
        InitStartCard();
    }
    public virtual void InitStartCard(){
        HandCards = new List<CardBase>{
            new BombCard(),
        };
        //randomly insert 3 number cards
        for (int i = 0; i < 3; i++)
        {
            HandCards.Add(new NumberCard(Random.Range(2, 5)));
        }
        //shuffle the cards
        CardsUtils.Shuffle(ref HandCards);
        AddCardEvent?.Invoke(HandCards);
    }
    public virtual void Hurt()
    {
        Health--;
        if (Health == 0)
        {
            DeathEvent?.Invoke();
        }
        HurtEvent?.Invoke();
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
        RemoveCardEvent?.Invoke(new List<int> { index });
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
            UIManager.Instance.ShowMessage("You can't play bomb card");
            AudioManager.Instance.Play("cannotplay");
            return false;
        }
        NumberCard numberCard = card as NumberCard;
        if (!playZone.TryAddCardToPlayZone(this, numberCard))
        {
            UIManager.Instance.ShowMessage("You can't play a smaller card");
            AudioManager.Instance.Play("cannotplay");
            return false;
        }
        AudioManager.Instance.Play("cardplace");
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
    [ContextMenu("Clear")]
    public void Clear()
    {
        HandCards.Clear();
        ClearEvent?.Invoke();
        InitStartCard();
    }

    public virtual CardBase DrawOpponent(PlayerBase opponent)
    {
        return null;
    }

    public bool HasLargerCard(NumberCard card)
    {
        foreach (var handCard in HandCards)
        {
            if (handCard is NumberCard)
            {
                NumberCard numberCard = handCard as NumberCard;
                if (numberCard.LargerThan(card))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
