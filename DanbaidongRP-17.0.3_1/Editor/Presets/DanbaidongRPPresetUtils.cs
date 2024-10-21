using System.Linq;
using UnityEditor.Presets;


namespace UnityEditor.Rendering.Universal
{
    public class DanbaidongRPPresetUtils
    {
        public static readonly string pipelinePresetsPath = "Packages/com.unity.render-pipelines.danbaidong/Editor/Presets/";

        public static Preset LoadPreset(string presetName)
        {
            return AssetDatabase.LoadAssetAtPath<Preset>(pipelinePresetsPath + presetName);
        }

        public static void InsertAsFirstDefault(ref DefaultPreset defaultPreset)
        {
            var type = defaultPreset.preset.GetPresetType();
            if (type.IsValidDefault())
            {
                var list = Preset.GetDefaultPresetsForType(type).ToList();
                list.Insert(0, defaultPreset);
                Preset.SetDefaultPresetsForType(type, list.ToArray());
            }
        }

        public static bool CheckPresetIsDefault(ref DefaultPreset defaultPreset)
        {
            var preset = defaultPreset.preset;
            var filter = defaultPreset.filter;
            var type = preset.GetPresetType();
            if (type.IsValidDefault())
            {
                var list = Preset.GetDefaultPresetsForType(type).ToList();
                foreach (var p in list)
                {
                    if (p.filter.Equals(filter) && p.preset == preset)
                        return true;
                }
            }
            return false;
        }
    }
}
