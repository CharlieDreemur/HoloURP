using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
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

    // public override void DrawCards(int n = 1)
    // {
    //     base.DrawCards(3);
    // }

    public override void PunishOpponent(PlayerBase opponent)
    {
        int index = 0;
        float moveDuration = 1f;
        Debug.Log("AIPlayer:PunishOpponent");
        CardPlayer cardPlayer = (CardPlayer)opponent;
        //draw one card from opponent
        if (cardPlayer.HandCards.Count > 0)
        {
            CardBase card = cardPlayer.HandCards[index];
            GameObject cardModel = cardPlayer.handZoneVisual.CardModels[index];
            Sequence sequence = DOTween.Sequence();
            sequence.Append(cardModel.transform.DOMove(_handTransform.position, moveDuration));
            sequence.Join(cardModel.transform.DORotateQuaternion(_handTransform.rotation, moveDuration));
            sequence.OnComplete(() =>
            {
                cardPlayer.RemoveCard(card);
                AddCard(card);
                Destroy(cardModel);
                cardPlayer.handZoneVisual.HideHand();
            });

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
