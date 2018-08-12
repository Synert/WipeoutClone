Shader "Custom/WaveShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		//LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float time = _Time.w;
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, float2(IN.uv_MainTex.y + sin(time * IN.uv_MainTex.y + IN.uv_MainTex.x), IN.uv_MainTex.x - cos(time * IN.uv_MainTex.x + IN.uv_MainTex.y))) * _Color;
			clip(frac((IN.uv_MainTex.y*6-time*0.6) * 5) - 0.5);
			clip(frac((IN.uv_MainTex.y * 12 - time * 0.8) * 5) - 0.4);
			fixed4 c = tex2D(_MainTex, float2(IN.uv_MainTex.x - time * 0.5, IN.uv_MainTex.y + sin(IN.uv_MainTex.x + time * 1.5) * 0.5)) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a * c.a;
			float val = IN.uv_MainTex.y + sin(IN.uv_MainTex.x + time * 1.5) * 0.5;
			if (val > 3) val = -1;
			if (val < 1) val = -1;
			if (o.Alpha == 0) val = -1;
			clip(val);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
