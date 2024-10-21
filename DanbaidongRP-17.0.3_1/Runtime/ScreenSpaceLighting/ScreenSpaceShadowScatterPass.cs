using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
    internal class ScreenSpaceShadowScatterPass : ScriptableRenderPass
    {
        // Public Variables

        // Private Variables
        private Material m_Material;
        private bool m_EnableShadowScatter;

        // Constants

        // Statics

        public ScreenSpaceShadowScatterPass(RenderPassEvent evt, Shader shadowScatterShader)
        {
            renderPassEvent = evt;

            Material Load(Shader shader)
            {
                if (shader == null)
                {
                    Debug.LogError($"Missing shader. ColorGradingLutPass render pass will not execute. Check for missing reference in the renderer resources.");
                    return null;
                }

                return CoreUtils.CreateEngineMaterial(shader);
            }

            m_Material = Load(shadowScatterShader);

        }

        public bool Setup(UniversalResourceData resourceData)
        {
            if (!resourceData.directionalShadowsTexture.IsValid() || !resourceData.screenSpaceShadowsTexture.IsValid())
                return false;

            var stack = VolumeManager.instance.stack;
            var shadowsVolumeSettings = stack.GetComponent<Shadows>();
            m_EnableShadowScatter = shadowsVolumeSettings != null && (shadowsVolumeSettings.shadowScatterMode.value != ShadowScatterMode.None);

            return true;
        }

        private class PassData
        {
            internal Material material;

            internal TextureHandle dirShadowmapTex;
            internal TextureHandle screenSpaceShadowmapTex;
            internal TextureHandle shadowScatterTex;
        }

        /// <summary>
        /// Initialize the shared pass data.
        /// </summary>
        /// <param name="passData"></param>
        private void InitPassData(RenderGraph renderGraph, PassData passData, UniversalCameraData cameraData, UniversalResourceData resourceData)
        {
            RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
            desc.width = RenderingUtils.DivRoundUp(desc.width, 2);
            desc.height = RenderingUtils.DivRoundUp(desc.height, 2);
            desc.colorFormat = RenderTextureFormat.R8;
            desc.depthBufferBits = 0;

            passData.material = m_Material;

            passData.dirShadowmapTex = resourceData.directionalShadowsTexture;
            passData.screenSpaceShadowmapTex = resourceData.screenSpaceShadowsTexture;

            passData.shadowScatterTex = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_ShadowScatterTexture", true, Color.white, FilterMode.Bilinear);
        }

        private static void ExecutePass(PassData data, CommandBuffer cmd)
        {
            Blitter.BlitCameraTexture(cmd, data.shadowScatterTex, data.shadowScatterTex, 
                RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, data.material, 0);
        }

        internal TextureHandle Render(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (!m_EnableShadowScatter)
                return renderGraph.defaultResources.whiteTexture;


            // Update keywords and other shader params
            int historyFramCount = 0;
            var historyRTSystem = HistoryFrameRTSystem.GetOrCreate(frameData.Get<UniversalCameraData>().camera);
            if (historyRTSystem != null)
                historyFramCount = historyRTSystem.historyFrameCount;

            m_Material.SetFloat(ShaderConstants._CamHistoryFrameCount, historyFramCount);

            using (var builder = renderGraph.AddUnsafePass<PassData>("Render ShadowScatter", out var passData, ProfilingSampler.Get(URPProfileId.RenderShadowScatter)))
            {
                // Access resources
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();
                UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();

                // Setup passData
                InitPassData(renderGraph, passData, cameraData, resourceData);

                // Setup builder state
                builder.UseTexture(passData.dirShadowmapTex, AccessFlags.Read);
                builder.UseTexture(passData.screenSpaceShadowmapTex, AccessFlags.ReadWrite);
                builder.UseTexture(passData.shadowScatterTex, AccessFlags.ReadWrite);

                builder.AllowPassCulling(false);
                //builder.AllowGlobalStateModification(true);
                //builder.EnableAsyncCompute(true);

                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) =>
                {
                    CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                    ExecutePass(data, cmd);
                });

                return passData.shadowScatterTex;
            }
        }

        /// <summary>
        /// Cleans up resources used by the pass.
        /// </summary>
        public void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }

        static class ShaderConstants
        {
            public static readonly int _CamHistoryFrameCount = Shader.PropertyToID("_CamHistoryFrameCount");
        }
    }

}

