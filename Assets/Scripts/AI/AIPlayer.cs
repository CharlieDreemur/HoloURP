using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;
public class AIPlayer : PlayerBase
{

    void Awake()
    {
        DeathEvent.AddListener(() =>
        {
            UIManager.Instance.WinGame();
        });
    }
    public void PlayRandomCard(PlayerContext context)
    {

        if (HandCards.Count > 0)
        {
            NumberCard card = FindBestCard();
            if (card == null)
            {
                Debug.Log("AIPlayer:PlayRandomCard:PlayCardAtIndex:Failed");
                AnimationController.Instance.SetExpression(ExpressionType.Sad);
                context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.EndTurn(), 3f));
            }
            else
            {
                bool result = PlayCard(card);
                if (result == true)
                {
                    context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.AdvanceRound(), 5f));
                }
                else
                {
                    Debug.Log("Should not reach here");
                    context.cardGameManager.StartCoroutine(context.cardGameManager.WaitForSeconds(() => context.cardGameManager.EndTurn(), 3f));
                }
            }


        }
    }
    public override void Hurt()
    {
        AnimationController.Instance.SetExpression(ExpressionType.Sad);
        AudioManager.Instance.Play("corrupt");
        base.Hurt();
    }
    // public override void DrawCards(int n = 1)
    // {
    //     base.DrawCards(3);
    // }

    public override CardBase DrawOpponent(PlayerBase opponent)
    {
        //randomly draw one card from opponent
        AnimationController.Instance.SetMotionState("TakeCard");
        int index = Random.Range(0, opponent.HandCards.Count);
        Debug.Log("AIPlayer:PunishOpponent");
        CardPlayer cardPlayer = (CardPlayer)opponent;
        CardBase card;
        //draw one card from opponent
        if (cardPlayer.HandCards.Count > 0)
        {
            card = cardPlayer.HandCards[index];
            GameObject cardModel = cardPlayer.handZoneVisual.CardModels[index];
            UnityAction pickCallback = () =>
            {
                AIHandZoneVisual hand = (AIHandZoneVisual)handZoneVisual;
                cardModel.transform.SetParent(hand.RightHandTransform);
                cardModel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.3f).OnComplete(() =>
                {
                    AnimationController.Instance.SetMotionState("Idle");
                });
            };
            StartCoroutine(WaitAndDo(2.1f, pickCallback));

            UnityAction finishCallback = () =>
            {
                Debug.Log("AIPlayer:DrawOpponent:Finish");
                cardPlayer.RemoveCard(card);
                AddCard(card);
                Destroy(cardModel);
                cardPlayer.handZoneVisual.HideHand();
                if (card is NumberCard)
                {
                    AnimationController.Instance.SetExpression(ExpressionType.Happy);
                    CardGameManager.Instance.AdvanceTurn();
                }
                else
                {
                    Hurt();
                    StartCoroutine(WaitAndDo(1f, ()=>CardGameManager.Instance.Reset()));
                }
            };
            StartCoroutine(WaitAndDo(4.95f, finishCallback));
            return card;
        }
        else
        {
            Debug.Log("Opponent has no card to draw");
        }
        return null;
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
            if (card.LargerThan(lastCardNumber))
            {
                if (bestCard == null || card.Number < bestCard.Number)
                {
                    bestCard = card;
                }
            }
        }
        return bestCard;
    }
    public void SwitchExpression(int CurrentCardIndex)
    {
        if (CurrentCardIndex < 0)
        {
            return;
        }
        else if (CurrentCardIndex >= HandCards.Count)
        {
            return;
        }
        //if the card at the index is a bomb card, switch to the bomb expression
        if (HandCards[CurrentCardIndex] is BombCard)
        {
            AnimationController.Instance.SetExpression(ExpressionType.Sad);
        }
        else
        {
            AnimationController.Instance.SetExpression(ExpressionType.Happy);
        }
    }

    public IEnumerator WaitAndDo(float time, UnityAction callback)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
}
