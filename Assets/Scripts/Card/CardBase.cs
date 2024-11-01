using UnityEngine;
public interface ICard
{
    void OnUse();
    void OnDraw();
    void OnRemove();
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
    public virtual void OnUse()
    {
        Debug.Log($"CardBase used");
    }


    public virtual void OnDraw()
    {
        Debug.Log($"CardBase used");
    }
    public virtual void OnRemove()
    {
        Debug.Log($"CardBase used");
    }
    
}
