using System;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHIIndirectDispatchArgs
    {
        public uint GroupCountX;
        public uint GroupCountY;
        public uint GroupCountZ;
        public RHIIndirectDispatchArgs(in uint groupCountX, in uint groupCountY, in uint groupCountZ)
        {
            GroupCountX = groupCountX;
            GroupCountY = groupCountY;
            GroupCountZ = groupCountZ;
        }
    }

    public struct RHIIndirectDrawArgs
    {
        public uint VertexCount;
        public uint InstanceCount;
        public uint StartVertexLocation;
        public uint StartInstanceLocation;
        public RHIIndirectDrawArgs(in uint vertexCount, in uint instanceCount, in uint startVertexLocation, in uint startInstanceLocation)
        {
            VertexCount = vertexCount;
            InstanceCount = instanceCount;
            StartVertexLocation = startVertexLocation;
            StartInstanceLocation = startInstanceLocation;
        }
    }

    public struct RHIIndirectDrawIndexedArgs
    {
        public uint IndexCount;
        public uint InstanceCount;
        public uint StartIndexLocation;
        public int BaseVertexLocation;
        public uint StartInstanceLocation;
        public RHIIndirectDrawIndexedArgs(in uint indexCount, in uint instanceCount, in uint startIndexLocation, in int baseVertexLocation, in uint startInstanceLocation)
        {
            IndexCount = indexCount;
            InstanceCount = instanceCount;
            StartIndexLocation = startIndexLocation;
            BaseVertexLocation = baseVertexLocation;
            StartInstanceLocation = startInstanceLocation;
        }
    }

    public struct RHITextureSubResourceDescriptor
    {
        public uint slice;
        public uint mipLevel;
        public uint3 origin;
    }

    public struct RHIShadingRateDescriptor
    {
        public EShadingRate shadingRate;
        public RHITexture? shadingRateTexture;
        public EShadingRateCombiner shadingRateCombiner;

        public RHIShadingRateDescriptor(in EShadingRate shadingRate, in EShadingRateCombiner shadingRateCombiner = EShadingRateCombiner.Max)
        {
            this.shadingRate = shadingRate;
            this.shadingRateTexture = null;
            this.shadingRateCombiner = shadingRateCombiner;
        }

        public RHIShadingRateDescriptor(RHITexture shadingRateTexture, in EShadingRateCombiner shadingRateCombiner = EShadingRateCombiner.Max)
        {
            this.shadingRate = EShadingRate.Rate1x1;
            this.shadingRateTexture = shadingRateTexture;
            this.shadingRateCombiner = shadingRateCombiner;
        }
    }

    public struct RHIColorAttachmentDescriptor
    {
        public ELoadOp loadOp;
        public EStoreOp storeOp;
        public float4 clearValue;
        public RHITextureView renderTarget;
        public RHITextureView resolveTarget;
    }

    public struct RHIDepthStencilAttachmentDescriptor
    {
        public bool depthReadOnly;
        public float depthClearValue;
        public ELoadOp depthLoadOp;
        public EStoreOp depthStoreOp;
        public bool stencilReadOnly;
        public int stencilClearValue;
        public ELoadOp stencilLoadOp;
        public EStoreOp stencilStoreOp;
        public RHITextureView depthStencilTarget;
    }

    public struct RHIGraphicsPassDescriptor
    {
        public string? name;
        public RHIShadingRateDescriptor? shadingRateDescriptor;
        public RHIDepthStencilAttachmentDescriptor? depthStencilAttachmentDescriptor;
        public Memory<RHIColorAttachmentDescriptor> colorAttachmentDescriptors;
        // TODO timestampWrites https://gpuweb.github.io/gpuweb/#render-pass-encoder-creation
        // TODO occlusionQuerySet https://gpuweb.github.io/gpuweb/#render-pass-encoder-creation
    }

    public struct RHIBlitPassScoper : IDisposable
    {
        RHIBlitEncoder m_BlitEncoder;

        internal RHIBlitPassScoper(RHIBlitEncoder blitEncoder)
        {
            m_BlitEncoder = blitEncoder;
        }

        public void Dispose()
        {
            m_BlitEncoder.EndPass();
        }
    }

    public struct RHIComputePassScoper : IDisposable
    {
        RHIComputeEncoder m_ComputeEncoder;

        internal RHIComputePassScoper(RHIComputeEncoder computeEncoder)
        {
            m_ComputeEncoder = computeEncoder;
        }

        public void Dispose()
        {
            m_ComputeEncoder.EndPass();
        }
    }

    public struct RHIGraphicsPassScoper : IDisposable
    {
        RHIGraphicsEncoder m_GraphicsEncoder;

        internal RHIGraphicsPassScoper(RHIGraphicsEncoder graphicsEncoder)
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
        public RHIBlitPassScoper BeginScopedPass(string? name = null)
        {
            BeginPass(name);
            return new RHIBlitPassScoper(this);
        }

        public abstract void BeginPass(string? name);
        public abstract void CopyBufferToBuffer(RHIBuffer src, in int srcOffset, RHIBuffer dst, in int dstOffset, in int size);
        public abstract void CopyBufferToTexture(RHIBuffer src, RHITexture dst, in RHITextureSubResourceDescriptor subResourceDescriptor, in int3 size);
        public abstract void CopyTextureToBuffer(RHITexture src, RHIBuffer dst, in RHITextureSubResourceDescriptor subResourceDescriptor, in int3 size);
        public abstract void CopyTextureToTexture(RHITexture src, in RHITextureSubResourceDescriptor srcSubResourceDescriptor, RHITexture dst, in RHITextureSubResourceDescriptor dstSubResourceDescriptor, in int3 size);
        public abstract void ResourceBarrier(in RHIBarrier barrier);
        public abstract void ResourceBarrier(in Memory<RHIBarrier> barriers);
        public abstract void PushDebugGroup(string name);
        public abstract void PopDebugGroup();
        public abstract void EndPass();
        // TODO WriteTimeStamp(...)
        // TODO ResolveQuery(...)
    }

    public abstract class RHIComputeEncoder : Disposal
    {
        public RHIComputePassScoper BeginScopedPass(string? name)
        {
            BeginPass(name);
            return new RHIComputePassScoper(this);
        }

        public abstract void BeginPass(string? name);
        public abstract void SetPipelineState(RHIComputePipelineState pipelineState);
        public abstract void SetPipelineLayout(RHIPipelineLayout pipelineLayout);
        public abstract void SetBindGroup(RHIBindGroup bindGroup);
        public abstract void Dispatch(in uint groupCountX, in uint groupCountY, in uint groupCountZ);
        public abstract void DispatchIndirect(RHIBuffer argsBuffer, in uint argsOffset);
        public abstract void PushDebugGroup(string name);
        public abstract void PopDebugGroup();
        public abstract void EndPass();
    }

    public abstract class RHIGraphicsEncoder : Disposal
    {
        public RHIGraphicsPassScoper BeginScopedPass(in RHIGraphicsPassDescriptor descriptor)
        {
            BeginPass(descriptor);
            return new RHIGraphicsPassScoper(this);
        }

        public abstract void BeginPass(in RHIGraphicsPassDescriptor descriptor);
        public abstract void SetPipelineState(RHIGraphicsPipelineState pipelineState);
        public abstract void SetPipelineLayout(RHIPipelineLayout pipelineLayout);
        public abstract void SetViewport(in Viewport viewport);
        public abstract void SetViewport(in Memory<Viewport> viewports);
        public abstract void SetScissorRect(in Rect rect);
        public abstract void SetScissorRect(in Memory<Rect> rects);
        public abstract void SetStencilRef(in uint value);
        public abstract void SetBlendFactor(in float value);
        public abstract void SetBlendFactor(in Memory<float> values);
        public abstract void SetBindGroup(RHIBindGroup bindGroup);
        public abstract void SetVertexBuffer(RHIBuffer buffer, in uint slot = 0, uint offset = 0);
        public abstract void SetIndexBuffer(RHIBuffer buffer, EIndexFormat format, uint offset = 0);
        public abstract void SetPrimitiveTopology(in EPrimitiveTopology primitiveTopology);
        public abstract void Draw(in uint vertexCount, in uint instanceCount, in uint firstVertex, in uint firstInstance);
        public abstract void DrawIndexed(in uint indexCount, in uint instanceCount, in uint firstIndex, in uint baseVertex, in uint firstInstance);
        public abstract void DrawIndirect(RHIBuffer argsBuffer, uint offset);
        public abstract void DrawIndexedIndirect(RHIBuffer argsBuffer, uint offset);
        public abstract void DrawMultiIndexedIndirect(RHIIndirectCommandBuffer indirectCommandBuffer);
        public abstract void PushDebugGroup(string name);
        public abstract void PopDebugGroup();
        // TODO ExecuteBundles(...)
        public abstract void EndPass();
    }
}
