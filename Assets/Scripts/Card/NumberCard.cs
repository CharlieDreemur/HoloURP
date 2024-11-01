using UnityEngine;


[System.Serializable]
public class NumberCard : CardBase
{
    [SerializeField]
    public int Number = 2; //card number will be in the range of 2 to 5

    public NumberCard(int cardNumber)
    {
        this.Number = cardNumber;
        this.description = $"Number Card {cardNumber}";
    }
    public NumberCard(int cardNumber, string description)
    {
        this.Number = cardNumber;
        this.description = description;
    }

    public bool LargerThan(NumberCard other)
    {
        return LargerThan(other.Number);
    }

    public bool LargerThan(int other)
    {
        return this.Number >= other;
    }


    public override void OnUse()
    {
        //Debug.Log($"{Number} used: {description}");
    }


    public override void OnDraw()
    {
        //Debug.Log($"{Number} was drawn.");
    }
    public override void OnRemove()
    {
        //Debug.Log($"{Number} was discarded.");
    }

}
