using System;
using InfinityEngine.Core.Object;
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
        public FRHIFence fence;
        public EExecuteType executeType;
        public FRHICommandBuffer cmdBuffer;
        public FRHICommandContext cmdContext;
    }

    public abstract class FRHIContext : FDisposal
    {
        public virtual bool copyQueueState => false;
        public virtual ulong copyFrequency => 0;
        public virtual ulong computeFrequency => 0;
        public virtual ulong graphicsFrequency => 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract FRHICommandContext SelectContext(in EContextType contextType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHICommandBuffer CreateCommandBuffer(in EContextType contextType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHICommandBuffer GetCommandBuffer(in EContextType contextType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseCommandBuffer(FRHICommandBuffer cmdBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void WriteToFence(in EContextType contextType, FRHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void WaitForFence(in EContextType contextType, FRHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ExecuteCommandBuffer(FRHICommandBuffer cmdBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void Submit();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHISwapChain CreateSwapChain(string name, in uint width, in uint height, in IntPtr windowPtr);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIFence CreateFence(string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIFence GetFence(string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseFence(FRHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIQuery CreateQuery(in EQueryType queryType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIQuery GetQuery(in EQueryType queryType, string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseQuery(FRHIQuery query);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIComputePipelineState CreateComputePipelineState(in FRHIComputePipelineDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIRayTracePipelineState CreateRayTracePipelineState(in FRHIRayTracePipelineDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIGraphicsPipelineState CreateGraphicsPipelineState(in FRHIGraphicsPipelineDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CreateSamplerState();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CreateVertexInputLayout();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CreateResourceInputLayout();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIBuffer CreateBuffer(in FBufferDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIBufferRef GetBuffer(in FBufferDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseBuffer(in FRHIBufferRef bufferRef);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHITexture CreateTexture(in FTextureDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHITextureRef GetTexture(in FTextureDescriptor descriptor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ReleaseTexture(FRHITextureRef textureRef);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIMemoryReadback CreateMemoryReadback(string requestName, bool bProfiler);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIIndexBufferView CreateIndexBufferView(FRHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIVertexBufferView CreateVertexBufferView(FRHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIDeptnStencilView CreateDepthStencilView(FRHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIRenderTargetView CreateRenderTargetView(FRHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIConstantBufferView CreateConstantBufferView(FRHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIShaderResourceView CreateShaderResourceView(FRHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIShaderResourceView CreateShaderResourceView(FRHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIUnorderedAccessView CreateUnorderedAccessView(FRHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIUnorderedAccessView CreateUnorderedAccessView(FRHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract FRHIResourceSet CreateResourceSet(in uint count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SubmitContext(FRHIContext context)
        {
            context.Submit();
        }
    }
}
