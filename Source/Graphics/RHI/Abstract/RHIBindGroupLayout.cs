using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupLayoutElement
    {
        public int Count;
        public int BindSlot;
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
