using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(VisualSky))]
    sealed class VisualSkyEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_SkyType;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<VisualSky>(serializedObject);

            m_SkyType = Unpack(o.Find(x => x.skyType));
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_SkyType);
            EditorGUILayout.HelpBox("Add \"" + (SkyType)(m_SkyType.value.intValue) + " Sky\" override to see settings", MessageType.Info);
        }
    }
}
