using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[RequireComponent(typeof(SpriteRenderer))]
public class CardBackVisual : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField]
    private GameObject cardBack;
    private Material cardFrontMaterial;

    void Awake(){
        cardFrontMaterial = cardBack.GetComponent<SpriteRenderer>().material;
    }


    [ContextMenu("SelectCard")]
    public void SelectCard()
    {
        cardFrontMaterial.SetFloat("_InnerOutlineThickness", 3f);
    }

    [ContextMenu("DeselectCard")]
    public void DeselectCard()
    {
        cardFrontMaterial.SetFloat("_InnerOutlineThickness", 0.0f);
    }
}
