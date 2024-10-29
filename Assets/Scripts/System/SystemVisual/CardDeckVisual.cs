using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;
[RequireComponent(typeof(CardDeck))]
public class CardDeckVisual : MonoBehaviour
{
    [SerializeField]
    private CardDeck _cardDeck;
    [Header("Deck Visuals")]
    [SerializeField]
    private GameObject _deckBodyPrefab;
    [SerializeField]

    [Header("Draw Setting")]
    private float _drawDuration = 0.5f;
    [SerializeField]
    private float _deckBodySpacing = 0.002f;
    [SerializeField]
    private float _drawCardHorizontalSpacing = 0.1f;
    [Header("Debug")]
    [SerializeField]
    private List<GameObject> _cardModels = new List<GameObject>();

    void Awake()
    {
        _cardDeck.DrawCardAnimationEvent.AddListener(PlayDrawCardAnimatin);
        _cardDeck.AddCardsEvent.AddListener(AddCardVisuals);
    }

     public void AddCardVisuals(List<CardBase> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardModel = Instantiate(_deckBodyPrefab, transform);
            cardModel.transform.localPosition = new Vector3(0, 0, -i * _deckBodySpacing);
            cardModel.GetComponent<CardVisual>().SetCard(cards[i]);
            _cardModels.Add(cardModel);
        }
    }

    private void PlayDrawCardAnimatin(PlayerBase player, int n, UnityAction callback = null)
    {
        float totalWidth = (n - 1) * _drawCardHorizontalSpacing;
        float startOffset = -totalWidth / 2;
        for (int i = 0; i < n; i++)
        {
            GameObject topCard = _cardModels[_cardModels.Count - 1];
            //Debug.Log("Drawing card from deck:" + topCard);
            _cardModels.RemoveAt(_cardModels.Count - 1);
            Debug.Log("Player hand transform:" + player._handTransform);
            Vector3 targetPosition = player._handTransform.position + new Vector3(startOffset + (i * _drawCardHorizontalSpacing), 0, 0);
            Sequence drawSequence = DOTween.Sequence();
            drawSequence.Append(topCard.transform.DOMove(targetPosition, _drawDuration).SetEase(Ease.OutCubic));
            drawSequence.Join(topCard.transform.DORotateQuaternion(player._handTransform.rotation, _drawDuration).SetEase(Ease.OutCubic));
            //if it's the last card, invoke the callback
            if (i == n - 1)
            {
                drawSequence.OnComplete(() =>
                {
                    //Debug.Log("Drawing next card.");
                    Destroy(topCard);
                    callback?.Invoke();
                });
            }
            else
            {
                drawSequence.OnComplete(() =>
                {
                    //Debug.Log("Drawing next card.");
                    Destroy(topCard);
                });
            }
        }
    }

}
