using System;

namespace Infinity.Graphics
{
    public struct RHIFunctionDescriptor
    {
        public int Size;
        public IntPtr ByteCode;
        public string EntryName;
        public EFunctionStage FunctionStage;
    }

    public abstract class RHIFunction : Disposal
    {
        public RHIFunctionDescriptor Descriptor
        {
            get
            {
                return m_Descriptor;
            }
        }

        protected RHIFunctionDescriptor m_Descriptor;
    }
}
