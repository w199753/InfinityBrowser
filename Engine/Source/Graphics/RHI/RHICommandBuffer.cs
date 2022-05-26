using System;
using System.Collections.Generic;
using Infinity.Mathmatics;
using System.Runtime.CompilerServices;
using Infinity.Mathmatics.Geometry;

namespace Infinity.Graphics
{
    public enum EExecuteType
    {
        Signal = 0,
        Wait = 1,
        Execute = 2
    }

    public enum EShadingRate
    {
        Rate1x1 = 0,
        Rate1x2 = 1,
        Rate2x1 = 4,
        Rate2x2 = 5,
        Rate2x4 = 6,
        Rate4x2 = 9,
        Rate4x4 = 10
    }

    public enum EPrimitiveTopology
    {
        LineList = 0,
        LineListWithAdjacency = 1,
        LineStrip = 2,
        LineStripWithAdjacency = 3,
        Patch_List = 4,
        PointList = 5,
        TriangleList = 6,
        TriangleListWithAdjacency = 7,
        TriangleStrip = 8,
        TriangleStripWithAdjacency = 9,
        Undefined = 10
    }

    public enum EShadingRateCombiner
    {
        Min = 0,
        Max = 1,
        Sum = 2,
        Override = 3,
        Passthrough = 4
    }

    public abstract class RHICommandBuffer : Disposal
    {
        public string name;
        internal int poolIndex;
        internal EContextType contextType;
        protected EPrimitiveTopology m_TopologyType;

        internal RHICommandBuffer(string name, RHIDevice device, in EContextType contextType)
        {
            this.name = name;
            this.contextType = contextType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Clear();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void Close();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void BeginEvent(string name);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void EndEvent();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void BeginQuery(RHIQuery query);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void EndQuery(RHIQuery query);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Barriers(in ReadOnlySpan<FResourceBarrierInfo> barrierBatch);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Transition(RHIResource resource, EResourceState stateBefore, EResourceState stateAfter, int subresource = -1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ClearBuffer(RHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ClearTexture(RHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyBufferToBuffer(RHIBuffer srcBuffer, RHIBuffer dscBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyBufferToTexture(RHIBuffer srcBuffer, RHITexture dscTexture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyTextureToBuffer(RHITexture srcTexture, RHIBuffer dscBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyTextureToTexture(RHITexture srcTexture, RHITexture dscTexture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyAccelerationStructure();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void BuildAccelerationStructure();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetComputePipelineState(RHIComputePipelineState computePipelineState);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetComputeResourceBind(in uint slot, RHIResourceSet resourceSet);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchCompute(in uint sizeX, in uint sizeY, in uint sizeZ);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchComputeIndirect(RHIBuffer argsBuffer, in uint argsOffset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetRayTracePipelineState(RHIRayTracePipelineState rayTracePipelineState);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetRayTraceResourceBind(in uint slot, RHIResourceSet resourceSet);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchRay(in uint sizeX, in uint sizeY, in uint sizeZ);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchRayIndirect(RHIBuffer argsBuffer, in uint argsOffset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetScissor(in Rect rect);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetViewport(in Viewport viewport);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void BeginRenderPass(RHITexture depthBuffer, params RHITexture[] colorBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void EndRenderPass();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ClearRenderTarget(RHIRenderTargetView renderTargetView, in float4 color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetStencilRef(in uint refValue);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetBlendFactor(in float blendFactor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetDepthBound(in float min, in float max);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetShadingRate(RHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetShadingRate(in EShadingRate shadingRate, in EShadingRateCombiner combiner);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetPrimitiveTopology(in EPrimitiveTopology topologyType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetGraphicsPipelineState(RHIGraphicsPipelineState graphicsPipelineState);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetIndexBuffer(RHIIndexBufferView indexBufferView);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetVertexBuffer(in uint slot, RHIVertexBufferView vertexBufferView);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetGraphicsResourceBind(in uint slot, RHIResourceSet resourceSet);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawIndexInstanced(in uint indexCount, in uint startIndex, in int startVertex, in uint instanceCount, in uint startInstance);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawMultiIndexInstanced(RHIBuffer argsBuffer, in uint argsOffset, RHIBuffer countBuffer, in uint countOffset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawIndexInstancedIndirect(RHIBuffer argsBuffer, in uint argsOffset);
    }

    internal class RHICommandBufferPool : Disposal
    {
        EContextType m_ContextType;
        Stack<RHICommandBuffer> m_Pooled;
        RHIContext m_Context;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Pooled.Count; } }

        internal RHICommandBufferPool(RHIContext context, EContextType contextType)
        {
            m_ContextType = contextType;
            m_Context = context;
            m_Pooled = new Stack<RHICommandBuffer>(64);
        }

        public RHICommandBuffer GetTemporary(string name)
        {
            RHICommandBuffer cmdBuffer;
            if (m_Pooled.Count == 0) {
                ++countAll;
                cmdBuffer = m_Context.CreateCommandBuffer(m_ContextType, name);
            } else {
                cmdBuffer = m_Pooled.Pop();
            }
            cmdBuffer.name = name;
            return cmdBuffer;
        }

        public void ReleaseTemporary(RHICommandBuffer cmdBuffer)
        {
            m_Pooled.Push(cmdBuffer);
        }

        protected override void Release()
        {
            m_Context = null;
            foreach (RHICommandBuffer cmdBuffer in m_Pooled)
            {
                cmdBuffer.Dispose();
            }
        }
    }
}
