Shader "Custom/HullOutline"
{
    Properties
    {
        _OutlineThickness ("OutlineThickness", Float) = 1 // multiplier to extrude the outline mesh
        _OutlineColor ("OutlineColor", Color) = (0, 0, 0, 1)
        _FillColor ("FillColor", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            Name "Hull Fill"
            Tags { "Queue"="Transparent" "RenderType"="Transparent" }

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // input properties
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(v.vertex);
                output.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return output;
            }

            // fragment input Properties
            float4 _FillColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sample = tex2D(_MainTex, i.uv);
                return _FillColor * sample.rgba;
                // return texColor.rgba;
            }
            ENDCG
        }

        Pass
        {
            Name "Hull Outline"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct vertexProperties
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
            };

            // input Properties
            float _OutlineThickness;
            fixed4 _OutlineColor;

            v2f vert (vertexProperties input)
            {
                v2f output;
                output.positionCS = UnityObjectToClipPos(input.positionOS * _OutlineThickness);

                return output;
            }

            fixed4 frag (v2f input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
