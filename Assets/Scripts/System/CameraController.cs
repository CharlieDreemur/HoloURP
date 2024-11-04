using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

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
    public List<Transform> cameraPath;
    [SerializeField]
    public float lookSensitivity = 1f;
    [SerializeField]
    public float maxYawAngle = 90f;
    [SerializeField]
    public float maxLookAngle = 90f;
    [SerializeField]
    private float currentYaw = 0f;
    [SerializeField]
    private float currentPitch = 0f;
    [SerializeField]
    public float edgeThreshold = 0.1f;
    [SerializeField]
    public bool isFreeLook = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if(isFreeLook) HandleEdgeMouseLook();
    }

    private void HandleEdgeMouseLook()
    {
        Vector3 mousePosition = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate screen edge thresholds
        float leftThreshold = screenWidth * edgeThreshold;
        float rightThreshold = screenWidth * (1 - edgeThreshold);
        float topThreshold = screenHeight * (1 - edgeThreshold);
        float bottomThreshold = screenHeight * edgeThreshold;

        // Check if mouse is at the screen edges and rotate accordingly
        if (mousePosition.x < leftThreshold)
        {
            // Rotate left
            currentYaw -= lookSensitivity * Time.deltaTime;
        }
        else if (mousePosition.x > rightThreshold)
        {
            // Rotate right
            currentYaw += lookSensitivity * Time.deltaTime;
        }

        if (mousePosition.y > topThreshold)
        {
            // Rotate up
            currentPitch -= lookSensitivity * Time.deltaTime;
        }
        else if (mousePosition.y < bottomThreshold)
        {
            // Rotate down
            currentPitch += lookSensitivity * Time.deltaTime;
        }

        // Clamp the rotation angles
        currentYaw = Mathf.Clamp(currentYaw, -maxYawAngle, maxYawAngle);
        currentPitch = Mathf.Clamp(currentPitch, -maxLookAngle, maxLookAngle);

        // Apply rotation based on clamped pitch and yaw
        transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    public void ShakeCamera()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato)
            .OnComplete(() => transform.position = originalPosition); // Reset to original position after shake
    }
    [ContextMenu("PlayCameraAnimation")]
    public void PlayCameraAnimation()
    {
        Debug.Log("PlayCameraAnimation");
        isFreeLook = false;
        AnimateCameraPath(cameraPath, 1f, 1f);
    }

    public void RestoreCamera()
    {
        Sequence cameraSequence = DOTween.Sequence();
        cameraSequence.Append(transform.DOLocalRotateQuaternion(originalRotation, 0.5F * rotateDuration).SetEase(Ease.InOutSine));
        cameraSequence.Join(transform.DOScale(originalScale, 0.5F *scaleDuration).SetEase(Ease.InOutSine));
        cameraSequence.Join(transform.DOMove(originalPosition, 0.5F *rotateDuration).SetEase(Ease.InOutSine));
        cameraSequence.OnComplete(() =>
        {
            isFreeLook = true;
        });
    }
    
    public void AnimateCameraPath(List<Transform> targetTransforms, float moveDuration, float rotationDuration)
    {
        Sequence cameraSequence = DOTween.Sequence();

        foreach (Transform target in targetTransforms)
        {
            // Move to the next target position
            cameraSequence.Append(transform.DOMove(target.position, moveDuration).SetEase(Ease.InOutSine));

            // Rotate to the next target rotation
            cameraSequence.Join(transform.DORotateQuaternion(target.rotation, rotationDuration).SetEase(Ease.InOutSine));
        }

    }

}
