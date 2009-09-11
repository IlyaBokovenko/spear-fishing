Shader "My Shaders/Dot3" {
	Properties {
	   _MainTex ("Base (RGB)", 2D) = "white" {}
	   _BumpMap ("Bump", 2D) = "white" {}
	}
	SubShader {
	   Pass {
	      SetTexture[_BumpMap] {
	         constantColor [_DotLightDirection]
	         combine texture dot3 constant
	      }
	      SetTexture[_MainTex] {
	         combine texture * previous
	      }
	   }
	}
}