// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "4liceD/Armband Audiolink"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_TextOutline("TextOutline", Range( 0 , 10)) = 1
		_NormalMap("Rotation", Vector) = (0.5,0,0,0)
		_EmissionMap("OutlineColor", Color) = (0,0,0,0)
		_GradientWidth("Gradient Width", Float) = 10
		_GradientSpacing("Gradient Spacing", Float) = 2
		_Channel("Audiolink Channel", Range( 0 , 3)) = 0
		_Subpixel("Subpixel", 2D) = "white" {}
		_Intensity("Intensity", Range( 0 , 10)) = 10
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0"}
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		#include "UnityShaderVariables.cginc"
		
		
		#include "Packages/com.llealloo.audiolink/Runtime/Shaders/AudioLink.cginc"
		
		struct Input
		{
			float2 uv_texcoord;
		};
		uniform float4 _EmissionMap;
		uniform sampler2D _MainTex;
		uniform float2 _NormalMap;
		uniform float4 _MainTex_ST;
		uniform float _Cutoff = 0.5;
		
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
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#include "Packages/com.llealloo.audiolink/Runtime/Shaders/AudioLink.cginc"
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Udon_VideoTex;
		float4 _Udon_VideoTex_TexelSize;
		uniform float4 _Udon_VideoTex_ST;
		uniform float _GradientWidth;
		uniform float _GradientSpacing;
		uniform float _Channel;
		uniform float4 _EmissionMap;
		uniform sampler2D _MainTex;
		uniform float2 _NormalMap;
		uniform float4 _MainTex_ST;
		uniform float _TextOutline;
		uniform sampler2D _Subpixel;
		uniform float4 _Subpixel_ST;
		uniform float _Intensity;
		uniform float _Cutoff = 0.5;


		inline float AudioLinkData3_g5( int Band, int Delay )
		{
			return AudioLinkData( ALPASS_AUDIOLINK + uint2( Delay, Band ) ).rrrr;
		}


		float IfAudioLinkv2Exists1_g10(  )
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


		inline float AudioLinkLerpMultiline1_g8( float Sample )
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
			float2 uv_Udon_VideoTex = i.uv_texcoord * _Udon_VideoTex_ST.xy + _Udon_VideoTex_ST.zw;
			float4 temp_cast_1 = 0;
			float temp_output_51_0 = ( abs( ( i.uv_texcoord.y - 0.5 ) ) * _GradientWidth );
			int Band3_g5 = (int)_Channel;
			int Delay3_g5 = 0;
			float localAudioLinkData3_g5 = AudioLinkData3_g5( Band3_g5 , Delay3_g5 );
			float localIfAudioLinkv2Exists1_g10 = IfAudioLinkv2Exists1_g10();
			float temp_output_81_0 = localIfAudioLinkv2Exists1_g10;
			float temp_output_55_0 = saturate( ( temp_output_51_0 - ( ( _GradientSpacing * ( 1.0 - localAudioLinkData3_g5 ) * temp_output_81_0 ) + ( _GradientSpacing * ( 1.0 - temp_output_81_0 ) ) ) ) );
			float4 OutlineColor74 = _EmissionMap;
			float Sample1_g8 = i.uv_texcoord.x;
			float localAudioLinkLerpMultiline1_g8 = AudioLinkLerpMultiline1_g8( Sample1_g8 );
			float isAudiolink111 = temp_output_81_0;
			float mulTime7 = _Time.y * 0.5;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner9 = ( mulTime7 * _NormalMap + uv_MainTex);
			float4 tex2DNode1 = tex2D( _MainTex, panner9 );
			float4 albedo17 = tex2DNode1;
			float2 uv_Subpixel = i.uv_texcoord * _Subpixel_ST.xy + _Subpixel_ST.zw;
			o.Emission = ( ( ( ( _Udon_VideoTex_TexelSize.z > (float)16 ? tex2D( _Udon_VideoTex, uv_Udon_VideoTex ) : temp_cast_1 ) + ( temp_output_55_0 * OutlineColor74 ) + ( ( saturate( ( 0.04 - abs( ( ( localAudioLinkLerpMultiline1_g8 + 0.5 ) - temp_output_51_0 ) ) ) ) / 0.1 ) * isAudiolink111 * OutlineColor74 ) + ( OutlineColor74 * fwidth( tex2DNode1.a ) * _TextOutline ) + ( albedo17 * ( 1.0 - temp_output_55_0 ) ) ) * tex2D( _Subpixel, uv_Subpixel ) ) * _Intensity ).rgb;
			float temp_output_78_0 = 1.0;
			o.Metallic = temp_output_78_0;
			o.Smoothness = temp_output_78_0;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.SimpleTimeNode;7;-1399.636,210.6826;Inherit;False;1;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;9;-1012.814,68.73538;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-590.6533,-11.59169;Inherit;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;43;-753.4368,433.6231;Inherit;False;1;False;Masked;0;0;Off;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1132.247,-313.9581;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;-1456.197,-683.1346;Inherit;False;17;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-1581.46,-327.0148;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1855.761,-358.2143;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;-2429.695,-470.782;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;12;-1265.814,81.73538;Inherit;False;Property;_NormalMap;Rotation;3;0;Create;False;0;0;0;False;0;False;0.5,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-777.7418,-553.8596;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;44;-1324.865,415.8023;Inherit;False;Property;_EmissionMap;OutlineColor;4;0;Create;False;0;0;0;False;0;False;0,0,0,0;0.3185747,0,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1064.663,414.5382;Inherit;False;OutlineColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;72;-1106.986,-506.62;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-1345.745,-343.4492;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1668.01,-16.80035;Inherit;True;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-2936.928,-644.0143;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;-2209.658,-171.5658;Inherit;False;Property;_GradientWidth;Gradient Width;5;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-277.1597,168.6582;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;83;-2477.742,427.7005;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-2395.137,172.2638;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-2339.222,332.3881;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-2110.472,234.5344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;80;-3162.936,358.8996;Inherit;False;4BandAmplitude;-1;;5;f5073bb9076c4e24481a28578c80bed5;0;2;2;INT;0;False;4;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;93;-2601.129,252.9306;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;104;-1327.315,-1075.207;Inherit;False;Waveform;-1;;8;86000a57e77967c4ea51f70716038ec2;0;1;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;105;-1004.395,-1048.297;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-374.8223,-867.5303;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;81;-2910.864,477.127;Inherit;False;IsAudioLink;-1;;10;e83fef6181013ba4bacf30a3d9a31d37;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;-2675.442,636.736;Inherit;False;isAudiolink;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;113;-599.1228,-737.5316;Inherit;False;111;isAudiolink;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-1357.767,-176.757;Inherit;False;74;OutlineColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-603.9132,-607.9995;Inherit;False;74;OutlineColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-349.3718,-356.3348;Inherit;True;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-839.1602,166.4553;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;dcb30f2079e56cc40acc68464f6d1635;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;50;-2088.463,-411.5143;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FWidthOpNode;123;-831.0308,-185.0558;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-663.105,-235.8313;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;109;-641.6956,-1079.886;Inherit;False;DrawLine;-1;;11;b931a6c4da53ab6489d06086e5e19048;0;4;5;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;-1159.346,-53.54928;Inherit;False;Property;_TextOutline;TextOutline;2;0;Create;True;0;0;0;False;0;False;1;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-2812.551,2.66401;Inherit;False;Property;_GradientSpacing;Gradient Spacing;6;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2702.309,-350.5145;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-999.7155,-834.1866;Inherit;False;Constant;_WaveformLineSmoothing;Waveform Line Smoothing;6;0;Create;True;0;0;0;False;0;False;0.1;0.01;0.0001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-1004.396,-932.4664;Inherit;False;Constant;_WaveformThickness;Waveform Thickness;4;0;Create;True;0;0;0;False;0;False;0.04;0.02;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-1356.565,-946.5065;Inherit;False;Constant;_WaveformHeight;Waveform Height;1;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1.577674,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;4liceD/Armband Audiolink;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3232.615,207.8469;Inherit;False;Property;_Channel;Audiolink Channel;7;0;Create;False;0;0;0;False;0;False;0;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1183.066,616.5688;Inherit;False;Constant;_MetallicGlossMap;Outline Distance;4;0;Create;False;0;0;0;False;0;False;-0.01;-0.01;-1;-0.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;102;-1700.545,-1064.676;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;126;-106.1346,-990.393;Inherit;True;Property;;;11;0;Create;False;0;0;0;True;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexelSizeNode;127;364.2717,-1112.141;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;128;647.2319,-993.0536;Inherit;True;2;4;0;FLOAT;0;False;1;INT;0;False;2;COLOR;0,0,0,0;False;3;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;129;386.2318,-875.0536;Inherit;False;Constant;_Int0;Int 0;11;0;Create;True;0;0;0;False;0;False;16;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;131;399.7545,-713.8434;Inherit;False;Constant;_Int1;Int 1;11;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;132;138.9666,-1339.83;Inherit;True;Global;_Udon_VideoTex;_Udon_VideoTex;10;0;Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;227.5332,-256.4016;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;394.5331,-217.4016;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;136;302.5333,-458.4015;Inherit;True;Property;_Subpixel;Subpixel;8;0;Create;True;0;0;0;False;0;False;-1;1310b96e2c47e4142a4f50423d96f285;1310b96e2c47e4142a4f50423d96f285;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;135;65.533,-164.4015;Inherit;False;Property;_Intensity;Intensity;9;0;Create;True;0;0;0;False;0;False;10;10;0;10;0;1;FLOAT;0
