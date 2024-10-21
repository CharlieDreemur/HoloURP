Shader "DanbaidongRP/Helpers/CharacterFringe"
{
    Properties
    {
        // FringeShadowCaster Props
        [FoldoutBegin(_FoldoutFringeShadowEnd)]_FoldoutFringeShadow("FringeShadow Caster (Stencil)", float) = 0
            _ScreenOffsetScaleX("ScreenOffsetScaleX", Range(-2, 2))         = 1
            _ScreenOffsetScaleY("ScreenOffsetScaleY", Range(-2, 2))         = 1

            [Title(Shadow Caster Stencil)]
            _FriStencil("Stencil ID", Float) = 96 // SHADINGMODELS_CHARACTER
            [Enum(UnityEngine.Rendering.CompareFunction)]
            _FriStencilComp("Stencil Comparison", Float) = 3
            [Enum(UnityEngine.Rendering.StencilOp)]
            _FriStencilOp("Stencil Operation", Float) = 3
            _FriStencilWriteMask("Stencil Write Mask", Float) = 97 // SHADINGMODELS_CHARACTER + 1
            _FriStencilReadMask("Stencil Read Mask", Float) = 97
            _FriColorMask("Color Mask", Float) = 0
        [FoldoutEnd]_FoldoutFringeShadowEnd("_FoldoutFringeShadowEnd", float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Geometry-99"
            "IgnoreProjector" = "True"
        }
        LOD 300

        // GBuffer: write depth and normal
        Pass
        {
            Name "GBufferBase"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }

            Stencil
            {
                Ref[_FriStencil]
                Comp[_FriStencilComp]
                Pass[_FriStencilOp]
                ReadMask[_FriStencilReadMask]
                WriteMask[_FriStencilWriteMask]
            }

            // -------------------------------------
            // Render State Commands
            Cull Back
            ZWrite Off
            ColorMask R

            HLSLPROGRAM
            #pragma target 4.5

            // Deferred Rendering Path does not support the OpenGL-based graphics API:
            // Desktop OpenGL, OpenGL ES 3.0, WebGL 2.0.
            #pragma exclude_renderers gles3 glcore

            // -------------------------------------
            // Shader Stages
            #pragma vertex GBufferPassVertex
            #pragma fragment GBufferPassFragment

            // -------------------------------------
            // Material Keywords


            // -------------------------------------
            // Universal Pipeline keywords
            // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            // #pragma multi_compile_fragment _ _SHADOWS_SOFT
            // #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            // #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
            // #include_with_pragmas "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/RenderingLayers.hlsl"

            // -------------------------------------
            // Unity defined keywords
            // #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            // #pragma multi_compile _ SHADOWS_SHADOWMASK
            // #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            // #pragma multi_compile _ LIGHTMAP_ON
            // #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            // #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            // #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #include_with_pragmas "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/UnityGBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/Material/PBRToon/PBRToon.hlsl"


            CBUFFER_START(UnityPerMaterial)
            float _ScreenOffsetScaleX;
            float _ScreenOffsetScaleY;
            float3 _HeadCenterWS;
            CBUFFER_END

            struct Attributes 
            {
                float4 vertex   :POSITION;
                float3 normal   :NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings 
            {
                float4 positionHCS   :SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings GBufferPassVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                Light mainLight = GetMainLight();
                float3 lightDirWS = normalize(mainLight.direction);
                float3 lightDirVS = normalize(TransformWorldToViewDir(lightDirWS));

                // Cam is Upward: let shadow close to face.
                float3 camDirOS = normalize(TransformWorldToObjectDir(GetCameraPositionWS() - _HeadCenterWS));
                float camDirFactor = 1 - smoothstep(0.1, 0.9, camDirOS.y);

                float3 positionVS = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
                
                positionVS.x -= 0.0045 * lightDirVS.x * _ScreenOffsetScaleX;
                positionVS.y -= 0.0075 * _ScreenOffsetScaleY * camDirFactor;
                o.positionHCS = TransformWViewToHClip(positionVS);

                return o;
            }

            // We only output GBuffer0 flag.
            float4 GBufferPassFragment(Varyings i) : SV_Target0
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                
                uint toonFlags = 0;
                toonFlags |= kToonFlagHairShadow;

                return EncodeToonFlags(toonFlags);
            }
            ENDHLSL

        }


    }
    
    CustomEditor "UnityEditor.DanbaidongGUI.DanbaidongGUI"
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
