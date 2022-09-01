using System;

namespace Infinity.Graphics
{
    public struct RHIShaderDescriptor
    {
        public int Size;
        public IntPtr ByteCode;
        public string EntryName;
        public EShaderStage ShaderStage;
    }

    public abstract class RHIShader : Disposal
    {
        public RHIShaderDescriptor Descriptor
        {
            get
            {
                return m_Descriptor;
            }
        }

        protected RHIShaderDescriptor m_Descriptor;
    }
}
