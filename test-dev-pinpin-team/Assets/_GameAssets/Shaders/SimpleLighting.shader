// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Pinpin/CustomLighting/Simple"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		[Header(Shadow)][Space]_ShadowColor("Shadow Color", Color) = (0,0,0,0)
		_RampThreshold("Ramp Threshold", Range( -1 , 1)) = 0
		_RampSmooth("Ramp Smooth", Range( 0.01 , 2)) = 1
		_ShadowIntensity("Shadow Intensity", Range( 0 , 1)) = 1
		_ExtraShadowIntensity("Extra Shadow Intensity", Range( 0 , 1)) = 1
		[Header(Other)][Space]_Saturation("Saturation", Range( 0 , 5)) = 1
		_LuminosityValue("Luminosity Value", Range( 0 , 5)) = 1
		_HueShift("Hue Shift", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _HueShift;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform float4 _ShadowColor;
		uniform float _RampThreshold;
		uniform float _RampSmooth;
		uniform float _ExtraShadowIntensity;
		uniform float _ShadowIntensity;
		uniform float _Saturation;
		uniform float _LuminosityValue;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode255 = tex2D( _MainTex, uv_MainTex );
			float3 hsvTorgb2_g93 = RGBToHSV( ( tex2DNode255 * _Color ).rgb );
			float3 hsvTorgb3_g93 = HSVToRGB( float3(( _HueShift + hsvTorgb2_g93.x ),hsvTorgb2_g93.y,hsvTorgb2_g93.z) );
			float3 Color183 = hsvTorgb3_g93;
			float3 hsvTorgb2_g92 = RGBToHSV( ( tex2DNode255 * _ShadowColor ).rgb );
			float3 hsvTorgb3_g92 = HSVToRGB( float3(( _HueShift + hsvTorgb2_g92.x ),hsvTorgb2_g92.y,hsvTorgb2_g92.z) );
			float3 ShadowColor85 = hsvTorgb3_g92;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float dotResult18_g104 = dot( ase_worldlightDir , ase_normWorldNormal );
			float clampResult96_g104 = clamp( ( _RampThreshold + dotResult18_g104 ) , 0.0 , 1.0 );
			float saferPower83_g104 = abs( clampResult96_g104 );
			float lerpResult97_g104 = lerp( 1.0 , ase_lightAtten , _ExtraShadowIntensity);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_93_0_g104 = _ShadowIntensity;
			float4 lerpResult43_g104 = lerp( float4( Color183 , 0.0 ) , float4( ShadowColor85 , 0.0 ) , float4( ( ( 1.0 - saturate( ( ( saturate( pow( saferPower83_g104 , _RampSmooth ) ) * ( lerpResult97_g104 * ase_lightColor.rgb ) ) * ase_lightColor.a ) ) ) * temp_output_93_0_g104 ) , 0.0 ));
			float4 Lighting26_g104 = lerpResult43_g104;
			float3 hsvTorgb3_g105 = RGBToHSV( saturate( Lighting26_g104 ).rgb );
			float3 hsvTorgb4_g105 = HSVToRGB( float3(hsvTorgb3_g105.x,( hsvTorgb3_g105.y * _Saturation ),hsvTorgb3_g105.z) );
			float3 hsvTorgb3_g106 = RGBToHSV( float4( hsvTorgb4_g105 , 0.0 ).rgb );
			float3 hsvTorgb4_g106 = HSVToRGB( float3(hsvTorgb3_g106.x,hsvTorgb3_g106.y,( hsvTorgb3_g106.z * _LuminosityValue )) );
			c.rgb = hsvTorgb4_g106;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
2638;931;2560;1361;684.7373;720.1895;1;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;54;-1240.209,-1090.215;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;255;-1024.615,-1090.264;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;86;-1023.712,-722.352;Inherit;False;Property;_ShadowColor;Shadow Color;2;0;Create;True;0;0;0;False;2;Header(Shadow);Space;False;0,0,0,0;0.6415094,0,0.2767363,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;57;-1029.708,-895.3396;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.9056604,0.1324315,0.1324315,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;253;-649.6147,-874.2643;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;254;-632.6147,-708.2643;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;252;-515.5837,-768.9216;Inherit;False;Property;_HueShift;Hue Shift;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;251;-205.5842,-693.9216;Inherit;False;HueShift;-1;;92;7bf085d763c41b347b965962bb17edb3;0;2;1;COLOR;0,0,0,0;False;5;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;250;-205.5842,-869.9216;Inherit;False;HueShift;-1;;93;7bf085d763c41b347b965962bb17edb3;0;2;1;COLOR;0,0,0,0;False;5;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-57.68213,-871.6831;Inherit;False;Color;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;-52.4962,-697.8127;Inherit;False;ShadowColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;196;216.3625,-547.9308;Inherit;False;Property;_Saturation;Saturation;7;0;Create;True;0;0;0;False;2;Header(Other);Space;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;303.3521,-966.5892;Inherit;False;85;ShadowColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;185;313.4584,-1046.86;Inherit;False;183;Color;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;257;222.2185,-632.6082;Inherit;False;Property;_ExtraShadowIntensity;Extra Shadow Intensity;6;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;211.6243,-805.6066;Inherit;False;Property;_RampSmooth;Ramp Smooth;4;0;Create;True;0;0;0;False;0;False;1;1;0.01;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;211.2772,-892.3825;Inherit;False;Property;_RampThreshold;Ramp Threshold;3;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;214;210.5236,-726.5345;Inherit;False;Property;_ShadowIntensity;Shadow Intensity;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;259;217.8381,-468.7186;Inherit;False;Property;_LuminosityValue;Luminosity Value;8;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;261;761.4958,-935.8599;Inherit;False;SimpleLighting;-1;;104;24d7923e5f8b42a42b4b035b15d4c927;0;8;36;COLOR;1,1,1,0;False;44;COLOR;0,0,0,0;False;53;FLOAT;0;False;84;FLOAT;1;False;93;FLOAT;1;False;98;FLOAT;1;False;37;FLOAT;1;False;100;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1070.403,-1178.236;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Pinpin/CustomLighting/Simple;False;False;False;False;True;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.02;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;255;0;54;0
WireConnection;253;0;255;0
WireConnection;253;1;57;0
WireConnection;254;0;255;0
WireConnection;254;1;86;0
WireConnection;251;1;254;0
WireConnection;251;5;252;0
WireConnection;250;1;253;0
WireConnection;250;5;252;0
WireConnection;183;0;250;0
WireConnection;85;0;251;0
WireConnection;261;36;185;0
WireConnection;261;44;87;0
WireConnection;261;53;172;0
WireConnection;261;84;200;0
WireConnection;261;93;214;0
WireConnection;261;98;257;0
WireConnection;261;37;196;0
WireConnection;261;100;259;0
WireConnection;0;13;261;0
ASEEND*/
//CHKSM=8157337BDCABB2ABF017607D77CD0DF49C9F9A00