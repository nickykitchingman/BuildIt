Shader "Custom/BacksideTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackAlpha ("Back Transparency", Range(0, 1)) = 0.1
        _Ambience ("Ambience", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparency" "LightMode"="ForwardBase" }
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
                fixed4 diff : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BackAlpha;
            float _Ambience;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = normalize(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                // lighting
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(v.normal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                // fog
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
                fixed face = dot(i.viewDir, i.normal);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                // transparency on backside
                col.a *= face > 0 ? 1 : _BackAlpha;
                // lighting
                col.rgb *= clamp(i.diff, _Ambience, 1);
                return col;
            }
            ENDCG
        }
    }
}
