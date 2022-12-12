using System;
using Infinity.Core;

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

    public abstract class RHIFunctionTable : Disposal
    {
        public abstract void SetRayGenerationShader(string exportName, RHIBindGroup bindGroup);
        public abstract void AddMissShader(string exportName, RHIBindGroup bindGroup);
        public abstract void AddHitGroup(string exportName, RHIBindGroup bindGroup);
        public abstract void AddCallableShader(string exportName, RHIBindGroup bindGroup);
        public abstract void ClearMissShaders();
        public abstract void ClearHitShaders();
        public abstract void ClearCallableShaders();
    }
}
