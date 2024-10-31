using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(SpriteRenderer))]
public class CardVisual : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField]
    private CardSpriteSO cardTextureSO;
    private Material cardMaterial;
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    void Awake(){
        cardMaterial = GetComponent<SpriteRenderer>().material;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
    }
    public void SetCard(ICard card)
    {
        if (card is NumberCard){
            NumberCard numberCard = (NumberCard)card;
            //Debug.Log("Set card number: " + numberCard.cardNumber);
            spriteRenderer.sprite = cardTextureSO.numberCardSprite[numberCard.Number-2]; //minus 1 because the card number is in the range of 1 to 4
        }
        else if (card is BombCard)
        {
            spriteRenderer.sprite = cardTextureSO.bombCardSprite;
        }
        else
        {
            Debug.LogError("Card type not found!");
        }
    }


    [ContextMenu("SelectCard")]
    public void SelectCard()
    {
        cardMaterial.SetFloat("_InnerOutlineThickness", 3f);
    }

    [ContextMenu("DeselectCard")]
    public void DeselectCard()
    {
        cardMaterial.SetFloat("_InnerOutlineThickness", 0.0f);
    }
}
