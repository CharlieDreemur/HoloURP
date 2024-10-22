using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardVisualizer : MonoBehaviour
{
    [Header("Card Settings")]
    public float drawDuration = 1.0f;
    private Vector3 initialPosition;  
    private Quaternion initialRotation;
    public bool IsSelected {
        get { return isSelected; }
        set { 
            isSelected = value; 
            if (isSelected)
            {
                SelectCard();
            }
            else
            {
                DeselectCard();
            }
        }
    }
    private bool isSelected = false;
    private Vector3 defaultScale;
    [ContextMenu("Init")]
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        defaultScale = transform.localScale;
    }

    [ContextMenu("DrawCard")]
    public void DrawCard(Vector3 targetPosition)
    {
        // Animate moving to target position with DoTween
        transform.DOMove(targetPosition, drawDuration).SetEase(Ease.OutCubic);
        transform.DORotate(new Vector3(0, 360, 0), drawDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic).OnComplete(() =>
            {
                Debug.Log("Card drawn!");
            });
    }

    [ContextMenu("ResetCard")]
    public void ResetCard()
    {
        transform.DOMove(initialPosition, drawDuration).SetEase(Ease.OutCubic);
        transform.DORotateQuaternion(initialRotation, drawDuration).SetEase(Ease.OutCubic);
    }

    [ContextMenu("SelectCard")]
    public void SelectCard()
    {
        transform.DOScale(defaultScale * 1.2f, 0.5f).SetEase(Ease.OutBounce);
    }

    [ContextMenu("DeselectCard")]
    public void DeselectCard()
    {
        transform.DOScale(defaultScale, 0.5f).SetEase(Ease.OutBounce);
    }

    void OnMouseDown()
    {
        Debug.Log("Card clicked!");
        IsSelected = !IsSelected;
    }
}
