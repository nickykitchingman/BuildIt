Shader "Unlit/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blur ("Blue", Range(0, 0.1)) = 0.05
    }
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float _Blur;

            fixed4 frag (v2f i) : SV_Target
            {
                 //calculate aspect ratio
                float invAspect = _ScreenParams.y / _ScreenParams.x;
                //init color variable
                float4 col = 0;
                //iterate over blur samples
                for(float index = 0; index < 10; index++){
                    //get uv coordinate of sample
                    float2 uv = i.uv + float2((index/9 - 0.5) * _Blur * invAspect, 0);
                    //add color at position to color
                    col += tex2D(_MainTex, uv);
                }
                //divide the sum of values by the amount of samples
                col = col / 10;
                return col;
            }
            ENDCG
        }
    }
}
