using System;

namespace Infinity.Graphics
{
    public struct RHIShaderCreateInfo
    {
        public int size;
        public IntPtr byteCode;
    }

    public abstract class RHIShader : Disposal
    {

    }
}
