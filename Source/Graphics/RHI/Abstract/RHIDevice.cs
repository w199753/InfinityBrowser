using System;

namespace Infinity.Graphics
{
    public struct RHIDeviceDescriptor
    {
        public int QueueInfoCount => QueueInfos.Length;
        public Memory<RHIQueueDescriptor> QueueInfos;
    }

    public abstract class RHIDevice : Disposal
    {
        public abstract int GetQueueCount(in EQueueType type);
        public abstract RHIQueue GetQueue(in EQueueType type, in int index);
        public abstract RHIFence CreateFence();
        public abstract RHISemaphore CreateSemaphore();
        public abstract RHIQuery CreateQuery(in RHIQueryDescription descriptor);
        public abstract RHIHeap CreateHeap(in RHIHeapDescription descriptor);
        public abstract RHIBuffer CreateBuffer(in RHIBufferDescriptor descriptor);
        public abstract RHITexture CreateTexture(in RHITextureDescriptor descriptor);
        public abstract RHISampler CreateSampler(in RHISamplerDescriptor descriptor);
        public abstract RHIShader CreateShader(in RHIShaderDescriptor descriptor);
        public abstract RHISwapChain CreateSwapChain(in RHISwapChainDescriptor descriptor);
        public abstract RHIBindGroupLayout CreateBindGroupLayout(in RHIBindGroupLayoutDescriptor descriptor);
        public abstract RHIBindGroup CreateBindGroup(in RHIBindGroupDescriptor descriptor);
        public abstract RHIComputePipeline CreateComputePipeline(in RHIComputePipelineDescriptor descriptor);
        public abstract RHIRaytracingPipeline CreateRaytracingPipeline(in RHIRaytracingPipelineDescriptor descriptor);
        public abstract RHIMeshletPipeline CreateMeshletPipeline(in RHIMeshletPipelineDescriptor descriptor);
        public abstract RHIGraphicsPipeline CreateGraphicsPipeline(in RHIGraphicsPipelineDescriptor descriptor);

        internal abstract RHIPipelineLayout CreatePipelineLayout(in RHIPipelineLayoutDescriptor descriptor);
    }
}
