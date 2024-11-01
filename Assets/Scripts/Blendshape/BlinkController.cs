using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BlinkController : MonoBehaviour
{
    public SkinnedMeshRenderer faceMesh; // Assign the SkinnedMeshRenderer with blend shapes here
    public int blinkIndex; // The index of the blend shape for blinking
    public int eyebowIndex; // The index of the blend shape for blinking
    public float blinkDuration = 0.15f; // Time for each blink (up and down)
    public float blinkMinInterval = 4.0f;
    public float blinkMaxInterval = 6.0f;

    private void Start()
    {
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float blinkInterval = Random.Range(blinkMinInterval, blinkMaxInterval);
            yield return new WaitForSeconds(blinkInterval);
            PlayBlinkAnimation(blinkIndex);
            PlayBlinkAnimation(eyebowIndex);
        }
    }

    private void PlayBlinkAnimation(int index)
    {
        // Animate the blend shape weight from 0 to 100 and back to 0 for a blink
        faceMesh.SetBlendShapeWeight(index, 0);
        DOTween.To(
            () => faceMesh.GetBlendShapeWeight(index),
            weight => faceMesh.SetBlendShapeWeight(index, weight),
            100f,
            blinkDuration
        )
        .SetEase(Ease.InOutSine)
        .OnComplete(() => 
        {
            DOTween.To(
                () => faceMesh.GetBlendShapeWeight(index),
                weight => faceMesh.SetBlendShapeWeight(index, weight),
                0f,
                blinkDuration
            )
            .SetEase(Ease.InOutSine);
        });
    }
}
