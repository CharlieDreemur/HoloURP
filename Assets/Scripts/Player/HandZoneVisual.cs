using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class HandZoneVisual : MonoBehaviour
{
    [SerializeField]
    private CardPlayer _playerStats;
    [SerializeField]
    private Transform _handTransform;
    [Header("Fan Settings")]
    public GameObject cardPrefab;

    public float verticalSpacing = 0.1f;
    public float horizontalSpacing = 0.1f;
    public float fanAngle = 45.0f;
    public float maxSelfRotation = 30.0f;

    [Header("Animation Settings")]
    public float arrangeDuration = 0.5f; // Duration of the fan arrangement animation
    public float playDuration = 0.8f; // Duration of the throw animation
    public float verticalHideOffset = 0.5f; // Vertical offset when hiding the hand
    public float hideDuration = 0.5f; // Duration of the hide animation
    public float randomTableOffset = 0.1f; // Random offset for the table position
    public Transform tableTransform; // Position on the table where played cards will go
    public PlayZoneVisual playZoneVisual;
    [SerializeField]
    private bool _isPlaySequence = false;
    public int CurrentCardIndex
    {
        get { return _currentCardIndex; }
        set
        {
            if (value < 0)
            {
                value = _cardModels.Count - 1;
            }
            _currentCardIndex = Mathf.Clamp(value, 0, _cardModels.Count - 1);
        }
    }
    [SerializeField]
    private int _currentCardIndex = 0;
    [SerializeField]
    private List<GameObject> _cardModels;
    [SerializeField]
    private Vector3 _originalPos;
    [SerializeField]
    private bool _isHandHidden = false;
    void Awake()
    {
        _originalPos = _handTransform.position;
        _playerStats.AddCardEvent.AddListener(AddCardVisuals);
        _playerStats.PlayCardAnimationEvent.AddListener(RunPlayCardAnimation);
    }

    void Start()
    {
    }

    // }
    [ContextMenu("ArrangeCardsInFan")]
    public void ArrangeCardsInFan()
    {
        if (_cardModels == null || _cardModels.Count == 0) return;
        _isPlaySequence = true;
        Sequence arrangeSequence = DOTween.Sequence();
        if (_cardModels.Count == 1)
        {
            Vector3 cardPosition = new Vector3(0, 0, 0) + _handTransform.position;
            Quaternion cardRotation = Quaternion.Euler(0, 0, 0);
            arrangeSequence.Append(_cardModels[0].transform.DOMove(cardPosition, arrangeDuration).SetEase(Ease.OutCubic));
            arrangeSequence.Join(_cardModels[0].transform.DORotateQuaternion(cardRotation, arrangeDuration).SetEase(Ease.OutCubic));
            arrangeSequence.OnComplete(() =>
            {
                _isPlaySequence = false;
            });
            return;
        }

        // Calculate the angle step based on the number of cards
        float angleStep = fanAngle / (_cardModels.Count - 1);
        float startAngle = -fanAngle / 2;

        for (int i = 0; i < _cardModels.Count; i++)
        {
            float angle = startAngle + (i * angleStep);
            Vector3 cardPosition = CalculateCardPosition(i);
            Quaternion cardRotation = CalculateCardRotation(angle, i);
            arrangeSequence.Join(_cardModels[i].transform.DOMove(cardPosition, arrangeDuration).SetEase(Ease.OutCubic));
            arrangeSequence.Join(_cardModels[i].transform.DORotateQuaternion(cardRotation, arrangeDuration).SetEase(Ease.OutCubic));
        }
        arrangeSequence.OnComplete(() =>
        {
            _isPlaySequence = false;
        });
    }

    private Vector3 CalculateCardPosition(int index)
    {
        // Calculate horizontal position based on index and horizontalSpacing
        float x = (index - (_cardModels.Count - 1) / 2.0f) * horizontalSpacing;

        // Calculate vertical offset, with the middle card having the most displacement
        float verticalEffectFactor = Mathf.Abs(index - (_cardModels.Count - 1) / 2.0f) / (_cardModels.Count - 1) * 2;
        float y = Mathf.Lerp(verticalSpacing, 0, verticalEffectFactor);
        return new Vector3(x, y, 0) + _handTransform.position;
    }

    private Quaternion CalculateCardRotation(float angle, int index)
    {
        // Determine the proportion of the self-rotation based on the card's index in the fan
        float proportion = 1 - (float)index / (_cardModels.Count - 1);
        float rotateAngel = Mathf.Lerp(-maxSelfRotation, maxSelfRotation, proportion);

        // Combine the card's self-rotation with its rotation facing outward
        return Quaternion.Euler(0, angle, rotateAngel);
    }
    public void AddCardVisuals(List<CardBase> cards)
    {
        foreach (CardBase card in cards)
        {
            GameObject cardModel = Instantiate(cardPrefab, _handTransform);
            cardModel.GetComponent<CardVisual>().SetCard(card);
            _cardModels.Add(cardModel);
        }
        if (_isHandHidden)
        {
            ShowHand();
        }
        else
        {
            ArrangeCardsInFan();
        }
    }

    // Method to play a card and move it to the table with a throw effect
    private void RunPlayCardAnimation(List<int> indexes)
    {
        for (int i = 0; i < indexes.Count; i++)
        {
            int index = indexes[i];
            GameObject card = _cardModels[index];
            card.GetComponent<CardVisual>().DeselectCard();
            _cardModels.RemoveAt(index);
            card.transform.SetParent(tableTransform);
            CurrentCardIndex--;
            Sequence slideSequence = DOTween.Sequence();
            //do a random offset for the table position
            Vector3 randomDestPos = tableTransform.position + new Vector3(Random.Range(-randomTableOffset, randomTableOffset), 0, Random.Range(-randomTableOffset, randomTableOffset));
            slideSequence.Join(card.transform.DOMove(randomDestPos, playDuration).SetEase(Ease.OutCubic));

            slideSequence.Join(card.transform.DORotate(new Vector3(90, 0, Random.Range(-20f, 20f)), playDuration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
            HideHand();
            // After throw, add the card to the table's list of cards
            slideSequence.OnComplete(() =>
            {
                //List<CardBase> cards = new List<CardBase>{
                playZoneVisual.AddCardModelsToTable(card);
            });
        }
    }
    public void HideShowHand()
    {
        if (_isHandHidden)
        {
            ShowHand();
        }
        else
        {
            HideHand();
        }
    }
    public void HideHand(UnityAction callback = null)
    {
        if (_isPlaySequence || _isHandHidden)
        {
            return;
        }
        //Debug.Log("Hiding hand");
        _isHandHidden = true;
        _isPlaySequence = true;
        Sequence hideSequence = DOTween.Sequence();
        hideSequence.Append(_handTransform.DOMoveY(_originalPos.y - verticalHideOffset, hideDuration).SetEase(Ease.OutCubic));
        hideSequence.OnComplete(() =>
        {
            //Debug.Log("Hiding hand complete");
            _isPlaySequence = false;
            callback?.Invoke();
        });
    }

    public void ShowHand(UnityAction callback = null)
    {
        if (_isPlaySequence || !_isHandHidden)
        {
            return;
        }
        //Debug.Log("Showing hand");
        _isHandHidden = false;
        _isPlaySequence = true;
        Sequence showSequence = DOTween.Sequence();
        showSequence.Append(_handTransform.DOMoveY(_originalPos.y, hideDuration).SetEase(Ease.OutCubic));
        showSequence.OnComplete(() =>
        {
            //Debug.Log("Showing hand complete");
            ArrangeCardsInFan();
            callback?.Invoke();

        });
    }
    public void NavigateLeft()
    {
        if (_cardModels.Count == 0)
        {
            return;
        }
        //Debug.Log("NavigateLeft");
        _cardModels[CurrentCardIndex].GetComponent<CardVisual>().DeselectCard();
        CurrentCardIndex--;
        _cardModels[CurrentCardIndex].GetComponent<CardVisual>().SelectCard();

    }

    public void NavigateRight()
    {
        if (_cardModels.Count == 0)
        {
            return;
        }
        //Debug.Log("NavigateRight");
        _cardModels[CurrentCardIndex].GetComponent<CardVisual>().DeselectCard();
        CurrentCardIndex++;
        _cardModels[CurrentCardIndex].GetComponent<CardVisual>().SelectCard();
    }

}
