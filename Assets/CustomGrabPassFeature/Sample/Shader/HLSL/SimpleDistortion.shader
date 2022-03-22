Shader "CatDarkGame/SimpleDistortion"
{
    Properties
    { 
        [Header(Noise)]
        _NoiseTex ("Texture(R)", 2D) = "black" {}
        _NoiseSpeed("Tiling Speed", float) = 1.0
        _NoiseIntensity("Intensity", float) = 0.041
        
        [Space(10)]
        _Alpha("Alpha", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        
        Pass
        {
            Cull back							
            ZWrite Off							
            Blend SrcAlpha OneMinusSrcAlpha	
            
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma prefer_hlslcc gles   
            #pragma exclude_renderers d3d11_9x  
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPosition : TEXCOORD1;
            }; 
            
            // SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                sampler2D _NoiseTex;
                float4 _NoiseTex_ST;
                float _NoiseSpeed;
                float _NoiseIntensity;
                float _Alpha;
            CBUFFER_END
            
            // Global Texture Property
            sampler2D _CustomCameraTexture;
            //sampler2D _CameraOpaqueTexture;
             

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
 
                // ScreenUV 계산
                o.screenPosition = ComputeScreenPos(o.vertex);	
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Noise
                float2 noiseTexUV = i.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
                noiseTexUV += _NoiseSpeed * _Time.y;
                float noise = tex2D(_NoiseTex, noiseTexUV).r;
                
                // GrapPass
                float4 grabPassUV = i.screenPosition;
                grabPassUV.xy += noise * _NoiseIntensity;
                float4 grabPass = tex2Dproj(_CustomCameraTexture, grabPassUV);
                //float4 grabPass = tex2Dproj(_CameraOpaqueTexture, grabPassUV);	
                
                float4 final = grabPass;
                final.a = _Alpha;
                return final;
            }
            
            ENDHLSL
        }
    }
}