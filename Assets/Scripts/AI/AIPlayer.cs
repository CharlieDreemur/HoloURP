using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIPlayer : PlayerBase
{
    public void PlayRandomCard(PlayerContext context)
    {
        if (HandCards.Count > 0)
        {
            NumberCard card = FindBestCard();
            if (card == null)
            {
                Debug.Log("AIPlayer:PlayRandomCard:PlayCardAtIndex:Failed");
                context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.AdvanceTurn(), 1.5f));
            }
            else
            {
                bool result = PlayCard(card);
                if (result == true)
                {
                    context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.AdvanceRound(), 1.5f));
                }
                else
                {
                    Debug.Log("Should not reach here");
                    context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.AdvanceTurn(), 1.5f));
                }
            }


        }
    }

    public NumberCard FindBestCard()
    {
        int lastCardNumber = playZone.GetLastCardNumber();
        NumberCard bestCard = null;
        foreach (CardBase c in HandCards)
        {
            if (c is BombCard)
            {
                continue;
            }
            NumberCard card = c as NumberCard;
            if (card.Number > lastCardNumber)
            {
                if (bestCard == null || card.Number < bestCard.Number)
                {
                    bestCard = card;
                }
            }
        }
        return bestCard;
    }

}
