using UnityEngine;
using DG.Tweening;
public class DrawOpponentCard : MonoBehaviour
{
    public AIPlayer aiPlayer;
    public AIHandZoneVisual aiHand;
    public CardPlayer player;
    public Transform playerHandTransform;
    [SerializeField]
    private float _drawCardDuration = 0.5f;
    public int CurrentCardIndex
    {
        get { return _currentCardIndex; }
        set
        {
            if (value < 0)
            {
                value = aiHand.CardModels.Count - 1;
            }
            _currentCardIndex = Mathf.Clamp(value, 0, aiHand.CardModels.Count - 1);
        }
    }
    [SerializeField]
    private int _currentCardIndex = 0;

    public CardBase DrawCard()
    {
        CardBase card = aiPlayer.HandCards[CurrentCardIndex];
        aiHand.CardModels[CurrentCardIndex].GetComponent<CardBackVisual>().DeselectCard();
        GameObject cardModel = aiHand.CardModels[CurrentCardIndex];
        aiPlayer.RemoveCard(card);
        cardModel.transform.SetParent(playerHandTransform);
        cardModel.transform.DOMove(playerHandTransform.position, _drawCardDuration).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            Destroy(cardModel);
            player.AddCard(card);
        });
        return card;
    }
    public void NavigateRight()
    {
        if (aiHand.CardModels.Count == 0)
        {
            return;
        }
        //Debug.Log("NavigateLeft");
        aiHand.CardModels[CurrentCardIndex].GetComponent<CardBackVisual>().DeselectCard();
        CurrentCardIndex--;
        aiHand.CardModels[CurrentCardIndex].GetComponent<CardBackVisual>().SelectCard();
        aiPlayer.SwitchExpression(CurrentCardIndex);

    }

    public void NavigateLeft()
    {
        if (aiHand.CardModels.Count == 0)
        {
            return;
        }
        //Debug.Log("NavigateRight");
        aiHand.CardModels[CurrentCardIndex].GetComponent<CardBackVisual>().DeselectCard();
        CurrentCardIndex++;
        aiHand.CardModels[CurrentCardIndex].GetComponent<CardBackVisual>().SelectCard();
        aiPlayer.SwitchExpression(CurrentCardIndex);
    }
}
