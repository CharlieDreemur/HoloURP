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

[System.Serializable]
public abstract class CardBase: ICard
{
    [SerializeField]
    protected string description = "CardBase";
    public virtual void Use()
    {
        Debug.Log($"CardBase used");
    }


    public virtual void OnDraw()
    {
        Debug.Log($"CardBase used");
    }
    public virtual void OnDiscard()
    {
        Debug.Log($"CardBase used");
    }
    
}
