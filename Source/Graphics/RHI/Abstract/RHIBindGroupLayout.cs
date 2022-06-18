using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupLayoutElement
    {
        public int slot;
        public EBindType bindType;
        public EShaderStageFlags shaderStage;
    }
    
    public struct RHIBindGroupLayoutCreateInfo
    {
        public int layoutIndex;
        public int elementCount => elements.Length;
        public Memory<RHIBindGroupLayoutElement> elements;
    }

    public abstract class RHIBindGroupLayout : Disposal
    {

    }
}
