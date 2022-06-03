using System;
using System.Diagnostics;
using Infinity.Mathmatics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
    internal unsafe class D3DBlitEncoder : RHIBlitEncoder
    {
        private D3DCommandBuffer m_D3DCommandBuffer;

        public D3DBlitEncoder(D3DCommandBuffer cmdBuffer)
        {
            m_D3DCommandBuffer = cmdBuffer;
        }

        public override void BeginPass()
        {
            
        }

        public override void CopyBufferToBuffer(RHIBuffer src, in int srcOffset, RHIBuffer dst, in int dstOffset, in int size)
        {
            D3DBuffer srcBuffer = src as D3DBuffer;
            D3DBuffer dstBuffer = dst as D3DBuffer;
            m_D3DCommandBuffer.NativeCommandList->CopyBufferRegion(dstBuffer.NativeResource, (ulong)dstOffset, srcBuffer.NativeResource, (ulong)srcOffset, (ulong)size);
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
            //D3DTexture srcTexture = src as D3DTexture;
            //D3DTexture dstTexture = dst as D3DTexture;

            //D3D12_TEXTURE_COPY_LOCATION srcLocation = new D3D12_TEXTURE_COPY_LOCATION(srcTexture.NativeResource, new D3D12_PLACED_SUBRESOURCE_FOOTPRINT());
            //m_D3DCommandBuffer.NativeCommandList->CopyTextureRegion();
        }

        public override void ResourceBarrier(in RHIBarrier barrier)
        {
            ID3D12Resource* resource;
            D3D12_RESOURCE_STATES beforeState;
            D3D12_RESOURCE_STATES afterState;
            if (barrier.Type == EResourceType.Buffer)
            {
                D3DBuffer buffer = barrier.Buffer.handle as D3DBuffer;
                Debug.Assert(buffer != null);

                resource = buffer.NativeResource;
                beforeState = D3DUtility.ConvertToNativeBufferState(barrier.Buffer.before);
                afterState = D3DUtility.ConvertToNativeBufferState(barrier.Buffer.after);
            }
            else
            {
                D3DTexture texture = barrier.Texture.handle as D3DTexture;
                Debug.Assert(texture != null);

                resource = texture.NativeResource;
                beforeState = D3DUtility.ConvertToNativeTextureState(barrier.Texture.before);
                afterState = D3DUtility.ConvertToNativeTextureState(barrier.Texture.after);
            }

            D3D12_RESOURCE_BARRIER resourceBarrier = D3D12_RESOURCE_BARRIER.InitTransition(resource, beforeState, afterState);
            m_D3DCommandBuffer.NativeCommandList->ResourceBarrier(1, &resourceBarrier);
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
                    D3DBuffer buffer = barrier.Buffer.handle as D3DBuffer;
                    Debug.Assert(buffer != null);

                    resource = buffer.NativeResource;
                    beforeState = D3DUtility.ConvertToNativeBufferState(barrier.Buffer.before);
                    afterState = D3DUtility.ConvertToNativeBufferState(barrier.Buffer.after);
                }
                else
                {
                    D3DTexture texture = barrier.Texture.handle as D3DTexture;
                    Debug.Assert(texture != null);

                    resource = texture.NativeResource;
                    beforeState = D3DUtility.ConvertToNativeTextureState(barrier.Texture.before);
                    afterState = D3DUtility.ConvertToNativeTextureState(barrier.Texture.after);
                }

                resourceBarriers[i] = D3D12_RESOURCE_BARRIER.InitTransition(resource, beforeState, afterState);
            }

            m_D3DCommandBuffer.NativeCommandList->ResourceBarrier((uint)barriers.Length, resourceBarriers);
        }

        public override void EndPass()
        {

        }

        protected override void Release()
        {
            m_D3DCommandBuffer = null;
        }
    }

    internal unsafe class D3DComputeEncoder : RHIComputeEncoder
    {
        private D3DCommandBuffer m_D3DCommandBuffer;
        private D3DComputePipeline m_D3DComputePipeline;

        public D3DComputeEncoder(D3DCommandBuffer cmdBuffer)
        {
            m_D3DCommandBuffer = cmdBuffer;
        }

        public override void BeginPass()
        {
  
        }

        public override void SetPipeline(RHIComputePipeline pipeline)
        {
            m_D3DComputePipeline = pipeline as D3DComputePipeline;
            Debug.Assert(m_D3DComputePipeline != null);

            m_D3DCommandBuffer.NativeCommandList->SetPipelineState(m_D3DComputePipeline.NativePipelineState);
            m_D3DCommandBuffer.NativeCommandList->SetComputeRootSignature(m_D3DComputePipeline.PipelineLayout.NativeRootSignature);
        }

        public override void SetBindGroup(in int layoutIndex, RHIBindGroup bindGroup)
        {
            D3DBindGroup d3dBindGroup = bindGroup as D3DBindGroup;
            D3DBindGroupLayout bindGroupLayout = d3dBindGroup.BindGroupLayout;
            D3DPipelineLayout pipelineLayout = m_D3DComputePipeline.PipelineLayout;

            Debug.Assert(m_D3DComputePipeline != null);
            Debug.Assert(layoutIndex == bindGroupLayout.LayoutIndex);

            List<D3DBindGroupParameter> bindings = d3dBindGroup.BindingParameters;
            foreach(D3DBindGroupParameter binding in bindings) 
            {
                foreach (EShaderStageFlags shaderStage in Enum.GetValues(typeof(EShaderStageFlags)))
                {
                    if(shaderStage == EShaderStageFlags.MAX) { continue; }

                    D3DBindingTypeAndRootParameterIndex? parameter = pipelineLayout.QueryRootDescriptorParameterIndex(shaderStage, layoutIndex, binding.slot);
                    if (!parameter.HasValue)
                    {
                        return;
                    }
                    Debug.Assert(parameter.Value.bindType == binding.bindType);
                    m_D3DCommandBuffer.NativeCommandList->SetGraphicsRootDescriptorTable((uint)parameter.Value.index, binding.dx12GpuDescriptorHandle);
                }
            }
        }

        public override void Dispatch(in uint groupCountX, in uint groupCountY, in uint groupCountZ)
        {
            m_D3DCommandBuffer.NativeCommandList->Dispatch(groupCountX, groupCountY, groupCountZ);
        }

        public override void EndPass()
        {

        }

        protected override void Release()
        {

        }
    }

    internal unsafe class D3DGraphicsEncoder : RHIGraphicsEncoder
    {
        private D3DCommandBuffer m_D3DCommandBuffer;
        private D3DGraphicsPipeline m_D3DGraphicsPipeline;

        public D3DGraphicsEncoder(D3DCommandBuffer cmdBuffer)
        {
            m_D3DCommandBuffer = cmdBuffer;
        }

        public override void BeginPass(in RHIGraphicsPassBeginInfo beginInfo)
        {
            // set render targets
            D3D12_CPU_DESCRIPTOR_HANDLE* rtvHandles = stackalloc D3D12_CPU_DESCRIPTOR_HANDLE[beginInfo.colorAttachmentCount];
            for (int i = 0; i < beginInfo.colorAttachmentCount; ++i)
            {
                D3DTextureView textureView = beginInfo.colorAttachments.Span[i].view as D3DTextureView;
                Debug.Assert(textureView != null);

                rtvHandles[i] = textureView.NativeCpuDescriptorHandle;
            }

            D3D12_CPU_DESCRIPTOR_HANDLE? dsvHandle = null;
            if (beginInfo.depthStencilAttachment != null)
            {
                D3DTextureView textureView = beginInfo.depthStencilAttachment?.view as D3DTextureView;
                Debug.Assert(textureView != null);

                dsvHandle = textureView.NativeCpuDescriptorHandle;
            }
            m_D3DCommandBuffer.NativeCommandList->OMSetRenderTargets((uint)beginInfo.colorAttachmentCount, rtvHandles, false, dsvHandle.HasValue ? (D3D12_CPU_DESCRIPTOR_HANDLE*)&dsvHandle : null);

            // clear render targets
            for (int i = 0; i < beginInfo.colorAttachmentCount; ++i)
            {
                ref RHIGraphicsPassColorAttachment colorAttachment = ref beginInfo.colorAttachments.Span[i];

                if (colorAttachment.loadOp != ELoadOp.Clear)
                {
                    continue;
                }

                float4 clearValue = colorAttachment.clearValue;
                m_D3DCommandBuffer.NativeCommandList->ClearRenderTargetView(rtvHandles[i], (float*)&clearValue, 0, null);
            }
            if (dsvHandle.HasValue)
            {
                RHIGraphicsPassDepthStencilAttachment? depthStencilAttachment = beginInfo.depthStencilAttachment;
                if (depthStencilAttachment?.depthLoadOp != ELoadOp.Clear && depthStencilAttachment?.stencilLoadOp != ELoadOp.Clear)
                {
                    return;
                }

                m_D3DCommandBuffer.NativeCommandList->ClearDepthStencilView(dsvHandle.Value, D3DUtility.GetDX12ClearFlagByDSA(depthStencilAttachment.Value), depthStencilAttachment.Value.depthClearValue, Convert.ToByte(depthStencilAttachment.Value.stencilClearValue), 0, null);
            }
        }

        public override void SetPipeline(RHIGraphicsPipeline pipeline)
        {
            m_D3DGraphicsPipeline = pipeline as D3DGraphicsPipeline;
            Debug.Assert(m_D3DGraphicsPipeline != null);

            m_D3DCommandBuffer.NativeCommandList->SetPipelineState(m_D3DGraphicsPipeline.NativePipelineState);
            m_D3DCommandBuffer.NativeCommandList->SetGraphicsRootSignature(m_D3DGraphicsPipeline.PipelineLayout.NativeRootSignature);
        }

        public override void SetScissor(in uint left, in uint top, in uint right, in uint bottom)
        {
            RECT scissor = new RECT((int)left, (int)top, (int)right, (int)bottom);
            m_D3DCommandBuffer.NativeCommandList->RSSetScissorRects(1, &scissor);
        }

        public override void SetViewport(in float x, in float y, in float width, in float height, in float minDepth, in float maxDepth)
        {
            D3D12_VIEWPORT viewport = new D3D12_VIEWPORT(x, y, width, height, minDepth, maxDepth);
            m_D3DCommandBuffer.NativeCommandList->RSSetViewports(1, &viewport);
        }

        public override void SetBlendConstant(in float constants)
        {
            float tempConstants = constants;
            m_D3DCommandBuffer.NativeCommandList->OMSetBlendFactor(&tempConstants);
        }

        public override void SetStencilReference(in uint reference)
        {
            m_D3DCommandBuffer.NativeCommandList->OMSetStencilRef(reference);
        }

        public override void SetBindGroup(in uint layoutIndex, RHIBindGroup bindGroup)
        {
            throw new NotImplementedException();
        }

        public override void SetIndexBuffer(RHIBufferView bufferView)
        {
            D3DBufferView d3dBufferView = bufferView as D3DBufferView;
            D3D12_INDEX_BUFFER_VIEW indexBufferView = d3dBufferView.NativeIndexBufferView;
            m_D3DCommandBuffer.NativeCommandList->IASetIndexBuffer(&indexBufferView);
        }

        public override void SetVertexBuffer(in uint slot, RHIBufferView bufferView)
        {
            D3DBufferView d3dBufferView = bufferView as D3DBufferView;
            D3D12_VERTEX_BUFFER_VIEW vertexBufferView = d3dBufferView.NativeVertexBufferView;
            m_D3DCommandBuffer.NativeCommandList->IASetVertexBuffers(slot, 1, &vertexBufferView);
        }

        public override void SetPrimitiveTopology(in EPrimitiveTopology primitiveTopology)
        {
            m_D3DCommandBuffer.NativeCommandList->IASetPrimitiveTopology(D3DUtility.ConvertToNativePrimitiveTopology(primitiveTopology));
        }

        public override void Draw(in uint vertexCount, in uint instanceCount, in uint firstVertex, in uint firstInstance)
        {
            m_D3DCommandBuffer.NativeCommandList->DrawInstanced(vertexCount, instanceCount, firstVertex, firstInstance);
        }

        public override void DrawIndexed(in uint indexCount, in uint instanceCount, in uint firstIndex, in uint baseVertex, in uint firstInstance)
        {
            m_D3DCommandBuffer.NativeCommandList->DrawIndexedInstanced(indexCount, instanceCount, firstIndex, (int)baseVertex, firstInstance);
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
