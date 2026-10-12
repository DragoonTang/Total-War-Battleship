Shader "MFEP_Stream"
{
    Properties
    {
        _Ripples_Displacement("Ripples Displacement", 2D) = "gray" {}
        [Normal] _Ripples("Ripples", 2D) = "bump" {}
        [Normal] _Ripples2("Ripples 2", 2D) = "bump" {}
        _Color("Color", Color) = (0, 0, 0, 0)
        _Displacement("Displacement", Range(0, 1)) = 0
        _Metallic("Metallic", Range(0, 1)) = 0
        _Base_Smoothness("Base Smoothness", Range(0, 1)) = 0
        _Speed("Speed", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_Ripples_Displacement);
            SAMPLER(sampler_Ripples_Displacement);
            TEXTURE2D(_Ripples);
            SAMPLER(sampler_Ripples);
            TEXTURE2D(_Ripples2);
            SAMPLER(sampler_Ripples2);
            CBUFFER_START(UnityPerMaterial)
                float4 _Ripples2_ST;
                half4 _Color;
                half _Displacement;
                half _Metallic;
                half _Base_Smoothness;
                half _Speed;
            CBUFFER_END
            struct Attributes
            {
                float4 positionOS : POSITION;
                half3 normalOS : NORMAL;
                half4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                half3 tangentWS : TEXCOORD2;
                half3 bitangentWS : TEXCOORD3;
                float2 uv : TEXCOORD4;
                half4 color : COLOR;
                half fogFactor : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                float2 scrollingUV = input.uv + _Time.x * _Speed;
                half3 displacementSample = SAMPLE_TEXTURE2D_LOD(
                    _Ripples_Displacement, sampler_Ripples_Displacement, scrollingUV, 0).rgb;
                input.positionOS.xyz += displacementSample * input.normalOS * _Displacement;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;
                output.tangentWS = normalInputs.tangentWS;
                output.bitangentWS = normalInputs.bitangentWS;
                output.uv = input.uv;
                output.color = input.color;
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float2 scrollingUV = input.uv + _Time.x * _Speed;
                float2 secondUV = input.uv * _Ripples2_ST.xy + _Ripples2_ST.zw;
                half3 firstNormal = UnpackNormal(SAMPLE_TEXTURE2D(_Ripples, sampler_Ripples, scrollingUV));
                half3 secondNormal = UnpackNormal(SAMPLE_TEXTURE2D(_Ripples2, sampler_Ripples2, secondUV));
                half3 normalTS = normalize(firstNormal + secondNormal);
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = NormalizeNormalPerPixel(
                    TransformTangentToWorld(normalTS, half3x3(input.tangentWS, input.bitangentWS, input.normalWS)));
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                lightingInput.fogCoord = input.fogFactor;
                lightingInput.bakedGI = SampleSH(lightingInput.normalWS);
                lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                lightingInput.shadowMask = half4(1, 1, 1, 1);
                SurfaceData surface = (SurfaceData)0;
                surface.albedo = (_Color * input.color).rgb;
                surface.metallic = _Metallic;
                surface.smoothness = _Base_Smoothness;
                surface.normalTS = normalTS;
                surface.occlusion = 1;
                surface.alpha = 1;
                half4 result = UniversalFragmentPBR(lightingInput, surface);
                result.rgb = MixFog(result.rgb, lightingInput.fogCoord);
                return result;
            }
            ENDHLSL
        }
    }
}
