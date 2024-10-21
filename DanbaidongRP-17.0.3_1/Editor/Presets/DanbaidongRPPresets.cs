using UnityEditor.Presets;

namespace UnityEditor.Rendering.Universal
{
    public class DanbaidongRPPresets
    {
        public DefaultPreset directionalLightPreset;
        public DefaultPreset pointLightPreset;
        public DefaultPreset spotLightPreset;
        public DefaultPreset meshRendererPreset;
        //public DefaultPreset skinnedMeshRendererPreset; // there are some problems with skinned mesh preset.

        public static class PresetFilter
        {
            public static readonly string dirLight = "Directional";
            public static readonly string pointLight = "Point";
            public static readonly string spotLight = "Spot";
            public static readonly string meshRenderer = string.Empty;
        }

        private static class PresetsName
        {
            public static readonly string dirLight = "DirectionalLight.preset";
            public static readonly string pointLight = "PointLight.preset";
            public static readonly string spotLight = "SpotLight.preset";
            public static readonly string meshRenderer = "MeshRenderer.preset";
        }
        public void LoadPipelinePresets()
        {
            directionalLightPreset = new DefaultPreset(PresetFilter.dirLight, DanbaidongRPPresetUtils.LoadPreset(PresetsName.dirLight));
            pointLightPreset = new DefaultPreset(PresetFilter.pointLight, DanbaidongRPPresetUtils.LoadPreset(PresetsName.pointLight));
            spotLightPreset = new DefaultPreset(PresetFilter.spotLight, DanbaidongRPPresetUtils.LoadPreset(PresetsName.spotLight));
            meshRendererPreset = new DefaultPreset(PresetFilter.meshRenderer, DanbaidongRPPresetUtils.LoadPreset(PresetsName.meshRenderer));
        }
    }

}
