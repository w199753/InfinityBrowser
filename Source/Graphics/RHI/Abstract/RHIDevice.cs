using System;

namespace Infinity.Graphics
{
    public struct RHIDeviceDescriptor
    {
        public uint queueInfoCount => (uint)queueInfos.Length;
        public Memory<RHIQueueDescriptor> queueInfos;
    }

    public abstract class RHIDevice : Disposal
    {
        public abstract int GetQueueCount(in EQueueType type);
        public abstract RHIQueue GetQueue(in EQueueType type, in int index);
        public abstract RHIFence CreateFence();
        public abstract RHISwapChain CreateSwapChain(in RHISwapChainDescriptor descriptor);
        public abstract RHIBuffer CreateBuffer(in RHIBufferDescriptor descriptor);
        public abstract RHITexture CreateTexture(in RHITextureDescriptor descriptor);
        public abstract RHISampler CreateSampler(in RHISamplerDescriptor descriptor);
        public abstract RHIShader CreateShader(in RHIShaderDescriptor descriptor);
        public abstract RHIBindGroupLayout CreateBindGroupLayout(in RHIBindGroupLayoutDescriptor descriptor);
        public abstract RHIBindGroup CreateBindGroup(in RHIBindGroupDescriptor descriptor);
        public abstract RHIPipelineLayout CreatePipelineLayout(in RHIPipelineLayoutDescriptor descriptor);
        public abstract RHIComputePipeline CreateComputePipeline(in RHIComputePipelineDescriptor descriptor);
        public abstract RHIGraphicsPipeline CreateGraphicsPipeline(in RHIGraphicsPipelineDescriptor descriptor);
    }
}
