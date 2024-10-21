using UnityEngine.UIElements;

namespace UnityEditor.Rendering.Universal
{
    enum InclusiveMode
    {
        DanbaidongRP = 1 << 0,
        XRManagement = 1 << 1,
        VR = XRManagement | 1 << 2, //XRManagement is inside VR and will be indented
        DXR = 1 << 3,
        DXROptional = DXR | 1 << 4,
    }

    enum QualityScope { Global, Raytracing }

    static class InclusiveScopeExtention
    {
        public static bool Contains(this InclusiveMode thisScope, InclusiveMode scope)
            => ((~thisScope) & scope) == 0;
    }

    public class DanbaidongWizardEditorUtils
    {
        internal const string FormatingPath = @"Packages/com.unity.render-pipelines.danbaidong/Editor/DanbaidongWizard/USS/Formating";
        internal const string WizardSheetPath = @"Packages/com.unity.render-pipelines.danbaidong/Editor/DanbaidongWizard/USS/Wizard";

        private static (StyleSheet baseSkin, StyleSheet professionalSkin, StyleSheet personalSkin) LoadStyleSheets(string basePath)
        => (
            AssetDatabase.LoadAssetAtPath<StyleSheet>($"{basePath}.uss"),
            AssetDatabase.LoadAssetAtPath<StyleSheet>($"{basePath}Light.uss"),
            AssetDatabase.LoadAssetAtPath<StyleSheet>($"{basePath}Dark.uss")
        );

        internal static void AddStyleSheets(VisualElement element, string baseSkinPath)
        {
            (StyleSheet @base, StyleSheet personal, StyleSheet professional) = LoadStyleSheets(baseSkinPath);
            element.styleSheets.Add(@base);
            if (EditorGUIUtility.isProSkin)
            {
                if (professional != null && !professional.Equals(null))
                    element.styleSheets.Add(professional);
            }
            else
            {
                if (personal != null && !personal.Equals(null))
                    element.styleSheets.Add(personal);
            }
        }
    }
}
