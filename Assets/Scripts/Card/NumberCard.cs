using UnityEngine;


[System.Serializable]
public class NumberCard: CardBase
{
    [SerializeField]
    public int Number = 1; //card number will be in the range of 1 to 4

    protected ICardEffect cardEffect;
    public NumberCard(int cardNumber)
    {
        this.Number = cardNumber;
        this.description = $"Number Card {cardNumber}";
    }
    public NumberCard(int cardNumber, string description, ICardEffect effect)
    {
        this.Number = cardNumber;
        this.description = description;
        this.cardEffect = effect;
    }


    public bool LargerThan(NumberCard other)
    {
        return this.Number > other.Number;
    }

    public override void Use()
    {
        Debug.Log($"{Number} used: {description}");
        cardEffect.ApplyEffect();
    }


    public override void OnDraw()
    {
        Debug.Log($"{Number} was drawn.");
    }
    public override void OnDiscard()
    {
        Debug.Log($"{Number} was discarded.");
    }
    
}