WireConnection;9;0;6;0
WireConnection;9;2;12;0
WireConnection;9;1;7;0
WireConnection;17;0;1;0
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
WireConnection;83;0;81;0
WireConnection;84;0;54;0
WireConnection;84;1;93;0
WireConnection;84;2;81;0
WireConnection;85;0;54;0
WireConnection;85;1;83;0
WireConnection;86;0;84;0
WireConnection;86;1;85;0
WireConnection;80;2;82;0
WireConnection;93;0;80;0
WireConnection;104;2;102;1
WireConnection;105;0;104;0
WireConnection;105;1;103;0
WireConnection;110;0;109;0
WireConnection;110;1;113;0
WireConnection;110;2;114;0
WireConnection;111;0;81;0
WireConnection;65;0;128;0
WireConnection;65;1;56;0
WireConnection;65;2;110;0
WireConnection;65;3;124;0
WireConnection;65;4;73;0
WireConnection;1;1;9;0
WireConnection;50;0;49;0
WireConnection;123;0;1;4
WireConnection;124;0;58;0
WireConnection;124;1;123;0
WireConnection;124;2;125;0
WireConnection;109;5;51;0
WireConnection;109;1;105;0
WireConnection;109;2;107;0
WireConnection;109;3;108;0
WireConnection;0;2;134;0
WireConnection;0;3;78;0
WireConnection;0;4;78;0
WireConnection;0;10;1;4
WireConnection;0;11;43;0
WireConnection;126;0;132;0
WireConnection;127;0;132;0
WireConnection;128;0;127;3
WireConnection;128;1;129;0
WireConnection;128;2;126;0
WireConnection;128;3;131;0
WireConnection;133;0;65;0
WireConnection;133;1;136;0
WireConnection;134;0;133;0
WireConnection;134;1;135;0
ASEEND*/
//CHKSM=63B8CABA4B148139F3685E5F1C00C85A74EC1341