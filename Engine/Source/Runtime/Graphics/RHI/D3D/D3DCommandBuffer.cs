using System;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using InfinityEngine.Core.Mathmatics;
using System.Runtime.CompilerServices;
using InfinityEngine.Core.Mathmatics.Geometry;
using System.Runtime.InteropServices;

namespace InfinityEngine.Graphics.RHI.D3D
{
    public unsafe class FD3DCommandBuffer : FRHICommandBuffer
    {
        internal ID3D12CommandAllocator* nativeCmdPool;
        internal ID3D12GraphicsCommandList5* nativeCmdList;

        internal FD3DCommandBuffer(string name, FRHIDevice device, FRHICommandContext cmdContext, in EContextType contextType) : base(name, device, contextType)
        {
            FD3DDevice d3dDevice = (FD3DDevice)device;
            FD3DCommandContext d3dCmdContext = (FD3DCommandContext)cmdContext;
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
        public override void BeginQuery(FRHIQuery query)
        {
            FD3DQuery d3dQuery = (FD3DQuery)query;
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
        public override void EndQuery(FRHIQuery query)
        {
            FD3DQuery d3dQuery = (FD3DQuery)query;
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
        public override void Transition(FRHIResource resource, EResourceState stateBefore, EResourceState stateAfter, int subresource = -1)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ClearBuffer(FRHIBuffer buffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ClearTexture(FRHITexture texture)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyBufferToBuffer(FRHIBuffer srcBuffer, FRHIBuffer dscBuffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyBufferToTexture(FRHIBuffer srcBuffer, FRHITexture dscTexture)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyTextureToBuffer(FRHITexture srcTexture, FRHIBuffer dscBuffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CopyTextureToTexture(FRHITexture srcTexture, FRHITexture dscTexture)
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
        public override void SetComputePipelineState(FRHIComputePipelineState computePipelineState)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetComputeResourceBind(in uint slot, FRHIResourceSet resourceSet)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchCompute(in uint sizeX, in uint sizeY, in uint sizeZ)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchComputeIndirect(FRHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetRayTracePipelineState(FRHIRayTracePipelineState rayTracePipelineState)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetRayTraceResourceBind(in uint slot, FRHIResourceSet resourceSet)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchRay(in uint sizeX, in uint sizeY, in uint sizeZ)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DispatchRayIndirect(FRHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetScissor(in FRect rect)
        {
            nativeCmdList->RSSetScissorRects(1, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetViewport(in FViewport viewport)
        {
            nativeCmdList->RSSetViewports(1, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void BeginRenderPass(FRHITexture depthBuffer, params FRHITexture[] colorBuffer)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndRenderPass()
        {
            nativeCmdList->EndRenderPass();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ClearRenderTarget(FRHIRenderTargetView renderTargetView, in float4 color)
        {
            float4 copyColor = color;
            FD3DRenderTargetView d3dRTV = (FD3DRenderTargetView)renderTargetView;
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
        public override void SetShadingRate(FRHITexture texture)
        {
            FD3DTexture d3dTexture = (FD3DTexture)texture;
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
        public override void SetGraphicsPipelineState(FRHIGraphicsPipelineState graphicsPipelineState)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetIndexBuffer(FRHIIndexBufferView indexBufferView) 
        {
            /*D3D12_INDEX_BUFFER_VIEW indexBufferView;
            indexBufferView.Format = DXGI_FORMAT.DXGI_FORMAT_R32_UINT;
            indexBufferView.SizeInBytes = (uint)(indexBuffer.descriptor.count * indexBuffer.descriptor.stride);
            indexBufferView.BufferLocation = ((FD3DBuffer)indexBuffer).defaultResource->GetGPUVirtualAddress();
            nativeCmdList->IASetIndexBuffer(&indexBufferView);*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetVertexBuffer(in uint slot, FRHIVertexBufferView vertexBufferView) 
        {
            /*D3D12_VERTEX_BUFFER_VIEW vertexBufferView;
            vertexBufferView.SizeInBytes = (uint)(vertexBuffer.descriptor.count * vertexBuffer.descriptor.stride);
            vertexBufferView.StrideInBytes = (uint)(vertexBuffer.descriptor.stride);
            vertexBufferView.BufferLocation = ((FD3DBuffer)vertexBuffer).defaultResource->GetGPUVirtualAddress();
            nativeCmdList->IASetVertexBuffers(slot, 1, &vertexBufferView);*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetGraphicsResourceBind(in uint slot, FRHIResourceSet resourceSet)
        {
            nativeCmdList->SetGraphicsRootDescriptorTable(slot, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawIndexInstanced(in uint indexCount, in uint startIndex, in int startVertex, in uint instanceCount, in uint startInstance)
        {
            nativeCmdList->DrawIndexedInstanced(indexCount, instanceCount, startIndex, startVertex, startInstance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawMultiIndexInstanced(FRHIBuffer argsBuffer, in uint argsOffset, FRHIBuffer countBuffer, in uint countOffset)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void DrawIndexInstancedIndirect(FRHIBuffer argsBuffer, in uint argsOffset)
        {

        }

        protected override void Release()
        {
            nativeCmdList->Release();
            nativeCmdPool->Release();
        }

        public static implicit operator ID3D12GraphicsCommandList5*(FD3DCommandBuffer cmdBuffer) { return cmdBuffer.nativeCmdList; }
    }
}
