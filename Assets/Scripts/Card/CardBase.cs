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

public abstract class CardBase: ICard
{
    public string CardName { get; private set; }
    public string Description { get; private set; }

    protected ICardEffect cardEffect;

    public CardBase(string cardName, string description, ICardEffect effect)
    {
        this.CardName = cardName;
        this.Description = description;
        this.cardEffect = effect;
    }
    public void Use()
    {
        Debug.Log($"{CardName} used: {Description}");
        cardEffect.ApplyEffect();
    }


    public virtual void OnDraw()
    {
        Debug.Log($"{CardName} was drawn.");
    }
    public virtual void OnDiscard()
    {
        Debug.Log($"{CardName} was discarded.");
    }
    
}
