Shader "Custom/HologramShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Distortion ("Distortion", Float) = 0.1
		_DistortionSpeed ("DistortionSpeed", Float) = 1000.0
		_BobAmount ("Bobbing", Float) = 0.05
		_BobSpeed ("Bob speed", Float) = 1.0
		_SpeedMain ("Scroll speed 1", Float) = 30.0
		_SpeedSecondary ("Scroll speed 2", Float) = 70.0
		_SizeMain ("Cut size 1", Float) = 20.0
		_SizeSecondary ("Cut size 2", Float) = 40.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 200
		Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _SpeedMain, _SpeedSecondary, _SizeMain, _SizeSecondary,
			_Distortion, _DistortionSpeed, _BobSpeed, _BobAmount;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float rand(float2 co)
		{
			return frac((sin(dot(co.xy, float2(12.345 * _Time.w, 67.890 * _Time.w))) * 12345.67890 + _Time.w));
		}

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float time = _Time.w;
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, float2(IN.uv_MainTex.y + sin(time * IN.uv_MainTex.y + IN.uv_MainTex.x), IN.uv_MainTex.x - cos(time * IN.uv_MainTex.x + IN.uv_MainTex.y))) * _Color;
			//fixed4 c = tex2D(_MainTex, float2(IN.uv_MainTex.x - time * 0.5, IN.uv_MainTex.y + sin(IN.uv_MainTex.x + time * 1.0) * 0.15)) * _Color;
			fixed4 c = tex2D(_MainTex, float2(IN.uv_MainTex.x + sin(time + IN.uv_MainTex.y * _DistortionSpeed) * (_Distortion / 100),
				IN.uv_MainTex.y + sin(IN.uv_MainTex.x + time * _BobSpeed) * _BobAmount)) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a * c.a;

			if (frac((IN.uv_MainTex.y * _SizeMain - time * _SpeedMain) * 5) - 0.1 <= 0.0)
			{
				o.Alpha /= 2;
			}
			if (frac((IN.uv_MainTex.y * _SizeSecondary - time * _SpeedSecondary) * 5) - 0.1 <= 0.0)
			{
				o.Alpha /= 5;
			}
			
			o.Alpha /= 0.75 + rand(IN.uv_MainTex) * 5;

			float val = 1;// IN.uv_MainTex.y + sin(IN.uv_MainTex.x + time * 1.0) * 0.01;
			//if (val > 0.75) val = -1;
			//if (val < 0.25) val = -1;
			if (o.Alpha <= 0.05) val = -1;
			clip(val);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
