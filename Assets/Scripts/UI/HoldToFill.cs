using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToFill : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text fillText;
    [SerializeField] private float holdDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private InputActionReference holdActionReference;

    private bool isHolding = false;
    private float holdTime = 0f;

    private void OnEnable()
    {
        fillImage.fillAmount = 0f;
        fillImage.color = new Color(fillImage.color.r, fillImage.color.g, fillImage.color.b, 0f);
        fillText.color = new Color(fillText.color.r, fillText.color.g, fillText.color.b, 0f);
        holdActionReference.action.started += OnHoldStarted;
        holdActionReference.action.canceled += OnHoldCanceled;
    }

    private void OnDisable()
    {
        // Disable the action and unsubscribe from the events
        holdActionReference.action.started -= OnHoldStarted;
        holdActionReference.action.canceled -= OnHoldCanceled;
    }

    private void Update()
    {
        if (isHolding)
        {
            // Increment the hold time and update the fill amount
            holdTime += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(holdTime / holdDuration);

            // If the hold duration is reached, trigger the action
            if (holdTime >= holdDuration)
            {
                OnHoldCompleted();
                ResetHold();
            }
        }
    }

    private void OnHoldStarted(InputAction.CallbackContext context)
    {
        // Start holding when the button is pressed
        isHolding = true;
        holdTime = 0f;
        fillImage.fillAmount = 0f;
        fillImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutCubic);
        fillText.DOFade(1f, fadeDuration).SetEase(Ease.InOutCubic);
    }

    private void OnHoldCanceled(InputAction.CallbackContext context)
    {
        ResetHold();
    }

    private void OnHoldCompleted()
    {
        
    }

    private void ResetHold()
    {
        isHolding = false;
        holdTime = 0f;
        fillText.DOFade(0f, fadeDuration).SetEase(Ease.InOutCubic);
        fillImage.DOFade(0f, fadeDuration).SetEase(Ease.InOutCubic).OnComplete(() => fillImage.fillAmount = 0f);
    }
}
