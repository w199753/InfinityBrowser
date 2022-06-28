using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupLayoutElement
    {
        public int slot;
        public int count;
        public EBindType bindType;
        public EShaderStageFlag shaderStage;
    }
    
    public struct RHIBindGroupLayoutDescriptor
    {
        public int layoutIndex;
        public int elementCount => elements.Length;
        public Memory<RHIBindGroupLayoutElement> elements;
    }

    public abstract class RHIBindGroupLayout : Disposal
    {

    }
}
