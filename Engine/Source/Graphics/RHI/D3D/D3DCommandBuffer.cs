using System;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using InfinityEngine.Mathmatics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using InfinityEngine.Mathmatics.Geometry;

namespace InfinityEngine.Graphics
{
    public unsafe class D3DCommandBuffer : RHICommandBuffer
    {
        internal ID3D12CommandAllocator* nativeCmdPool;
        internal ID3D12GraphicsCommandList5* nativeCmdList;

        internal D3DCommandBuffer(string name, RHIDevice device, RHICommandContext cmdContext, in EContextType contextType) : base(name, device, contextType)
        {
            D3DDevice d3dDevice = (D3DDevice)device;
            D3DCommandContext d3dCmdContext = (D3DCommandContext)cmdContext;
            this.nativeCmdPool = d3dCmdContext.nativeCmdAllocator;

            ID3D12GraphicsCommandList5* commandListPtr;
            d3dDevice.nativeDevice->CreateCommandList(0, (D3D12_COMMAND_LIST_TYPE)contextType, nativeCmdPool, null, Windows.__uuidof<ID3D12GraphicsCommandList5>(), (void**)&commandListPtr);
            fixed (char* namePtr = name + "_CmdList")
            {
                commandListPtr->SetName((ushort*)namePtr);
            }
            nativeCmdList = commandListPtr;
            Close();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Clear()
        {
            nativeCmdList->Reset(nativeCmdPool, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Close()
        {
            nativeCmdList->Close();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void BeginEvent(string name)
        {
            /*int byteSize = name.Length * sizeof(char);
            void* ptr = stackalloc byte[byteSize];
            name.CopyTo(new Span<char>(ptr, name.Length));
            nativeCmdList->BeginEvent(0, ptr, (uint)byteSize);*/

            if (name != null)
            {
                IntPtr intPtr = Marshal.StringToHGlobalUni(name);
                nativeCmdList->BeginEvent(0, intPtr.ToPointer(), (uint)name.Length * 2);
                Marshal.FreeHGlobal(intPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndEvent()
        {
            nativeCmdList->EndEvent();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void BeginQuery(RHIQuery query)
        {
            D3DQuery d3dQuery = (D3DQuery)query;
            if (d3dQuery.queryContext.IsReady)
            {
                if (d3dQuery.queryContext.IsTimeQuery) {
                    nativeCmdList->EndQuery(d3dQuery.queryContext.queryHeap, d3dQuery.queryContext.queryType.GetNativeQueryType(), (uint)query.indexHead);
                } else {
                    nativeCmdList->BeginQuery(d3dQuery.queryContext.queryHeap, d3dQuery.queryContext.queryType.GetNativeQueryType(), (uint)query.indexHead);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndQuery(RHIQuery query)
        {
            D3DQuery d3dQuery = (D3DQuery)query;
            if (d3dQuery.queryContext.IsReady)
            {
                nativeCmdList->EndQuery(d3dQuery.queryContext.queryHeap, d3dQuery.queryContext.queryType.GetNativeQueryType(), (uint)query.indexLast);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Barriers(in ReadOnlySpan<FResourceBarrierInfo> barrierBatch)
        {
            //nativeCmdList->ResourceBarrier
            //Vortice.Direct3D12.ID3D12GraphicsCommandList
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Transition(RHIResource resource, EResourceState stateBefore, EResourceState stateAfter, int subresource = -1)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ClearBuffer(RHIBuffer buffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ClearTexture(RHITexture texture)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyBufferToBuffer(RHIBuffer srcBuffer, RHIBuffer dscBuffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyBufferToTexture(RHIBuffer srcBuffer, RHITexture dscTexture)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyTextureToBuffer(RHITexture srcTexture, RHIBuffer dscBuffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyTextureToTexture(RHITexture srcTexture, RHITexture dscTexture)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyAccelerationStructure()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void BuildAccelerationStructure()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetComputePipelineState(RHIComputePipelineState computePipelineState)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetComputeResourceBind(in uint slot, RHIResourceSet resourceSet)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchCompute(in uint sizeX, in uint sizeY, in uint sizeZ)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchComputeIndirect(RHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetRayTracePipelineState(RHIRayTracePipelineState rayTracePipelineState)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetRayTraceResourceBind(in uint slot, RHIResourceSet resourceSet)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchRay(in uint sizeX, in uint sizeY, in uint sizeZ)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchRayIndirect(RHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetScissor(in Rect rect)
        {
            nativeCmdList->RSSetScissorRects(1, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetViewport(in Viewport viewport)
        {
            nativeCmdList->RSSetViewports(1, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void BeginRenderPass(RHITexture depthBuffer, params RHITexture[] colorBuffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndRenderPass()
        {
            nativeCmdList->EndRenderPass();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ClearRenderTarget(RHIRenderTargetView renderTargetView, in float4 color)
        {
            float4 copyColor = color;
            D3DRenderTargetView d3dRTV = (D3DRenderTargetView)renderTargetView;
            nativeCmdList->ClearRenderTargetView(d3dRTV.descriptorHandle, (float*)&copyColor, 0, null);
            //Vortice.Direct3D12.ID3D12GraphicsCommandList
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetStencilRef(in uint refValue)
        {
            nativeCmdList->OMSetStencilRef(refValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetBlendFactor(in float blendFactor)
        {
            float factor = blendFactor;
            nativeCmdList->OMSetBlendFactor(&factor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetDepthBound(in float min, in float max)
        {
            nativeCmdList->OMSetDepthBounds(min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetShadingRate(RHITexture texture)
        {
            D3DTexture d3dTexture = (D3DTexture)texture;
            nativeCmdList->RSSetShadingRateImage(d3dTexture.defaultResource);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetShadingRate(in EShadingRate shadingRate, in EShadingRateCombiner combiner)
        {
            nativeCmdList->RSSetShadingRate((D3D12_SHADING_RATE)shadingRate, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetPrimitiveTopology(in EPrimitiveTopology topologyType)
        {
            m_TopologyType = topologyType;
            nativeCmdList->IASetPrimitiveTopology((D3D_PRIMITIVE_TOPOLOGY)topologyType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetGraphicsPipelineState(RHIGraphicsPipelineState graphicsPipelineState)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetIndexBuffer(RHIIndexBufferView indexBufferView) 
        {
            /*D3D12_INDEX_BUFFER_VIEW indexBufferView;
            indexBufferView.Format = DXGI_FORMAT.DXGI_FORMAT_R32_UINT;
            indexBufferView.SizeInBytes = (uint)(indexBuffer.descriptor.count * indexBuffer.descriptor.stride);
            indexBufferView.BufferLocation = ((D3DBuffer)indexBuffer).defaultResource->GetGPUVirtualAddress();
            nativeCmdList->IASetIndexBuffer(&indexBufferView);*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetVertexBuffer(in uint slot, RHIVertexBufferView vertexBufferView) 
        {
            /*D3D12_VERTEX_BUFFER_VIEW vertexBufferView;
            vertexBufferView.SizeInBytes = (uint)(vertexBuffer.descriptor.count * vertexBuffer.descriptor.stride);
            vertexBufferView.StrideInBytes = (uint)(vertexBuffer.descriptor.stride);
            vertexBufferView.BufferLocation = ((D3DBuffer)vertexBuffer).defaultResource->GetGPUVirtualAddress();
            nativeCmdList->IASetVertexBuffers(slot, 1, &vertexBufferView);*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetGraphicsResourceBind(in uint slot, RHIResourceSet resourceSet)
        {
            nativeCmdList->SetGraphicsRootDescriptorTable(slot, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawIndexInstanced(in uint indexCount, in uint startIndex, in int startVertex, in uint instanceCount, in uint startInstance)
        {
            nativeCmdList->DrawIndexedInstanced(indexCount, instanceCount, startIndex, startVertex, startInstance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawMultiIndexInstanced(RHIBuffer argsBuffer, in uint argsOffset, RHIBuffer countBuffer, in uint countOffset)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawIndexInstancedIndirect(RHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        protected override void Release()
        {
            nativeCmdList->Release();
            nativeCmdPool->Release();
        }

        public static implicit operator ID3D12GraphicsCommandList5*(D3DCommandBuffer cmdBuffer) { return cmdBuffer.nativeCmdList; }
    }
}
