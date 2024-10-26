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
    private Transform _playerHandTransform;
    [SerializeField]
    private float _deckBodySpacing = 0.002f;
    [SerializeField]
    private float _drawCardHorizontalSpacing = 0.1f;
    [Header("Debug")]
    [SerializeField]
    private List<GameObject> _deckVisuals = new List<GameObject>();

    void Awake()
    {
        _cardDeck.DrawCardAnimationEvent.AddListener(PlayDrawCardAnimatin);
        _cardDeck.InitFinishEvent.AddListener(InitVisual);
    }

    private void InitVisual()
    {
        for (int i = 0; i < _cardDeck.cardDecks.Count; i++)
        {
            GameObject newDeckBody = Instantiate(_deckBodyPrefab, transform);
            newDeckBody.transform.localPosition = new Vector3(0, 0, i * _deckBodySpacing);
            _deckVisuals.Add(newDeckBody);
            CardVisual cardVisual = newDeckBody.GetComponent<CardVisual>();
            cardVisual.SetCard(_cardDeck.cardDecks[_cardDeck.cardDecks.Count - i - 1]);
        }
    }


    private void PlayDrawCardAnimatin(int n, UnityAction callback = null)
    {
        float totalWidth = (n - 1) * _drawCardHorizontalSpacing;
        float startOffset = -totalWidth / 2;
        for (int i = 0; i < n; i++)
        {
            GameObject topCard = _deckVisuals[_deckVisuals.Count - 1];
            Debug.Log("Drawing card from deck:" + topCard);
            _deckVisuals.RemoveAt(_deckVisuals.Count - 1);
            Vector3 targetPosition = _playerHandTransform.position + new Vector3(startOffset + (i * _drawCardHorizontalSpacing), 0, 0);
            Sequence drawSequence = DOTween.Sequence();
            drawSequence.Append(topCard.transform.DOMove(targetPosition, _drawDuration).SetEase(Ease.OutCubic));
            drawSequence.Join(topCard.transform.DORotateQuaternion(_playerHandTransform.rotation, _drawDuration).SetEase(Ease.OutCubic));
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
