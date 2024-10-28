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

public enum OwnerType
{
    None,
    Player,
    AI
}

[System.Serializable]
public abstract class CardBase: ICard
{
    [SerializeField]
    protected string description = "CardBase";
    [SerializeField]
    protected OwnerType ownerType = OwnerType.None;
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
