using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(ProceduralToonSky))]
    sealed class ProceduralToonSkyEditor : SkySettingsEditor
    {
        SerializedDataParameter m_Material;


        public override void OnEnable()
        {
            base.OnEnable();

            m_CommonUIElementsMask = (uint)SkySettingsUIElement.UpdateMode
                                    | (uint)SkySettingsUIElement.SkyIntensity
                                    | (uint)SkySettingsUIElement.IncludeSunInBaking;

            var o = new PropertyFetcher<ProceduralToonSky>(serializedObject);

            m_Material = Unpack(o.Find(x => x.material));

        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_Material);


            base.CommonSkySettingsGUI();
        }
    }
}
