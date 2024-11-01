using System.Collections.Generic;
using UnityEngine;
public enum ExpressionType
{
    Neutral,
    Sad,
    Happy,
    Surprised,
    Thinking,
}
public class ExpressionSwitcher : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public VoiceClipGroupSO voiceClipGroupSO;
    public void SwitchExpression(ExpressionType expression)
    {
        int index = (int)expression;
        animator.SetInteger("Expression", index);
    }

    private void PlayAudioClip(ExpressionType expression)
    {
        foreach (var item in voiceClipGroupSO.voiceClipGroups)
        {
            if (item.type == expression)
            {
                //randomly select a clip from the list
                int index = Random.Range(0, item.audioClips.Count);
                audioSource.Stop();
                audioSource.clip = item.audioClips[index];
                audioSource.Play();
                return;
            }
        }
    }
}
