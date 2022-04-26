using System;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
    public enum EContextType
    {
        Copy = 3,
        Compute = 2,
        Graphics = 0
    }

    internal struct FExecuteInfo
    {
        public RHIFence fence;
        public EExecuteType executeType;
        public RHICommandBuffer cmdBuffer;
        public RHICommandContext cmdContext;
    }

    public abstract class RHIContext : Disposal
    {
        public virtual bool copyQueueState => false;
        public virtual ulong copyFrequency => 0;
        public virtual ulong computeFrequency => 0;
        public virtual ulong graphicsFrequency => 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract RHICommandContext SelectContext(in EContextType contextType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHICommandBuffer CreateCommandBuffer(in EContextType contextType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHICommandBuffer GetCommandBuffer(in EContextType contextType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseCommandBuffer(RHICommandBuffer cmdBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void WriteToFence(in EContextType contextType, RHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void WaitForFence(in EContextType contextType, RHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ExecuteCommandBuffer(RHICommandBuffer cmdBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void Submit();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHISwapChain CreateSwapChain(string name, in uint width, in uint height, in IntPtr windowPtr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIFence CreateFence(string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIFence GetFence(string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseFence(RHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIQuery CreateQuery(in EQueryType queryType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIQuery GetQuery(in EQueryType queryType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseQuery(RHIQuery query);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIComputePipelineState CreateComputePipelineState(in RHIComputePipelineDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIRayTracePipelineState CreateRayTracePipelineState(in RHIRayTracePipelineDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIGraphicsPipelineState CreateGraphicsPipelineState(in RHIGraphicsPipelineDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CreateSamplerState();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CreateVertexInputLayout();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CreateResourceInputLayout();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIBuffer CreateBuffer(in BufferDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIBufferRef GetBuffer(in BufferDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseBuffer(in RHIBufferRef bufferRef);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHITexture CreateTexture(in TextureDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHITextureRef GetTexture(in TextureDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseTexture(RHITextureRef textureRef);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIMemoryReadback CreateMemoryReadback(string requestName, bool bProfiler);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIIndexBufferView CreateIndexBufferView(RHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIVertexBufferView CreateVertexBufferView(RHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIDeptnStencilView CreateDepthStencilView(RHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIRenderTargetView CreateRenderTargetView(RHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIConstantBufferView CreateConstantBufferView(RHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIShaderResourceView CreateShaderResourceView(RHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIShaderResourceView CreateShaderResourceView(RHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIUnorderedAccessView CreateUnorderedAccessView(RHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIUnorderedAccessView CreateUnorderedAccessView(RHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RHIResourceSet CreateResourceSet(in uint count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubmitContext(RHIContext context)
        {
            context.Submit();
        }
    }
}
