Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.2)) = 0.02
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        Lighting Off 
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offsets[8] = {
                    float2(-_OutlineWidth, -_OutlineWidth), float2(0, -_OutlineWidth), float2(_OutlineWidth, -_OutlineWidth),
                    float2(-_OutlineWidth,  0),                                float2(_OutlineWidth,  0),
                    float2(-_OutlineWidth,  _OutlineWidth), float2(0, _OutlineWidth), float2(_OutlineWidth, _OutlineWidth)
                };

                float alpha = 0.0;
                for (int j = 0; j < 8; j++)
                {
                    alpha += tex2D(_MainTex, i.uv + offsets[j]).a;
                }

                float4 texColor = tex2D(_MainTex, i.uv);
                if (alpha > 0.0 && texColor.a == 0.0)
                {
                    return _OutlineColor;
                }
                return texColor;
            }
            ENDCG
        }
    }
}
