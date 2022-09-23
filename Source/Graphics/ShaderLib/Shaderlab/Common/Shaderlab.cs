using Infinity.Mathmatics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Shaderlib
{
    public enum EShaderlabCullMode : byte
    {
        Front       = 0,
        Back        = 1,
        None        = 2
    };

    public enum EShaderlabComparator : byte
    {
        Less        = 0,
        Greater     = 1,
        LEqual      = 2,
        GEqual      = 3,
        NotEqual    = 4,
        Always      = 5,
        Never       = 6
    };

    public enum EShaderlabZWriteMode : byte
    {
        On          = 0,
        Off         = 1
    };

    public enum EShaderlabChannel : byte
    {
        Off         = 0,
        R           = 1,
        G           = 2,
        B           = 4,
        A           = 8,
    };

    public enum EShaderlabStateType : byte
    {
        CullMode    = 0,
        ZTest       = 1,
        ZWriteMode  = 2,
        ColorMask   = 3
    };

    public enum EShaderlabPropertyType
    {
        Int = 0,
        Float = 1,
        Vector = 2,
    };

    public struct PropertyValue
    {
        string Name;
        int IntParam;
        float FloatParam;
        float4 VectorParam;
    };

    public struct Property
    {
        float2 Range;
        PropertyValue Value;
        string ParamName;
        string DisplayName;
        List<string> Metas;
    };

    public struct CommonState
    {
        EShaderlabCullMode CullMode;
        EShaderlabComparator ZTestMode;
        EShaderlabZWriteMode ZWriteMode;
        EShaderlabChannel ColorMaskChannel;
    };

    // Same as Pass in unity
    public struct Kernel
    {
        string HlslCode;
        CommonState CommonState;
        Dictionary<string, string> Tags;
    };

    // Same as SubShader in unity, we assusme there is always only one category in one shader file
    public struct Category
    {
        List<Kernel> Kernels;
        Dictionary<string, string> Tags;
    };

    // ShaderLab is designed like unity shaderlab
    public struct ShaderLab
    {
        string Name;
        Category Category;
        uint PropertyCapacity;
        List<uint> Offsets;
        List<Property> Properties;
    };
}
