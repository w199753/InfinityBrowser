Shader "InfinityPipeline/InfinityLit"
{
	Properties
	{
        [Header (Microface)]
        _AlbedoTex ("albedo Map", 2D) = "white" {}
	}

	Category
	{
		Kernel
		{
			Tags {"Name" = "Depth", "Type" = "Graphics"}
			ZWrite On ZTest LEqual Cull Back 
			ColorMask 0 

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			struct Attributes
			{
				float2 uv0 : TEXCOORD0;
				float4 vertexOS : POSITION;
			};

			struct Varyings
			{
				float2 uv0 : TEXCOORD0;
				float4 vertexCS : SV_POSITION;
			};

			cbuffer PerCamera
			{
				float4x4 matrix_VP;
			};
			cbuffer PerObject
			{
				float4x4 matrix_World;
				float4x4 matrix_Object;
			};

			#include "../Private/Common.hlsl"

			Varyings Vert(Attributes In)
			{
				Varyings Out = (Varyings)0;

				Out.uv0 = In.uv0;
				float4 WorldPos = mul(matrix_World, float4(In.vertexOS.xyz, 1.0));
				Out.vertexCS = mul(matrix_VP, WorldPos);
				return Out;
			}

			float4 Frag(Varyings In) : SV_Target
			{
				return 0;
			}
			ENDHLSL
		}

		Kernel
		{
			Tags {"Name" = "GBuffer", "Type" = "Graphics"}
			ZWrite On ZTest LEqual Cull Back 

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			struct Attributes
			{
				float2 uv0 : TEXCOORD0;
				float3 normalOS : NORMAL;
				float4 vertexOS : POSITION;
			};

			struct Varyings
			{
				float2 uv0 : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float4 vertexWS : TEXCOORD2;
				float4 vertexCS : SV_POSITION;
			};

			cbuffer PerCamera
			{
				float4x4 matrix_VP;
			};
			cbuffer PerObject
			{
				float4x4 matrix_World;
				float4x4 matrix_Object;
			};
			cbuffer PerMaterial
			{
				float _Specular;
				float4 _AlbedoColor;
			};	
			Texture2D _AlbedoTex; 
			SamplerState sampler_AlbedoTex;

			#include "../Private/Common.hlsl"
			
			Varyings Vert(Attributes In)
			{
				Varyings Out = (Varyings)0;

				Out.uv0 = In.uv0;
				Out.normalWS = normalize(mul((float3x3)matrix_World, In.normalOS));
				Out.vertexWS = mul(matrix_World, float4(In.vertexOS.xyz, 1.0));
				Out.vertexCS = mul(matrix_VP, Out.vertexWS);
				return Out;
			}
			
			void Frag(Varyings In, out float4 GBufferA : SV_Target0, out float4 GBufferB : SV_Target1)
			{
				float3 albedo = _AlbedoTex.Sample(sampler_AlbedoTex, In.uv0).rgb;

				GBufferA = float4(albedo, 1);
				GBufferB = float4((In.normalWS * 0.5 + 0.5), 1);
			}
			ENDHLSL
		}

		Kernel
		{
			Tags {"Name" = "Forward", "Type" = "Graphics"}
			ZWrite Off ZTest Equal Cull Back 

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			struct Attributes
			{
				float2 uv0 : TEXCOORD0;
				float3 normalOS : NORMAL;
				float4 vertexOS : POSITION;
			};

			struct Varyings
			{
				float2 uv0 : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float4 vertexWS : TEXCOORD2;
				float4 vertexCS : SV_POSITION;
			};

			cbuffer PerCamera
			{
				float4x4 matrix_VP;
			};
			cbuffer PerObject
			{
				float4x4 matrix_World;
				float4x4 matrix_Object;
			};
			cbuffer PerMaterial
			{
				float _Specular;
				float4 _AlbedoColor;
			};	
			Texture2D _AlbedoTex; 
			SamplerState sampler_AlbedoTex;

			#include "../Private/Common.hlsl"
			
			Varyings Vert(Attributes In)
			{
				Varyings Out = (Varyings)0;

				Out.uv0 = In.uv0;
				Out.normal = normalize(mul((float3x3)matrix_World, In.normalOS));
				Out.vertexWS = mul(matrix_World, float4(In.vertexOS.xyz, 1.0));
				Out.vertexCS = mul(matrix_VP, Out.vertexWS);
				return Out;
			}
			
			void Frag(Varyings In, out float4 Diffuse : SV_Target0, out float4 Specular : SV_Target1)
			{
				float3 worldPos = In.vertexWS.xyz;
				float3 albedo = _AlbedoTex.Sample(sampler_AlbedoTex, In.uv).rgb;

				Diffuse = float4(albedo, 1);
				Specular = float4(albedo, 1);
			}
			ENDHLSL
		}

		Kernel
		{
			Tags {"Name" = "Blur", "Type" = "Compute"}

			HLSLPROGRAM
			#pragma compute Main

			cbuffer PerDispatch
			{
				float4 Resolution;
			};	
			RWTexture2D<float4> UAV_Output;

			#include "../Private/Common.hlsl"

			[numthreads(8, 8, 1)]
			void Main(uint3 id : SV_DispatchThreadID)
			{
				UAV_Output[id.xy] = float4(id.x & id.y, (id.x & 15) / 15, (id.y & 15) / 15, 0);
			}
			ENDHLSL
		}

		Kernel
		{
			Tags {"Name" = "RTAO", "Type" = "RayTracing"}

			HLSLPROGRAM
			#pragma miss Miss
			#pragma anyHit AnyHit
			#pragma closeHit ClosestHit
			#pragma rayGeneration RayGeneration

			struct AOAttributeData
			{
				// Barycentric value of the intersection
				float2 barycentrics;
			};

			struct AORayPayload
			{
				float HitDistance;
			};

			cbuffer PerMaterial
			{
				float _Specular;
				float4 _AlbedoColor;
			};	
			RWTexture2D<float4> UAV_Output;

			#include "../Private/Common.hlsl"

			[shader("raygeneration")]
			void RayGeneration()
			{
				uint2 dispatchIdx = DispatchRaysIndex().xy;
				UAV_Output[dispatchIdx] = float4(dispatchIdx.x & dispatchIdx.y, (dispatchIdx.x & 15) / 15, (dispatchIdx.y & 15) / 15, 0);
			}

			[shader("miss")]
			void Miss(inout AORayPayload rayPayload : SV_RayPayload)
			{
				rayPayload.HitDistance = -1;
			}

			[shader("anyhit")]
			void AnyHit(inout AORayPayload rayPayload : SV_RayPayload, AOAttributeData attributeData : SV_IntersectionAttributes)
			{
				IgnoreHit();
			}

			[shader("closesthit")]
			void ClosestHit(inout AORayPayload rayPayload : SV_RayPayload, AOAttributeData attributeData : SV_IntersectionAttributes)
			{
				rayPayload.HitDistance = RayTCurrent();
				//Calculate_VertexData(FragInput);
			}
			ENDHLSL
		}
	}
}
