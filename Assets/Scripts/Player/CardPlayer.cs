
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CardEvent : UnityEvent<List<CardBase>> { }
public class CardPlayer : PlayerBase
{
    public HandZoneVisual handZoneVisual;
    public  DrawOpponentCard drawOpponentCard;
    void Awake(){
        DeathEvent.AddListener(() => {
            UIManager.Instance.ShowMessage("Gameover!");
        });
    }
    public bool PlayCard()
    {
        return base.PlayCardAtIndex(handZoneVisual.CurrentCardIndex);
    }
    [ContextMenu("DrawCards")]
    public void TestDrawCards()
    {
        DrawCards();
    }
    public override void DrawCards(int n = 1)
    {
        base.DrawCards(n);
        handZoneVisual.ShowHand();
        cardDeck.DrawCards(this, n);
    }

}
