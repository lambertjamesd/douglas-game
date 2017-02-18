Shader "GrayscaleLol" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NightFactor ("Night Factor", Range(0, 1)) = 0
        _DarkColor ("Dark Color", Color) = (0,0,0,1)
        _MidColor ("Mid Color", Color) = (0,0,0.9,1)
        _LightColor ("Light Color", Color) = (0.1,0.8,1.0,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200


	     CGPROGRAM
	     #pragma surface surf Lambert
	     sampler2D _MainTex;
	     float _NightFactor;
	     float4 _DarkColor;
	     float4 _MidColor;
	     float4 _LightColor;
	     struct Input {
	         float2 uv_MainTex;
	     };
	     void surf (Input IN, inout SurfaceOutput o) {
	         half4 c = tex2D (_MainTex, IN.uv_MainTex);
	         
	         half grey = (c.r + c.g + c.b)/3;
	         
	         half4 nightColor = 
	         	(1 - saturate(grey * 2)) * _DarkColor +
	         	(1 - saturate(abs(1 - grey * 2))) * _MidColor +
	         	saturate(grey * 2 - 1) * _LightColor;
	         
	         o.Albedo = lerp(c, nightColor, _NightFactor);
	         o.Alpha = c.a;
	     }
	     ENDCG
	 }
 	FallBack "Diffuse"
 }