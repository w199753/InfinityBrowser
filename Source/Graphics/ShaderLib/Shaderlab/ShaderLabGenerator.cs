using System;
using System.Linq;
using System.Text;
using Infinity.Container;
using System.Threading.Tasks;
using System.Collections.Generic;
using Infinity.Mathmatics;

namespace Infinity.Shaderlib
{
    public struct ShaderLabCompileResult
    {
        string Name;
        int IntParam;
        float FloatParam;
        float4 VectorParam;
    };

    public class ShaderLabGenerator : ShaderLabBaseVisitor<ShaderLabCompileResult>
    {

    }
}
