using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class AIHandZoneVisual : HandZoneVisualBase
{
    public Transform LeftHandTransform;
    public Transform RightHandTransform;
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

    protected override void RunPlayCardAnimation(List<int> indexes)
    {
        AnimationController.Instance.PlayAudioClip(ExpressionType.Thinking);
        AnimationController.Instance.SetMotionState("playcard");
        List<GameObject> cards = new List<GameObject>();
        UnityAction prepareAnimation = () =>
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                int index = indexes[i];
                GameObject card = _cardModels[index];
                card.GetComponent<CardVisual>().DeselectCard();
                _cardModels.RemoveAt(index);
                card.transform.SetParent(RightHandTransform);
                card.transform.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.InOutCubic);
                CurrentCardIndex--;
                cards.Add(card);
            }
        };

        StartCoroutine(waitAndDo(2.9f, prepareAnimation));
        UnityAction playCardAnimation = () =>
        {
            Debug.Log("play card move animation");
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject card = cards[i];
                card.transform.SetParent(tableTransform);
                Sequence slideSequence = DOTween.Sequence();
                Vector3 randomDestPos = tableTransform.position + new Vector3(Random.Range(-randomTableOffset, randomTableOffset), 0, Random.Range(-randomTableOffset, randomTableOffset));
                slideSequence.Join(card.transform.DOMove(randomDestPos, 0.1f).SetEase(Ease.OutCubic));
                slideSequence.Join(card.transform.DORotate(new Vector3(90, 0, Random.Range(-20f, 20f)), 0.1f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
            }
        };
    
        StartCoroutine(waitAndDo(3.4f, playCardAnimation));
        UnityAction finishCallback = () =>
            {
                Debug.Log("animation end, add cards to table");
                playZoneVisual.AddCardModelsToTable(cards);
                ArrangeCardsInFan();
            };
        StartCoroutine(waitAndDo(4.3f, finishCallback));

    }
    private IEnumerator waitAndDo(float waitTime, UnityAction callback)
    {
        yield return new WaitForSeconds(waitTime);
        callback();
    }
}
