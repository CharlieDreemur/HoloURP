

namespace UnityEngine.Rendering.Universal
{
    [ExecuteAlways]
    [AddComponentMenu("Rendering/Character Render Helper")]
    public class CharacterRenderHelper : MonoBehaviour
    {
        public Transform faceTrans;
        public Material[] faceMaterial;

        void Update()
        {
            if (faceTrans && faceMaterial.Length > 0)
            {
                foreach (Material mat in faceMaterial)
                {
                    mat.SetVector(ShaderConstants._FaceRightDirWS, faceTrans.right);
                    mat.SetVector(ShaderConstants._FaceFrontDirWS, faceTrans.forward);
                    mat.SetVector(ShaderConstants._HeadCenterWS, faceTrans.position);

                    mat.SetMatrix(ShaderConstants._WorldToHeadMatrix, faceTrans.worldToLocalMatrix);
                }
            }
        }

        static class ShaderConstants
        {
            public static readonly int _FaceRightDirWS = Shader.PropertyToID("_FaceRightDirWS");
            public static readonly int _FaceFrontDirWS = Shader.PropertyToID("_FaceFrontDirWS");
            public static readonly int _HeadCenterWS = Shader.PropertyToID("_HeadCenterWS");
            public static readonly int _WorldToHeadMatrix = Shader.PropertyToID("_WorldToHeadMatrix");
        }
    }

}