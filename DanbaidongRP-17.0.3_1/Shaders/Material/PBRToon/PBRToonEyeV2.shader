Shader "DanbaidongRP/PBRToon/EyeV2"
{
    Properties
    {
        [FoldoutBegin(_FoldoutTexEnd)]_FoldoutTex("Textures", Float) = 0
            _BaseColor                              ("BaseColor", Color)                    = (1,1,1,1)
            _BaseMap                                ("BaseMap(diff alpha)", 2D)             = "white" {}

            [Header(Parallax)]
            _ParallaxScale                          ("ParallaxScale", Range(0, 1))          = 1.0
            _ParallaxMaskEdge                       ("MaskEdge", Range(0, 1))               = 0.8
            _ParallaxMaskEdgeOffset                 ("MaskEdgeOffset", Range(0, 1))         = 0.2
        [FoldoutEnd]_FoldoutTexEnd("_FoldoutEnd", Float) = 0

        [FoldoutBegin(_FoldoutPBRPropEnd)]_FoldoutPBRProp("PBR Properties", Float) = 0
            _Metallic                               ("Metallic", Range(0, 1))               = 0.5
            _Smoothness                             ("Smoothness", Range(0, 1))             = 0.5
            _Occlusion                              ("Occlusion", Range(0, 1))              = 1
        [FoldoutEnd]_FoldoutPBRPropEnd("_FoldoutPBRPropEnd", Float) = 0

        // Direct Light
        [FoldoutBegin(_FoldoutDirectLightEnd)]_FoldoutDirectLight("Direct Light", Float) = 0
            [HDR]_SelfLight                         ("SelfLight", Color)                    = (1,1,1,1)
            _MainLightColorLerp                     ("Unity Light or SelfLight", Range(0, 1))= 0.5
            _DirectOcclusion                        ("DirectOcclusion", Range(0, 1))        = 0.1
            
            [NoScaleOffset]
            _FaceLightMap                           ("FaceLightMap", 2D)                    = "white" {}

            [Title(Shadow)]
            _ShadowColor                            ("ShadowColor", Color)                  = (0,0,0,1)
            _ShadowOffset                           ("ShadowOffset", Range(-1, 1))          = 0.5
            _ShadowSmoothNdotL                      ("ShadowSmoothNdotL", Range(0, 1))      = 0.25
            _ShadowSmoothScene                      ("ShadowSmoothScene", Range(0, 1))      = 0.1
            _ShadowStrength                         ("ShadowStrength", Range(0, 1))         = 1.0
        [FoldoutEnd]_FoldoutDirectLightEnd("_FoldoutEnd", Float) = 0

        // Ramp
        [FoldoutBegin(_FoldoutShadowRampEnd, _SHADOW_RAMP)]_FoldoutShadowRamp("ShadowRamp", Float) = 0
        [HideInInspector]_SHADOW_RAMP("_SHADOW_RAMP", Float) = 0
            [Ramp]_ShadowRampTex                    ("ShadowRampTex", 2D)                   = "white" { }
        [FoldoutEnd]_FoldoutShadowRampEnd("_FoldoutEnd", Float) = 0

        // Indirect Light
        [FoldoutBegin(_FoldoutIndirectLightEnd)]_FoldoutIndirectLight("Indirect Light", Float) = 0
            [Title(Diffuse)]
            [HDR]_SelfEnvColor                      ("SelfEnvColor", Color)                 = (0.5,0.5,0.5,0.5)
            _EnvColorLerp                           ("Unity SH or SelfEnv", Range(0, 1))    = 0.5
            _IndirDiffUpDirSH                       ("IndirDiffUpDirSH", Range(0, 1))       = 0.0
            _IndirDiffIntensity                     ("IndirDiffIntensity", Range(0, 1))     = 1.0
            [Title(Specular)]
            [Toggle(_INDIR_CUBEMAP)]_INDIR_CUBEMAP("_INDIR_CUBEMAP", Float)         = 0
            [NoScaleOffset]
            _IndirSpecCubemap                       ("SpecCube", Cube)                      = "black" {}

            _IndirSpecCubeWeight                    ("SpecCubeWeight", Range(0, 1))         = 0.5
            _IndirSpecIntensity                     ("IndirSpecIntensity", Range(0.01, 5))  = 1.0
        [FoldoutEnd]_FoldoutIndirectLightEnd("_FoldoutEnd", Float) = 0

        // Emission, Rim, etc.
        [FoldoutBegin(_FoldoutEmissRimEnd)]_FoldoutEmissRim("Emission, Rim, etc.", Float) = 0
            [Title(Emission)]
            [HDR]_EmissionCol                       ("EmissionCol", Color)                  = (0,0,0,1)
            _EmissionMap                            ("EmissionMap", 2D)                     = "white" {}
        [FoldoutEnd]_FoldoutEmissRimEnd("_FoldoutEnd", Float) = 0

        // EyelashOutter
        [FoldoutBegin(_FoldoutEyelashEnd, PassSwitch, CharacterTransparent)]_FoldoutEyelash("Eyelash", Float) = 0
            _BaseColorOutter                        ("BaseColor", Color)                    = (1,1,1,1)
            _BaseMapOutter                          ("BaseMap_d", 2D)                       = "white" {}
            _AlphaOutter                            ("Alpha", Range(0, 1))                  = 1
        [FoldoutEnd]_FoldoutEyelashEnd("_FoldoutEnd", Float) = 0

        [Space(10)][Title(MaterialFlags)]
        [KeysEnum(FLAG_HAIRSHADOW, FLAG_EYELASH, FLAG_HAIRMASK)]
        _ToonFlagsKeywords                          ("ToonFlags", Float)                    = -1
        
        // Other Settings
        [Title(OtherSettings)]
        [Enum(UnityEngine.Rendering.CullMode)] 
        _Cull                                       ("Cull Mode", Float)                    = 2
        [Toggle(_ALPHATEST_ON)]_AlphaClip           ("Alpha Clip", Float)                   = 0
        _Cutoff                                     ("Cutoff", Range(0, 1))                 = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"="Geometry-110"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Character"
        }
        LOD 300

        // GBuffer: write depth and normal
        UsePass "DanbaidongRP/PBRToon/Base/GBufferBase"

        // CharacterForward: shading
        Pass
        {
            Name "CharacterForward"
            Tags
            {
                "LightMode" = "CharacterForward"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite Off
            ZTest Equal
            Cull [_Cull]

            HLSLPROGRAM
            #pragma target 4.5

            // -------------------------------------
            // Shader Stages
            #pragma vertex ForwardToonVert
            #pragma fragment ForwardToonFrag

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _SHADOW_RAMP
            #pragma shader_feature_local _INDIR_CUBEMAP

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            // #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _PEROBJECT_SCREEN_SPACE_SHADOW
            #pragma multi_compile _ _GPU_LIGHTS_CLUSTER
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            // #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _LIGHT_LAYERS
            #include_with_pragmas "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/RenderingLayers.hlsl"

            // -------------------------------------
            // Unity defined keywords
            // #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            // #pragma multi_compile _ SHADOWS_SHADOWMASK
            // #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            // #pragma multi_compile _ LIGHTMAP_ON
            // #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            // #pragma multi_compile _ USE_LEGACY_LIGHTMAPS
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
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DeclareDepthTexture.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/GPUCulledLights.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/PreIntegratedFGD.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/PerObjectShadows.hlsl"

            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/Material/PBRToon/PBRToon.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float3  _BaseColor;
            float4  _BaseMap_ST;

            //Parallax
            float   _ParallaxScale;
            float   _ParallaxMaskEdge;
            float   _ParallaxMaskEdgeOffset;

            // PBR Properties
            float   _Metallic;
            float   _Smoothness;
            float   _Occlusion;

            // Direct Light
            float4  _SelfLight;
            float   _MainLightColorLerp;
            float   _DirectOcclusion;

            // Shadow
            float4  _ShadowColor;
            float   _ShadowOffset;
            float   _ShadowSmoothNdotL;
            float   _ShadowSmoothScene;
            float   _ShadowStrength;

            // Indirect
            float4  _SelfEnvColor;
            float   _EnvColorLerp;
            float   _IndirDiffUpDirSH;
            float   _IndirDiffIntensity;
            float   _IndirSpecCubeWeight;
            float   _IndirSpecIntensity;

            // Emission
            float4  _EmissionCol;

            // FaceDirection
            float3 _FaceRightDirWS;
            float3 _FaceFrontDirWS;

            // Alpha Test
            float   _Cutoff;
            CBUFFER_END


            TEXTURE2D_X(_GBuffer0); // Toon Flags

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_EmissionMap);
            SAMPLER(sampler_EmissionMap);

            TEXTURE2D(_FaceLightMap);
            SAMPLER(sampler_FaceLightMap);

            TEXTURE2D(_ShadowRampTex);
            SAMPLER(sampler_ShadowRampTex);

            TEXTURECUBE(_IndirSpecCubemap);

            struct Attributes
            {
                float4 vertex       :POSITION;
                float3 normal       :NORMAL;
                float4 tangent      :TANGENT;
                float4 color        :COLOR;
                float2 uv0          :TEXCOORD0;
                float2 uv1          :TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID 
            };

            struct Varyings 
            {
                float4 positionHCS      :SV_POSITION;
                float3 positionWS       :TEXCOORD0;
                float3 normalWS         :TEXCOORD1;
                float3 tangentWS        :TEXCOORD2;
                float3 biTangentWS      :TEXCOORD3;
                float4 color            :TEXCOORD4;
                float4 uv               :TEXCOORD5;// xy:uv0 zw:uv1
                // Other Props
                float2 faceLightDot     :TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings ForwardToonVert(Attributes v)
            {
                Varyings o;
                ZERO_INITIALIZE(Varyings, o);
                
                UNITY_SETUP_INSTANCE_ID(v); 
                UNITY_TRANSFER_INSTANCE_ID(v,o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionHCS = TransformObjectToHClip(v.vertex.xyz);
                o.positionWS = TransformObjectToWorld(v.vertex.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normal);
                o.tangentWS = TransformObjectToWorldDir(v.tangent.xyz);
                o.biTangentWS = cross(o.normalWS, o.tangentWS) * v.tangent.w * GetOddNegativeScale();
                o.color = v.color;
                o.uv.xy = TRANSFORM_TEX(v.uv0.xy, _BaseMap);
                o.uv.zw = v.uv1.xy;

                // Face lightmap dot value
                {
                    Light mainLight = GetMainLight();
                    float3 lightDirWS = mainLight.direction;
                    lightDirWS.xz = normalize(lightDirWS.xz);
                    _FaceRightDirWS.xz = normalize(_FaceRightDirWS.xz);
                    o.faceLightDot.x = dot(lightDirWS.xz, _FaceRightDirWS.xz);
                    o.faceLightDot.y = saturate(dot(-lightDirWS.xz, _FaceFrontDirWS.xz) * 0.5 + _ShadowOffset);
                }

                return o;
            }



            float4 ForwardToonFrag(Varyings i) : SV_Target0
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float  depth = i.positionHCS.z;
                float2 UV = i.uv.xy;
                float2 UV1 = i.uv.zw;
                float3 positionWS = i.positionWS;
                float2 screenUV = i.positionHCS.xy / _ScreenParams.xy;
                TransformScreenUV(screenUV);

                // FaceLightMap
                float2 faceLightMapUV = UV1;
                faceLightMapUV.x = 1 - faceLightMapUV.x;
                faceLightMapUV.x = i.faceLightDot.x < 0 ? 1 - faceLightMapUV.x : faceLightMapUV.x;

                //Parallax
                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
                float3 viewDirOS = TransformWorldToObjectDir(viewDirWS);
                viewDirOS = normalize(viewDirOS);
                float2 parallaxOffset = viewDirOS.xy;
                parallaxOffset.y *= -1;
                float2 parallaxUV = i.uv + _ParallaxScale * parallaxOffset;
                //parallaxMask
                float2 centerVec = i.uv - float2(0.5, 0.5);
                half centerDist = dot(centerVec, centerVec);
                half parallaxMask = smoothstep(_ParallaxMaskEdge, _ParallaxMaskEdge + _ParallaxMaskEdgeOffset, 1 - centerDist);


                // Tex Sample
                float4 mainTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, lerp(UV, parallaxUV, parallaxMask));
                float4 emissionTex = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, lerp(UV, parallaxUV, parallaxMask));
                float4 faceLightMap = SAMPLE_TEXTURE2D(_FaceLightMap, sampler_FaceLightMap, faceLightMapUV);

                // Property prepare
                float emission               = 1;
                float metallic               = _Metallic;
                float smoothness             = _Smoothness;
                float occlusion              = _Occlusion;
                float directOcclusion        = 1;
                float3 albedo = mainTex.rgb * _BaseColor.rgb;

                float perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(smoothness);
                float roughness           = PerceptualRoughnessToRoughness(perceptualRoughness);
                float roughnessSquare     = max(roughness * roughness, FLT_MIN);

                float3 normalWS = SafeNormalize(i.normalWS);

                // Rim Light
                // float3 normalVS = TransformWorldToViewNormal(normalWS);
                // normalVS = SafeNormalize(normalVS);

                float NdotV = dot(normalWS, viewDirWS);
                float clampedNdotV = ClampNdotV(NdotV);

                uint meshRenderingLayers = GetMeshRenderingLayer();

                DirectLighting directLighting;
                IndirectLighting indirectLighting;
                ZERO_INITIALIZE(DirectLighting, directLighting);
                ZERO_INITIALIZE(IndirectLighting, indirectLighting);

                float3 diffuseColor = ComputeDiffuseColor(albedo, metallic);
                float3 fresnel0 = ComputeFresnel0(albedo, metallic, DEFAULT_SPECULAR_VALUE);

                float3 specularFGD;
                float  diffuseFGD;
                float  reflectivity;
                GetPreIntegratedFGDGGXAndDisneyDiffuse(clampedNdotV, perceptualRoughness, fresnel0, specularFGD, diffuseFGD, reflectivity);
                float energyCompensation = 1.0 / reflectivity - 1.0;

                // float directRimArea = GetCharacterDirectRimLightArea(normalVS, screenUV, depth, _DirectRimWidth);


                // Face Property prepare
                float faceSDF = faceLightMap.r;
                float faceShadowArea = faceLightMap.a;

                float hairShadowArea = 1;
                float4 gbuffer0 = SAMPLE_TEXTURE2D_X_LOD(_GBuffer0, sampler_PointClamp, screenUV, 0);
                uint toonFlags = DecodeToonFlags(gbuffer0.r);
                if ((toonFlags & kToonFlagHairShadow) != 0)
                {
                    hairShadowArea = 0;
                }

                // Accumulate Direct
                // Directional Lights
                uint lightIndex = 0;
                for (lightIndex = 0; lightIndex < _DirectionalLightCount; lightIndex++)
                {
                    DirectionalLightData dirLight = g_DirectionalLightDatas[lightIndex];

                    #ifdef _LIGHT_LAYERS
                    if (IsMatchingLightLayer(dirLight.lightLayerMask, meshRenderingLayers))
                    #endif
                    {
                        dirLight.lightColor = lerp(dirLight.lightColor, _SelfLight.rgb, _MainLightColorLerp);

                        float3 lightDirWS = dirLight.lightDirection;
                        float NdotL = dot(normalWS, lightDirWS);
                        
                        float clampedNdotL = saturate(NdotL);
                        float halfLambert = NdotL * 0.5 + 0.5;
                        float clampedRoughness = max(roughness, dirLight.minRoughness);

                        float LdotV, NdotH, LdotH, invLenLV;
                        GetBSDFAngle(viewDirWS, lightDirWS, NdotL, NdotV, LdotV, NdotH, LdotH, invLenLV);
                        float3 lightDirVS = TransformWorldToViewDir(lightDirWS);
                        lightDirVS = SafeNormalize(lightDirVS);

                        // Shadow
                        // Remap Shadow area for NPR diffuse, but we should use clampedNdotL for PBR specular.
                        float shadowAttenuation = 1;
                        if (lightIndex == 0)
                        {
                            // Apply Shadows
                            // TODO: add different direct light shadowmap
                            shadowAttenuation = SAMPLE_TEXTURE2D(_ScreenSpaceShadowmapTexture, sampler_PointClamp, screenUV).x;
                            // #ifdef _PEROBJECT_SCREEN_SPACE_SHADOW
                            // shadowAttenuation = min(shadowAttenuation, SamplePerObjectScreenSpaceShadowmap(screenUV));
                            // #endif
                        }
                        
                        float shadowNdotL = SigmoidSharp(halfLambert, _ShadowOffset, _ShadowSmoothNdotL * 5);
                        float faceMapShadow = SigmoidSharp(faceSDF, i.faceLightDot.y, _ShadowSmoothNdotL * 5) * faceShadowArea;
                        float shadowScene = SigmoidSharp(shadowAttenuation, 0.5, _ShadowSmoothScene * 5);
                        float shadowArea = min(faceMapShadow, shadowScene); // Face Modify shadow
                        shadowArea = min(shadowArea, hairShadowArea);
                        shadowArea = lerp(1, shadowArea, _ShadowStrength);

                        float3 shadowRamp = lerp(_ShadowColor.rgb, float3(1, 1, 1), shadowArea);
                        #ifdef _SHADOW_RAMP
                        shadowRamp = SampleDirectShadowRamp(TEXTURE2D_ARGS(_ShadowRampTex, sampler_ShadowRampTex), shadowArea).xyz;
                        #endif

                        // BRDF
                        float3 F = F_Schlick(fresnel0, LdotH);
                        float DV = DV_SmithJointGGX(NdotH, abs(NdotL), clampedNdotV, clampedRoughness);
                        float3 specTerm = F * DV;
                        float diffTerm = Lambert();

                        #ifdef _SHADOW_RAMP
                        float specRange = saturate(DV);
                        float3 specRampCol = SampleDirectSpecularRamp(TEXTURE2D_ARGS(_ShadowRampTex, sampler_ShadowRampTex), specRange).xyz;
                        specTerm = F * clamp(specRampCol.rgb + DV, 0, 10);
                        #endif

                        // Accumulate
                        directLighting.diffuse += diffuseColor * diffTerm * shadowRamp * dirLight.lightColor * directOcclusion;
                        directLighting.specular += specTerm * clampedNdotL * shadowScene * dirLight.lightColor * directOcclusion;
                    }
                }

                // Punctual Lights
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

                            // Lighting Logical Code Begins
                            float3 lightDirWS = lightDirection;
                            float NdotL = dot(normalWS, lightDirWS);
                            
                            float clampedNdotL = saturate(NdotL);
                            float clampedRoughness = max(roughness, gpuLight.minRoughness);

                            float LdotV, NdotH, LdotH, invLenLV;
                            GetBSDFAngle(viewDirWS, lightDirWS, NdotL, NdotV, LdotV, NdotH, LdotH, invLenLV);


                            float3 F = F_Schlick(fresnel0, LdotH);
                            float DV = DV_SmithJointGGX(NdotH, abs(NdotL), clampedNdotV, clampedRoughness);
                            float3 specTerm = F * DV;
                            float diffTerm = Lambert();

                            diffTerm *= clampedNdotL;
                            specTerm *= clampedNdotL;

                            directLighting.diffuse += diffuseColor * diffTerm * gpuLight.lightColor * attenuation * gpuLight.baseContribution;
                            directLighting.specular += specTerm * gpuLight.lightColor * attenuation * gpuLight.baseContribution;
                        }

                        v_lightListOffset++;
                    }
                }

                // Accumulate Indirect
                // Indirect Diffuse
                EvaluateIndirectDiffuse(indirectLighting, diffuseColor, normalWS, _IndirDiffUpDirSH, _SelfEnvColor, _EnvColorLerp, diffuseFGD);

                // Indirect Specular
                float3 reflectDirWS = reflect(-viewDirWS, normalWS);
                float reflectionHierarchyWeight = 0.0; // Max: 1.0

                #if defined(_INDIR_CUBEMAP)
                EvaluateIndirectSpecular_Cubemap(indirectLighting, TEXTURECUBE_ARGS(_IndirSpecCubemap, sampler_LinearRepeat), 
                                                reflectDirWS, perceptualRoughness, specularFGD,
                                                reflectionHierarchyWeight, _IndirSpecCubeWeight);
                #endif

                EvaluateIndirectSpecular_Sky(indirectLighting, reflectDirWS, perceptualRoughness, specularFGD,
                                            reflectionHierarchyWeight, 1.0);

                // Emission
                float3 emissResult = emission * lerp(_EmissionCol.rgb, _EmissionCol.rgb * albedo.rgb, _EmissionCol.a);
                emissResult *= emissionTex.rgb;
                // PostEvaluate occlusion and energyCompensation
                float3 resultColor = PostEvaluate(directLighting, indirectLighting, occlusion, fresnel0, energyCompensation, _IndirDiffIntensity, _IndirSpecIntensity);
                resultColor += emissResult;


                return float4(resultColor, 1);
            }
            ENDHLSL

        }
        
        // EyelashOutter
        UsePass "DanbaidongRP/Helpers/CharacterEyelashOutter/CharacterTransparent"

        // Outline
        // UsePass "DanbaidongRP/Helpers/Outline/ForwardOutline"

        // ShadowCaster
        UsePass "DanbaidongRP/PBRToon/Base/ShadowCaster"

        // DepthOnly
        UsePass "DanbaidongRP/PBRToon/Base/DepthOnly"

        // // DepthNormals
        // UsePass "DanbaidongRP/PBRToon/Base/DepthNormals"

    }

    SubShader
    {
        Tags{ "RayTracingRenderPipeline" = "DanbaidongRP" }
        Pass
        {
            Name "IndirectDXR"
            Tags{ "LightMode" = "IndirectDXR" }

            HLSLPROGRAM

            // -------------------------------------
            // Shader Stages
            #pragma only_renderers d3d11 xboxseries ps5
            #pragma raytracing surface_shader

      
            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            // #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            // #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            // #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            // #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            // #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            // #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            // #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            // #pragma multi_compile_fragment _ _LIGHT_COOKIES
            // #pragma multi_compile _ _LIGHT_LAYERS
            // #pragma multi_compile _ _FORWARD_PLUS
            // #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            // #include_with_pragmas "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/RenderingLayers.hlsl"


            // -------------------------------------
            // Unity defined keywords
            // #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            // #pragma multi_compile _ SHADOWS_SHADOWMASK
            // #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            // #pragma multi_compile _ LIGHTMAP_ON
            // #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            // #pragma multi_compile _ USE_LEGACY_LIGHTMAPS
            // #pragma multi_compile _ LOD_FADE_CROSSFADE
            // #pragma multi_compile_fog
            // #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // #include_with_pragmas "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/ProbeVolumeVariants.hlsl"

            //--------------------------------------
            // GPU Instancing
            // #pragma multi_compile_instancing
            // #pragma instancing_options renderinglayer
            // #include_with_pragmas "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/DOTS.hlsl"


            // List all the attributes needed in raytracing shader
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/ShaderLibrary/Core.hlsl"


            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/Raytracing/ShaderVariablesRaytracing.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/Raytracing/RaytracingIntersection.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/Raytracing/RaytracingFragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/Raytracing/RaytracingLighting.hlsl"
            #include "Packages/com.unity.render-pipelines.danbaidong/Shaders/Raytracing/RayTracingCommon.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float3  _BaseColor;
            float4  _BaseMap_ST;

            //Parallax
            float   _ParallaxScale;
            float   _ParallaxMaskEdge;
            float   _ParallaxMaskEdgeOffset;

            // PBR Properties
            float   _Metallic;
            float   _Smoothness;
            float   _Occlusion;

            // Direct Light
            float4  _SelfLight;
            float   _MainLightColorLerp;
            float   _DirectOcclusion;

            // Shadow
            float4  _ShadowColor;
            float   _ShadowOffset;
            float   _ShadowSmoothNdotL;
            float   _ShadowSmoothScene;
            float   _ShadowStrength;

            // Indirect
            float4  _SelfEnvColor;
            float   _EnvColorLerp;
            float   _IndirDiffUpDirSH;
            float   _IndirDiffIntensity;
            float   _IndirSpecCubeWeight;
            float   _IndirSpecIntensity;

            // Emission
            float4  _EmissionCol;

            // FaceDirection
            float3 _FaceRightDirWS;
            float3 _FaceFrontDirWS;

            // Alpha Test
            float   _Cutoff;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);


            // Generic function that handles the reflection code
            [shader("closesthit")]
            void ClosestHitMain(inout RayIntersection rayIntersection : SV_RayPayload, AttributeData attributeData : SV_IntersectionAttributes)
            {
                // Make sure to add the additional travel distance
                rayIntersection.t = RayTCurrent();
                rayIntersection.cone.width += rayIntersection.t * rayIntersection.cone.spreadAngle;

                // Hit point data.
                IntersectionVertex currentVertex;
                FragInputs fragInput;
                GetCurrentVertexAndBuildFragInputs(attributeData, currentVertex, fragInput);
                PositionInputs posInput = GetPositionInput(rayIntersection.pixelCoord, _ScreenSize.zw, fragInput.positionRWS);

                float3 viewDirWS = -WorldRayDirection();


                            
                float3 reflected = float3(0.0, 0.0, 0.0);
                float reflectedWeight = 0.0;
                // Multi bounce indirect if needed
                #ifdef MULTI_BOUNCE_INDIRECT
                if (rayIntersection.remainingDepth < _RaytracingMaxRecursion)
                {
                    // TODO: Multi bounce
                }
                #endif /* MULTI_BOUNCE_INDIRECT */

                // Fill the ray context
                RayContext rayContext;
                rayContext.reflection = reflected;
                rayContext.reflectionWeight = reflectedWeight;
                rayContext.transmission = 0.0;
                rayContext.transmissionWeight = 0.0;
                #ifdef MULTI_BOUNCE_INDIRECT
                rayContext.useAPV = _RayTracingDiffuseLightingOnly ? rayIntersection.remainingDepth == _RaytracingMaxRecursion : 1;
                #else
                rayContext.useAPV = 1;
                #endif


                float2 uv = fragInput.texCoord0.xy;
                uv = TRANSFORM_TEX(uv, _BaseMap);


                // Tex Sample
                float4 mainTex = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, uv, 0);
                // float4 pbrMask = SAMPLE_TEXTURE2D_LOD(_PBRMask, sampler_PBRMask, uv, 0);
                // float3 bumpTS = UnpackNormalScale(SAMPLE_TEXTURE2D_LOD(_NormalMap, sampler_NormalMap, uv, 0), _NormalScale);

                // Property prepare
                float emission          = 1 - mainTex.a;
                float metallic          = 0;
                float smoothness        = 0;
                float occlusion         = 1;
                float3 albedo           = mainTex.rgb * _BaseColor.rgb;

                float3 emissResult = emission * lerp(_EmissionCol.rgb, _EmissionCol.rgb * albedo.rgb, _EmissionCol.a);


                float3 normalWS = fragInput.tangentToWorld[2];

                // Ray traced Lighting
                RayTracingShadingData shadingData = InitRayTracingShadingData(posInput, albedo, metallic, smoothness, occlusion, normalWS, viewDirWS);


                RayTracingLightingOutput output = RayTracedToon(posInput, shadingData, rayContext, _SelfLight, _MainLightColorLerp, _SelfEnvColor, _EnvColorLerp);


                rayIntersection.color = output.diffuseLighting + output.specularLighting + emissResult;
            }

            // Generic function that handles the reflection code
            [shader("anyhit")]
            void AnyHitMain(inout RayIntersection rayIntersection : SV_RayPayload, AttributeData attributeData : SV_IntersectionAttributes)
            {
                rayIntersection.color = 0.4;
                IgnoreHit();
            }
            ENDHLSL
        }
    }

    CustomEditor "UnityEditor.DanbaidongGUI.DanbaidongGUI"
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}