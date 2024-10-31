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
                context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.EndTurn(), 1.5f));
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
                    context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.EndTurn(), 1.5f));
                }
            }


        }
    }

    public override void PunishOpponent(PlayerBase opponent)
    {
        Debug.Log("AIPlayer:PunishOpponent");
        //draw one card from opponent
        if (opponent.HandCards.Count > 0)
        {
            CardBase card = opponent.HandCards[0];
            opponent.HandCards.Remove(card);
            HandCards.Add(card);
            opponent.RemoveCardEvent?.Invoke(new List<CardBase> { card });
            AddCardEvent?.Invoke(new List<CardBase> { card });
        }
        else
        {
            Debug.Log("Opponent has no card to draw");
        }
    }

    private NumberCard FindBestCard()
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
