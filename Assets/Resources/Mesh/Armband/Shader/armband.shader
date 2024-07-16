// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "4liceD/Armband"
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
		_Smoothness("Reflection", Range( 0 , 1)) = 1
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
		uniform float4 _EmissionMap;
		uniform sampler2D _MainTex;
		uniform float2 _NormalMap;
		uniform float4 _MainTex_ST;
		uniform float _TextOutline;
		uniform sampler2D _Subpixel;
		uniform float4 _Subpixel_ST;
		uniform float _Intensity;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.5;

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
			float temp_output_55_0 = saturate( ( ( abs( ( i.uv_texcoord.y - 0.5 ) ) * _GradientWidth ) - _GradientSpacing ) );
			float4 OutlineColor74 = _EmissionMap;
			float mulTime7 = _Time.y * 0.5;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner9 = ( mulTime7 * _NormalMap + uv_MainTex);
			float4 tex2DNode1 = tex2D( _MainTex, panner9 );
			float4 albedo17 = tex2DNode1;
			float2 uv_Subpixel = i.uv_texcoord * _Subpixel_ST.xy + _Subpixel_ST.zw;
			o.Emission = ( ( ( ( _Udon_VideoTex_TexelSize.z > (float)16 ? tex2D( _Udon_VideoTex, uv_Udon_VideoTex ) : temp_cast_1 ) + ( temp_output_55_0 * OutlineColor74 ) + ( OutlineColor74 * fwidth( tex2DNode1.a ) * _TextOutline ) + ( albedo17 * ( 1.0 - temp_output_55_0 ) ) ) * tex2D( _Subpixel, uv_Subpixel ) ) * _Intensity ).rgb;
			float temp_output_78_0 = _Smoothness;
			o.Metallic = temp_output_78_0;
			o.Smoothness = temp_output_78_0;
			o.Alpha = 0;
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
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1132.247,-313.9581;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;-1456.197,-683.1346;Inherit;False;17;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-1581.46,-327.0148;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1855.761,-358.2143;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;50;-2088.463,-411.5143;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2702.309,-350.5145;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;-2429.695,-470.782;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;12;-1265.814,81.73538;Inherit;False;Property;_NormalMap;Rotation;3;0;Create;False;0;0;0;False;0;False;0.5,0;0.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-777.7418,-553.8596;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;44;-1324.865,415.8023;Inherit;False;Property;_EmissionMap;OutlineColor;4;0;Create;False;0;0;0;False;0;False;0,0,0,0;0.2508912,0,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1064.663,414.5382;Inherit;False;OutlineColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-839.1602,166.4553;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;e0439577e95fc96499ca1d099512aa01;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;72;-1106.986,-506.62;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-1357.767,-176.757;Inherit;False;74;OutlineColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;55;-1345.745,-343.4492;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-363.0906,-315.1784;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1668.01,-16.80035;Inherit;True;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-2936.928,-644.0143;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OutlineNode;43;-726.1303,447.2764;Inherit;False;1;False;Masked;0;0;Off;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-1653.636,265.9153;Inherit;False;TextureCoordinates;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-277.1597,168.6582;Inherit;False;Property;_Smoothness;Reflection;7;0;Create;False;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2209.658,-171.5658;Inherit;False;Property;_GradientWidth;Gradient Width;5;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1849.261,-133.3148;Inherit;False;Property;_GradientSpacing;Gradient Spacing;6;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-492.6259,170.6787;Inherit;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-622.1774,-103.1448;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-1170.096,-58.40198;Inherit;False;Property;_TextOutline;TextOutline;2;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.FWidthOpNode;79;-838.8915,-34.32098;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,-1.836729;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;4liceD/Armband;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-1183.066,616.5688;Inherit;False;Constant;_MetallicGlossMap;Outline Distance;4;0;Create;False;0;0;0;False;0;False;-0.01;-0.01;-1;-0.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;82;-307.1218,-1102.775;Inherit;True;Property;;;11;0;Create;False;0;0;0;True;0;False;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexelSizeNode;83;163.2844,-1224.523;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;84;446.2447,-1105.436;Inherit;True;2;4;0;FLOAT;0;False;1;INT;0;False;2;COLOR;0,0,0,0;False;3;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;85;185.2446,-987.436;Inherit;False;Constant;_Int0;Int 0;11;0;Create;True;0;0;0;False;0;False;16;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;86;198.7672,-826.2258;Inherit;False;Constant;_Int1;Int 1;11;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;87;-62.02063,-1452.212;Inherit;True;Global;_Udon_VideoTex;_Udon_VideoTex;10;0;Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;168.7964,-286.223;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;335.7962,-247.223;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;90;243.7965,-488.2229;Inherit;True;Property;_Subpixel;Subpixel;8;0;Create;True;0;0;0;False;0;False;-1;1310b96e2c47e4142a4f50423d96f285;1310b96e2c47e4142a4f50423d96f285;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;91;6.796154,-194.223;Inherit;False;Property;_Intensity;Intensity;9;0;Create;True;0;0;0;False;0;False;10;10;0;10;0;1;FLOAT;0
WireConnection;9;0;6;0
WireConnection;9;2;12;0
WireConnection;9;1;7;0
WireConnection;56;0;55;0
WireConnection;56;1;58;0
WireConnection;53;0;51;0
WireConnection;53;1;54;0
WireConnection;51;0;50;0
WireConnection;51;1;52;0
WireConnection;50;0;49;0
WireConnection;49;0;77;2
WireConnection;49;1;48;0
WireConnection;73;0;40;0
WireConnection;73;1;72;0
WireConnection;74;0;44;0
WireConnection;1;1;9;0
WireConnection;72;0;55;0
WireConnection;55;0;53;0
WireConnection;65;0;84;0
WireConnection;65;1;56;0
WireConnection;65;2;80;0
WireConnection;65;3;73;0
WireConnection;43;0;74;0
WireConnection;43;2;1;4
WireConnection;43;1;45;0
WireConnection;46;0;6;2
WireConnection;17;0;1;0
WireConnection;80;0;58;0
WireConnection;80;1;79;0
WireConnection;80;2;81;0
WireConnection;79;0;1;4
WireConnection;0;2;89;0
WireConnection;0;3;78;0
WireConnection;0;4;78;0
WireConnection;0;10;1;4
WireConnection;0;11;43;0
WireConnection;82;0;87;0
WireConnection;83;0;87;0
WireConnection;84;0;83;3
WireConnection;84;1;85;0
WireConnection;84;2;82;0
WireConnection;84;3;86;0
WireConnection;88;0;65;0
WireConnection;88;1;90;0
WireConnection;89;0;88;0
WireConnection;89;1;91;0
ASEEND*/
//CHKSM=2E410CA0B9988FB4E41D0D46217C2A4B1A239A12