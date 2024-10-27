using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
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
    void Awake(){
        _playZone.AddCardsIntoDeckEvent.AddListener(PlayAddCardsIntoDeckAnimation);
    }
    // Add a card to the table
    public void AddCardModelsToTable(GameObject card)
    {
        card.transform.SetParent(transform);
        cardModels.Add(card);
    }

    // Remove all cards from the table and return them
    public void PlayAddCardsIntoDeckAnimation()
    {
        Debug.Log("PlayAddCardsIntoDeckAnimation");
        for(int i = 0; i < cardModels.Count; i++)
        {
            Destroy(cardModels[i]);
        }
    }

    public void ReturnCardsToDeck()
    {

        Sequence returnSequence = DOTween.Sequence();
        for (int i = 0; i < cardModels.Count; i++)
        {
            GameObject card = cardModels[i];
            Vector3 targetPosition = _cardDeckTransform.position;

            Vector3 randomOffset = new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                0
            );

            returnSequence
                .Insert(i * delayBetweenCards, card.transform.DOMove(targetPosition + randomOffset, moveDuration).SetEase(Ease.InOutCubic))
                .Insert(i * delayBetweenCards, card.transform.DORotate(new Vector3(0, 180, 0), moveDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic));
        }

        // Clear the list after the animation is completed
        returnSequence.OnComplete(() =>
        {
            foreach (var card in cardModels)
            {
                Destroy(card); // Or disable the card, or pool it for reuse if you are using a pooling system
            }
            cardModels.Clear();
        });
    }
}
