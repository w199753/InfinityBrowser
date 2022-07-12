using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupLayoutElement
    {
        public int slot;
        public int count;
        public EBindType bindType;
        public EShaderStage shaderStage;
    }
    
    public struct RHIBindGroupLayoutDescriptor
    {
        public int index;
        public Memory<RHIBindGroupLayoutElement> elements;
    }

    public abstract class RHIBindGroupLayout : Disposal
    {

    }
}
