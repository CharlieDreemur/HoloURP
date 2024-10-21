using Unity.Mathematics;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Generates an in-place depth pyramid 
    /// TODO: Mip-mapping depth is problematic for precision at lower mips, generate a packed atlas instead
    /// </summary>
    public class DepthPyramidPass : ScriptableRenderPass
    {
        private ComputeShader m_Shader;
        private int m_DepthDownsampleKernel;

        private int[] m_SrcOffset;
        private int[] m_DstOffset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="computeShader"></param>
        public DepthPyramidPass(RenderPassEvent evt, ComputeShader computeShader)
        {
            base.profilingSampler = new ProfilingSampler("DepthPyramid");
            renderPassEvent = evt;

            m_Shader = computeShader;
            m_DepthDownsampleKernel = m_Shader.FindKernel("KDepthDownsample8DualUav");

            m_SrcOffset = new int[4];
            m_DstOffset = new int[4];
        }

        private class PassData
        {
            internal TextureHandle targetTexture;
            internal RenderingUtils.PackedMipChainInfo mipChainInfo;
            internal ComputeShader cs;
            internal int kernel;
            internal bool mip0AlreadyComputed;
        }


        internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle depthMipChainTexture, RenderingUtils.PackedMipChainInfo mipChainInfo, bool mip0AlreadyComputed = false)
        {
            using (var builder = renderGraph.AddComputePass<PassData>("Depth Pyramid", out var passData, base.profilingSampler))
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                var mipLevelOffsetBuffer = GraphicsBufferSystem.instance.GetGraphicsBuffer<int2>(GraphicsBufferSystemBufferID.DepthPyramidMipLevelOffset, 15, "depthPramidMipLevelOffsetBuffer");
                mipChainInfo.GetOffsetBufferData(mipLevelOffsetBuffer);

                resourceData.cameraDepthPyramidInfo = mipChainInfo;
                resourceData.cameraDepthPyramidMipLevelOffsets = renderGraph.ImportBuffer(mipLevelOffsetBuffer);

                passData.targetTexture = depthMipChainTexture;
                passData.mipChainInfo = mipChainInfo;
                passData.cs = m_Shader;
                passData.kernel = m_DepthDownsampleKernel;
                passData.mip0AlreadyComputed = mip0AlreadyComputed;

                builder.UseTexture(passData.targetTexture, AccessFlags.Write);
                builder.UseBuffer(resourceData.cameraDepthPyramidMipLevelOffsets, AccessFlags.Write); // Assign to Write

                builder.AllowPassCulling(false);
#if DANBAIDONGRP_ASYNC_COMPUTE
                builder.EnableAsyncCompute(true);
#endif

                builder.SetRenderFunc((PassData data, ComputeGraphContext context) =>
                {
                    RenderMinDepthPyramid(context.cmd, data.cs, data.kernel, data.targetTexture, data.mipChainInfo, data.mip0AlreadyComputed, ref m_SrcOffset, ref m_DstOffset);
                });
            }
        }

        static void RenderMinDepthPyramid(ComputeCommandBuffer cmd, ComputeShader cs, int kernel, TextureHandle texture, RenderingUtils.PackedMipChainInfo info, bool mip0AlreadyComputed, ref int[] srcOffsets, ref int[] dstOffsets)
        {
            // TODO: Do it 1x MIP at a time for now. In the future, do 4x MIPs per pass, or even use a single pass.
            // Note: Gather() doesn't take a LOD parameter and we cannot bind an SRV of a MIP level,
            // and we don't support Min samplers either. So we are forced to perform 4x loads.
            for (int i = 1; i < info.mipLevelCount; i++)
            {
                if (mip0AlreadyComputed && i == 1) continue;

                Vector2Int dstSize = info.mipLevelSizes[i];
                Vector2Int dstOffset = info.mipLevelOffsets[i];
                Vector2Int srcSize = info.mipLevelSizes[i - 1];
                Vector2Int srcOffset = info.mipLevelOffsets[i - 1];
                Vector2Int srcLimit = srcOffset + srcSize - Vector2Int.one;

                srcOffsets[0] = srcOffset.x;
                srcOffsets[1] = srcOffset.y;
                srcOffsets[2] = srcLimit.x;
                srcOffsets[3] = srcLimit.y;

                dstOffsets[0] = dstOffset.x;
                dstOffsets[1] = dstOffset.y;
                dstOffsets[2] = 0;
                dstOffsets[3] = 0;

                cmd.SetComputeIntParams(cs, ShaderConstants._SrcOffsetAndLimit, srcOffsets);
                cmd.SetComputeIntParams(cs, ShaderConstants._DstOffset, dstOffsets);
                cmd.SetComputeTextureParam(cs, kernel, ShaderConstants._DepthMipChain, texture);
                cmd.DispatchCompute(cs, kernel, RenderingUtils.DivRoundUp(dstSize.x, 8), RenderingUtils.DivRoundUp(dstSize.y, 8), ((RTHandle)texture).rt.volumeDepth);
            }
        }

        public static class ShaderConstants
        {
            public static readonly int _SrcOffsetAndLimit = Shader.PropertyToID("_SrcOffsetAndLimit");
            public static readonly int _DstOffset = Shader.PropertyToID("_DstOffset");
            public static readonly int _DepthMipChain = Shader.PropertyToID("_DepthMipChain");
        }
    }

}
