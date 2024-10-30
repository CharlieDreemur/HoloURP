using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// PlayZone represents the area on the table where cards are placed after being played.
/// </summary>
public class PlayZoneVisual : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.5f; // Duration for each card movement
    [SerializeField] private float delayBetweenCards = 0.1f; // Delay between each card moving
    [SerializeField]
    private List<GameObject> cardModels = new List<GameObject>();
    [SerializeField]
    private PlayZone _playZone;
    [SerializeField]
    private Transform _cardDeckTransform;
    [SerializeField]
    private CardVisual _LastCardVisual;
    private Material _LastCardVisualMaterial;
    [SerializeField]
    private float _LastCardShowDuration = 0.5f;
    void Awake(){
        _playZone.AddCardsToDeckEvent.AddListener(PlayAddCardsIntoDeckAnimation);
        _playZone.HideLastCardEvent.AddListener(HidePlayZoneLastCard);
        _playZone.ShowLastCardEvent.AddListener(ShowPlayZoneLastCard);
        _LastCardVisualMaterial = _LastCardVisual.GetComponent<SpriteRenderer>().material;
        HidePlayZoneLastCard();
    }
    public void HidePlayZoneLastCard()
    {
        _LastCardVisualMaterial.SetFloat("_HologramMinAlpha", 0f);
        _LastCardVisualMaterial.SetFloat("_HologramMaxAlpha", 0f);
    }
    public void ShowPlayZoneLastCard(NumberCard card)
    {
        Debug.Log("UpdatePlayZoneLastCard");
        _LastCardVisualMaterial.DOFloat(0.1f, "_HologramMinAlpha", _LastCardShowDuration).SetEase(Ease.InOutCubic);
        _LastCardVisualMaterial.DOFloat(0.75f, "_HologramMaxAlpha", _LastCardShowDuration).SetEase(Ease.InOutCubic);
        _LastCardVisual.SetCard(card);
    }
    // Add a card to the table
    public void AddCardModelsToTable(GameObject card)
    {
        card.transform.SetParent(transform);
        cardModels.Add(card);
    }

    public void PlayAddCardsIntoDeckAnimation(UnityAction callback)
    {
        Debug.Log("PlayAddCardsIntoDeckAnimation");
        Sequence returnSequence = DOTween.Sequence();
        for (int i = 0; i < cardModels.Count; i++)
        {
            GameObject card = cardModels[i];
            Vector3 targetPosition = _cardDeckTransform.position;

            returnSequence
                .Insert(i * delayBetweenCards, card.transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutCubic))
                .Insert(i * delayBetweenCards, card.transform.DORotate(new Vector3(270, 0, 0), moveDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic));
        }

        // Clear the list after the animation is completed
        returnSequence.OnComplete(() =>
        {
            foreach (var card in cardModels)
            {
                Destroy(card);
            }
            cardModels.Clear();
            callback?.Invoke();
        });
    }
}
