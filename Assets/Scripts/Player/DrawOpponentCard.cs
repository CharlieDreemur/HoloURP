using UnityEngine;
using DG.Tweening;
using System.Collections;
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
    private int _previousIndex = 0;
    [SerializeField]
    private float timeSinceLastChange = 0f;
    [SerializeField]
    private int _currentCardIndex = 0;
    [SerializeField]
    private float switchDelay = 1.0f;
    public void EnableExpressionSwitcher()
    {
        StartCoroutine(CheckIndexChange());
    }
    public void DisableExpressionSwitcher()
    {
        StopCoroutine(CheckIndexChange());
    }
    public CardBase DrawCard()
    {
        AudioManager.Instance.Play("carddraw");
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
    }

    private IEnumerator CheckIndexChange()
    {
        while (true)
        {
            if (CurrentCardIndex == _previousIndex)
            {
                timeSinceLastChange += Time.deltaTime;
                if (timeSinceLastChange >= switchDelay)
                {
                    aiPlayer.SwitchExpression(CurrentCardIndex);
                    timeSinceLastChange = 0f;
                }
            }
            else
            {
                timeSinceLastChange = 0f;
                _previousIndex = CurrentCardIndex;
            }

            yield return null; // Wait until the next frame
        }
    }
}
