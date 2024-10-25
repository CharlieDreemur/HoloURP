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
    [Header("Debug")]
    [SerializeField]
    private List<GameObject> _deckVisuals = new List<GameObject>();

    void Awake(){
        _cardDeck.DrawCardAnimationEvent.AddListener(PlayDrawCardAnimatin);
        _cardDeck.InitFinishEvent.AddListener(InitVisual);
    }

    private void InitVisual()
    {
        for (int i = 0; i < _cardDeck.cards.Count; i++)
        {
            GameObject newDeckBody = Instantiate(_deckBodyPrefab, transform);
            newDeckBody.transform.localPosition = new Vector3(0, 0, i * _deckBodySpacing);
            _deckVisuals.Add(newDeckBody);
            CardVisual cardVisual = newDeckBody.GetComponent<CardVisual>();
            cardVisual.SetCard(_cardDeck.cards[_cardDeck.cards.Count-i-1]);
        }
    }


    private void PlayDrawCardAnimatin(int n, UnityAction callback = null)
    {

        for (int i = 0; i < n; i++)
        {
            GameObject topCard = _deckVisuals[_deckVisuals.Count - 1];
            Debug.Log("Drawing card from deck:" + topCard);
            _deckVisuals.RemoveAt(_deckVisuals.Count - 1);
            topCard.transform.DOMove(_playerHandTransform.position, _drawDuration).SetEase(Ease.OutCubic);
            topCard.transform.DORotateQuaternion(_playerHandTransform.rotation, _drawDuration).SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    Debug.Log("Card drawn to hand.");
                    Destroy(topCard);
                    callback?.Invoke();
                });
        }
    }

}
