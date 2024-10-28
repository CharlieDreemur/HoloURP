using UnityEngine;


[System.Serializable]
public class NumberCard: CardBase
{
    [SerializeField]
    public int cardNumber = 1; //card number will be in the range of 1 to 4

    protected ICardEffect cardEffect;
    public NumberCard(int cardNumber)
    {
        this.cardNumber = cardNumber;
        this.description = $"Number Card {cardNumber}";
    }
    public NumberCard(int cardNumber, string description, ICardEffect effect)
    {
        this.cardNumber = cardNumber;
        this.description = description;
        this.cardEffect = effect;
    }
    public override void Use()
    {
        Debug.Log($"{cardNumber} used: {description}");
        cardEffect.ApplyEffect();
    }


    public override void OnDraw()
    {
        Debug.Log($"{cardNumber} was drawn.");
    }
    public override void OnDiscard()
    {
        Debug.Log($"{cardNumber} was discarded.");
    }
    
}
