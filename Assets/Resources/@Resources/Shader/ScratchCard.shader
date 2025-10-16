Shader "Custom/ScratchCardFront"
{
    Properties
    {
        _FrontTex ("Front Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "black" {}
        _ShineColor ("Shine Color", Color) = (1,1,1,1)
        _ShineIntensity ("Shine Intensity", Range(0,2)) = 1
        _ShinePower ("Shine Power", Range(0.1,5)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _FrontTex;
            sampler2D _MaskTex;

            float4 _ShineColor;
            float _ShineIntensity;
            float _ShinePower;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = UnityWorldSpaceViewDir(worldPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 front = tex2D(_FrontTex, i.uv);
                fixed mask = tex2D(_MaskTex, i.uv).r;

                float3 V = normalize(i.viewDir);
                float3 N = normalize(i.worldNormal);
                float fresnel = pow(1.0 - saturate(dot(V, N)), _ShinePower);
                float3 shine = fresnel * _ShineColor.rgb * _ShineIntensity;

                front.rgb += shine;

                // ±‹»˘ ∫Œ∫–¿ª ≈ı∏Ì«œ∞‘
                front.a = 1.0 - mask;

                return front;
            }
            ENDHLSL
        }
    }
}
