using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.5f;
    public int shakeVibrato = 10;
    public float rotateDuration = 1f;
    public float rotateAngle = 15f;
    public float scaleDuration = 0.5f;
    public float scaleMultiplier = 1.2f;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    void Awake(){
        if(Instance == null){
            Instance = this;
        }
        else if(Instance != this){
            Destroy(gameObject);
        }
    }

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    // Method to shake the camera
    public void ShakeCamera()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato)
            .OnComplete(() => transform.position = originalPosition); // Reset to original position after shake
    }

    // Method to rotate the camera by a certain angle
    public void RotateCamera()
    {
        transform.DORotate(new Vector3(0, rotateAngle, 0), rotateDuration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => transform.rotation = originalRotation); // Reset to original rotation after rotate
    }

    // Method to scale the camera up
    public void ScaleUp()
    {
        transform.DOScale(originalScale * scaleMultiplier, scaleDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => transform.DOScale(originalScale, scaleDuration)); // Return to original scale
    }

    // Method to scale the camera down
    public void ScaleDown()
    {
        transform.DOScale(originalScale / scaleMultiplier, scaleDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => transform.DOScale(originalScale, scaleDuration)); // Return to original scale
    }
}
