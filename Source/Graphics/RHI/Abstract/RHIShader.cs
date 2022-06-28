using System;

namespace Infinity.Graphics
{
    public struct RHIShaderDescriptor
    {
        public int size;
        public IntPtr byteCode;
    }

    public abstract class RHIShader : Disposal
    {

    }
}
