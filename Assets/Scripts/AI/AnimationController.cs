using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public enum ExpressionType
{
    Neutral,
    Sad,
    Happy,
    Surprised,
    Thinking,
}
public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance;
    private Animator animator;

    private readonly int idleTrigger = Animator.StringToHash("Idle");
    private readonly int playCardTrigger = Animator.StringToHash("PlayCard");
    private readonly int drawCardTrigger = Animator.StringToHash("DrawCard");
    private readonly int takeCardTrigger = Animator.StringToHash("TakeCard");
    private readonly int motionParam = Animator.StringToHash("Motion");

    // Expression parameter (int) on a separate layer
    private readonly int happyExpression = Animator.StringToHash("Happy");
    private readonly int sadExpression = Animator.StringToHash("Sad");
    private readonly int neutralExpression = Animator.StringToHash("Neutral");
    private readonly int thinkExpression = Animator.StringToHash("Think");
    public VoiceClipGroupSO voiceClipGroupSO;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple AnimationController instances detected.");
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found.");
            return;
        }
    }


    public void SetMotionState(int motion)
    {
        Debug.Log("SetMotionState " + motion);
        animator.SetInteger(motionParam, motion);
    }
    public void SetMotionState(string state)
    {
        ResetMotionTriggers();

        switch (state.ToLower())
        {
            case "idle":
                animator.SetTrigger(idleTrigger);
                break;
            case "playcard":
                animator.SetTrigger(playCardTrigger);
                break;
            case "drawcard":
                animator.SetTrigger(drawCardTrigger);
                break;
            case "takecard":
                animator.SetTrigger(takeCardTrigger);
                break;
            default:
                Debug.LogWarning("Invalid motion state");
                break;
        }
    }
    public void SetExpression(ExpressionType expression, bool audio=true)
    {
        ResetExpressionTriggers();
        switch (expression)
        {
            case ExpressionType.Neutral:
                animator.SetTrigger(neutralExpression);
                break;
            case ExpressionType.Sad:
                animator.SetTrigger(sadExpression);
                break;
            case ExpressionType.Happy:
                animator.SetTrigger(happyExpression);
                break;
            case ExpressionType.Thinking:
                animator.SetTrigger(thinkExpression);
                break;
            default:
                Debug.LogWarning("Invalid expression type");
                break;
        }
        if (audio) PlayAudioClip(expression);
        //reset back to neutral expression after a while
        StartCoroutine(WaitAndDo(2.0f, () =>
        {
            animator.SetTrigger(neutralExpression);
        }));
    }
    public void SetCallBack(string state, UnityAction callback)
    {
        StartCoroutine(WaitForAnimationToEnd(state, callback));
    }

    private IEnumerator WaitForAnimationToEnd(string state, UnityAction callback)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        int targetHash = GetTriggerHash(state);
        Debug.Log("animation time:" + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        while (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != targetHash)
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.1f)
        {
            yield return null;
        }

        callback?.Invoke();
    }

    private IEnumerator WaitAndDo(float waitTime, UnityAction callback)
    {
        yield return new WaitForSeconds(waitTime);
        callback?.Invoke();
    }
    private int GetTriggerHash(string state)
    {
        switch (state.ToLower())
        {
            case "idle": return idleTrigger;
            case "playcard": return playCardTrigger;
            case "drawcard": return drawCardTrigger;
            case "takecard": return takeCardTrigger;
            default:
                Debug.LogWarning("Invalid state name.");
                return -1;
        }
    }
    private void ResetMotionTriggers()
    {
        animator.ResetTrigger(idleTrigger);
        animator.ResetTrigger(playCardTrigger);
        animator.ResetTrigger(drawCardTrigger);
        animator.ResetTrigger(takeCardTrigger);
    }

    private void ResetExpressionTriggers()
    {
        animator.ResetTrigger(happyExpression);
        animator.ResetTrigger(sadExpression);
        animator.ResetTrigger(neutralExpression);
        animator.ResetTrigger(thinkExpression);
    }

    private void PlayAudioClip(ExpressionType expression)
    {
        foreach (var item in voiceClipGroupSO.voiceClipGroups)
        {
            if (item.type == expression)
            {
                //randomly select a clip from the list
                int index = Random.Range(0, item.audioClips.Count);
                AudioManager.Instance.Play(item.audioClips[index]);
                return;
            }
        }
    }

}
