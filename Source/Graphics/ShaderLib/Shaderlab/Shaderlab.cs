using Infinity.Mathmatics;
using System.Collections.Generic;

namespace Infinity.Shaderlib
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
}
