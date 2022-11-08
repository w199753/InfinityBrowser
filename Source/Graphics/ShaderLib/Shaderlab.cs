using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using TerraFX.Interop.Windows;
using System.Text.RegularExpressions;

namespace Infinity.Shaderlab
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
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
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

        private string m_Name;
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
        public Dictionary<string, string> Tags
        {
            get { return m_Tags; }
            set { m_Tags = value; }
        }

        private List<ShaderlabPass> m_Passes;
        private Dictionary<string, string> m_Tags;

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
        public string PropertyName
        {
            get { return m_PropertyName; }
            set { m_PropertyName = value; }
        }
        public string DescriptionName
        {
            get { return m_DescriptionName; }
            set { m_DescriptionName = value; }
        }
        public List<string> Attributes
        {
            get { return m_Attributes; }
            set { m_Attributes = value; }
        }
        public EShaderlabPropertyType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        public Vector4? FloatValue
        {
            get { return m_FloatValue; }
            set { m_FloatValue = value; }
        }
        public Vector4? RangeValue
        {
            get { return m_RangeValue; }
            set { m_RangeValue = value; }
        }
        public Vector4? ColorValue
        {
            get { return m_ColorValue; }
            set { m_ColorValue = value; }
        }
        public Vector4? VectorValue
        {
            get { return m_VectorValue; }
            set { m_VectorValue = value; }
        }
        public ShaderlabTextureProperty? TextureValue
        {
            get { return m_TextureValue; }
            set { m_TextureValue = value; }
        }

        private string m_PropertyName;
        private string m_DescriptionName;
        private List<string> m_Attributes;
        private EShaderlabPropertyType m_Type;
        private Vector4? m_FloatValue;
        private Vector4? m_RangeValue;
        private Vector4? m_ColorValue;
        private Vector4? m_VectorValue;
        private ShaderlabTextureProperty? m_TextureValue;
    };

    public struct ShaderlabRenderState
    {
        public ShaderlabFloatProperty ColorMask
        {
            get { return m_ColorMask; }
            set { m_ColorMask = value; }
        }
        public ShaderlabFloatProperty AlphaToMask
        {
            get { return m_AlphaToMask; }
            set { m_AlphaToMask = value; }
        }
        public ShaderlabFloatProperty OffsetFactor
        {
            get { return m_OffsetFactor; }
            set { m_OffsetFactor = value; }
        }
        public ShaderlabFloatProperty OffsetUnits
        {
            get { return m_OffsetUnits; }
            set { m_OffsetUnits = value; }
        }
        public ShaderlabFloatProperty ZTest
        {
            get { return m_ZTest; }
            set { m_ZTest = value; }
        }
        public ShaderlabFloatProperty ZWrite
        {
            get { return m_ZWrite; }
            set { m_ZWrite = value; }
        }
        public ShaderlabFloatProperty Culling
        {
            get { return m_Culling; }
            set { m_Culling = value; }
        }
        public ShaderlabFloatProperty BlendOp
        {
            get { return m_BlendOp; }
            set { m_BlendOp = value; }
        }
        public ShaderlabFloatProperty BlendOpAlpha
        {
            get { return m_BlendOpAlpha; }
            set { m_BlendOpAlpha = value; }
        }
        public ShaderlabFloatProperty SrcBlend
        {
            get { return m_SrcBlend; }
            set { m_SrcBlend = value; }
        }
        public ShaderlabFloatProperty DstBlend
        {
            get { return m_DstBlend; }
            set { m_DstBlend = value; }
        }
        public ShaderlabFloatProperty SrcBlendAlpha
        {
            get { return m_SrcBlendAlpha; }
            set { m_SrcBlendAlpha = value; }
        }
        public ShaderlabFloatProperty DstBlendAlpha
        {
            get { return m_DstBlendAlpha; }
            set { m_DstBlendAlpha = value; }
        }
        public ShaderlabFloatProperty StencilRef
        {
            get { return m_StencilRef; }
            set { m_StencilRef = value; }
        }
        public ShaderlabFloatProperty StencilReadMask
        {
            get { return m_StencilReadMask; }
            set { m_StencilReadMask = value; }
        }
        public ShaderlabFloatProperty StencilWriteMask
        {
            get { return m_StencilWriteMask; }
            set { m_StencilWriteMask = value; }
        }
        public ShaderlabStencilOp StencilOp
        {
            get { return m_StencilOp; }
            set { m_StencilOp = value; }
        }
        public ShaderlabStencilOp StencilOpBack
        {
            get { return m_StencilOpBack; }
            set { m_StencilOpBack = value; }
        }
        public ShaderlabStencilOp StencilOpFront
        {
            get { return m_StencilOpFront; }
            set { m_StencilOpFront = value; }
        }

        private ShaderlabFloatProperty m_ColorMask;
        private ShaderlabFloatProperty m_AlphaToMask;
        private ShaderlabFloatProperty m_OffsetFactor;
        private ShaderlabFloatProperty m_OffsetUnits;
        private ShaderlabFloatProperty m_ZTest;
        private ShaderlabFloatProperty m_ZWrite;
        private ShaderlabFloatProperty m_Culling;
        private ShaderlabFloatProperty m_BlendOp;
        private ShaderlabFloatProperty m_BlendOpAlpha;
        private ShaderlabFloatProperty m_SrcBlend;
        private ShaderlabFloatProperty m_DstBlend;
        private ShaderlabFloatProperty m_SrcBlendAlpha;
        private ShaderlabFloatProperty m_DstBlendAlpha;
        private ShaderlabFloatProperty m_StencilRef;
        private ShaderlabFloatProperty m_StencilReadMask;
        private ShaderlabFloatProperty m_StencilWriteMask;
        private ShaderlabStencilOp m_StencilOp;
        private ShaderlabStencilOp m_StencilOpBack;
        private ShaderlabStencilOp m_StencilOpFront;
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
    };

    public static class ShaderlabUtility
    {
        public static Shaderlab ParseShaderlabFromFile(string filePath)
        {
            string source = File.ReadAllText(filePath);
            return ParseShaderlabFromSource(source);
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
            foreach(Match match in regex.Matches(source))
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
