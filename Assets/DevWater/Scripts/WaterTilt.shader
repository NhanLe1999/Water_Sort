Shader "Custom/WaterTilt"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Tilt ("Tilt Amount", Range(-1, 1)) = 0
        _Color ("Tint Color", Color) = (1,1,1,1) // Thêm thuộc tính màu
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Nhận màu từ SpriteRenderer
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR; // Truyền màu tới fragment shader
            };

            sampler2D _MainTex;
            float _Tilt;
            fixed4 _Color; // Biến màu

            v2f vert (appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                // Dịch UV theo góc nghiêng để tạo hiệu ứng
                OUT.uv.x += _Tilt * (0.5 - IN.uv.y); 

                // Lấy màu từ SpriteRenderer
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, IN.uv);
                return texColor * IN.color; // Kết hợp màu sprite với texture
            }
            ENDCG
        }
    }
}
