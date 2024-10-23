using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
public class HandZone : MonoBehaviour
{
    [Header("Fan Settings")]
    public GameObject cardPrefab;
    public int cardCount = 5;
    public float verticalSpacing = 0.1f;
    public float horizontalSpacing = 0.1f;
    public float fanAngle = 45.0f;
    public float maxSelfRotation = 30.0f;

    [Header("Animation Settings")]
    public float arrangeDuration = 0.5f; // Duration of the fan arrangement animation
    public float playDuration = 0.8f; // Duration of the throw animation

    public Transform tablePosition; // Position on the table where played cards will go
    public TableZone tableZone;
    public int CurrentCardIndex
    {
        get { return currentCardIndex; }
        set {
            if (value < 0)
            {
                currentCardIndex = 0;
                return;
            }
            if (value >= cards.Count)
            {
                currentCardIndex = cards.Count - 1;
                return;
            }
            currentCardIndex = value;
        }
    }
    [SerializeField]
    private int currentCardIndex = 0;
    private int previousCardCount;
    private List<GameObject> cards;
    private InputControls controls;
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    void Awake(){
        controls = new InputControls();
        controls.Player.PlayCard.performed += ctx => PlayCard(CurrentCardIndex);
        controls.Player.NavigateLeft.performed += ctx => NavigateLeft();
        controls.Player.NavigateRight.performed += ctx => NavigateRight();

    }

    void Start()
    {
        cards = new List<GameObject>();
        previousCardCount = cardCount;
        UpdateCardList();
        ArrangeCardsInFan();
        //cards[CurrentCardIndex].GetComponent<CardVisualizer>().SelectCard();
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
                //Destroy(cardToRemove);
            }
        }
    }

    // Method to play a card and move it to the table with a throw effect
    public void PlayCard(int index)
    {
        Debug.Log("Playing card " + index);
        if (index < 0 || index >= cards.Count)
        {
            Debug.LogError("Invalid card index.");
            return;
        }
        GameObject card = cards[index];
        card.GetComponent<CardVisualizer>().DeselectCard();
        cards.Remove(card);

        Sequence slideSequence = DOTween.Sequence();
        slideSequence.Append(card.transform.DOMove(tablePosition.position, playDuration).SetEase(Ease.OutCubic));

        slideSequence.Join(card.transform.DORotate(new Vector3(0, 0, Random.Range(-20f, 20f)), playDuration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));

        // After throw, add the card to the table's list of cards
        slideSequence.OnComplete(() =>
        {
            tableZone.AddCardToTable(card);
            Destroy(card);
        });

    }
    private void NavigateLeft()
    {
        cards[CurrentCardIndex].GetComponent<CardVisualizer>().DeselectCard();
        CurrentCardIndex--;
        cards[CurrentCardIndex].GetComponent<CardVisualizer>().SelectCard();

    }

    private void NavigateRight()
    {
        cards[CurrentCardIndex].GetComponent<CardVisualizer>().DeselectCard();
        CurrentCardIndex++;
        cards[CurrentCardIndex].GetComponent<CardVisualizer>().SelectCard();
    }

}
