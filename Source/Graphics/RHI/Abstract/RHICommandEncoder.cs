using Infinity.Mathmatics;
using System;

namespace Infinity.Graphics
{
    public struct RHITextureSubResourceInfo
    {
        public uint slice;
        public uint mipLevel;
        public uint3 origin;
        public ETextureAspect aspect;
    }

    public struct RHIGraphicsPassColorAttachment
    {
        public RHITextureView? view;
        public RHITextureView? resolveTarget;
        public ELoadOp loadOp;
        public EStoreOp storeOp;
        public float4 clearValue;
    }

    public struct RHIGraphicsPassDepthStencilAttachment
    {
        public RHITextureView view;
        public float depthClearValue;
        public ELoadOp depthLoadOp;
        public EStoreOp depthStoreOp;
        public bool depthReadOnly;
        public int stencilClearValue;
        public ELoadOp stencilLoadOp;
        public EStoreOp stencilStoreOp;
        public bool stencilReadOnly;
    }

    public struct RHIGraphicsPassBeginInfo
    {
        public string name;
        public int colorAttachmentCount => colorAttachments.Length;
        public RHIGraphicsPassDepthStencilAttachment? depthStencilAttachment;
        public Memory<RHIGraphicsPassColorAttachment> colorAttachments;
        // TODO timestampWrites #see https://gpuweb.github.io/gpuweb/#render-pass-encoder-creation
        // TODO occlusionQuerySet #see https://gpuweb.github.io/gpuweb/#render-pass-encoder-creation
    }

    public struct RHIScopedBlitPassRef : IDisposable
    {
        RHIBlitEncoder m_BlitEncoder;

        internal RHIScopedBlitPassRef(RHIBlitEncoder blitEncoder)
        {
            m_BlitEncoder = blitEncoder;
        }

        public void Dispose()
        {
            m_BlitEncoder.EndPass();
        }
    }

    public struct RHIScopedComputePassRef : IDisposable
    {
        RHIComputeEncoder m_ComputeEncoder;

        internal RHIScopedComputePassRef(RHIComputeEncoder computeEncoder)
        {
            m_ComputeEncoder = computeEncoder;
        }

        public void Dispose()
        {
            m_ComputeEncoder.EndPass();
        }
    }

    public struct RHIScopedGraphicsPassRef : IDisposable
    {
        RHIGraphicsEncoder m_GraphicsEncoder;

        internal RHIScopedGraphicsPassRef(RHIGraphicsEncoder graphicsEncoder)
        {
            m_GraphicsEncoder = graphicsEncoder;
        }

        public void Dispose()
        {
            m_GraphicsEncoder.EndPass();
        }
    }

    public abstract class RHIBlitEncoder : Disposal
    {
        public RHIScopedBlitPassRef BeginScopedPass(string name)
        {
            BeginPass(name);
            return new RHIScopedBlitPassRef(this);
        }

        public abstract void BeginPass(string name);
        public abstract void CopyBufferToBuffer(RHIBuffer src, in int srcOffset, RHIBuffer dst, in int dstOffset, in int size);
        public abstract void CopyBufferToTexture(RHIBuffer src, RHITexture dst, in RHITextureSubResourceInfo subResourceInfo, in int3 size);
        public abstract void CopyTextureToBuffer(RHITexture src, RHIBuffer dst, in RHITextureSubResourceInfo subResourceInfo, in int3 size);
        public abstract void CopyTextureToTexture(RHITexture src, in RHITextureSubResourceInfo srcSubResourceInfo, RHITexture dst, in RHITextureSubResourceInfo dstSubResourceInfo, in int3 size);
        public abstract void ResourceBarrier(in RHIBarrier barrier);
        public abstract void ResourceBarrier(in Memory<RHIBarrier> barriers);
        public abstract void PushDebugGroup(string name);
        public abstract void PopDebugGroup();
        public abstract void EndPass();
        // TODO PushDebugMark(...)
        // TODO PullDebugMark(...)
        // TODO WriteTimeStamp(...)
        // TODO ResolveQuerySet(...)
    }

    public abstract class RHIComputeEncoder : Disposal
    {
        public RHIScopedComputePassRef BeginScopedPass(string name)
        {
            BeginPass(name);
            return new RHIScopedComputePassRef(this);
        }

        public abstract void BeginPass(string name);
        public abstract void SetPipeline(RHIComputePipeline pipeline);
        public abstract void SetBindGroup(RHIBindGroup bindGroup, in int layoutIndex);
        public abstract void Dispatch(in uint groupCountX, in uint groupCountY, in uint groupCountZ);
        public abstract void PushDebugGroup(string name);
        public abstract void PopDebugGroup();
        public abstract void EndPass();
        // TODO DispatchIndirect(...)
    }

    public abstract class RHIGraphicsEncoder : Disposal
    {
        public RHIScopedGraphicsPassRef BeginScopedPass(in RHIGraphicsPassBeginInfo beginInfo)
        {
            BeginPass(beginInfo);
            return new RHIScopedGraphicsPassRef(this);
        }

        public abstract void BeginPass(in RHIGraphicsPassBeginInfo beginInfo);
        public abstract void SetPipeline(RHIGraphicsPipeline pipeline);
        public abstract void SetScissor(in uint left, in uint top, in uint right, in uint bottom);
        public abstract void SetViewport(in float x, in float y, in float width, in float height, in float minDepth, in float maxDepth);
        public abstract void SetBlendConstant(in float constants);
        public abstract void SetStencilReference(in uint reference);
        public abstract void SetBindGroup(RHIBindGroup bindGroup, in int layoutIndex);
        public abstract void SetIndexBuffer(RHIBufferView bufferView);
        public abstract void SetVertexBuffer(in uint slot, RHIBufferView bufferView);
        public abstract void SetPrimitiveTopology(in EPrimitiveTopology primitiveTopology);
        public abstract void Draw(in uint vertexCount, in uint instanceCount, in uint firstVertex, in uint firstInstance);
        public abstract void DrawIndexed(in uint indexCount, in uint instanceCount, in uint firstIndex, in uint baseVertex, in uint firstInstance);
        public abstract void PushDebugGroup(string name);
        public abstract void PopDebugGroup();
        // TODO DrawIndirect(...)
        // TODO DrawIndexedIndirect(...)
        // TODO DrawMultiIndexedIndirect(...)
        // TODO ExecuteBundles(...)
        public abstract void EndPass();
    }
}
