
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CardEvent : UnityEvent<List<CardBase>> { }
public class CardPlayer : PlayerBase
{
    public HandZoneVisual handZoneVisual;
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
        handZoneVisual.ShowHand();
        cardDeck.DrawCards(this, n);
    }

    public override void PunishOpponent(PlayerBase opponent)
    {
        Debug.Log("CardPlayer:PunishOpponent");
        //draw one card from opponent
        if (opponent.HandCards.Count > 0)
        {
            CardBase card = opponent.HandCards[handZoneVisual.CurrentCardIndex];
            opponent.RemoveCard(card);
            AddCard(card);
        }
        else
        {
            Debug.Log("Opponent has no card to draw");
        }
    }
}
