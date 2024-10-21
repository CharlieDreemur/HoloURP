
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
    // Render all deferred shading models.
    internal class DeferredLighting : ScriptableRenderPass
    {
        DeferredLights m_DeferredLights;

        // Public Variables

        // Private Variables
        private ComputeShader m_DeferredLightingCS;
        private int m_DeferredClassifyTilesKernel;
        private int m_DeferredLightingKernel;


        // Constants
        private const int c_deferredLightingTileSize = 16;

        // Statics

        public DeferredLighting(RenderPassEvent evt, DeferredLights deferredLights, ComputeShader deferredLightingCS)
        {
            base.profilingSampler = new ProfilingSampler(nameof(DeferredLighting));
            base.renderPassEvent = evt;
            m_DeferredLights = deferredLights;

            m_DeferredLightingCS = deferredLightingCS;

            m_DeferredClassifyTilesKernel = m_DeferredLightingCS.FindKernel("DeferredClassifyTiles");
            m_DeferredLightingKernel = m_DeferredLightingCS.FindKernel("DeferredLighting0");
        }

        public static ShadingModels GetShadingModels(int modelIndex)
        {
            switch(modelIndex)
            {
                case 0:
                    return ShadingModels.Lit;
                case 1:
                    return ShadingModels.SimpleLit;
                case 2:
                    return ShadingModels.Character;
                default:
                    return ShadingModels.Lit;
            }
        }


        private class PassData
        {
            internal UniversalResourceData resourceData;
            internal UniversalCameraData cameraData;
            internal UniversalLightData lightData;
            internal UniversalShadowData shadowData;

            internal TextureHandle lightingHandle;
            internal TextureHandle stencilHandle;
            internal TextureHandle[] gbuffer;
            internal DeferredLights deferredLights;

            // Compute shader
            internal ComputeShader deferredLightingCS;
            internal int deferredClassifyTilesKernel;
            internal int deferredLightingKernel;

            internal int numTilesX;
            internal int numTilesY;

            // Compute Buffers
            internal BufferHandle dispatchIndirectBuffer;
            internal BufferHandle tileListBuffer;

            // PreIntergratedFGD
            internal TextureHandle FGD_GGXAndDisneyDiffuse;

            // Sky Ambient
            internal BufferHandle ambientProbe;
            // Sky Reflect
            internal TextureHandle reflectProbe;

            // Lighting Buffers (SSAO, SSR, SSGI, SSShadow)
            internal TextureHandle SSRLightingTexture;
            internal TextureHandle SSShadowsTexture;
            internal TextureHandle shadowScatterTexture;
        }

        static void ExecutePass(PassData data, ComputeGraphContext context)
        {
            var cmd = context.cmd;

            // Due to async compute, we set global keywords here.
            cmd.SetKeyword(ShaderGlobalKeywords.ScreenSpaceReflection, data.SSRLightingTexture.IsValid());


            // BuildIndirect
            {
                cmd.SetComputeTextureParam(data.deferredLightingCS, data.deferredClassifyTilesKernel, "_StencilTexture", data.stencilHandle, 0, RenderTextureSubElement.Stencil);

                cmd.SetComputeBufferParam(data.deferredLightingCS, data.deferredClassifyTilesKernel, "g_DispatchIndirectBuffer", data.dispatchIndirectBuffer);
                cmd.SetComputeBufferParam(data.deferredLightingCS, data.deferredClassifyTilesKernel, "g_TileList", data.tileListBuffer);

                cmd.DispatchCompute(data.deferredLightingCS, data.deferredClassifyTilesKernel, data.numTilesX, data.numTilesY, 1);
            }

            // Lighting
            {
                // Bind PreIntegratedFGD before deferred shading.
                PreIntegratedFGD.instance.Bind(cmd, PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse, data.FGD_GGXAndDisneyDiffuse);

                for (int modelIndex = 0; modelIndex < (int)ShadingModels.CurModelsNum; modelIndex++)
                {
                    var kernelIndex = data.deferredLightingKernel + modelIndex;
                    cmd.SetComputeIntParam(data.deferredLightingCS, "_ShadingModelIndex", modelIndex);
                    cmd.SetComputeIntParam(data.deferredLightingCS, "_ShadingModelStencil", (int)GetShadingModels(modelIndex));
                    cmd.SetComputeVectorParam(data.deferredLightingCS, "_TilesNum", new Vector2(data.numTilesX, data.numTilesY));

                    cmd.SetComputeBufferParam(data.deferredLightingCS, kernelIndex, "g_TileList", data.tileListBuffer);
                    cmd.SetComputeTextureParam(data.deferredLightingCS, kernelIndex, "_LightingTexture", data.lightingHandle);
                    cmd.SetComputeTextureParam(data.deferredLightingCS, kernelIndex, "_StencilTexture", data.stencilHandle, 0, RenderTextureSubElement.Stencil);

                    cmd.SetComputeBufferParam(data.deferredLightingCS, kernelIndex, "_AmbientProbeData", data.ambientProbe);
                    cmd.SetComputeTextureParam(data.deferredLightingCS, kernelIndex, "_SkyTexture", data.reflectProbe);

                    // ScreenSpaceLighting ShaderVariables
                    cmd.SetComputeTextureParam(data.deferredLightingCS, kernelIndex, "_ScreenSpaceShadowmapTexture", data.SSShadowsTexture);
                    if (data.shadowScatterTexture.IsValid())
                        cmd.SetComputeTextureParam(data.deferredLightingCS, kernelIndex, "_ShadowScatterTexture", data.shadowScatterTexture);
                    if (data.SSRLightingTexture.IsValid())
                        cmd.SetComputeTextureParam(data.deferredLightingCS, kernelIndex, "_SSRLightingTexture", data.SSRLightingTexture);

                    cmd.DispatchCompute(data.deferredLightingCS, kernelIndex, data.dispatchIndirectBuffer, (uint)modelIndex * 3 * sizeof(uint));
                }
            }
        }

        internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle color, TextureHandle depth, TextureHandle[] gbuffer)
        {
            using (var builder = renderGraph.AddComputePass<PassData>("Deferred Lighting", out var passData, base.profilingSampler))
            {
                // Access resources
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();
                UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();

                // Setup passData
                passData.cameraData = cameraData;
                passData.lightData = lightData;
                passData.shadowData = shadowData;

                passData.lightingHandle = color;
                passData.stencilHandle = depth;
                passData.deferredLights = m_DeferredLights;

                passData.deferredLightingCS = m_DeferredLightingCS;
                passData.deferredClassifyTilesKernel = m_DeferredClassifyTilesKernel;
                passData.deferredLightingKernel = m_DeferredLightingKernel;

                var width = cameraData.cameraTargetDescriptor.width;
                var height = cameraData.cameraTargetDescriptor.height;
                passData.numTilesX = RenderingUtils.DivRoundUp(width, c_deferredLightingTileSize);
                passData.numTilesY = RenderingUtils.DivRoundUp(height, c_deferredLightingTileSize);

                var bufferSystem = GraphicsBufferSystem.instance;
                var dispatchIndirectBuffer = bufferSystem.GetGraphicsBuffer<uint>(GraphicsBufferSystemBufferID.DeferredLightingIndirect, 
                    (int)ShadingModels.CurModelsNum * 3, 
                    "dispatchIndirectBuffer", 
                    GraphicsBuffer.Target.IndirectArguments);
                passData.dispatchIndirectBuffer = renderGraph.ImportBuffer(dispatchIndirectBuffer);
                var tileListBufferDesc = new BufferDesc((int)ShadingModels.CurModelsNum * passData.numTilesX * passData.numTilesY, sizeof(uint), "tileListBuffer");
                passData.tileListBuffer = renderGraph.CreateBuffer(tileListBufferDesc);

                passData.FGD_GGXAndDisneyDiffuse = PreIntegratedFGD.instance.ImportToRenderGraph(renderGraph, PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);

                // Sky Environment
                passData.ambientProbe = resourceData.skyAmbientProbe;
                passData.reflectProbe = resourceData.skyReflectionProbe;

                // Lighting Buffers (SSAO, SSR, SSGI, SSShadow)
                passData.SSRLightingTexture = resourceData.ssrLightingTexture;
                passData.SSShadowsTexture = resourceData.screenSpaceShadowsTexture;
                passData.shadowScatterTexture = resourceData.shadowScatterTexture;

                // Declare input/output
                builder.UseTexture(passData.lightingHandle, AccessFlags.ReadWrite);
                builder.UseTexture(passData.stencilHandle, AccessFlags.Read);
                builder.UseBuffer(passData.dispatchIndirectBuffer, AccessFlags.ReadWrite);
                builder.UseBuffer(passData.tileListBuffer, AccessFlags.ReadWrite);
                builder.UseTexture(passData.FGD_GGXAndDisneyDiffuse, AccessFlags.Read);

                builder.UseBuffer(passData.ambientProbe, AccessFlags.Read);
                builder.UseTexture(passData.reflectProbe, AccessFlags.Read);

                builder.UseTexture(passData.SSShadowsTexture, AccessFlags.Read);
                if (passData.shadowScatterTexture.IsValid())
                    builder.UseTexture(passData.shadowScatterTexture, AccessFlags.Read);

                if (passData.SSRLightingTexture.IsValid())
                    builder.UseTexture(passData.SSRLightingTexture, AccessFlags.Read);

                for (int i = 0; i < gbuffer.Length; ++i)
                {
                    if (i != m_DeferredLights.GBufferLightingIndex)
                        builder.UseTexture(gbuffer[i], AccessFlags.Read);
                }
                
                // Setup builder state
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, ComputeGraphContext context) =>
                {
                    ExecutePass(data, context);
                });
            }
        }

        // ScriptableRenderPass
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            m_DeferredLights.OnCameraCleanup(cmd);
        }
    }
}
