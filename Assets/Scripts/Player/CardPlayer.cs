
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

    public override void DrawCards(int n = 1)
    {
        handZoneVisual.ShowHand();
        cardDeck.DrawCards(this, n);
    }
}
