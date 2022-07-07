using System;

namespace Infinity.Graphics
{
    public struct RHIShaderDescriptor
    {
        public int size;
        public IntPtr byteCode;
        public string entryName;
        public EShaderStage shaderStage;
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
