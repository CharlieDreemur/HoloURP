using System;
using UnityEngine.Assertions;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
    public enum BlueNoiseTexFormat
    {
        _128R,
        _128RG
    }
    /// <summary>
    /// A bank of nvidia pre-generated spatiotemporal blue noise textures.
    /// ref: https://github.com/NVIDIAGameWorks/SpatiotemporalBlueNoiseSDK/tree/main
    /// </summary>
    public sealed class BlueNoiseSystem : IDisposable
    {
        public static BlueNoiseSystem m_Instance = null;
        public static int blueNoiseArraySize = 64;

        readonly Texture2D[] m_Textures128R;
        readonly Texture2D[] m_Textures128RG;

        Texture2DArray m_TextureArray128R;
        Texture2DArray m_TextureArray128RG;

        RTHandle m_TextureHandle128R;
        RTHandle m_TextureHandle128RG;

        /// <summary>
        /// Spatiotemporal blue noise valuse[0,1] with R single-channel 128x128 textures.
        /// </summary>
        public Texture2D[] textures128R { get { return m_Textures128R; } }

        /// <summary>
        /// Spatiotemporal blue noise valuse[0,1] with RG multi-channel 128x128 textures.
        /// </summary>
        public Texture2D[] textures128RG { get { return m_Textures128RG; } }

        public Texture2DArray textureArray128R { get { return m_TextureArray128R; } }
        public Texture2DArray textureArray128RG { get { return m_TextureArray128RG; } }

        public RTHandle textureHandle128R { get { return m_TextureHandle128R; } }
        public RTHandle textureHandle128RG { get { return m_TextureHandle128RG; } }


        public static readonly int s_STBNVec1Texture = Shader.PropertyToID("_STBNVec1Texture");
        public static readonly int s_STBNVec2Texture = Shader.PropertyToID("_STBNVec2Texture");
        public static readonly int s_STBNIndex = Shader.PropertyToID("_STBNIndex");

        private BlueNoiseSystem(UniversalRenderPipelineRuntimeTextures runtimeTextures)
        {
            InitTextures(128, TextureFormat.R16, runtimeTextures.blueNoise128RTex, out m_Textures128R, out m_TextureArray128R, out m_TextureHandle128R);
            InitTextures(128, TextureFormat.RG32, runtimeTextures.blueNoise128RGTex, out m_Textures128RG, out m_TextureArray128RG, out m_TextureHandle128RG);
        }

        /// <summary>
        /// Initialize BlueNoiseSystem.
        /// </summary>
        /// <param name="resources"></param>
        internal static void Initialize(UniversalRenderPipelineRuntimeTextures runtimeTextures)
        {
            if (m_Instance == null)
                m_Instance = new BlueNoiseSystem(runtimeTextures);
        }

        /// <summary>
        /// Try get blueNoise instance, could be null if not initialized before.
        /// </summary>
        /// <returns>null if none initialized</returns>
        public static BlueNoiseSystem TryGetInstance()
        {
            return m_Instance;
        }


        public static void ClearAll()
        {
            if (m_Instance != null)
                m_Instance.Dispose();

            m_Instance = null;
        }

        /// <summary>
        /// Cleanups up internal textures.
        /// </summary>
        public void Dispose()
        {
            CoreUtils.Destroy(m_TextureArray128R);
            CoreUtils.Destroy(m_TextureArray128RG);

            RTHandles.Release(m_TextureHandle128R);
            RTHandles.Release(m_TextureHandle128RG);

            m_TextureArray128R = null;
            m_TextureArray128RG = null;
        }

        static void InitTextures(int size, TextureFormat format, Texture2D[] sourceTextures, out Texture2D[] destination, out Texture2DArray destinationArray, out RTHandle destinationHandle)
        {
            Assert.IsNotNull(sourceTextures);

            int len = sourceTextures.Length;

            Assert.IsTrue(len > 0);

            destination = new Texture2D[len];
            destinationArray = new Texture2DArray(size, size, len, format, false, true);
            destinationArray.hideFlags = HideFlags.HideAndDontSave;

            for (int i = 0; i < len; i++)
            {
                var noiseTex = sourceTextures[i];

                // Fail safe; should never happen unless the resources asset is broken
                if (noiseTex == null)
                {
                    destination[i] = Texture2D.whiteTexture;
                    continue;
                }

                destination[i] = noiseTex;
                Graphics.CopyTexture(noiseTex, 0, 0, destinationArray, i, 0);
            }

            destinationHandle = RTHandles.Alloc(destinationArray);
        }

        /// <summary>
        /// Bind spatiotemporal blue noise texture with given index (loop in blueNoiseArraySize).
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="textureIndex"></param>
        public static void BindSTBNParams(BlueNoiseTexFormat format, ComputeCommandBuffer cmd, ComputeShader computeShader, int kernel, TextureHandle texture, int frameCount)
        {
            var texID = (format == BlueNoiseTexFormat._128R) ? s_STBNVec1Texture : s_STBNVec2Texture;
            cmd.SetComputeTextureParam(computeShader, kernel, texID, texture);
            cmd.SetComputeIntParam(computeShader, s_STBNIndex, frameCount % blueNoiseArraySize);
        }
    }
}
