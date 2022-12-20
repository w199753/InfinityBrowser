using System;
using Infinity.Core;

namespace Infinity.Graphics
{
    public struct RHIFunctionDescriptor
    {
        public uint ByteSize;
        public IntPtr ByteCode;
        public string EntryName;
        public EFunctionType Type;
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

    public abstract class RHIFunctionTable : Disposal
    {
        public abstract void SetRayGenerationProgram(string exportName, params RHIBindGroup[] bindGroup);
        public abstract void AddMissProgram(string exportName, params RHIBindGroup[] bindGroup);
        public abstract void AddHitGroupProgram(string exportName, params RHIBindGroup[] bindGroup);
        public abstract void ClearMissPrograms();
        public abstract void ClearHitGroupPrograms();
    }
}
