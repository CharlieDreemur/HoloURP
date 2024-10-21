Shader "DanbaidongRP/Helpers/Outline"
{
    Properties
    {
        // Outline Props
        [FoldoutBegin(_FoldoutOutlineEnd)]_FoldoutOutline("Outline", float) = 0
            [KeysEnum(SN_VertColor, SN_VertNormal)]
            _OutLineNormalSource            ("Smooth Normal Source", float)         = 0
            _OutlineColor                   ("Outline Color", Color)                = (0, 0, 0, 0.8)
            _OutlineWidth                   ("Width", Range(0, 10))                 = 1.0
            _OutlineClampScale              ("ClampScale", Range(0.01, 5))          = 1
            [Title(Lighting)]
            [HDR]_OutlineDirectLightingColor    ("DirectColor", color)              = (1,1,1,0.5)
            _OutlineDirectLightingOffset        ("DirectOffset", Range(-1, 1))      = 0.0
            [HDR]_OutlinePunctualLightingColor  ("PunctualColor", color)            = (1,1,1,0.5)
            _OutlinePunctualLightingOffset      ("PunctualOffset", Range(-1, 1))    = 0.0

        [FoldoutEnd]_FoldoutOutlineEnd("_FoldoutEnd", float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Geometry"
            "IgnoreProjector" = "True"
        }
        LOD 300

        // ForwardOutline
        Pass
        {
            Name "ForwardOutline"

            Tags
            {
                "LightMode" = "CharacterOutline"
            }

            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma target 4.5

            // -------------------------------------
            // Shader Stages
            #pragma vertex ToonOutlineVert
            #pragma fragment ToonOutlineFrag

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local SN_VertColor SN_VertNormal


            // -------------------------------------
            // Universal Pipeline keywords
            // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _GPU_LIGHTS_CLUSTER
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
            #include_with_pragmas "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/RenderingLayers.hlsl"

            // -------------------------------------
            // Unity defined keywords
            // #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            // #pragma multi_compile _ SHADOWS_SHADOWMASK
            // #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            // #pragma multi_compile _ LIGHTMAP_ON
            // #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

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
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/GPUCulledLights.hlsl"


            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineClampScale;

            // Lighting
            float4 _OutlineDirectLightingColor;
            float _OutlineDirectLightingOffset;
            float4 _OutlinePunctualLightingColor;
            float _OutlinePunctualLightingOffset;

            float _AlphaClip;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct Attributes
            {
                float4 vertex   :POSITION;
                float3 normal   :NORMAL;
                float4 tangent  :TANGENT;
                float4 color    :COLOR; // rgb:SmoothNormal a:outlineWidth
                float4 uv       :TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS   :SV_POSITION;
                float3 positionWS   :TEXCOORD0;
                float3 normalWS     :TEXCOORD1;
                float4 uv           :TEXCOORD2;
                float4 color        :TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings ToonOutlineVert(Attributes v)
            {
                Varyings o = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 scaledScreenParams = GetScaledScreenParams();
                float ScaleX = abs(scaledScreenParams.x / scaledScreenParams.y);

                o.positionHCS = TransformObjectToHClip(v.vertex);
                o.positionWS = TransformObjectToWorld(v.vertex);

                float3 tangentOS = v.tangent.xyz;
                float3 normalOS = v.normal.xyz;
                float3 biTangentOS = cross(normalOS, tangentOS) * v.tangent.w * GetOddNegativeScale();

                #if defined(SN_VertColor)
                float3 smoothNormalTS = v.color.rgb * 2 - 1;
                float3x3 TBN_TSOS= float3x3(tangentOS, biTangentOS, normalOS);

                float3 smoothNormalOS = mul(smoothNormalTS, TBN_TSOS);
                smoothNormalOS = SafeNormalize(smoothNormalOS);
                normalOS = smoothNormalOS;
                #endif

                _OutlineWidth *= v.color.a;

                o.normalWS = TransformObjectToWorldNormal(normalOS);
                float3 normalCS = TransformWorldToHClipDir(o.normalWS);
                float2 extend = normalize(normalCS) * (_OutlineWidth*0.01); 
                extend.x /= ScaleX;


                //Clip space width control.
                float ctrl = clamp(1/(o.positionHCS.w + _OutlineClampScale),0,1);
                o.positionHCS.xy += extend * o.positionHCS.w * ctrl;
                o.uv.xy = TRANSFORM_TEX(v.uv,_BaseMap);

                o.color = v.color;
                return o;
            }

            float4 ToonOutlineFrag(Varyings i) : SV_Target0
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                // Alpha Clip
                float4 mainTexColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap,i.uv);
            #if _USE_ALPHA_CLIPPING
                clip(mainTexColor.a - _AlphaClip);
            #endif

                // Input
                float  depth = i.positionHCS.z;
                float3 positionWS = i.positionWS;
                float2 screenUV = i.positionHCS.xy / _ScreenParams.xy;
                TransformScreenUV(screenUV);

                // Property prepare
                float3 normalWS = SafeNormalize(i.normalWS);
                float3 normalVS = TransformWorldToViewNormal(normalWS);
                float directThreshold = saturate(0.8 - _OutlineDirectLightingOffset);
                float punctualThreshold = saturate(0.8 - _OutlinePunctualLightingOffset);

                float3 directLighting = 0;
                float3 punctualLighting = 0;

                // Direct Outline Lighting
                uint dirLightIndex = 0;
                for (dirLightIndex = 0; dirLightIndex < _DirectionalLightCount; dirLightIndex++)
                {
                    DirectionalLightData dirLight = g_DirectionalLightDatas[dirLightIndex];

                    #ifdef _LIGHT_LAYERS
                    if (IsMatchingLightLayer(dirLight.lightLayerMask, meshRenderingLayers))
                    #endif
                    {
                        float3 lightDirWS = dirLight.lightDirection;
                        float3 lightDirVS = TransformWorldToViewDir(lightDirWS);
                        float NdotL = dot(normalVS.xy, lightDirVS.xy);

                        float3 lightColor = lerp(_OutlineDirectLightingColor.rgb, _OutlineDirectLightingColor.rgb * dirLight.lightColor,  _OutlineDirectLightingColor.a);

                        directLighting += step(directThreshold, NdotL) * lightColor;
                    }
                }
                // TODO: Apply Shadow
                float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
                float shadowAttenuation = MainLightRealtimeShadow(shadowCoord);
                directLighting *= shadowAttenuation;


                // Punctual Outline Lighting
                uint lightCategory = LIGHTCATEGORY_PUNCTUAL;
                uint lightStart;
                uint lightCount;
                PositionInputs posInput = GetPositionInput(i.positionHCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
                GetCountAndStart(posInput, lightCategory, lightStart, lightCount);
                uint v_lightListOffset = 0;
                uint v_lightIdx = lightStart;

                if (lightCount > 0) // avoid 0 iteration warning.
                {
                    while (v_lightListOffset < lightCount)
                    {
                        v_lightIdx = FetchIndex(lightStart, v_lightListOffset);
                        if (v_lightIdx == -1)
                            break;

                        GPULightData gpuLight = FetchLight(v_lightIdx);

                        #ifdef _LIGHT_LAYERS
                        if (IsMatchingLightLayer(gpuLight.lightLayerMask, meshRenderingLayers))
                        #endif
                        {
                            float3 lightVector = gpuLight.lightPosWS - positionWS.xyz;
                            float distanceSqr = max(dot(lightVector, lightVector), FLT_MIN);
                            float3 lightDirection = float3(lightVector * rsqrt(distanceSqr));
                            float shadowMask = 1;

                            float distanceAtten = DistanceAttenuation(distanceSqr, gpuLight.lightAttenuation.xy) * AngleAttenuation(gpuLight.lightDirection.xyz, lightDirection, gpuLight.lightAttenuation.zw);
                            float shadowAtten = gpuLight.shadowType == 0 ? 1 : AdditionalLightShadow(gpuLight.shadowLightIndex, positionWS, lightDirection, shadowMask, gpuLight.lightOcclusionProbInfo);
                            float attenuation = distanceAtten * shadowAtten;

                            float NdotL = dot(normalWS, lightDirection);

                            float3 lightColor = lerp(_OutlinePunctualLightingColor.rgb, _OutlinePunctualLightingColor.rgb * gpuLight.lightColor,  _OutlinePunctualLightingColor.a);

                            punctualLighting += lightColor * step(punctualThreshold, NdotL) * attenuation * gpuLight.outlineContribution;
                        }

                        v_lightListOffset++;
                    }
                }


                float3 oulineColor = mainTexColor.rgb * _OutlineColor.rgb;
                oulineColor = lerp(mainTexColor.rgb, _OutlineColor.rgb, _OutlineColor.a);

                float3 result = oulineColor + directLighting + punctualLighting;
                result = AcesTonemap(result); // We don't want outline lighting too strong.

                return float4(result, 1);
            }

            ENDHLSL
        }

    }

    CustomEditor "UnityEditor.DanbaidongGUI.DanbaidongGUI"
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}