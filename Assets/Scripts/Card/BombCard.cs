using UnityEngine;
[System.Serializable]
public class BombCard : CardBase
{
    public int CountDown = 0;
    public const int MAX_COUNTDOWN = 5;
    public const int MIN_COUNTDOWN = 3;
    public BombCard(System.Random rng)
    {
        //randomly set countdown between min and max
        CountDown = rng.Next(MIN_COUNTDOWN, MAX_COUNTDOWN + 1);
        this.description = $"Bomb Card!";
    }
    public override void OnDiscard()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDraw()
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

}
