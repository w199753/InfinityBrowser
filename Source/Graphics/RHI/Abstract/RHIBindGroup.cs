using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupElement
    {
        public int slot;
        public EBindingType bindType;
        public RHISampler sampler;
        public RHIBufferView bufferView;
        public RHITextureView textureView;
    }

    public struct RHIBindGroupCreateInfo
    {
        public int elementCount => elements.Length;
        public RHIBindGroupLayout layout;
        public Memory<RHIBindGroupElement> elements;
    }

    public abstract class RHIBindGroup : Disposal
    {

    }
}
