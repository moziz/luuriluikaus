Shader "Custom/tvShader" {
	Properties {
		//_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask("Mask Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		Lighting On
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		//Material {

       //     Diffuse [_Color]
       // }
		
		Pass
		{
			SetTexture [_Mask] {
			combine texture
			//constantColor [_Color]
			}
			SetTexture [_MainTex] {combine texture, previous}
		
		}		
	}  
}
