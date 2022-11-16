using System;
using System.IO;
using System.Numerics;
using Infinity.Mathmatics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

/*namespace Infinity.Shaderlib
{
    public enum EShaderlabCullMode : byte
    {
        Front = 0,
        Back = 1,
        None = 2
    };

    public enum EShaderlabComparator : byte
    {
        Less = 0,
        Greater = 1,
        LEqual = 2,
        GEqual = 3,
        NotEqual = 4,
        Always = 5,
        Never = 6
    };

    public enum EShaderlabZWriteMode : byte
    {
        On = 0,
        Off = 1
    };

    public enum EShaderlabChannel : byte
    {
        Off = 0,
        R = 1,
        G = 2,
        B = 4,
        A = 8,
    };

    public enum EShaderlabStateType : byte
    {
        CullMode = 0,
        ZTest = 1,
        ZWriteMode = 2,
        ColorMask = 3
    };

    public enum EShaderlabPropertyType
    {
        Int = 0,
        Float = 1,
        Vector = 2,
    };

    public struct ShaderlabPropertyValue
    {
        public string Name;
        public int IntParam;
        public float FloatParam;
        public float4 VectorParam;
    };

    public struct ShaderlabProperty
    {
        public float2 Range;
        public ShaderlabPropertyValue Value;
        public string ParamName;
        public string DisplayName;
        public List<string> Metas;
    };

    public struct ShaderlabCommonState
    {
        public EShaderlabCullMode CullMode;
        public EShaderlabComparator ZTestMode;
        public EShaderlabZWriteMode ZWriteMode;
        public EShaderlabChannel ColorMaskChannel;
    };

    // Same as Pass in unity
    public struct ShaderlabKernel
    {
        public string HlslCode;
        public ShaderlabCommonState CommonState;
        public Dictionary<string, string> Tags;
    };

    // Same as SubShader in unity, we assusme there is always only one category in one shader file
    public struct ShaderlabCategory
    {
        public List<ShaderlabKernel> Kernels;
        public Dictionary<string, string> Tags;
    };

    // ShaderLab is designed like unity shaderlab
    public struct Shaderlab
    {
        public string Name;
        public ShaderlabCategory Category;
        public uint PropertyCapacity;
        public List<uint> Offsets;
        public List<ShaderlabProperty> Properties;
    };
}*/

namespace Infinity.Shaderlib
{
    public enum EShaderlabBlendOp
    {
        BlendOpAdd = 0,
        BlendOpSub,
        BlendOpRevSub,
        BlendOpMin,
        BlendOpMax,
        BlendOpLogicalClear,
        BlendOpLogicalSet,
        BlendOpLogicalCopy,
        BlendOpLogicalCopyInverted,
        BlendOpLogicalNoop,
        BlendOpLogicalInvert,
        BlendOpLogicalAnd,
        BlendOpLogicalNand,
        BlendOpLogicalOr,
        BlendOpLogicalNor,
        BlendOpLogicalXor,
        BlendOpLogicalEquiv,
        BlendOpLogicalAndReverse,
        BlendOpLogicalAndInverted,
        BlendOpLogicalOrReverse,
        BlendOpLogicalOrInverted,
        Max,
    };

    public enum EShaderlabCullMode
    {
        Unknown = -1,
        CullOff = 0,
        CullFront,
        CullBack,
        CullFrontAndBack,
        Max
    };

    public enum EShaderlabStencilOp
    {
        StencilOpKeep = 0,
        StencilOpZero,
        StencilOpReplace,
        StencilOpIncrSat,
        StencilOpDecrSat,
        StencilOpInvert,
        StencilOpIncrWrap,
        StencilOpDecrWrap,
        Max
    };

    public enum EShaderlabBlendMode
    {
        BlendZero = 0,
        BlendOne,
        BlendDstColor,
        BlendSrcColor,
        BlendOneMinusDstColor,
        BlendSrcAlpha,
        BlendOneMinusSrcColor,
        BlendDstAlpha,
        BlendOneMinusDstAlpha,
        BlendSrcAlphaSaturate,
        BlendOneMinusSrcAlpha,
        Max
    };

    public enum EShaderlabShaderStage
    {
        ProgramVertex = 0,
        ProgramFragment,
        ProgramMesh,
        ProgramTask,
        ProgramCompute,
        ProgramRayGen,
        ProgramRayInt,
        ProgramRayAHit,
        ProgramRayCHit,
        ProgramRayMiss,
        ProgramRayRcall,
        Max
    };

