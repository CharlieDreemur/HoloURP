using UnityEngine;
[System.Serializable]
public class BombCard : CardBase
{
    public BombCard()
    {
        //randomly set countdown between min and max
        this.description = $"Bomb Card!";
    }
    public override void OnRemove()
    {
        Debug.Log($"Bomb Card discarded: {description}");
    }

    public override void OnDraw()
    {
        Debug.Log($"Bomb Card drawn: {description}");
    }

    public override void OnUse()
    {
        Debug.Log($"Bomb Card used: {description}");
    }

}
