using System;
using System.Diagnostics;
using Infinity.Mathmatics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
    internal unsafe class Dx12BlitEncoder : RHIBlitEncoder
    {
        private Dx12CommandBuffer m_Dx12CommandBuffer;

        public Dx12BlitEncoder(Dx12CommandBuffer cmdBuffer)
        {
            m_Dx12CommandBuffer = cmdBuffer;
        }

        public override void BeginPass()
        {
            
        }

        public override void CopyBufferToBuffer(RHIBuffer src, in int srcOffset, RHIBuffer dst, in int dstOffset, in int size)
        {
            Dx12Buffer srcBuffer = src as Dx12Buffer;
            Dx12Buffer dstBuffer = dst as Dx12Buffer;
            m_Dx12CommandBuffer.NativeCommandList->CopyBufferRegion(dstBuffer.NativeResource, (ulong)dstOffset, srcBuffer.NativeResource, (ulong)srcOffset, (ulong)size);
        }

        public override void CopyBufferToTexture(RHIBuffer src, RHITexture dst, in RHITextureSubResourceInfo subResourceInfo, in int3 size)
        {
            throw new NotImplementedException();
        }

        public override void CopyTextureToBuffer(RHITexture src, RHIBuffer dst, in RHITextureSubResourceInfo subResourceInfo, in int3 size)
        {
            throw new NotImplementedException();
        }

        public override void CopyTextureToTexture(RHITexture src, in RHITextureSubResourceInfo srcSubResourceInfo, RHITexture dst, in RHITextureSubResourceInfo dstSubResourceInfo, in int3 size)
        {
            //Dx12Texture srcTexture = src as Dx12Texture;
            //Dx12Texture dstTexture = dst as Dx12Texture;

            //D3D12_TEXTURE_COPY_LOCATION srcLocation = new D3D12_TEXTURE_COPY_LOCATION(srcTexture.NativeResource, new D3D12_PLACED_SUBRESOURCE_FOOTPRINT());
            //m_Dx12CommandBuffer.NativeCommandList->CopyTextureRegion();
        }

        public override void ResourceBarrier(in RHIBarrier barrier)
        {
            ID3D12Resource* resource;
            D3D12_RESOURCE_STATES beforeState;
            D3D12_RESOURCE_STATES afterState;
            if (barrier.Type == EResourceType.Buffer)
            {
                Dx12Buffer buffer = barrier.Buffer.handle as Dx12Buffer;
                Debug.Assert(buffer != null);

                resource = buffer.NativeResource;
                beforeState = Dx12Utility.ConvertToNativeBufferState(barrier.Buffer.before);
                afterState = Dx12Utility.ConvertToNativeBufferState(barrier.Buffer.after);
            }
            else
            {
                Dx12Texture texture = barrier.Texture.handle as Dx12Texture;
                Debug.Assert(texture != null);

                resource = texture.NativeResource;
                beforeState = Dx12Utility.ConvertToNativeTextureState(barrier.Texture.before);
                afterState = Dx12Utility.ConvertToNativeTextureState(barrier.Texture.after);
            }

            D3D12_RESOURCE_BARRIER resourceBarrier = D3D12_RESOURCE_BARRIER.InitTransition(resource, beforeState, afterState);
            m_Dx12CommandBuffer.NativeCommandList->ResourceBarrier(1, &resourceBarrier);
        }

        public override void ResourceBarrier(in Memory<RHIBarrier> barriers)
        {
            ID3D12Resource* resource;
            D3D12_RESOURCE_STATES beforeState;
            D3D12_RESOURCE_STATES afterState;
            D3D12_RESOURCE_BARRIER* resourceBarriers = stackalloc D3D12_RESOURCE_BARRIER[barriers.Length];

            for(int i = 0; i < barriers.Length; ++i)
            {
                ref RHIBarrier barrier = ref barriers.Span[i];

                if (barrier.Type == EResourceType.Buffer)
                {
                    Dx12Buffer buffer = barrier.Buffer.handle as Dx12Buffer;
                    Debug.Assert(buffer != null);

                    resource = buffer.NativeResource;
                    beforeState = Dx12Utility.ConvertToNativeBufferState(barrier.Buffer.before);
                    afterState = Dx12Utility.ConvertToNativeBufferState(barrier.Buffer.after);
                }
                else
                {
                    Dx12Texture texture = barrier.Texture.handle as Dx12Texture;
                    Debug.Assert(texture != null);

                    resource = texture.NativeResource;
                    beforeState = Dx12Utility.ConvertToNativeTextureState(barrier.Texture.before);
                    afterState = Dx12Utility.ConvertToNativeTextureState(barrier.Texture.after);
                }

                resourceBarriers[i] = D3D12_RESOURCE_BARRIER.InitTransition(resource, beforeState, afterState);
            }

            m_Dx12CommandBuffer.NativeCommandList->ResourceBarrier((uint)barriers.Length, resourceBarriers);
        }

        public override void EndPass()
        {

        }

        protected override void Release()
        {

        }
    }

    internal unsafe class Dx12ComputeEncoder : RHIComputeEncoder
    {
        private Dx12CommandBuffer m_Dx12CommandBuffer;
        private Dx12ComputePipeline m_Dx12ComputePipeline;

        public Dx12ComputeEncoder(Dx12CommandBuffer cmdBuffer)
        {
            m_Dx12CommandBuffer = cmdBuffer;
        }

        public override void BeginPass()
        {
  
        }

        public override void SetPipeline(RHIComputePipeline pipeline)
        {
            m_Dx12ComputePipeline = pipeline as Dx12ComputePipeline;
            Debug.Assert(m_Dx12ComputePipeline != null);

            m_Dx12CommandBuffer.NativeCommandList->SetPipelineState(m_Dx12ComputePipeline.NativePipelineState);
            m_Dx12CommandBuffer.NativeCommandList->SetComputeRootSignature(m_Dx12ComputePipeline.PipelineLayout.NativeRootSignature);
        }

        public override void SetBindGroup(in int layoutIndex, RHIBindGroup bindGroup)
        {
            Dx12BindGroup dx12BindGroup = bindGroup as Dx12BindGroup;
            Dx12BindGroupLayout bindGroupLayout = dx12BindGroup.BindGroupLayout;
            Dx12PipelineLayout pipelineLayout = m_Dx12ComputePipeline.PipelineLayout;

            Debug.Assert(m_Dx12ComputePipeline != null);
            Debug.Assert(layoutIndex == bindGroupLayout.LayoutIndex);

            for (int i = 0; i < dx12BindGroup.BindParameters.Length; ++i) 
            {
                ref Dx12BindGroupParameter bindParameter = ref dx12BindGroup.BindParameters[i];

                Dx12BindingTypeAndRootParameterIndex? parameter = pipelineLayout.QueryRootDescriptorParameterIndex(EShaderStageFlags.Compute, layoutIndex, bindParameter.slot);
                if (parameter.HasValue)
                {
                    Debug.Assert(parameter.Value.bindType == bindParameter.bindType);
                    m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootDescriptorTable((uint)parameter.Value.index, bindParameter.dx12GpuDescriptorHandle);
                }
            }
        }

        public override void Dispatch(in uint groupCountX, in uint groupCountY, in uint groupCountZ)
        {
            m_Dx12CommandBuffer.NativeCommandList->Dispatch(groupCountX, groupCountY, groupCountZ);
        }

        public override void EndPass()
        {

        }

        protected override void Release()
        {

        }
    }

    internal unsafe class Dx12GraphicsEncoder : RHIGraphicsEncoder
    {
        private Dx12CommandBuffer m_Dx12CommandBuffer;
        private Dx12GraphicsPipeline m_Dx12GraphicsPipeline;

        public Dx12GraphicsEncoder(Dx12CommandBuffer cmdBuffer)
        {
            m_Dx12CommandBuffer = cmdBuffer;
        }

        public override void BeginPass(in RHIGraphicsPassBeginInfo beginInfo)
        {
            // set render targets
            D3D12_CPU_DESCRIPTOR_HANDLE* rtvHandles = stackalloc D3D12_CPU_DESCRIPTOR_HANDLE[beginInfo.colorAttachmentCount];
            for (int i = 0; i < beginInfo.colorAttachmentCount; ++i)
            {
                Dx12TextureView textureView = beginInfo.colorAttachments.Span[i].view as Dx12TextureView;
                Debug.Assert(textureView != null);

                rtvHandles[i] = textureView.NativeCpuDescriptorHandle;
            }

            D3D12_CPU_DESCRIPTOR_HANDLE? dsvHandle = null;
            if (beginInfo.depthStencilAttachment != null)
            {
                Dx12TextureView textureView = beginInfo.depthStencilAttachment?.view as Dx12TextureView;
                Debug.Assert(textureView != null);

                dsvHandle = textureView.NativeCpuDescriptorHandle;
            }
            m_Dx12CommandBuffer.NativeCommandList->OMSetRenderTargets((uint)beginInfo.colorAttachmentCount, rtvHandles, false, dsvHandle.HasValue ? (D3D12_CPU_DESCRIPTOR_HANDLE*)&dsvHandle : null);

            // clear render targets
            for (int i = 0; i < beginInfo.colorAttachmentCount; ++i)
            {
                ref RHIGraphicsPassColorAttachment colorAttachment = ref beginInfo.colorAttachments.Span[i];

                if (colorAttachment.loadOp != ELoadOp.Clear)
                {
                    continue;
                }

                float4 clearValue = colorAttachment.clearValue;
                m_Dx12CommandBuffer.NativeCommandList->ClearRenderTargetView(rtvHandles[i], (float*)&clearValue, 0, null);
            }
            if (dsvHandle.HasValue)
            {
                RHIGraphicsPassDepthStencilAttachment? depthStencilAttachment = beginInfo.depthStencilAttachment;
                if (depthStencilAttachment?.depthLoadOp != ELoadOp.Clear && depthStencilAttachment?.stencilLoadOp != ELoadOp.Clear)
                {
                    return;
                }

                m_Dx12CommandBuffer.NativeCommandList->ClearDepthStencilView(dsvHandle.Value, Dx12Utility.GetDX12ClearFlagByDSA(depthStencilAttachment.Value), depthStencilAttachment.Value.depthClearValue, Convert.ToByte(depthStencilAttachment.Value.stencilClearValue), 0, null);
            }
        }

        public override void SetPipeline(RHIGraphicsPipeline pipeline)
        {
            m_Dx12GraphicsPipeline = pipeline as Dx12GraphicsPipeline;
            Debug.Assert(m_Dx12GraphicsPipeline != null);

            m_Dx12CommandBuffer.NativeCommandList->SetPipelineState(m_Dx12GraphicsPipeline.NativePipelineState);
            m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootSignature(m_Dx12GraphicsPipeline.PipelineLayout.NativeRootSignature);
        }

        public override void SetScissor(in uint left, in uint top, in uint right, in uint bottom)
        {
            RECT scissor = new RECT((int)left, (int)top, (int)right, (int)bottom);
            m_Dx12CommandBuffer.NativeCommandList->RSSetScissorRects(1, &scissor);
        }

        public override void SetViewport(in float x, in float y, in float width, in float height, in float minDepth, in float maxDepth)
        {
            D3D12_VIEWPORT viewport = new D3D12_VIEWPORT(x, y, width, height, minDepth, maxDepth);
            m_Dx12CommandBuffer.NativeCommandList->RSSetViewports(1, &viewport);
        }

        public override void SetBlendConstant(in float constants)
        {
            float tempConstants = constants;
            m_Dx12CommandBuffer.NativeCommandList->OMSetBlendFactor(&tempConstants);
        }

        public override void SetStencilReference(in uint reference)
        {
            m_Dx12CommandBuffer.NativeCommandList->OMSetStencilRef(reference);
        }

        public override void SetBindGroup(in int layoutIndex, RHIBindGroup bindGroup)
        {
            Dx12BindGroup dx12BindGroup = bindGroup as Dx12BindGroup;
            Dx12BindGroupLayout bindGroupLayout = dx12BindGroup.BindGroupLayout;
            Dx12PipelineLayout pipelineLayout = m_Dx12GraphicsPipeline.PipelineLayout;

            Debug.Assert(m_Dx12GraphicsPipeline != null);
            Debug.Assert(layoutIndex == bindGroupLayout.LayoutIndex);

            for (int i = 0; i < dx12BindGroup.BindParameters.Length; ++i)
            {
                Dx12BindingTypeAndRootParameterIndex? parameter = null;
                ref Dx12BindGroupParameter bindParameter = ref dx12BindGroup.BindParameters[i];

                parameter = pipelineLayout.QueryRootDescriptorParameterIndex(EShaderStageFlags.Vertex, layoutIndex, bindParameter.slot);
                if (parameter.HasValue)
                {
                    Debug.Assert(parameter.Value.bindType == bindParameter.bindType);
                    m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootDescriptorTable((uint)parameter.Value.index, bindParameter.dx12GpuDescriptorHandle);
                }

                parameter = pipelineLayout.QueryRootDescriptorParameterIndex(EShaderStageFlags.Fragment, layoutIndex, bindParameter.slot);
                if (parameter.HasValue)
                {
                    Debug.Assert(parameter.Value.bindType == bindParameter.bindType);
                    m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootDescriptorTable((uint)parameter.Value.index, bindParameter.dx12GpuDescriptorHandle);
                }
            }
        }

        public override void SetIndexBuffer(RHIBufferView bufferView)
        {
            Dx12BufferView dx12BufferView = bufferView as Dx12BufferView;
            D3D12_INDEX_BUFFER_VIEW indexBufferView = dx12BufferView.NativeIndexBufferView;
            m_Dx12CommandBuffer.NativeCommandList->IASetIndexBuffer(&indexBufferView);
        }

        public override void SetVertexBuffer(in uint slot, RHIBufferView bufferView)
        {
            Dx12BufferView dx12BufferView = bufferView as Dx12BufferView;
            D3D12_VERTEX_BUFFER_VIEW vertexBufferView = dx12BufferView.NativeVertexBufferView;
            m_Dx12CommandBuffer.NativeCommandList->IASetVertexBuffers(slot, 1, &vertexBufferView);
        }

        public override void SetPrimitiveTopology(in EPrimitiveTopology primitiveTopology)
        {
            m_Dx12CommandBuffer.NativeCommandList->IASetPrimitiveTopology(Dx12Utility.ConvertToNativePrimitiveTopology(primitiveTopology));
        }

        public override void Draw(in uint vertexCount, in uint instanceCount, in uint firstVertex, in uint firstInstance)
        {
            m_Dx12CommandBuffer.NativeCommandList->DrawInstanced(vertexCount, instanceCount, firstVertex, firstInstance);
        }

        public override void DrawIndexed(in uint indexCount, in uint instanceCount, in uint firstIndex, in uint baseVertex, in uint firstInstance)
        {
            m_Dx12CommandBuffer.NativeCommandList->DrawIndexedInstanced(indexCount, instanceCount, firstIndex, (int)baseVertex, firstInstance);
        }

        public override void EndPass()
        {
            
        }

        protected override void Release()
        {

        }
    }
#pragma warning restore CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
}
