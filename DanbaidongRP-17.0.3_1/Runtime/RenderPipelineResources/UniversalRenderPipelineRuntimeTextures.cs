using System;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Class containing texture resources used in URP.
    /// </summary>
    [Serializable]
    [SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
    [Categorization.CategoryInfo(Name = "R: Runtime Textures", Order = 1000), HideInInspector]
    public class UniversalRenderPipelineRuntimeTextures : IRenderPipelineResources
    {
        [SerializeField][HideInInspector] private int m_Version = 1;

        /// <summary>
        ///  Version of the Texture resources
        /// </summary>
        public int version => m_Version;

        bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild => true;

        [SerializeField]
        [ResourcePath("Textures/BlueNoise64/L/LDR_LLL1_0.png")]
        private Texture2D m_BlueNoise64LTex;

        /// <summary>
        /// Pre-baked blue noise textures.
        /// </summary>
        public Texture2D blueNoise64LTex
        {
            get => m_BlueNoise64LTex;
            set => this.SetValueAndNotify(ref m_BlueNoise64LTex, value, nameof(m_BlueNoise64LTex));
        }

        [SerializeField]
        [ResourcePath("Textures/BayerMatrix.png")]
        private Texture2D m_BayerMatrixTex;

        /// <summary>
        /// Bayer matrix texture.
        /// </summary>
        public Texture2D bayerMatrixTex
        {
            get => m_BayerMatrixTex;
            set => this.SetValueAndNotify(ref m_BayerMatrixTex, value, nameof(m_BayerMatrixTex));
        }

        [SerializeField]
        [ResourcePath("Textures/DebugFont.tga")]
        private Texture2D m_DebugFontTex;

        /// <summary>
        /// Debug font texture.
        /// </summary>
        public Texture2D debugFontTexture
        {
            get => m_DebugFontTex;
            set => this.SetValueAndNotify(ref m_DebugFontTex, value, nameof(m_DebugFontTex));
        }

        /// <summary>
        /// Default HDRI Sky
        /// </summary>
        [SerializeField]
        [ResourcePath("Textures/Sky/DefaultHDRISky.exr")]
        private Cubemap m_DefaultHDRISky;

        public Cubemap defaultHDRISky
        {
            get => m_DefaultHDRISky;
            set => this.SetValueAndNotify(ref m_DefaultHDRISky, value, nameof(m_DefaultHDRISky));
        }

        /// <summary>
        /// STBN, Spatial-Temporal Blue Noise, vec1
        /// </summary>
        [SerializeField]
        [ResourceFormattedPaths("Textures/STBN/vec1/stbn_vec1_2Dx1D_128x128x64_{0}.png", 0, 64)]
        private Texture2D[] m_BlueNoise128RTex = new Texture2D[64];
        public Texture2D[] blueNoise128RTex
        {
            get => m_BlueNoise128RTex;
            set => this.SetValueAndNotify(ref m_BlueNoise128RTex, value);
        }

        /// <summary>
        /// STBN, Spatial-Temporal Blue Noise, vec2
        /// </summary>
        [SerializeField]
        [ResourceFormattedPaths("Textures/STBN/vec2/stbn_vec2_2Dx1D_128x128x64_{0}.png", 0, 64)]
        private Texture2D[] m_BlueNoise128RGTex = new Texture2D[64];
        public Texture2D[] blueNoise128RGTex
        {
            get => m_BlueNoise128RGTex;
            set => this.SetValueAndNotify(ref m_BlueNoise128RGTex, value);
        }

        [SerializeField]
        [ResourcePath("Textures/ShadowRamp/DirectionalShadowRamp.png")]
        private Texture2D m_DefaultDirShadowRampTex;

        /// <summary>
        /// Default directional shadowramp texture.
        /// </summary>
        public Texture2D defaultDirShadowRampTex
        {
            get => m_DefaultDirShadowRampTex;
            set => this.SetValueAndNotify(ref m_DefaultDirShadowRampTex, value, nameof(m_DefaultDirShadowRampTex));
        }
    }
}
