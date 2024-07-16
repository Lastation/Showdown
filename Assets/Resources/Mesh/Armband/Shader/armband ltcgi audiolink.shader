// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "4liceD/Armband ltcgi Audiolink"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		[Normal]_BumpMap("BumpMap", 2D) = "bump" {}
		_TextOutline("TextOutline", Range( 0 , 10)) = 1
		_NormalMap("Rotation", Vector) = (0.5,0,0,0)
		_EmissionMap("OutlineColor", Color) = (0,0,0,0)
		_GradientWidth("Gradient Width", Float) = 10
		_GradientSpacing("Gradient Spacing", Float) = 2
		_Channel("Audiolink Channel", Range( 0 , 3)) = 0
		_Subpixel("Subpixel", 2D) = "white" {}
		_Intensity("Intensity", Range( 0 , 10)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0"}
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		#include "Assets/_pi_/_LTCGI/Shaders/LTCGI.cginc"
		#include "Packages/com.llealloo.audiolink/Runtime/Shaders/AudioLink.cginc"
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = -0.01;
			v.vertex.xyz *= ( 1 + outlineVar);
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float4 OutlineColor74 = _EmissionMap;
			float mulTime7 = _Time.y * 0.5;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner9 = ( mulTime7 * _NormalMap + uv_MainTex);
			float4 tex2DNode1 = tex2D( _MainTex, panner9 );
			o.Emission = OutlineColor74.rgb;
			clip( tex2DNode1.a - _Cutoff );
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  "LTCGI"="ALWAYS" }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#include "Assets/_pi_/_LTCGI/Shaders/LTCGI.cginc"
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv2_texcoord2;
		};

		uniform sampler2D _Udon_VideoTex;
		float4 _Udon_VideoTex_TexelSize;
		uniform float4 _Udon_VideoTex_ST;
		uniform sampler2D _MainTex;
		uniform float2 _NormalMap;
		uniform float4 _MainTex_ST;
		uniform float _GradientWidth;
		uniform float _GradientSpacing;
		uniform float _Channel;
		uniform float4 _EmissionMap;
		uniform float _TextOutline;
		uniform sampler2D _Subpixel;
		uniform float4 _Subpixel_ST;
		uniform float _Intensity;
		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _Cutoff = 0.5;


		inline float AudioLinkData3_g5( int Band, int Delay )
		{
			return AudioLinkData( ALPASS_AUDIOLINK + uint2( Delay, Band ) ).rrrr;
		}


		float IfAudioLinkv2Exists1_g28(  )
		{
			int w = 0; 
			int h; 
			int res = 0;
			#ifndef SHADER_TARGET_SURFACE_ANALYSIS
			_AudioTexture.GetDimensions(w, h); 
			#endif
			if (w == 128) res = 1;
			return res;
		}


		inline float AudioLinkLerpMultiline1_g15( float Sample )
		{
			return AudioLinkLerpMultiline( ALPASS_WAVEFORM + uint2( Sample * 1024, 0 ) ).rrrr;;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_Udon_VideoTex = i.uv_texcoord * _Udon_VideoTex_ST.xy + _Udon_VideoTex_ST.zw;
			float4 temp_cast_1 = 0;
			float mulTime7 = _Time.y * 0.5;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner9 = ( mulTime7 * _NormalMap + uv_MainTex);
			float4 tex2DNode1 = tex2D( _MainTex, panner9 );
			float4 albedo17 = tex2DNode1;
			float temp_output_51_0 = ( abs( ( i.uv_texcoord.y - 0.5 ) ) * _GradientWidth );
			int Band3_g5 = (int)_Channel;
			int Delay3_g5 = 0;
			float localAudioLinkData3_g5 = AudioLinkData3_g5( Band3_g5 , Delay3_g5 );
			float localIfAudioLinkv2Exists1_g28 = IfAudioLinkv2Exists1_g28();
			float temp_output_159_0 = localIfAudioLinkv2Exists1_g28;
			float temp_output_55_0 = saturate( ( temp_output_51_0 - ( ( _GradientSpacing * ( 1.0 - localAudioLinkData3_g5 ) * temp_output_159_0 ) + ( _GradientSpacing * ( 1.0 - temp_output_159_0 ) ) ) ) );
			float4 OutlineColor74 = _EmissionMap;
			float Sample1_g15 = i.uv_texcoord.x;
			float localAudioLinkLerpMultiline1_g15 = AudioLinkLerpMultiline1_g15( Sample1_g15 );
			float isAudiolink111 = temp_output_159_0;
			float2 uv_Subpixel = i.uv_texcoord * _Subpixel_ST.xy + _Subpixel_ST.zw;
			float localLTCGI15_g14 = ( 0.0 );
			float3 ase_worldPos = i.worldPos;
			float3 worldPos15_g14 = ase_worldPos;
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 normalizeResult9_g14 = normalize( (WorldNormalVector( i , UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) ) )) );
			float3 worldNorm15_g14 = normalizeResult9_g14;
			float3 normalizeResult12_g14 = normalize( ( _WorldSpaceCameraPos - ase_worldPos ) );
			float3 cameraDir15_g14 = normalizeResult12_g14;
			float roughness15_g14 = ( 1.0 - 0.8 );
			float2 lightmapUV15_g14 = i.uv2_texcoord2;
			float3 diffuse15_g14 = float3( 0,0,0 );
			float3 specular15_g14 = float3( 0,0,0 );
			float specularIntensity15_g14 = 0;
			{
			LTCGI_Contribution(worldPos15_g14, worldNorm15_g14, cameraDir15_g14, roughness15_g14, lightmapUV15_g14, diffuse15_g14, specular15_g14, specularIntensity15_g14);
			}
			o.Emission = ( ( ( ( ( _Udon_VideoTex_TexelSize.z > (float)16 ? tex2D( _Udon_VideoTex, uv_Udon_VideoTex ) : temp_cast_1 ) + ( ( albedo17 * ( 1.0 - temp_output_55_0 ) ) + ( temp_output_55_0 * OutlineColor74 ) + ( ( saturate( ( 0.04 - abs( ( ( localAudioLinkLerpMultiline1_g15 + 0.5 ) - temp_output_51_0 ) ) ) ) / 0.1 ) * isAudiolink111 * OutlineColor74 ) + ( OutlineColor74 * fwidth( tex2DNode1.a ) * _TextOutline ) ) ) * tex2D( _Subpixel, uv_Subpixel ) ) * _Intensity ) + float4( ( specular15_g14 * 1.0 ) , 0.0 ) + float4( ( 0.5 * diffuse15_g14 ) , 0.0 ) ).rgb;
			float temp_output_78_0 = 0.8;
			o.Metallic = temp_output_78_0;
			o.Smoothness = temp_output_78_0;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				float4 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
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
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
Version=19200
Node;AmplifyShaderEditor.SimpleTimeNode;7;-1397.636,208.6826;Inherit;False;1;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;9;-1010.814,66.73538;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OutlineNode;43;-751.4368,431.6231;Inherit;False;1;False;Masked;0;0;Off;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1130.247,-315.9581;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-1579.46,-329.0148;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1853.761,-360.2143;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;-2427.695,-472.782;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;12;-1263.814,79.73538;Inherit;False;Property;_NormalMap;Rotation;4;0;Create;False;0;0;0;False;0;False;0.5,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-775.7418,-555.8596;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;44;-1322.865,413.8023;Inherit;False;Property;_EmissionMap;OutlineColor;5;0;Create;False;0;0;0;False;0;False;0,0,0,0;0.3185737,0,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1062.663,412.5382;Inherit;False;OutlineColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;72;-1104.986,-508.62;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-1343.745,-345.4492;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1666.01,-18.80035;Inherit;True;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-2934.928,-646.0143;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;-2207.658,-173.5658;Inherit;False;Property;_GradientWidth;Gradient Width;6;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;83;-2475.742,425.7005;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-2393.137,170.2638;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-2337.222,330.3881;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-2108.472,232.5344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;80;-3160.936,356.8996;Inherit;False;4BandAmplitude;-1;;5;f5073bb9076c4e24481a28578c80bed5;0;2;2;INT;0;False;4;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;93;-2599.129,250.9306;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;105;-1002.395,-1050.297;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-372.8223,-869.5303;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;-2673.442,634.736;Inherit;False;isAudiolink;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;113;-597.1228,-739.5316;Inherit;False;111;isAudiolink;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-1355.767,-178.757;Inherit;False;74;OutlineColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-601.9132,-609.9995;Inherit;False;74;OutlineColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-837.1602,164.4553;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;dcb30f2079e56cc40acc68464f6d1635;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;50;-2086.463,-413.5143;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-661.105,-237.8313;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-2810.551,0.66401;Inherit;False;Property;_GradientSpacing;Gradient Spacing;7;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2700.309,-352.5145;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-1354.565,-948.5065;Inherit;False;Constant;_WaveformHeight;Waveform Height;1;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3230.615,205.8469;Inherit;False;Property;_Channel;Audiolink Channel;8;0;Create;False;0;0;0;False;0;False;0;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1181.066,614.5688;Inherit;False;Constant;_MetallicGlossMap;Outline Distance;4;0;Create;False;0;0;0;False;0;False;-0.01;-0.01;-1;-0.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-305.3572,-358.3348;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-447.4294,659.4356;Inherit;False;Constant;_Float2;Float 2;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;131;-936.351,839.5298;Inherit;True;Property;_BumpMap;BumpMap;2;1;[Normal];Create;True;0;0;0;False;0;False;None;None;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;78;-203.1597,154.6582;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;0;False;0;False;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;109;-639.6956,-1081.886;Inherit;False;DrawLine;-1;;12;b931a6c4da53ab6489d06086e5e19048;0;4;5;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;132;-423.6883,481.8246;Inherit;False;LTCGI_Contribution;-1;;14;d3ea6060590627141a6e856295f0e87c;0;2;18;SAMPLER2D;0;False;21;FLOAT;0;False;3;FLOAT3;16;FLOAT;17;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;127;-284.771,293.5226;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;-1454.197,-685.1346;Inherit;False;17;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-588.6533,-13.59169;Inherit;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-367.0878,139.0361;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-550.9795,109.2672;Inherit;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-997.7155,-836.1866;Inherit;False;Constant;_WaveformLineSmoothing;Waveform Line Smoothing;6;0;Create;True;0;0;0;False;0;False;0.1;0.01;0.0001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-1002.396,-934.4664;Inherit;False;Constant;_WaveformThickness;Waveform Thickness;4;0;Create;True;0;0;0;False;0;False;0.04;0.02;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;104;-1325.315,-1077.207;Inherit;False;Waveform;-1;;15;86000a57e77967c4ea51f70716038ec2;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-1697.545,-1066.676;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FWidthOpNode;123;-842.1304,-130.5558;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;-1157.346,-55.54928;Inherit;False;Property;_TextOutline;TextOutline;3;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-106.1941,-229.0395;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;159;-2933.126,567.2194;Inherit;False;IsAudioLink;-1;;28;e83fef6181013ba4bacf30a3d9a31d37;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;168;484.4534,434.038;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-25.57465,570.0111;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;135;48.49725,-605.8149;Inherit;True;Property;;;11;0;Create;False;0;0;0;True;0;False;160;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexelSizeNode;164;518.9036,-727.5627;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;165;801.8637,-608.4755;Inherit;True;2;4;0;FLOAT;0;False;1;INT;0;False;2;COLOR;0,0,0,0;False;3;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;166;540.8637,-490.4755;Inherit;False;Constant;_Int0;Int 0;11;0;Create;True;0;0;0;False;0;False;16;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;167;554.3864,-329.2653;Inherit;False;Constant;_Int1;Int 1;11;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;160;293.5985,-955.2519;Inherit;True;Global;_Udon_VideoTex;_Udon_VideoTex;11;0;Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;363.4089,114.9223;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;530.4089,153.9223;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;143;201.4089,206.9223;Inherit;False;Property;_Intensity;Intensity;10;0;Create;True;0;0;0;False;0;False;1;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;142;438.4089,-87.0777;Inherit;True;Property;_Subpixel;Subpixel;9;0;Create;True;0;0;0;False;0;False;-1;1310b96e2c47e4142a4f50423d96f285;1310b96e2c47e4142a4f50423d96f285;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;861.6496,347.5111;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;4liceD/Armband ltcgi Audiolink;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;1;LTCGI=ALWAYS;False;0;0;False;;-1;0;False;;1;Include;Assets/_pi_/_LTCGI/Shaders/LTCGI.cginc;False;;Custom;True;0;0;;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;6;0
WireConnection;9;2;12;0
WireConnection;9;1;7;0
WireConnection;43;0;74;0
WireConnection;43;2;1;4
WireConnection;43;1;45;0
WireConnection;56;0;55;0
WireConnection;56;1;58;0
WireConnection;53;0;51;0
WireConnection;53;1;86;0
WireConnection;51;0;50;0
WireConnection;51;1;52;0
WireConnection;49;0;77;2
WireConnection;49;1;48;0
WireConnection;73;0;40;0
WireConnection;73;1;72;0
WireConnection;74;0;44;0
WireConnection;72;0;55;0
WireConnection;55;0;53;0
WireConnection;83;0;159;0
WireConnection;84;0;54;0
WireConnection;84;1;93;0
WireConnection;84;2;159;0
WireConnection;85;0;54;0
WireConnection;85;1;83;0
WireConnection;86;0;84;0
WireConnection;86;1;85;0
WireConnection;80;2;82;0
WireConnection;93;0;80;0
WireConnection;105;0;104;0
WireConnection;105;1;103;0
WireConnection;110;0;109;0
WireConnection;110;1;113;0
WireConnection;110;2;114;0
WireConnection;111;0;159;0
WireConnection;1;1;9;0
WireConnection;50;0;49;0
WireConnection;124;0;58;0
WireConnection;124;1;123;0
WireConnection;124;2;125;0
WireConnection;65;0;73;0
WireConnection;65;1;56;0
WireConnection;65;2;110;0
WireConnection;65;3;124;0
WireConnection;109;5;51;0
WireConnection;109;1;105;0
WireConnection;109;2;107;0
WireConnection;109;3;108;0
WireConnection;132;18;131;0
WireConnection;132;21;127;0
WireConnection;127;0;78;0
WireConnection;17;0;1;0
WireConnection;134;0;133;0
WireConnection;134;1;132;0
WireConnection;104;2;102;1
WireConnection;123;0;1;4
WireConnection;128;0;165;0
WireConnection;128;1;65;0
WireConnection;168;0;144;0
WireConnection;168;1;129;0
WireConnection;168;2;134;0
WireConnection;129;0;132;16
WireConnection;129;1;130;0
WireConnection;135;0;160;0
WireConnection;164;0;160;0
WireConnection;165;0;164;3
WireConnection;165;1;166;0
WireConnection;165;2;135;0
WireConnection;165;3;167;0
WireConnection;141;0;128;0
WireConnection;141;1;142;0
WireConnection;144;0;141;0
WireConnection;144;1;143;0
WireConnection;0;2;168;0
WireConnection;0;3;78;0
WireConnection;0;4;78;0
WireConnection;0;10;1;4
WireConnection;0;11;43;0
ASEEND*/
//CHKSM=B881F940FC5E8FBB1B6C1D9BD3C41A6FA2D70925