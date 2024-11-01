using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class AIHandZoneVisual : HandZoneVisualBase
{
    public override void AddCardVisuals(List<CardBase> cards)
    {
        foreach (CardBase card in cards)
        {
            GameObject cardModel = Instantiate(cardPrefab, _handTransform);
            cardModel.GetComponent<CardVisual>().SetCard(card);
            _cardModels.Add(cardModel);
        }
        if (_isHandHidden)
        {
            //ShowHand();
        }
        else
        {
            ArrangeCardsInFan();
        }
    }


}
