using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
    /// <summary>
    /// Copy the given depth buffer into the given destination depth buffer.
    ///
    /// You can use this pass to copy a depth buffer to a destination,
    /// so you can use it later in rendering. If the source texture has MSAA
    /// enabled, the pass uses a custom MSAA resolve. If the source texture
    /// does not have MSAA enabled, the pass uses a Blit or a Copy Texture
    /// operation, depending on what the current platform supports.
    /// </summary>
    public class GPUCopyPass : ScriptableRenderPass
    {
        private RTHandle m_Source { get; set; }
        private RTHandle m_Destination { get; set; }

        internal bool m_ShouldClear;

        private ComputeShader m_GPUCopy;
        private int m_SampleKernel_xyzw2x_8;
        private int m_SampleKernel_xyzw2x_1;

        static readonly int s_RectOffset = Shader.PropertyToID("_RectOffset");
        static readonly int s_Result = Shader.PropertyToID("_Result");
        static readonly int s_Source = Shader.PropertyToID("_Source");
        static int[] s_IntParams = new int[2];

        /// <summary>
        /// Creates a new <c>GPUCopyPass</c> instance.
        /// </summary>
        /// <param name="evt">The <c>RenderPassEvent</c> to use.</param>
        /// <param name="shouldClear">Controls whether it should do a clear before copying the depth.</param>
        /// <seealso cref="RenderPassEvent"/>
        public GPUCopyPass(RenderPassEvent evt, ComputeShader computeShader, bool shouldClear = false)
        {
            base.profilingSampler = new ProfilingSampler("GPUCopyDepth");
            renderPassEvent = evt;
            m_ShouldClear = shouldClear;

            m_GPUCopy = computeShader;
            m_SampleKernel_xyzw2x_8 = m_GPUCopy.FindKernel("KSampleCopy4_1_x_8");
            m_SampleKernel_xyzw2x_1 = m_GPUCopy.FindKernel("KSampleCopy4_1_x_1");
        }

        static void SampleCopyChannel(
            ComputeCommandBuffer cmd,
            ComputeShader cs,
            RectInt rect,
            int sourceID,
            TextureHandle source,
            int targetID,
            TextureHandle target,
            int slices,
            int kernel8,
            int kernel1)
        {
            RectInt main, topRow, rightCol, topRight;
            unsafe
            {
                RectInt* dispatch1Rects = stackalloc RectInt[3];
                int dispatch1RectCount = 0;
                RectInt dispatch8Rect = new RectInt(0, 0, 0, 0);

                if (TileLayoutUtils.TryLayoutByTiles(
                    rect,
                    8,
                    out main,
                    out topRow,
                    out rightCol,
                    out topRight))
                {
                    if (topRow.width > 0 && topRow.height > 0)
                    {
                        dispatch1Rects[dispatch1RectCount] = topRow;
                        ++dispatch1RectCount;
                    }
                    if (rightCol.width > 0 && rightCol.height > 0)
                    {
                        dispatch1Rects[dispatch1RectCount] = rightCol;
                        ++dispatch1RectCount;
                    }
                    if (topRight.width > 0 && topRight.height > 0)
                    {
                        dispatch1Rects[dispatch1RectCount] = topRight;
                        ++dispatch1RectCount;
                    }
                    dispatch8Rect = main;
                }
                else if (rect.width > 0 && rect.height > 0)
                {
                    dispatch1Rects[dispatch1RectCount] = rect;
                    ++dispatch1RectCount;
                }

                cmd.SetComputeTextureParam(cs, kernel8, sourceID, source);
                cmd.SetComputeTextureParam(cs, kernel1, sourceID, source);
                cmd.SetComputeTextureParam(cs, kernel8, targetID, target);
                cmd.SetComputeTextureParam(cs, kernel1, targetID, target);

                if (dispatch8Rect.width > 0 && dispatch8Rect.height > 0)
                {
                    var r = dispatch8Rect;
                    // Use intermediate array to avoid garbage
                    s_IntParams[0] = r.x;
                    s_IntParams[1] = r.y;
                    cmd.SetComputeIntParams(cs, s_RectOffset, s_IntParams);
                    cmd.DispatchCompute(cs, kernel8, (int)Mathf.Max(r.width / 8, 1), (int)Mathf.Max(r.height / 8, 1), slices);
                }

                for (int i = 0, c = dispatch1RectCount; i < c; ++i)
                {
                    var r = dispatch1Rects[i];
                    // Use intermediate array to avoid garbage
                    s_IntParams[0] = r.x;
                    s_IntParams[1] = r.y;
                    cmd.SetComputeIntParams(cs, s_RectOffset, s_IntParams);
                    cmd.DispatchCompute(cs, kernel1, (int)Mathf.Max(r.width, 1), (int)Mathf.Max(r.height, 1), slices);
                }
            }
        }

        static void SampleCopyChannel_xyzw2x(ComputeCommandBuffer cmd, PassData data)
        {
            RTHandle s = (RTHandle)data.source;
            RTHandle t = (RTHandle)data.destination;
            Debug.Assert(s.rt.volumeDepth == t.rt.volumeDepth);
            SampleCopyChannel(cmd, data.cs, new RectInt(0, 0, data.width, data.height), s_Source, data.source, s_Result, data.destination, s.rt.volumeDepth, data.kernel8Step, data.kernel1Step);
        }

        private class PassData
        {
            internal TextureHandle source;
            internal TextureHandle destination;
            internal ComputeShader cs;
            internal int kernel1Step;
            internal int kernel8Step;
            internal int width;
            internal int height;
        }

        //RenderGraph
        internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle destination, TextureHandle source, bool setGlobal = false)
        {
            if (m_GPUCopy == null)
                return;

            using (var builder = renderGraph.AddComputePass<PassData>("GPUCopy Depth", out var passData, base.profilingSampler))
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                passData.source = source;
                passData.destination = destination;
                passData.cs = m_GPUCopy;
                passData.kernel1Step = m_SampleKernel_xyzw2x_1;
                passData.kernel8Step = m_SampleKernel_xyzw2x_8;
                passData.width = cameraData.pixelWidth;
                passData.height = cameraData.pixelHeight;

                builder.UseTexture(source, AccessFlags.Read);
                builder.UseTexture(destination, AccessFlags.Write);
                builder.AllowPassCulling(false);
                if (setGlobal)
                    builder.SetGlobalTextureAfterPass(destination, Shader.PropertyToID("_CameraDepthTexture"));

                builder.SetRenderFunc((PassData data, ComputeGraphContext context) =>
                {
                    SampleCopyChannel_xyzw2x(context.cmd, data);
                });
            }
        }

        /// <summary>
        /// For Depthpyramid copy mip0, we will create pyramid texture here
        /// </summary>
        /// <param name="renderGraph"></param>
        /// <param name="frameData"></param>
        /// <param name="source"></param>
        /// <param name="depthPyramidDesc"></param>
        /// <returns>DepthPyramid texture.</returns>
        internal TextureHandle RenderDepthPyramidMip0(RenderGraph renderGraph, ContextContainer frameData, TextureHandle source, RenderTextureDescriptor depthPyramidDesc)
        {
            if (m_GPUCopy == null)
            {
                return TextureHandle.nullHandle;
            }

            using (var builder = renderGraph.AddComputePass<PassData>("GPUCopy DepthPyramidMip0", out var passData, base.profilingSampler))
            {
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                var createdTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, depthPyramidDesc, "_CameraDepthBufferMipChain", true);

                passData.source = source;
                passData.destination = createdTexture;
                passData.cs = m_GPUCopy;
                passData.kernel1Step = m_SampleKernel_xyzw2x_1;
                passData.kernel8Step = m_SampleKernel_xyzw2x_8;
                passData.width = cameraData.pixelWidth;
                passData.height = cameraData.pixelHeight;

                builder.UseTexture(source, AccessFlags.Read);
                builder.UseTexture(passData.destination, AccessFlags.Write);
                builder.AllowPassCulling(false);
#if DANBAIDONGRP_ASYNC_COMPUTE
                builder.EnableAsyncCompute(true);
#endif

                builder.SetRenderFunc((PassData data, ComputeGraphContext context) =>
                {
                    SampleCopyChannel_xyzw2x(context.cmd, data);
                });

                return createdTexture;
            }
        }
    }

}
