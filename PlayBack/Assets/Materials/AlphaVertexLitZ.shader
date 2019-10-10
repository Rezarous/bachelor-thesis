// Based on approach retrieved from: http://wiki.unity3d.com/index.php/AlphaVertexLitZ

Shader "Unlit/AlphaVertexLitZ"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,0.5)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		
		// Render into depth buffer only

		Pass
		{
			ZWrite On
			ColorMask 0
			ZTest Always
		}
			
		Pass
		{
			ZWrite On
			ColorMask 0
		}
		
		// Render normally
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}
		
			Lighting On
			SetTexture[_MainTex]
			{
				Combine texture * primary DOUBLE, texture * primary
			}
		}
	}
}