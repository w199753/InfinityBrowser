using System;
using Apple.Metal;
using InfinityEngine.Mathmatics;
using InfinityEngine.Mathmatics.Geometry;

namespace InfinityEngine.Graphics
{
    public unsafe class MtlCommandBuffer : RHICommandBuffer
    {
        private MTLCommandBuffer m_CmdBuffer;
        private MTLBlitCommandEncoder m_BlitCmdEncoder;
        private MTLRenderCommandEncoder m_RenderCmdEncoder;
        private MTLComputeCommandEncoder m_ComputeCmdEncoder;

        internal MtlCommandBuffer(string name, RHIDevice device, RHICommandContext cmdContext, EContextType contextType) : base(name, device, contextType)
        {
            m_BlitCmdEncoder = m_CmdBuffer.blitCommandEncoder();
            //m_RenderCmdEncoder = m_CmdBuffer.renderCommandEncoderWithDescriptor();
            m_ComputeCmdEncoder = m_CmdBuffer.computeCommandEncoder();
        }

        public override void Clear()
        {

        }

        internal override void Close()
        {

        }

        public override void BeginEvent(string name)
        {

        }

        public override void EndEvent()
        {

        }

        public override void BeginQuery(RHIQuery query)
        {

        }

        public override void EndQuery(RHIQuery query)
        {

        }

        public override void Barriers(in ReadOnlySpan<FResourceBarrierInfo> barrierBatch)
        {

        }

        public override void Transition(RHIResource resource, EResourceState stateBefore, EResourceState stateAfter, int subresource = -1)
        {

        }

        public override void ClearBuffer(RHIBuffer buffer)
        {

        }

        public override void ClearTexture(RHITexture texture)
        {

        }

        public override void CopyBufferToBuffer(RHIBuffer srcBuffer, RHIBuffer dscBuffer)
        {

        }

        public override void CopyBufferToTexture(RHIBuffer srcBuffer, RHITexture dscTexture)
        {

        }

        public override void CopyTextureToBuffer(RHITexture srcTexture, RHIBuffer dscBuffer)
        {

        }

        public override void CopyTextureToTexture(RHITexture srcTexture, RHITexture dscTexture)
        {

        }

        public override void CopyAccelerationStructure()
        {

        }

        public override void BuildAccelerationStructure()
        {

        }

        public override void SetComputePipelineState(RHIComputePipelineState computePipelineState)
        {

        }

        public override void SetComputeResourceBind(in uint slot, RHIResourceSet resourceSet)
        {

        }

        public override void DispatchCompute(in uint sizeX, in uint sizeY, in uint sizeZ)
        {

        }

        public override void DispatchComputeIndirect(RHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        public override void SetRayTracePipelineState(RHIRayTracePipelineState rayTracePipelineState)
        {

        }

        public override void SetRayTraceResourceBind(in uint slot, RHIResourceSet resourceSet)
        {

        }

        public override void DispatchRay(in uint sizeX, in uint sizeY, in uint sizeZ)
        {

        }

        public override void DispatchRayIndirect(RHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        public override void SetScissor(in Rect rect)
        {

        }

        public override void SetViewport(in Viewport viewport)
        {

        }

        public override void BeginRenderPass(RHITexture depthBuffer, params RHITexture[] colorBuffer)
        {

        }

        public override void EndRenderPass()
        {

        }

        public override void ClearRenderTarget(RHIRenderTargetView renderTargetView, in float4 color)
        {

        }

        public override void SetStencilRef(in uint refValue)
        {

        }

        public override void SetBlendFactor(in float blendFactor)
        {

        }

        public override void SetDepthBound(in float min, in float max)
        {

        }

        public override void SetShadingRate(RHITexture texture)
        {

        }

        public override void SetShadingRate(in EShadingRate shadingRate, in EShadingRateCombiner combiner)
        {

        }

        public override void SetPrimitiveTopology(in EPrimitiveTopology topologyType)
        {

        }

        public override void SetGraphicsPipelineState(RHIGraphicsPipelineState graphicsPipelineState)
        {

        }

        public override void SetIndexBuffer(RHIIndexBufferView indexBufferView)
        {

        }

        public override void SetVertexBuffer(in uint slot, RHIVertexBufferView vertexBufferView)
        {

        }

        public override void SetGraphicsResourceBind(in uint slot, RHIResourceSet resourceSet)
        {

        }

        public override void DrawIndexInstanced(in uint indexCount, in uint startIndex, in int startVertex, in uint instanceCount, in uint startInstance)
        {

        }

        public override void DrawMultiIndexInstanced(RHIBuffer argsBuffer, in uint argsOffset, RHIBuffer countBuffer, in uint countOffset)
        {

        }

        public override void DrawIndexInstancedIndirect(RHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        protected override void Release()
        {

        }

        //public static implicit operator ID3D12GraphicsCommandList5*(D3DCommandBuffer cmdBuffer) { return cmdBuffer.nativeCmdList; }
    }
}
