using System;

namespace Infinity.Graphics
{
    public struct RHIQueueInfo
    {
        public uint count;
        public EQueueType type;
    }

    public struct RHIDeviceCreateInfo
    {
        public uint queueInfoCount => (uint)queueInfos.Length;
        public Memory<RHIQueueInfo> queueInfos;
    }

    public abstract class RHIDevice : Disposal
    {
        public abstract int GetQueueCount(in EQueueType type);
        public abstract RHIQueue GetQueue(in EQueueType type, in int index);
        public abstract RHIFence CreateFence();
        public abstract RHISwapChain CreateSwapChain(in RHISwapChainCreateInfo createInfo);
        public abstract RHIBuffer CreateBuffer(in RHIBufferCreateInfo createInfo);
        public abstract RHITexture CreateTexture(in RHITextureCreateInfo createInfo);
        public abstract RHISampler CreateSampler(in RHISamplerCreateInfo createInfo);
        public abstract RHIShader CreateShader(in RHIShaderCreateInfo createInfo);
        public abstract RHIBindGroupLayout CreateBindGroupLayout(in RHIBindGroupLayoutCreateInfo createInfo);
        public abstract RHIBindGroup CreateBindGroup(in RHIBindGroupCreateInfo createInfo);
        public abstract RHIPipelineLayout CreatePipelineLayout(in RHIPipelineLayoutCreateInfo createInfo);
        public abstract RHIComputePipeline CreateComputePipeline(in RHIComputePipelineCreateInfo createInfo);
        public abstract RHIGraphicsPipeline CreateGraphicsPipeline(in RHIGraphicsPipelineCreateInfo createInfo);
    }
}
