using UnityEngine;

public interface ICardEffect
{
    void ApplyEffect();
}

public interface ICard
{
    void Use();
    void OnDraw();
    void OnDiscard();
}

public class NumberCard: ICard
{
    [SerializeField]
    private int cardNumber = 1;
    public string Description { get; private set; }

    protected ICardEffect cardEffect;

    public NumberCard(int cardNumber, string description, ICardEffect effect)
    {
        this.cardNumber = cardNumber;
        this.Description = description;
        this.cardEffect = effect;
    }
    public void Use()
    {
        Debug.Log($"{cardNumber} used: {Description}");
        cardEffect.ApplyEffect();
    }


    public virtual void OnDraw()
    {
        Debug.Log($"{cardNumber} was drawn.");
    }
    public virtual void OnDiscard()
    {
        Debug.Log($"{cardNumber} was discarded.");
    }
    
}