    public enum EShaderlabPropertyType
    {
        Int,
        Float,
        Range,
        Color,
        Vector,
        Texture,
    };

    public enum EShaderlabProgramType
    {
        HLSL = 0,
        Max
    };

    public enum EShaderlabShaderTarget
    {
        ShaderTargetOpenGL = 0,
        ShaderTargetGLES20,
        ShaderTargetGLES30,
        ShaderTargetMetalIOS,
        ShaderTargetMetalMac,
        ShaderTargetHLSL,
        ShaderTargetVulkan,
        Max
    };

    public enum EShaderlabColorWriteMask
    {
        ColorWriteNone = 0,
        ColorWriteA = 1,
        ColorWriteB = 2,
        ColorWriteG = 4,
        ColorWriteR = 8,
        ColorWriteAll = ColorWriteR | ColorWriteG | ColorWriteB | ColorWriteA
    };

    public enum EShaderlabCompareFunction
    {
        FuncUnknown = -1,
        FuncDisabled = 0,
        FuncNever,
        FuncLess,
        FuncEqual,
        FuncLEqual,
        FuncGreater,
        FuncNotEqual,
        FuncGEqual,
        FuncAlways,
        Max
    };

    public enum EShaderlabTextureDimension
    {
        Unknown = 0,
        Tex2D,
        TexDArray,
        Tex3D,
        TexCube,
        Max
    };

    public struct Shaderlab : IDisposable
    {
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public ShaderlabCategory Category
        {
            get { return m_Category; }
            set { m_Category = value; }
        }
        public List<ShaderlabProperties> Properties
        {
            get { return m_Properties; }
            set { m_Properties = value; }
        }

        private string m_Name;
        private ShaderlabCategory m_Category;
        private List<ShaderlabProperties> m_Properties;

        public void Dispose()
        {
            m_Category.Dispose();
        }
    }

    public struct ShaderlabPass
    {
        public string? Name
        {
            get 
            {
                Tags.TryGetValue("Name", out string? name);
                return name;
            }
        }
        public ShaderlabProgram Program
        {
            get { return m_Program; }
            set { m_Program = value; }
        }
        public ShaderlabRenderState? State
        {
            get { return m_State; }
            set { m_State = value; }
        }
        public Dictionary<string, string> Tags
        {
            get { return m_Tags; }
            set { m_Tags = value; }
        }

        private ShaderlabProgram m_Program;
        private ShaderlabRenderState? m_State;
        private Dictionary<string, string> m_Tags;
    };

    public struct ShaderlabProgram
    {
        public string Source
        {
            get { return m_Source; }
            set { m_Source = value; }
        }
        public EShaderlabProgramType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        private string m_Source;
        private EShaderlabProgramType m_Type;
    };

    public struct ShaderlabCategory : IDisposable
    {
        public List<ShaderlabPass> Passes
        {
            get { return m_Passes; }
            set { m_Passes = value; }
        }
        public Dictionary<string, string>? Tags
        {
            get { return m_Tags; }
            set { m_Tags = value; }
        }

        private List<ShaderlabPass> m_Passes;
        private Dictionary<string, string>? m_Tags;

        public void Dispose()
        {

        }
    };

    public struct ShaderlabStencilOp
    {
        public ShaderlabFloatProperty Comp
        {
            get { return m_Comp; }
            set { m_Comp = value; }
        }
        public ShaderlabFloatProperty Pass
        {
            get { return m_Pass; }
            set { m_Pass = value; }
        }
        public ShaderlabFloatProperty Fail
        {
            get { return m_Fail; }
            set { m_Fail = value; }
        }
        public ShaderlabFloatProperty ZFail
        {
            get { return m_ZFail; }
            set { m_ZFail = value; }
        }

        private ShaderlabFloatProperty m_Comp;
        private ShaderlabFloatProperty m_Pass;
        private ShaderlabFloatProperty m_Fail;
        private ShaderlabFloatProperty m_ZFail;
    };

