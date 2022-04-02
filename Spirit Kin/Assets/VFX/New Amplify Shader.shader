// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_Main("Main", 2D) = "white" {}
		_Emmission("Emmission", 2D) = "white" {}
		_Opacity("Opacity", Float) = 20
		_Noise("Noise", 2D) = "white" {}
		_Speed("Speed", Vector) = (0,0,0,0)
		_Emission("Emission", Float) = 2
		_Desaturation("Desaturation", Float) = 0
		[ASEEnd]_Color("Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float4 _Color;
				uniform sampler2D _Emmission;
				uniform float4 _Emmission_ST;
				uniform float _Desaturation;
				uniform float _Emission;
				uniform sampler2D _Main;
				uniform float4 _Main_ST;
				uniform float _Opacity;
				uniform sampler2D _Noise;
				uniform float4 _Speed;
				uniform float4 _Noise_ST;
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


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 uv_Emmission = i.texcoord.xy * _Emmission_ST.xy + _Emmission_ST.zw;
					float3 hsvTorgb40 = RGBToHSV( tex2D( _Emmission, uv_Emmission ).rgb );
					float3 hsvTorgb41 = HSVToRGB( float3(( hsvTorgb40.x + 0.0 ),hsvTorgb40.y,hsvTorgb40.z) );
					float3 desaturateInitialColor43 = hsvTorgb41;
					float desaturateDot43 = dot( desaturateInitialColor43, float3( 0.299, 0.587, 0.114 ));
					float3 desaturateVar43 = lerp( desaturateInitialColor43, desaturateDot43.xxx, _Desaturation );
					float4 _Vector1 = float4(-0.3,1,-2,1);
					float3 temp_cast_1 = (_Vector1.x).xxx;
					float3 temp_cast_2 = (_Vector1.y).xxx;
					float3 temp_cast_3 = (_Vector1.z).xxx;
					float3 temp_cast_4 = (_Vector1.w).xxx;
					float3 clampResult36 = clamp( (temp_cast_3 + (desaturateVar43 - temp_cast_1) * (temp_cast_4 - temp_cast_3) / (temp_cast_2 - temp_cast_1)) , float3( 0,0,0 ) , float3( 1,1,1 ) );
					float2 uv_Main = i.texcoord.xy * _Main_ST.xy + _Main_ST.zw;
					float clampResult5 = clamp( ( tex2D( _Main, uv_Main ).a * _Opacity ) , 0.0 , 1.0 );
					float2 appendResult23 = (float2(_Speed.z , _Speed.w));
					float4 uvs4_Noise = i.texcoord;
					uvs4_Noise.xy = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float2 panner24 = ( 1.0 * _Time.y * appendResult23 + uvs4_Noise.xy);
					float2 break26 = panner24;
					float4 texCoord30 = i.texcoord;
					texCoord30.xy = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 appendResult27 = (float2(break26.x , ( texCoord30.w + break26.y )));
					float T17 = uvs4_Noise.w;
					float W16 = uvs4_Noise.z;
					float3 _Vector0 = float3(0.07,0,1);
					float ifLocalVar12 = 0;
					if( ( tex2D( _Noise, appendResult27 ).a * T17 ) >= W16 )
					ifLocalVar12 = _Vector0.y;
					else
					ifLocalVar12 = _Vector0.z;
					float4 appendResult6 = (float4(( ( _Color * i.color ) + ( float4( clampResult36 , 0.0 ) * _Emission * i.color ) ).rgb , ( i.color.a * clampResult5 * ifLocalVar12 )));
					

					fixed4 col = appendResult6;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	
	
	
}
/*ASEBEGIN
Version=18935
16;588;1997;728;1748.107;-24.78958;1.3;True;False
Node;AmplifyShaderEditor.Vector4Node;21;-2160.929,109.7135;Float;False;Property;_Speed;Speed;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-1704.819,137.8138;Inherit;False;0;9;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;23;-1873.507,300.7087;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;32;-1198.792,-993.2949;Inherit;True;Property;_Emmission;Emmission;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RGBToHSVNode;40;-794.8835,-1015.669;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;24;-1437.488,337.0784;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;26;-1242.691,377.7575;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-1251.311,-220.6621;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-613.4026,-792.0884;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;41;-496.9925,-1012.858;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;44;-374.352,-848.8137;Float;False;Property;_Desaturation;Desaturation;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-1127.987,474.4937;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;35;-525.8674,-502.4777;Float;False;Constant;_Vector1;Vector 1;5;0;Create;True;0;0;0;False;0;False;-0.3,1,-2,1;-0.3,1,-2,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;43;-213.4294,-1002.589;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1389.204,226.8778;Float;False;T;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-1018.247,367.4284;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;33;-280.8153,-668.3783;Inherit;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;3;FLOAT3;0,0,0;False;4;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-588.6188,204.3457;Inherit;False;Property;_Opacity;Opacity;2;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-709.6539,315.5843;Inherit;True;Property;_Noise;Noise;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;19;-666.9754,666.0464;Inherit;False;17;T;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-730.4794,-24.85929;Inherit;True;Property;_Main;Main;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-1386.804,113.8777;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-321.9201,487.034;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;8;-124.8857,-345.1846;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;46;-44.08085,-1014.018;Inherit;False;Property;_Color;Color;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;13;-398.1842,679.2803;Inherit;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;0.07,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;36;101.6714,-634.2997;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-433.6899,35.35668;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-365.5232,593.8857;Inherit;False;16;W;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-245.8195,-413.461;Float;False;Property;_Emission;Emission;5;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;12;-118.7778,460.6797;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;5;-288.2826,52.88708;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;294.8057,-563.2802;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;59.27086,-786.5289;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;345.4581,-819.2755;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;235.5326,-148.2869;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-1956.149,-101.2327;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;25;-1802.945,-117.3551;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;440.293,-232.1694;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;643.0615,-237.2862;Float;False;True;-1;2;;0;9;New Amplify Shader;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;23;0;21;3
WireConnection;23;1;21;4
WireConnection;40;0;32;0
WireConnection;24;0;15;0
WireConnection;24;2;23;0
WireConnection;26;0;24;0
WireConnection;42;0;40;1
WireConnection;41;0;42;0
WireConnection;41;1;40;2
WireConnection;41;2;40;3
WireConnection;29;0;30;4
WireConnection;29;1;26;1
WireConnection;43;0;41;0
WireConnection;43;1;44;0
WireConnection;17;0;15;4
WireConnection;27;0;26;0
WireConnection;27;1;29;0
WireConnection;33;0;43;0
WireConnection;33;1;35;1
WireConnection;33;2;35;2
WireConnection;33;3;35;3
WireConnection;33;4;35;4
WireConnection;9;1;27;0
WireConnection;16;0;15;3
WireConnection;10;0;9;4
WireConnection;10;1;19;0
WireConnection;36;0;33;0
WireConnection;3;0;2;4
WireConnection;3;1;4;0
WireConnection;12;0;10;0
WireConnection;12;1;18;0
WireConnection;12;2;13;2
WireConnection;12;3;13;2
WireConnection;12;4;13;3
WireConnection;5;0;3;0
WireConnection;37;0;36;0
WireConnection;37;1;38;0
WireConnection;37;2;8;0
WireConnection;48;0;46;0
WireConnection;48;1;8;0
WireConnection;47;0;48;0
WireConnection;47;1;37;0
WireConnection;7;0;8;4
WireConnection;7;1;5;0
WireConnection;7;2;12;0
WireConnection;22;0;21;1
WireConnection;22;1;21;2
WireConnection;25;0;22;0
WireConnection;6;0;47;0
WireConnection;6;3;7;0
WireConnection;1;0;6;0
ASEEND*/
//CHKSM=53A3BF8F5554A9A395AC8D3DF60FCB9783405432