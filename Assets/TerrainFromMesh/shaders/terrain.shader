// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Vertex Terrain" {
	Properties {
		_VColor ("Vertex White", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (1,0,1,1)
		_RimDist("Rim Distance",Float) = 5
		_RimIncrease ("Rim Increase", Float) = 2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		//sampler2D _MainTex;
		half4 _VColor;
		half4 _Specularity;
		half4 _RimColor;
		half _RimDist;
		half _RimIncrease;
		
		struct Input {
			//float2 uv_MainTex;
			fixed4 color : COLOR;
			float3 viewDir;
			float dist;
		};

		half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half3 h = normalize (lightDir + viewDir);

			half diff = max (0, dot (s.Normal, lightDir));

			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 48.0) * _Specularity;

			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
			c.a = s.Alpha;
			
			return c;
		}

		void vert (inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input,data);
			data.dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
			data.dist = clamp( (data.dist - 10 * _RimDist) / _RimIncrease, 0.0, 1.0);
			
		}

		void surf (Input IN, inout SurfaceOutput o) {
			
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          	
          	o.Emission = IN.color.rgb * pow (rim, _RimColor.a * (3.0 * IN.dist));
			
			o.Albedo = IN.color.rgb;
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
