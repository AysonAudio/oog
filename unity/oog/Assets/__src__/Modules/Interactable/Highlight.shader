
/// <summary>
///     Draws a wireframe, or a color stroke on every edge in a mesh.
///     Animates the stroke alpha over time, following a bezier curve.
/// </summary> <param name="_WireframeThickness">
///     At 0, there is no wireframe.
///     At 0.5, the wireframe edges extrude inwards by 50% each, so it 100% covers the shape.
/// </param> <param name="_WireframeBlurFactor">
///     Strength of the alpha transition between a shape edge and an inner wireframe edge.
///     At 0, the wireframe is completely opaque.
///     At 1, it's opaque at the shape edge and transparent at the inner wireframe edge.
/// </param>

Shader "Custom/Interactable/Highlight" {
Properties {
    [MainTexture]
        _BaseMap ("Base Map", 2D) = "" {}
    [MainColor]
        _WireframeColor ("Wireframe Color", Color) = (1, 1, 1, 1)
        _WireframeThickness ("Wireframe Thickness", Range(0,0.5)) = 0.05
        _WireframeBlurFactor ("Wireframe Blur Factor", Range(0,1)) = 0.815
        _SpecularColor ("Specular Color", Color) = (0.9, 0.9, 0.9, 1)
        _Shininess ("Shininess", Range(0.001,0.999)) = 0.5
        _AnimFreq ("Animation Frequency", Float) = 1
}

SubShader {
    LOD 300
    Tags {
        "RenderType" = "Opaque"
        "RenderPipeline" = "UniversalPipeline"
        "UniversalMaterialType" = "Lit"
        "IgnoreProjector" = "True"
    }

                                            // ------------------------------------------------------------------
                                            //  Forward pass based on URP Lit Shader.
                                            //  Shades all light in a single pass.
                                            //  Ambient + Diffuse + Specular + Emission.
                                            // ------------------------------------------------------------------

    Pass {
    Name "ForwardLit"
    Tags {"LightMode" = "UniversalForward"}
    Blend One Zero, One Zero
    ZWrite On
    Cull Off
    AlphaToMask On

    HLSLPROGRAM
        #pragma target 2.0
        #pragma vertex interpolate_verts_to_frags
        #pragma geometry interpolate_tris_to_frags
        #pragma fragment rasterize_frag_color
        #pragma require geometry
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Assets/__src__/Modules/Math/Real3.hlsl"
        #include "Assets/__src__/Modules/Math/Blend.hlsl"

                                            ////////////////////////////////////
                                            #ifndef Material_Keywords
                                            ////////////////////////////////////

        #pragma shader_feature_local_fragment _EMISSION
        #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
        #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
        #pragma shader_feature_local_fragment _SPECULAR_SETUP

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef Universal_Pipeline_Keywords
                                            ////////////////////////////////////

        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _LIGHT_LAYERS

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef Unity_Defined_Keywords
                                            ////////////////////////////////////

        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ USE_LEGACY_LIGHTMAPS
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        #pragma multi_compile_fog
        #pragma multi_compile_fragment _ DEBUG_DISPLAY

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef GPU_Instancing
                                            ////////////////////////////////////

        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef DATA_TYPES
                                            ////////////////////////////////////

        struct VertexInputParams {
            float4 positionOS : POSITION;
            float4 normalOS: NORMAL;
            float2 uv : TEXCOORD0;
        };
        struct FragmentParams {
            linear float4 positionCS : SV_POSITION;
            linear float2 baseMapUV : TEXCOORD0;
            linear float2 lightMapUV : TEXCOORD1;
            linear float4 shadowCoords : TEXCOORD3;
            linear float4 ambientColor : TEXCOORD4;
            linear float4 diffuseColor : TEXCOORD5;
            linear float4 specularColor : TEXCOORD6;
            linear float3 baryWeights : TEXCOORD7;
        };

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef PROGRAM_VARIABLES
                                            ////////////////////////////////////

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _WireframeColor;
            half _WireframeThickness;
            half _WireframeBlurFactor;
            half4 _SpecularColor;
            half _Shininess;
            half _AnimFreq;
        CBUFFER_END
        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef VERTEX_SHADER
                                            ////////////////////////////////////

        FragmentParams interpolate_verts_to_frags(VertexInputParams IN) {
            // [VERTEX DATA] : ObjectSpace -> ClipSpace.
            float4 positionCS = TransformObjectToHClip(IN.positionOS.xyz);

            // [VERTEX DATA] : ObjectSpace.
            half3 dirFromCamOS = GetObjectSpaceNormalizeViewDir(IN.positionOS.xyz);

            // [VERTEX DATA] : ObjectSpace -> WorldSpace.
            VertexPositionInputs geomInfoWS = GetVertexPositionInputs(IN.positionOS.xyz);
            VertexNormalInputs normalInfoWS = GetVertexNormalInputs(IN.normalOS.xyz);
            float3 positionWS = geomInfoWS.positionWS;
            half3 normalWS = normalInfoWS.normalWS;

            // [VERTEX DATA] : All Spaces.
            float4 shadowCoords = GetShadowCoord(geomInfoWS);
            float2 baseMapUV = TRANSFORM_TEX(IN.uv, _BaseMap);
            float2 lightMapUV = IN.uv * unity_LightmapST.xy + unity_LightmapST.zw;

            // Calculate RGB increment from diffuse light and specular light.
            Light mainLight = GetMainLight();
            float3 mainLightDiffuse = LightingLambert(mainLight.color, mainLight.direction, normalWS);
            float3 mainLightSpecular = LightingSpecular(mainLight.color, mainLight.direction, normalWS, dirFromCamOS, _SpecularColor, 1 - _Shininess);

            float3 otherLightsDiffuse, otherLightsSpecular = 0;
            for (int i = 1; i < GetAdditionalLightsCount() + 1; i++) {
                Light light = GetAdditionalLight(i, positionWS);
                float3 lightDiffuse = LightingLambert(light.color, light.direction, normalWS);
                float3 lightSpecular = LightingSpecular(light.color, light.direction, normalWS, dirFromCamOS, _SpecularColor, 1 - _Shininess);
                otherLightsDiffuse += lightDiffuse;
                otherLightsSpecular += lightSpecular;
            }

            // Send vertex info to HLSL engine, which interpolates it to every pixel fragment.
            FragmentParams OUT;
            OUT.positionCS = positionCS;
            OUT.baseMapUV = baseMapUV;
            OUT.lightMapUV = lightMapUV;
            OUT.shadowCoords = shadowCoords;
            OUT.baryWeights = 0;
            OUT.ambientColor = float4(EvaluateAmbientProbe(normalWS), 1);
            OUT.diffuseColor = float4(mainLightDiffuse + otherLightsDiffuse, 1);
            OUT.specularColor = float4(mainLightSpecular + otherLightsSpecular, 1);
            return OUT;
        }

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef GEOMETRY_SHADER
                                            ////////////////////////////////////

        [maxvertexcount(3)]
        void interpolate_tris_to_frags(triangle FragmentParams IN[3], inout TriangleStream<FragmentParams> triStream) {
            FragmentParams vertex0 = IN[0];
            FragmentParams vertex1 = IN[1];
            FragmentParams vertex2 = IN[2];
            vertex0.baryWeights = float3(1,0,0);
            vertex1.baryWeights = float3(0,1,0);
            vertex2.baryWeights = float3(0,0,1);
            triStream.Append(vertex0);
            triStream.Append(vertex1);
            triStream.Append(vertex2);
            triStream.RestartStrip();
        }

                                                                        #endif
                                            ////////////////////////////////////
                                            #ifndef FRAGMENT_SHADER
                                            ////////////////////////////////////

        half4 rasterize_frag_color(FragmentParams IN) : SV_Target {
            // Locate base fragment color by mapping UV to base texture bitmap.
            // Apply lighting: ambient, diffuse, specular, lightmap emission, shadow.
            float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.baseMapUV);
            float4 ambient = IN.ambientColor * SampleAmbientOcclusion(IN.positionCS.xy);
            float4 diffuse = clamp(IN.diffuseColor, 0, baseColor);
            float4 emission = SAMPLE_TEXTURE2D(unity_Lightmap, samplerunity_Lightmap, IN.lightMapUV);
            baseColor *= clamp(ambient + diffuse + IN.specularColor + emission, 0, 1);
            baseColor *= MainLightRealtimeShadow(IN.shadowCoords);

            // If frag pixel is outside wireframe, use lit base fragment color.
            bool fragIsNearEdge = VectContainsMax(IN.baryWeights, _WireframeThickness);
            if (!fragIsNearEdge) return baseColor;

            // If frag pixel is inside wireframe, blend _WireframeColor with lit base fragment color.
            // Use alpha compositing, or alpha blending.
            // Alpha value is opaque at mesh edges, and fades out at triangle center, based on _OutlineBlurFactor.
            // Alpha value also animates over time, following a bezier curve.
            else {
                half secondsElapsed = _Time.y;
                half distFromEdge = MinComponent(IN.baryWeights);
                half percentDistFromEdge = distFromEdge / _WireframeThickness;

                half animWeight = BlendBezierInOut(secondsElapsed);
                half blurWeight = lerp(2, 1, _WireframeBlurFactor) - percentDistFromEdge;
                half wireframeOpacity = clamp(blurWeight * animWeight, 0, 1);
                return wireframeOpacity * _WireframeColor + (1 - wireframeOpacity) * baseColor;
            }
        }
    #endif
    ENDHLSL
}

                                            // ------------------------------------------------------------------
                                            //  Shadowcaster pass from URP Lit Shader.
                                            // ------------------------------------------------------------------

    Pass {
    Name "ShadowCaster"
    Tags {"LightMode" = "ShadowCaster"}
    ZWrite On
    ZTest LEqual
    ColorMask 0
    Cull Off
    HLSLPROGRAM
        #pragma target 2.0
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
        #pragma vertex ShadowPassVertex
        #pragma fragment ShadowPassFragment
        #pragma multi_compile_instancing
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
    ENDHLSL
}} FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
