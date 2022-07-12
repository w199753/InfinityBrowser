using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupElement
    {
        //public int slot;
        //public EBindType bindType;
        public RHIBufferView bufferView;
        public RHITextureView textureView;
        public RHISampler textureSampler;
    }

    public struct RHIBindGroupDescriptor
    {
        public RHIBindGroupLayout layout;
        public Memory<RHIBindGroupElement> elements;
    }

    public abstract class RHIBindGroup : Disposal
    {

    }
}
