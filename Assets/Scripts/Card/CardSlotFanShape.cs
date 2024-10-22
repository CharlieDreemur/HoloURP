using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSlotFanShape : MonoBehaviour
{
    [Header("Fan Settings")]
    public GameObject cardPrefab;
    public int cardCount = 5;
    public float verticalSpacing = 0.1f;
    public float horizontalSpacing = 0.1f;
    public float fanAngle = 45.0f;
    public float maxSelfRotation = 30.0f;
    private int previousCardCount;
    private List<GameObject> cards;

    [Header("Animation Settings")]
    public float arrangeDuration = 0.5f; // Duration of the fan arrangement animation

    void Start()
    {
        cards = new List<GameObject>();
        previousCardCount = cardCount;
        UpdateCardList();
        ArrangeCardsInFan();
    }

    void Update()
    {
        if (cardCount != previousCardCount)
        {
            UpdateCardList();
            previousCardCount = cardCount;
        }
        ArrangeCardsInFan();

    }
    [ContextMenu("ArrangeCardsInFan")]
    public void ArrangeCardsInFan()
    {
        if (cards == null || cards.Count == 0) return;

        // Calculate the angle step based on the number of cards
        float angleStep = fanAngle / (cards.Count - 1);
        float startAngle = -fanAngle / 2;

        for (int i = 0; i < cards.Count; i++)
        {
            float angle = startAngle + (i * angleStep);
            Vector3 cardPosition = CalculateCardPosition(i);
            Quaternion cardRotation = CalculateCardRotation(angle, i);

            cards[i].transform.DOMove(cardPosition, arrangeDuration).SetEase(Ease.OutCubic);
            cards[i].transform.DORotateQuaternion(cardRotation, arrangeDuration).SetEase(Ease.OutCubic);
        }
    }

    private Vector3 CalculateCardPosition(int index)
    {
        // Calculate horizontal position based on index and horizontalSpacing
        float x = (index - (cards.Count - 1) / 2.0f) * horizontalSpacing;

        // Calculate vertical offset, with the middle card having the most displacement
        float verticalEffectFactor = Mathf.Abs(index - (cards.Count - 1) / 2.0f) / (cards.Count - 1) * 2;
        float y = Mathf.Lerp(verticalSpacing, 0, verticalEffectFactor);
        return new Vector3(x, y, 0) + transform.position;
    }

    private Quaternion CalculateCardRotation(float angle, int index)
    {
        // Determine the proportion of the self-rotation based on the card's index in the fan
        float proportion = 1 - (float)index / (cards.Count - 1);
        float rotateAngel = Mathf.Lerp(-maxSelfRotation, maxSelfRotation, proportion);

        // Combine the card's self-rotation with its rotation facing outward
        return Quaternion.Euler(0, angle, rotateAngel);
    }
    private void UpdateCardList()
    {
        // Add or remove cards to match the specified card count
        if (cardCount > cards.Count)
        {
            // Add new cards if cardCount is increased
            int cardsToAdd = cardCount - cards.Count;
            for (int i = 0; i < cardsToAdd; i++)
            {
                GameObject newCard = Instantiate(cardPrefab, transform);
                cards.Add(newCard);
            }
        }
        else if (cardCount < cards.Count)
        {
            // Remove extra cards if cardCount is decreased
            int cardsToRemove = cards.Count - cardCount;
            for (int i = 0; i < cardsToRemove; i++)
            {
                GameObject cardToRemove = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                Destroy(cardToRemove);
            }
        }
    }

}
