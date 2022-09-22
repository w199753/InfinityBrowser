using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Shaderlib
{
    enum ECullMode : byte
    {
        Front       = 0,
        Back        = 1,
        None        = 2
    };

    enum EComparator : byte
    {
        Less        = 0,
        Greater     = 1,
        LEqual      = 2,
        GEqual      = 3,
        NotEqual    = 4,
        Always      = 5,
        Never       = 6
    };

    enum EZWriteMode : byte
    {
        On          = 0,
        Off         = 1
    };

    enum EChannel : byte
    {
        Off         = 0,
        R           = 1,
        G           = 2,
        B           = 4,
        A           = 8,
    };

    enum EStateType : byte
    {
        CullMode    = 0,
        ZTest       = 1,
        ZWriteMode  = 2,
        ColorMask   = 3
    };

    /*struct Property
    {
        std::string name;
        std::string displayName;
        std::vector<std::string> metas;
        std::pair<float, float> range;
        std::variant<int, float, Vec4, std::string>; value;
    };*/

    struct CommonState
    {
        ECullMode cullMode;
        EComparator zTestMode;
        EZWriteMode zWriteMode;
        EChannel colorMaskChannel;
    };

    // Same as Pass in unity
    struct Kernel
    {
        string hlsl;
        CommonState commonState;
        Dictionary<string, string> tags;
    };

    // Same as SubShader in unity, we assusme there is always only one category in one shader file
    struct Category
    {
        List<Kernel> kernels;
        Dictionary<string, string> tags;
    };

    // ShaderLab is designed like unity shaderlab
    struct ShaderLab
    {
        string name;
        Category category;
        uint propertyCapacity;
        List<uint> offsets;
        //List<Property> properties;
    };
}