    public struct ShaderlabProperties
    {
        public string DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; }
        }
        public string PropertyName
        {
            get { return m_PropertyName; }
            set { m_PropertyName = value; }
        }
        public List<string>? Attributes
        {
            get { return m_Attributes; }
            set { m_Attributes = value; }
        }
        public EShaderlabPropertyType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        public Vector4? ValueProperty
        {
            get { return m_ValueProperty; }
            set { m_ValueProperty = value; }
        }
        public ShaderlabTextureProperty? TextureProperty
        {
            get { return m_TextureProperty; }
            set { m_TextureProperty = value; }
        }

        private string m_DisplayName;
        private string m_PropertyName;
        private List<string>? m_Attributes;
        private EShaderlabPropertyType m_Type;
        private Vector4? m_ValueProperty;
        private ShaderlabTextureProperty? m_TextureProperty;

        public ShaderlabProperties(string displayName, string propertyName, List<string>? attributes, in EShaderlabPropertyType type, in Vector4 valueProperty)
        {
            m_DisplayName = displayName;
            m_PropertyName = propertyName;
            m_Attributes = attributes;
            m_Type = type;
            m_ValueProperty = valueProperty;
            m_TextureProperty = null;
        }

        public ShaderlabProperties(string displayName, string propertyName, List<string>? attributes, in EShaderlabPropertyType type, in ShaderlabTextureProperty textureProperty)
        {
            m_DisplayName = displayName;
            m_PropertyName = propertyName;
            m_Attributes = attributes;
            m_Type = type;
            m_ValueProperty = null;
            m_TextureProperty = textureProperty;
        }
    };

    public struct ShaderlabRenderState
    {
        public ShaderlabFloatProperty? ColorMask
        {
            get { return m_ColorMask; }
            set { m_ColorMask = value; }
        }
        public ShaderlabFloatProperty? AlphaToMask
        {
            get { return m_AlphaToMask; }
            set { m_AlphaToMask = value; }
        }
        public ShaderlabFloatProperty? OffsetFactor
        {
            get { return m_OffsetFactor; }
            set { m_OffsetFactor = value; }
        }
        public ShaderlabFloatProperty? OffsetUnits
        {
            get { return m_OffsetUnits; }
            set { m_OffsetUnits = value; }
        }
        public int? Cull
        {
            get { return m_Cull; }
            set { m_Cull = value; }
        }
        public int? ZTest
        {
            get { return m_ZTest; }
            set { m_ZTest = value; }
        }
        public int? ZWrite
        {
            get { return m_ZWrite; }
            set { m_ZWrite = value; }
        }
        public ShaderlabFloatProperty? BlendOp
        {
            get { return m_BlendOp; }
            set { m_BlendOp = value; }
        }
        public ShaderlabFloatProperty? BlendOpAlpha
        {
            get { return m_BlendOpAlpha; }
            set { m_BlendOpAlpha = value; }
        }
        public ShaderlabFloatProperty? SrcBlend
        {
            get { return m_SrcBlend; }
            set { m_SrcBlend = value; }
        }
        public ShaderlabFloatProperty? DstBlend
        {
            get { return m_DstBlend; }
            set { m_DstBlend = value; }
        }
        public ShaderlabFloatProperty? SrcBlendAlpha
        {
            get { return m_SrcBlendAlpha; }
            set { m_SrcBlendAlpha = value; }
        }
        public ShaderlabFloatProperty? DstBlendAlpha
        {
            get { return m_DstBlendAlpha; }
            set { m_DstBlendAlpha = value; }
        }
        public ShaderlabFloatProperty? StencilRef
        {
            get { return m_StencilRef; }
            set { m_StencilRef = value; }
        }
        public ShaderlabFloatProperty? StencilReadMask
        {
            get { return m_StencilReadMask; }
            set { m_StencilReadMask = value; }
        }
        public ShaderlabFloatProperty? StencilWriteMask
        {
            get { return m_StencilWriteMask; }
            set { m_StencilWriteMask = value; }
        }
        public ShaderlabStencilOp? StencilOp
        {
            get { return m_StencilOp; }
            set { m_StencilOp = value; }
        }
        public ShaderlabStencilOp? StencilOpBack
        {
            get { return m_StencilOpBack; }
            set { m_StencilOpBack = value; }
        }
        public ShaderlabStencilOp? StencilOpFront
        {
            get { return m_StencilOpFront; }
            set { m_StencilOpFront = value; }
        }

        private ShaderlabFloatProperty? m_ColorMask;
        private ShaderlabFloatProperty? m_AlphaToMask;
        private ShaderlabFloatProperty? m_OffsetFactor;
        private ShaderlabFloatProperty? m_OffsetUnits;
        private int? m_Cull;
        private int? m_ZTest;
        private int? m_ZWrite;
        private ShaderlabFloatProperty? m_BlendOp;
        private ShaderlabFloatProperty? m_BlendOpAlpha;
        private ShaderlabFloatProperty? m_SrcBlend;
        private ShaderlabFloatProperty? m_DstBlend;
        private ShaderlabFloatProperty? m_SrcBlendAlpha;
        private ShaderlabFloatProperty? m_DstBlendAlpha;
        private ShaderlabFloatProperty? m_StencilRef;
        private ShaderlabFloatProperty? m_StencilReadMask;
        private ShaderlabFloatProperty? m_StencilWriteMask;
        private ShaderlabStencilOp? m_StencilOp;
        private ShaderlabStencilOp? m_StencilOpBack;
        private ShaderlabStencilOp? m_StencilOpFront;

        public ShaderlabRenderState(in int cull, in int zTest, in int zWrite)
        {
            //this = new ShaderlabRenderState();
            m_Cull = cull;
            m_ZTest = zTest;
            m_ZWrite = zWrite;
        }
    };

    public struct ShaderlabFloatProperty
    {
        public float Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private float m_Value;
        private string m_Name;
    };

    public struct ShaderlabVectorProperty
    {
        public float X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public float Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        public float Z
        {
            get { return m_Z; }
            set { m_Z = value; }
        }
        public float W
        {
            get { return m_W; }
            set { m_W = value; }
        }
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private float m_X;
        private float m_Y;
        private float m_Z;
        private float m_W;
        private string m_Name;
    };

    public struct ShaderlabTextureProperty
    {
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public EShaderlabTextureDimension Dimension
        {
            get { return m_Dimension; }
            set { m_Dimension = value; }
        }

        private string m_Name;
        private EShaderlabTextureDimension m_Dimension;

        public ShaderlabTextureProperty(string name, in EShaderlabTextureDimension dimension)
        {
            m_Name = name;
            Dimension= dimension;
        }
    };

    public static class ShaderlabUtility
    {
        public static Shaderlab ParseShaderlabFromFile(string filePath)
        {
            //string source = File.ReadAllText(filePath);
            //return ParseShaderlabFromSource(source);

            List<ShaderlabPass> passes = new List<ShaderlabPass>();
            {
                ShaderlabPass pass0 = new ShaderlabPass();
                {
                    pass0.Tags = new Dictionary<string, string>(2);
                    pass0.Tags.TryAdd("Name", "Depth");
                    pass0.Tags.TryAdd("Type", "Graphics");
                    pass0.State = new ShaderlabRenderState(0, 1, 1);
                    pass0.Program = new ShaderlabProgram
                    {
                        Type = EShaderlabProgramType.HLSL,
                        Source = new string(@"
			            #include ""../Private/Common.hlsl""

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

			            cbuffer PerCamera : register(b0, space0);
			            {
				            float4x4 matrix_VP;
			            };
			            cbuffer PerObject : register(b1, space0);
			            {
				            float4x4 matrix_World;
				            float4x4 matrix_Object;
			            };
			            cbuffer PerMaterial : register(b2, space0);
			            {
				            int _IntValue;
				            float _SpecularValue;
				            float _MetallicValue;
				            float _RoughnessValue;
				            float4 _AlbedoColor;
			            };	
			            Texture2D _AlbedoMap : register(t0, space0); 
			            SamplerState sampler_AlbedoMap : register(s0, space0);

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
			            }")
                    };
                    passes.Add(pass0);
                }

                ShaderlabPass pass1 = new ShaderlabPass();
                {
                    pass1.Tags = new Dictionary<string, string>(2);
                    pass1.Tags.TryAdd("Name", "GBuffer");
                    pass1.Tags.TryAdd("Type", "Graphics");
                    pass1.State = new ShaderlabRenderState(0, 1, 1);
                    pass1.Program = new ShaderlabProgram
                    {
                        Type = EShaderlabProgramType.HLSL,
                        Source = new string(@"
			            #include ""../Private/Common.hlsl""

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

			            cbuffer PerCamera : register(b0, space0);
			            {
				            float4x4 matrix_VP;
			            };
			            cbuffer PerObject : register(b1, space0);
			            {
				            float4x4 matrix_World;
				            float4x4 matrix_Object;
			            };
			            cbuffer PerMaterial : register(b2, space0);
			            {
				            int _IntValue;
				            float _SpecularValue;
				            float _MetallicValue;
				            float _RoughnessValue;
				            float4 _AlbedoColor;
			            };	
			            Texture2D _AlbedoMap : register(t0, space0); 
			            SamplerState sampler_AlbedoMap : register(s0, space0);

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
				            float3 albedo = _AlbedoMap.Sample(sampler_AlbedoMap, In.uv0).rgb;

				            GBufferA = float4(albedo, 1);
				            GBufferB = float4((In.normalWS * 0.5 + 0.5), 1);
			            }")
                    };
                    passes.Add(pass1);
                }

                ShaderlabPass pass2 = new ShaderlabPass();
                {
                    pass2.Tags = new Dictionary<string, string>(2);
                    pass2.Tags.TryAdd("Name", "Forward");
                    pass2.Tags.TryAdd("Type", "Graphics");
                    pass2.State = new ShaderlabRenderState(0, 2, 0);
                    pass2.Program = new ShaderlabProgram
                    {
                        Type = EShaderlabProgramType.HLSL,
                        Source = new string(@"
			            #include ""../Private/Common.hlsl""

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

			            cbuffer PerCamera : register(b0, space0);
			            {
				            float4x4 matrix_VP;
			            };
			            cbuffer PerObject : register(b1, space0);
			            {
				            float4x4 matrix_World;
				            float4x4 matrix_Object;
			            };
			            cbuffer PerMaterial : register(b2, space0);
			            {
				            int _IntValue;
				            float _SpecularValue;
				            float _MetallicValue;
				            float _RoughnessValue;
				            float4 _AlbedoColor;
			            };	
			            Texture2D _AlbedoMap : register(t0, space0); 
			            SamplerState sampler_AlbedoMap : register(s0, space0);

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
				            float3 albedo = _AlbedoMap.Sample(sampler_AlbedoMap, In.uv).rgb;

				            Diffuse = float4(albedo, 1);
				            Specular = float4(albedo, 1);
			            }")
                    };
                    passes.Add(pass2);
                }

                ShaderlabPass pass3 = new ShaderlabPass();
                {
                    pass3.Tags = new Dictionary<string, string>(2);
                    pass3.Tags.TryAdd("Name", "IndexWrite");
                    pass3.Tags.TryAdd("Type", "Compute");
                    pass3.Program = new ShaderlabProgram
                    {
                        Type = EShaderlabProgramType.HLSL,
                        Source = new string(@"
			            #include ""../Private/Common.hlsl""

			            cbuffer PerDispatch : register(b0, space0);
			            {
				            float4 Resolution;
			            };	
			            RWTexture2D<float4> UAV_Output : register(u0, space0);

			            [numthreads(8, 8, 1)]
			            void Main(uint3 id : SV_DispatchThreadID)
			            {
				            UAV_Output[id.xy] = float4(id.x & id.y, (id.x & 15) / 15, (id.y & 15) / 15, 0);
			            }")
                    };
                    passes.Add(pass3);
                }

                ShaderlabPass pass4 = new ShaderlabPass();
                {
                    pass4.Tags = new Dictionary<string, string>(2);
                    pass4.Tags.TryAdd("Name", "RTAORayGen");
                    pass4.Tags.TryAdd("Type", "RayTracing");
                    pass4.Program = new ShaderlabProgram
                    {
                        Type = EShaderlabProgramType.HLSL,
                        Source = new string(@"
			            #include ""../Private/Common.hlsl""

			            struct AORayPayload
			            {
				            float HitDistance;
			            };

			            struct AOAttributeData
			            {
				            // Barycentric value of the intersection
				            float2 barycentrics;
			            };

			            cbuffer PerMaterial : register(b0, space0);
			            {
				            float _Specular;
				            float4 _AlbedoColor;
			            };
			            cbuffer PerDispatch : register(b1, space0);
			            {
				            float4 Resolution;
			            };
			            RWTexture2D<float4> UAV_Output;

			            [shader(""raygeneration"")]
			            void RayGeneration()
			            {
				            uint2 dispatchIdx = DispatchRaysIndex().xy;
				            uint2 launchDim   = DispatchRaysDimensions().xy;
				            float2 uv = dispatchIdx * Resolution.zw;

				            RayDesc rayDescriptor;
				            rayDescriptor.TMin      = 0;
				            rayDescriptor.TMax      = RTAO_Radius;
				            rayDescriptor.Origin    = float3(0, 0, 0);
				            rayDescriptor.Direction = float3(0, 0, 0);

        		            AORayPayload rayPayLoad;
        		            TraceRay(_RaytracingSceneStruct, RAY_FLAG_CULL_BACK_FACING_TRIANGLES, 0xFF, 0, 1, 0, rayDescriptor, rayPayLoad);

				            UAV_Output[dispatchIdx] = RayIntersectionAO.HitDistance < 0 ? 1 : 0;
			            }")
                    };
                    passes.Add(pass4);
                }

                ShaderlabPass pass5 = new ShaderlabPass();
                {
                    pass5.Tags = new Dictionary<string, string>(2);
                    pass5.Tags.TryAdd("Name", "RTAOHitGroup");
                    pass5.Tags.TryAdd("Type", "RayTracing");
                    pass5.Program = new ShaderlabProgram
                    {
                        Type = EShaderlabProgramType.HLSL,
                        Source = new string(@"
			            #include ""../Private/Common.hlsl""

			            struct AORayPayload
			            {
				            float HitDistance;
			            };

			            struct AOAttributeData
			            {
				            // Barycentric value of the intersection
				            float2 barycentrics;
			            };

			            cbuffer PerMaterial : register(b0, space0);
			            {
				            float _Specular;
				            float4 _AlbedoColor;
			            };	

			            [shader(""miss"")]
			            void Miss(inout AORayPayload rayPayload : SV_RayPayload)
			            {
				            rayPayload.HitDistance = -1;
			            }

			            [shader(""anyhit"")]
			            void AnyHit(inout AORayPayload rayPayload : SV_RayPayload, AOAttributeData attributeData : SV_IntersectionAttributes)
			            {
				            IgnoreHit();
			            }

			            [shader(""closesthit"")]
			            void ClosestHit(inout AORayPayload rayPayload : SV_RayPayload, AOAttributeData attributeData : SV_IntersectionAttributes)
			            {
				            rayPayload.HitDistance = RayTCurrent();
				            //CalculateVertexData(FragInput);
			            }")
                    };
                    passes.Add(pass5);
                }
            }

            ShaderlabCategory category = new ShaderlabCategory();
            {
                category.Passes = passes;
                category.Tags = new Dictionary<string, string>(3);
                category.Tags.TryAdd("Queue", "Geometry");
                category.Tags.TryAdd("RenderType", "Opaque");
                category.Tags.TryAdd("RenderPipeline", "InfinityRenderPipeline");
            }

            List<ShaderlabProperties> properties = new List<ShaderlabProperties>(6)
            {
                new ShaderlabProperties("AlbedoMap", "_AlbedoMap", null, EShaderlabPropertyType.Texture, new ShaderlabTextureProperty("white", EShaderlabTextureDimension.Tex2D)),
                new ShaderlabProperties("AlbedoColor", "_AlbedoColor", null, EShaderlabPropertyType.Color, new Vector4(1, 0.25f, 0.2f, 1)),
                new ShaderlabProperties("IntValue", "_IntValue", null, EShaderlabPropertyType.Int, new Vector4(233, 0, 0, 0)),
                new ShaderlabProperties("SpecularValue", "_SpecularValue", null, EShaderlabPropertyType.Range, new Vector4(0.5f, 0, 1, 0)),
                new ShaderlabProperties("MetallicValue", "_MetallicValue", null, EShaderlabPropertyType.Range, new Vector4(1, 0, 1, 0)),
                new ShaderlabProperties("RoughnessValue", "_RoughnessValue", null, EShaderlabPropertyType.Range, new Vector4(0.66f, 0, 1, 0)),
            };

            Shaderlab shaderLab = new Shaderlab();
            shaderLab.Name = "InfinityPipeline/InfinityLit";
            shaderLab.Category = category;
            shaderLab.Properties = properties;

            return shaderLab;
        }

        public static Shaderlab ParseShaderlabFromSource(string source)
        {
            Shaderlab shaderlab = new Shaderlab();
            shaderlab.Name = ParseShaderlabName(source);
            shaderlab.Category = ParseShaderlabCategory(source);
            shaderlab.Properties = ParseShaderlabProperties(source);
            return shaderlab;
        }

        internal static string ParseShaderlabName(string source)
        {
            Regex regex = new Regex(@"Shader "".+""");
            foreach (Match match in regex.Matches(source))
            {
                int count = match.Value.Length;
                return match.Value.Substring(8, count - 9);
            }
            throw new NotImplementedException("Shaderlab name is illegal");
        }

        internal static ShaderlabCategory ParseShaderlabCategory(string source)
        {
            throw new NotImplementedException("Shaderlab Category is illegal");
        }

        internal static List<ShaderlabProperties> ParseShaderlabProperties(string source)
        {
            throw new NotImplementedException("Shaderlab Properties is illegal");
        }
    }
}