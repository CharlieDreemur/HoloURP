using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
[RequireComponent(typeof(TMP_Text))]
public class TextIndicator : MonoBehaviour
{
    [SerializeField]
    private TMP_Text uiText;
    [SerializeField]
    private string text = "Your Turn!";
    [SerializeField]
    private float scaleDuration = 1.0f;

    [SerializeField]
    private float displayDuration = 1.5f;
    [SerializeField]
    private float scaleFactor = 1.5f;
    [SerializeField]
    private float showDuration = 0.5f;
    [SerializeField]
    private float fadeDuration = 0.5f;
    [SerializeField]
    private float dilateDuration = 0.5f;
    private Material textMaterial;
    private Vector3 originalScale;
    private void Awake()
    {
        originalScale = transform.localScale;
        TryGetComponent(out uiText);
        if (uiText == null)
        {
            Debug.LogError("TurnTextAnimator: No TextMeshPro component found on this object.");
        }
        uiText.text = text;
        uiText.fontMaterial = new Material(uiText.fontMaterial); //Material Instance
        textMaterial = uiText.fontMaterial;
        // Set the initial alpha of the text to 0 (invisible)
        SetTextAlpha(0);
    }
    [ContextMenu("AnimateText")]
    public void AnimateText(string text)
    {
        uiText.text = text;
        Sequence textSequence = DOTween.Sequence();
        textMaterial.SetFloat("_FaceDilate", 1f);
        //Scale up and show
        textSequence.Append(uiText.DOFade(1, showDuration).SetEase(Ease.InOutCubic));
        textSequence.Join(transform.DOScale(originalScale * scaleFactor, scaleDuration).SetEase(Ease.OutCubic));
        textSequence.Join(textMaterial.DOFloat(0, "_FaceDilate", dilateDuration).SetEase(Ease.OutCubic));
        //wait
        textSequence.AppendInterval(displayDuration);
        //fade 
        textSequence.Append(uiText.DOFade(0, fadeDuration).SetEase(Ease.InOutCubic));
        textSequence.onComplete += () =>
        {
            transform.localScale = originalScale;
            SetTextAlpha(0);
        };
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = uiText.color;
        color.a = alpha;
        uiText.color = color;
    }
}
