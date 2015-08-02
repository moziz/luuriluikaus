Shader "Custom/tvShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		Lighting On
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
        /*
        Pass {
			SetTexture [_Mask] {
			    combine texture
			}
			SetTexture [_MainTex] {combine texture, previous}
		}
        */

        Pass {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos: SV_POSITION;
                float2 uv: TEXCOORD0;
            };

            uniform sampler2D _MainTex;

            v2f vert(appdata_base v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 color;
                color = tex2D(_MainTex, i.uv);
                
                float scanline = (-i.uv.y * 3.0f - _Time * 128.0f + i.uv.x * 0.3f) % 1.0f;

                color += scanline * 0.05f + pow(2, sin(_Time * 16.0f)) * 0.025f;
                color.a = 1.0f;

                return color;
            }

            ENDCG
        }   
	}  
}
