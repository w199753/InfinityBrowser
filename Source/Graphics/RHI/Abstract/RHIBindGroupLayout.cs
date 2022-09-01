using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupLayoutElement
    {
        public int Slot;
        public int Count;
        public EBindType BindType;
        public EShaderStage ShaderStage;
    }
    
    public struct RHIBindGroupLayoutDescriptor
    {
        public int Index;
        public Memory<RHIBindGroupLayoutElement> Elements;
    }

    public abstract class RHIBindGroupLayout : Disposal
    {

    }
}
