using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct VoiceClipGroup
{
    public ExpressionType type;
    public List<AudioClip> audioClips;
}
[CreateAssetMenu(fileName = "VoiceClipGroupSO", menuName = "Scriptable Objects/VoiceClipGroupSO")]
public class VoiceClipGroupSO : ScriptableObject
{
    public List<VoiceClipGroup> voiceClipGroups;
}
