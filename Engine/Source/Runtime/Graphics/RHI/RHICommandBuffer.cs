using System;
using System.Collections.Generic;
using InfinityEngine.Core.Object;
using InfinityEngine.Core.Mathmatics;
using System.Runtime.CompilerServices;
using InfinityEngine.Core.Mathmatics.Geometry;

namespace InfinityEngine.Graphics.RHI
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

    public abstract class FRHICommandBuffer : FDisposal
    {
        public string name;
        internal int poolIndex;
        internal EContextType contextType;
        protected EPrimitiveTopology m_TopologyType;

        internal FRHICommandBuffer(string name, FRHIDevice device, in EContextType contextType)
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
        public abstract void BeginQuery(FRHIQuery query);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void EndQuery(FRHIQuery query);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Barriers(in ReadOnlySpan<FResourceBarrierInfo> barrierBatch);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Transition(FRHIResource resource, EResourceState stateBefore, EResourceState stateAfter, int subresource = -1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ClearBuffer(FRHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ClearTexture(FRHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyBufferToBuffer(FRHIBuffer srcBuffer, FRHIBuffer dscBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyBufferToTexture(FRHIBuffer srcBuffer, FRHITexture dscTexture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyTextureToBuffer(FRHITexture srcTexture, FRHIBuffer dscBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyTextureToTexture(FRHITexture srcTexture, FRHITexture dscTexture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyAccelerationStructure();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void BuildAccelerationStructure();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetComputePipelineState(FRHIComputePipelineState computePipelineState);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetComputeResourceBind(in uint slot, FRHIResourceSet resourceSet);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchCompute(in uint sizeX, in uint sizeY, in uint sizeZ);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchComputeIndirect(FRHIBuffer argsBuffer, in uint argsOffset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetRayTracePipelineState(FRHIRayTracePipelineState rayTracePipelineState);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetRayTraceResourceBind(in uint slot, FRHIResourceSet resourceSet);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchRay(in uint sizeX, in uint sizeY, in uint sizeZ);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DispatchRayIndirect(FRHIBuffer argsBuffer, in uint argsOffset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetScissor(in FRect rect);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetViewport(in FViewport viewport);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void BeginRenderPass(FRHITexture depthBuffer, params FRHITexture[] colorBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void EndRenderPass();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ClearRenderTarget(FRHIRenderTargetView renderTargetView, in float4 color);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetStencilRef(in uint refValue);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetBlendFactor(in float blendFactor);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetDepthBound(in float min, in float max);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetShadingRate(FRHITexture texture);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetShadingRate(in EShadingRate shadingRate, in EShadingRateCombiner combiner);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetPrimitiveTopology(in EPrimitiveTopology topologyType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetGraphicsPipelineState(FRHIGraphicsPipelineState graphicsPipelineState);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetIndexBuffer(FRHIIndexBufferView indexBufferView);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetVertexBuffer(in uint slot, FRHIVertexBufferView vertexBufferView);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetGraphicsResourceBind(in uint slot, FRHIResourceSet resourceSet);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawIndexInstanced(in uint indexCount, in uint startIndex, in int startVertex, in uint instanceCount, in uint startInstance);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawMultiIndexInstanced(FRHIBuffer argsBuffer, in uint argsOffset, FRHIBuffer countBuffer, in uint countOffset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void DrawIndexInstancedIndirect(FRHIBuffer argsBuffer, in uint argsOffset);
    }

    internal class FRHICommandBufferPool : FDisposal
    {
        EContextType m_ContextType;
        Stack<FRHICommandBuffer> m_Pooled;
        FRHIContext m_Context;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Pooled.Count; } }

        internal FRHICommandBufferPool(FRHIContext context, EContextType contextType)
        {
            m_ContextType = contextType;
            m_Context = context;
            m_Pooled = new Stack<FRHICommandBuffer>(64);
        }

        public FRHICommandBuffer GetTemporary(string name)
        {
            FRHICommandBuffer cmdBuffer;
            if (m_Pooled.Count == 0) {
                ++countAll;
                cmdBuffer = m_Context.CreateCommandBuffer(m_ContextType, name);
            } else {
                cmdBuffer = m_Pooled.Pop();
            }
            cmdBuffer.name = name;
            return cmdBuffer;
        }

        public void ReleaseTemporary(FRHICommandBuffer cmdBuffer)
        {
            m_Pooled.Push(cmdBuffer);
        }

        protected override void Release()
        {
            m_Context = null;
            foreach (FRHICommandBuffer cmdBuffer in m_Pooled)
            {
                cmdBuffer.Dispose();
            }
        }
    }
}
