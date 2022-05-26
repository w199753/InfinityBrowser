using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupEntry
    {
        public int slot;
        public EBindingType type;
        public RHISampler sampler;
        public RHIBufferView bufferView;
        public RHITextureView textureView;
    }

    public struct RHIBindGroupCreateInfo
    {
        public int entryCount;
        public RHIBindGroupLayout layout;
        public Memory<RHIBindGroupEntry> entries;
    }

    public abstract class RHIBindGroup : Disposal
    {

    }
}
