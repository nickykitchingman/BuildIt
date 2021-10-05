Shader "Unlit/Test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0, 1)) = .1
    }
		CGINCLUDE
#include "UnityCG.cginc"

			struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f {
			float4 pos : POSITION;
			float4 color : COLOR;
		};

		uniform float _Outline;
		uniform float4 _OutlineColor;

		v2f vert(appdata v) {
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f o;

		v.vertex *= (1 + _Outline);

		o.pos = UnityObjectToClipPos(v.vertex);

		//float3 norm   = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
		//float2 offset = TransformViewToProjection(norm.xy);

		o.color = _OutlineColor;
		return o;
	}
		ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            struct v1f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v1f vart (appdata v)
            {
                v1f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v1f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
