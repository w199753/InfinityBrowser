using System;
using System.Diagnostics;
using Infinity.Mathmatics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using System.Runtime.InteropServices;

namespace Infinity.Graphics
{
#pragma warning disable CS0414, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
    internal unsafe class Dx12BlitEncoder : RHIBlitEncoder
    {
        private Dx12CommandBuffer m_Dx12CommandBuffer;

        public Dx12BlitEncoder(Dx12CommandBuffer cmdBuffer)
        {
            m_Dx12CommandBuffer = cmdBuffer;
        }

        public override void BeginPass(string name)
        {
            PushDebugGroup(name);
        }

        public override void CopyBufferToBuffer(RHIBuffer srcBuffer, in int srcOffset, RHIBuffer dstBuffer, in int dstOffset, in int size)
        {
            Dx12Buffer dx12SrcBuffer = srcBuffer as Dx12Buffer;
            Dx12Buffer dx12DstBuffer = dstBuffer as Dx12Buffer;
            m_Dx12CommandBuffer.NativeCommandList->CopyBufferRegion(dx12SrcBuffer.NativeResource, (ulong)dstOffset, dx12DstBuffer.NativeResource, (ulong)srcOffset, (ulong)size);
        }

        public override void CopyBufferToTexture(in RHIBufferCopyDescriptor src, in RHITextureCopyDescriptor dst, in int3 size)
        {
            /*Dx12Buffer srcBuffer = src.buffer as Dx12Buffer;
            Dx12Texture dstTexture = dst.texture as Dx12Texture;

            D3D12_TEXTURE_COPY_LOCATION textureLocation = new D3D12_TEXTURE_COPY_LOCATION();
            textureLocation.pResource = dstTexture.NativeResource;
            textureLocation.Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX;
            textureLocation.SubresourceIndex = (uint)(dstTexture.Descriptor.mipCount * dst.arrayLayer + dst.mipLevel);

            m_Dx12CommandBuffer.NativeCommandList->CopyTextureRegion(&textureLocation);*/

            throw new NotImplementedException();
        }

        public override void CopyTextureToBuffer(in RHITextureCopyDescriptor src, in RHIBufferCopyDescriptor dst, in int3 size)
        {
            //Dx12Texture srcTexture = src as Dx12Texture;
            //Dx12Buffer dstBuffer = dst as Dx12Buffer;

            throw new NotImplementedException();
        }

        public override void CopyTextureToTexture(in RHITextureCopyDescriptor src, in RHITextureCopyDescriptor dst, in int3 size)
        {
            //Dx12Texture srcTexture = src as Dx12Texture;
            //Dx12Texture dstTexture = dst as Dx12Texture;

            //D3D12_TEXTURE_COPY_LOCATION srcLocation = new D3D12_TEXTURE_COPY_LOCATION(srcTexture.NativeResource, new D3D12_PLACED_SUBRESOURCE_FOOTPRINT());
            //m_Dx12CommandBuffer.NativeCommandList->CopyTextureRegion();

            throw new NotImplementedException();
        }

        public override void ResourceBarrier(in RHIBarrier barrier)
        {
            ID3D12Resource* resource;
            D3D12_RESOURCE_STATES beforeState;
            D3D12_RESOURCE_STATES afterState;
            if (barrier.Type == EResourceType.Buffer)
            {
                Dx12Buffer buffer = barrier.Buffer.Handle as Dx12Buffer;
                Debug.Assert(buffer != null);

                resource = buffer.NativeResource;
                beforeState = Dx12Utility.ConvertToDx12BufferState(barrier.Buffer.Before);
                afterState = Dx12Utility.ConvertToDx12BufferState(barrier.Buffer.After);
            }
            else
            {
                Dx12Texture texture = barrier.Texture.Handle as Dx12Texture;
                Debug.Assert(texture != null);

                resource = texture.NativeResource;
                beforeState = Dx12Utility.ConvertToDx12TextureState(barrier.Texture.Before);
                afterState = Dx12Utility.ConvertToDx12TextureState(barrier.Texture.After);
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
                    Dx12Buffer buffer = barrier.Buffer.Handle as Dx12Buffer;
                    Debug.Assert(buffer != null);

                    resource = buffer.NativeResource;
                    beforeState = Dx12Utility.ConvertToDx12BufferState(barrier.Buffer.Before);
                    afterState = Dx12Utility.ConvertToDx12BufferState(barrier.Buffer.After);
                }
                else
                {
                    Dx12Texture texture = barrier.Texture.Handle as Dx12Texture;
                    Debug.Assert(texture != null);

                    resource = texture.NativeResource;
                    beforeState = Dx12Utility.ConvertToDx12TextureState(barrier.Texture.Before);
                    afterState = Dx12Utility.ConvertToDx12TextureState(barrier.Texture.After);
                }

                resourceBarriers[i] = D3D12_RESOURCE_BARRIER.InitTransition(resource, beforeState, afterState);
            }

            m_Dx12CommandBuffer.NativeCommandList->ResourceBarrier((uint)barriers.Length, resourceBarriers);
        }

        public override void PushDebugGroup(string name)
        {
            IntPtr namePtr = Marshal.StringToHGlobalUni(name);
            m_Dx12CommandBuffer.NativeCommandList->BeginEvent(0, namePtr.ToPointer(), (uint)name.Length * 2);
            Marshal.FreeHGlobal(namePtr);
        }

        public override void PopDebugGroup()
        {
            m_Dx12CommandBuffer.NativeCommandList->EndEvent();
        }

        public override void EndPass()
        {
            PopDebugGroup();
        }

        protected override void Release()
        {

        }
    }

    internal unsafe class Dx12ComputeEncoder : RHIComputeEncoder
    {
        private Dx12CommandBuffer m_Dx12CommandBuffer;
        //private Dx12PipelineLayout? m_Dx12CachePipelineLayout;
        private Dx12ComputePipeline m_Dx12ComputePipeline;

        public Dx12ComputeEncoder(Dx12CommandBuffer cmdBuffer)
        {
            m_Dx12CommandBuffer = cmdBuffer;
        }

        public override void BeginPass(string name)
        {
            PushDebugGroup(name);
        }

        public override void SetPipeline(RHIComputePipeline pipeline)
        {
            /*m_Dx12ComputePipeline = pipeline as Dx12ComputePipeline;
            Dx12PipelineLayout dx12PipelineLayout = m_Dx12ComputePipeline.PipelineLayout;

            Debug.Assert(dx12PipelineLayout != null);
            if (m_Dx12CachePipelineLayout != dx12PipelineLayout)
            {
                m_Dx12CachePipelineLayout = dx12PipelineLayout;
                m_Dx12CommandBuffer.NativeCommandList->SetComputeRootSignature(dx12PipelineLayout.NativeRootSignature);
            }

            Debug.Assert(m_Dx12ComputePipeline != null);
            m_Dx12CommandBuffer.NativeCommandList->SetPipelineState(m_Dx12ComputePipeline.NativePipelineState);*/

            m_Dx12ComputePipeline = pipeline as Dx12ComputePipeline;

            Debug.Assert(m_Dx12ComputePipeline != null);
            m_Dx12CommandBuffer.NativeCommandList->SetPipelineState(m_Dx12ComputePipeline.NativePipelineState);
            m_Dx12CommandBuffer.NativeCommandList->SetComputeRootSignature(m_Dx12ComputePipeline.PipelineLayout.NativeRootSignature);
        }

        public override void SetBindGroup(RHIBindGroup bindGroup)
        {
            Dx12BindGroup dx12BindGroup = bindGroup as Dx12BindGroup;
            Dx12BindGroupLayout bindGroupLayout = dx12BindGroup.BindGroupLayout;

            Debug.Assert(m_Dx12ComputePipeline != null);

            for (int i = 0; i < dx12BindGroup.BindParameters.Length; ++i) 
            {
                Dx12PipelineLayout pipelineLayout = m_Dx12ComputePipeline.PipelineLayout;
                ref Dx12BindInfo bindInfo = ref bindGroupLayout.BindInfos[i];
                ref Dx12BindGroupParameter bindParameter = ref dx12BindGroup.BindParameters[i];

                Dx12BindTypeAndParameterSlot? parameter = pipelineLayout.QueryRootDescriptorParameterIndex(EShaderStage.Compute, bindGroupLayout.Index, bindInfo.BindSlot, bindInfo.BindType);
                if (parameter.HasValue)
                {
                    Debug.Assert(parameter.Value.BindType == bindInfo.BindType);
                    m_Dx12CommandBuffer.NativeCommandList->SetComputeRootDescriptorTable((uint)parameter.Value.Slot, bindParameter.Dx12GpuDescriptorHandle);
                }
            }
        }

        public override void Dispatch(in uint groupCountX, in uint groupCountY, in uint groupCountZ)
        {
            m_Dx12CommandBuffer.NativeCommandList->Dispatch(groupCountX, groupCountY, groupCountZ);
        }

        public override void DispatchIndirect(RHIBuffer argsBuffer, in uint argsOffset)
        {
            Dx12Buffer dx12Buffer = argsBuffer as Dx12Buffer;
            Dx12Device dx12Device = ((Dx12Queue)m_Dx12CommandBuffer.CommandPool.Queue).Dx12Device;
            m_Dx12CommandBuffer.NativeCommandList->ExecuteIndirect(dx12Device.DispatchIndirectSignature, 1, dx12Buffer.NativeResource, argsOffset, null, 0);
        }

        public override void PushDebugGroup(string name)
        {
            IntPtr namePtr = Marshal.StringToHGlobalUni(name);
            m_Dx12CommandBuffer.NativeCommandList->BeginEvent(0, namePtr.ToPointer(), (uint)name.Length * 2);
            Marshal.FreeHGlobal(namePtr);
        }

        public override void PopDebugGroup()
        {
            m_Dx12CommandBuffer.NativeCommandList->EndEvent();
        }

        public override void EndPass()
        {
            PopDebugGroup();
            //m_Dx12CachePipelineLayout = null;
        }

        protected override void Release()
        {

        }
    }

    internal unsafe class Dx12GraphicsEncoder : RHIGraphicsEncoder
    {
        private Dx12CommandBuffer m_Dx12CommandBuffer;
        //private Dx12PipelineLayout? m_Dx12CachePipelineLayout;
        private Dx12GraphicsPipeline m_Dx12GraphicsPipeline;

        public Dx12GraphicsEncoder(Dx12CommandBuffer cmdBuffer)
        {
            m_Dx12CommandBuffer = cmdBuffer;
        }

        public override void BeginPass(in RHIGraphicsPassDescriptor descriptor)
        {
            PushDebugGroup(descriptor.Name);

            // set render targets
            D3D12_CPU_DESCRIPTOR_HANDLE* rtvHandles = stackalloc D3D12_CPU_DESCRIPTOR_HANDLE[descriptor.ColorAttachmentDescriptors.Length];
            for (int i = 0; i < descriptor.ColorAttachmentDescriptors.Length; ++i)
            {
                Dx12TextureView textureView = descriptor.ColorAttachmentDescriptors.Span[i].RenderTarget as Dx12TextureView;
                Debug.Assert(textureView != null);

                rtvHandles[i] = textureView.NativeCpuDescriptorHandle;
            }

            D3D12_CPU_DESCRIPTOR_HANDLE? dsvHandle = null;
            if (descriptor.DepthStencilAttachmentDescriptor != null)
            {
                Dx12TextureView textureView = descriptor.DepthStencilAttachmentDescriptor?.DepthStencilTarget as Dx12TextureView;
                Debug.Assert(textureView != null);

                dsvHandle = textureView.NativeCpuDescriptorHandle;
            }
            m_Dx12CommandBuffer.NativeCommandList->OMSetRenderTargets((uint)descriptor.ColorAttachmentDescriptors.Length, rtvHandles, false, dsvHandle.HasValue ? (D3D12_CPU_DESCRIPTOR_HANDLE*)&dsvHandle : null);

            // clear render targets
            for (int i = 0; i < descriptor.ColorAttachmentDescriptors.Length; ++i)
            {
                ref RHIColorAttachmentDescriptor colorAttachmentDescriptor = ref descriptor.ColorAttachmentDescriptors.Span[i];

                if (colorAttachmentDescriptor.LoadOp != ELoadOp.Clear)
                {
                    continue;
                }

                float4 clearValue = colorAttachmentDescriptor.ClearValue;
                m_Dx12CommandBuffer.NativeCommandList->ClearRenderTargetView(rtvHandles[i], (float*)&clearValue, 0, null);
            }
            if (dsvHandle.HasValue)
            {
                RHIDepthStencilAttachmentDescriptor? depthStencilAttachmentDescriptor = descriptor.DepthStencilAttachmentDescriptor;
                if (depthStencilAttachmentDescriptor?.DepthLoadOp != ELoadOp.Clear && depthStencilAttachmentDescriptor?.StencilLoadOp != ELoadOp.Clear)
                {
                    return;
                }

                m_Dx12CommandBuffer.NativeCommandList->ClearDepthStencilView(dsvHandle.Value, Dx12Utility.GetDx12ClearFlagByDSA(depthStencilAttachmentDescriptor.Value), depthStencilAttachmentDescriptor.Value.DepthClearValue, Convert.ToByte(depthStencilAttachmentDescriptor.Value.StencilClearValue), 0, null);
            }
            
            if(descriptor.ShadingRateDescriptor.HasValue)
            {
                if(descriptor.ShadingRateDescriptor.Value.ShadingRateTexture != null)
                {
                    D3D12_SHADING_RATE_COMBINER shadingRateCombiner = Dx12Utility.ConvertToDx12ShadingRateCombiner(descriptor.ShadingRateDescriptor.Value.ShadingRateCombiner);
                    Dx12Texture dx12Texture = descriptor.ShadingRateDescriptor.Value.ShadingRateTexture as Dx12Texture;
                    m_Dx12CommandBuffer.NativeCommandList->RSSetShadingRate(Dx12Utility.ConvertToDx12ShadingRate(descriptor.ShadingRateDescriptor.Value.ShadingRate), &shadingRateCombiner);
                    m_Dx12CommandBuffer.NativeCommandList->RSSetShadingRateImage(dx12Texture.NativeResource);
                }
                else
                {
                    //D3D12_SHADING_RATE_COMBINER* shadingRateCombiners = stackalloc D3D12_SHADING_RATE_COMBINER[2] { D3D12_SHADING_RATE_COMBINER.D3D12_SHADING_RATE_COMBINER_MAX, D3D12_SHADING_RATE_COMBINER.D3D12_SHADING_RATE_COMBINER_MAX };
                    //m_Dx12CommandBuffer.NativeCommandList->RSSetShadingRate(Dx12Utility.ConvertToDx12ShadingRate(beginInfo.ShadingRateInfo.Value.ShadingRate), shadingRateCombiners);
                    m_Dx12CommandBuffer.NativeCommandList->RSSetShadingRate(Dx12Utility.ConvertToDx12ShadingRate(descriptor.ShadingRateDescriptor.Value.ShadingRate), null);
                }
            }
        }

        public override void SetPipeline(RHIGraphicsPipeline pipeline)
        {
            /*m_Dx12GraphicsPipeline = pipeline as Dx12GraphicsPipeline;
            Dx12PipelineLayout dx12PipelineLayout = m_Dx12GraphicsPipeline.PipelineLayout;

            Debug.Assert(dx12PipelineLayout != null);
            if(m_Dx12CachePipelineLayout != dx12PipelineLayout)
            {
                m_Dx12CachePipelineLayout = dx12PipelineLayout;
                m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootSignature(dx12PipelineLayout.NativeRootSignature);
            }

            Debug.Assert(m_Dx12GraphicsPipeline != null);
            m_Dx12CommandBuffer.NativeCommandList->SetPipelineState(m_Dx12GraphicsPipeline.NativePipelineState);
            m_Dx12CommandBuffer.NativeCommandList->OMSetStencilRef((uint)m_Dx12GraphicsPipeline.StencilRef);
            m_Dx12CommandBuffer.NativeCommandList->IASetPrimitiveTopology(m_Dx12GraphicsPipeline.PrimitiveTopology);*/

            m_Dx12GraphicsPipeline = pipeline as Dx12GraphicsPipeline;

            Debug.Assert(m_Dx12GraphicsPipeline != null);
            m_Dx12CommandBuffer.NativeCommandList->SetPipelineState(m_Dx12GraphicsPipeline.NativePipelineState);
            m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootSignature(m_Dx12GraphicsPipeline.PipelineLayout.NativeRootSignature);
            m_Dx12CommandBuffer.NativeCommandList->OMSetStencilRef((uint)m_Dx12GraphicsPipeline.StencilRef);
            m_Dx12CommandBuffer.NativeCommandList->IASetPrimitiveTopology(m_Dx12GraphicsPipeline.PrimitiveTopology);
        }

        public override void SetViewport(in Viewport viewport)
        {
            D3D12_VIEWPORT tempViewport = new D3D12_VIEWPORT(viewport.TopLeftX, viewport.TopLeftY, viewport.Width, viewport.Height, viewport.MinDepth, viewport.MaxDepth);
            m_Dx12CommandBuffer.NativeCommandList->RSSetViewports(1, &tempViewport);
        }

        public override void SetViewport(in Memory<Viewport> viewports)
        {
            throw new NotImplementedException();
        }

        public override void SetScissorRect(in Rect rect)
        {
            RECT tempScissor = new RECT((int)rect.left, (int)rect.top, (int)rect.right, (int)rect.bottom);
            m_Dx12CommandBuffer.NativeCommandList->RSSetScissorRects(1, &tempScissor);
        }

        public override void SetScissorRect(in Memory<Rect> rects)
        {
            throw new NotImplementedException();
        }

        public override void SetBlendFactor(in float4 value)
        {
            float4 tempValue = value;
            m_Dx12CommandBuffer.NativeCommandList->OMSetBlendFactor((float*)&tempValue); 
        }

        public override void SetBindGroup(RHIBindGroup bindGroup)
        {
            Dx12BindGroup dx12BindGroup = bindGroup as Dx12BindGroup;
            Dx12BindGroupLayout bindGroupLayout = dx12BindGroup.BindGroupLayout;
            Debug.Assert(m_Dx12GraphicsPipeline != null);

            for (int i = 0; i < dx12BindGroup.BindParameters.Length; ++i)
            {
                Dx12PipelineLayout pipelineLayout = m_Dx12GraphicsPipeline.PipelineLayout;

                Dx12BindTypeAndParameterSlot? parameter = null;
                ref Dx12BindInfo bindInfo = ref bindGroupLayout.BindInfos[i];
                ref Dx12BindGroupParameter bindParameter = ref dx12BindGroup.BindParameters[i];

                parameter = pipelineLayout.QueryRootDescriptorParameterIndex(EShaderStage.Vertex, bindGroupLayout.Index, bindInfo.BindSlot, bindInfo.BindType);
                if (parameter.HasValue)
                {
                    Debug.Assert(parameter.Value.BindType == bindInfo.BindType);
                    m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootDescriptorTable((uint)parameter.Value.Slot, bindParameter.Dx12GpuDescriptorHandle);
                }

                parameter = pipelineLayout.QueryRootDescriptorParameterIndex(EShaderStage.Fragment, bindGroupLayout.Index, bindInfo.BindSlot, bindInfo.BindType);
                if (parameter.HasValue)
                {
                    Debug.Assert(parameter.Value.BindType == bindInfo.BindType);
                    m_Dx12CommandBuffer.NativeCommandList->SetGraphicsRootDescriptorTable((uint)parameter.Value.Slot, bindParameter.Dx12GpuDescriptorHandle);
                }
            }
        }

        public override void SetVertexBuffer(RHIBuffer buffer, in uint slot = 0, in uint offset = 0)
        {
            Dx12Buffer dx12Buffer = buffer as Dx12Buffer;
            D3D12_VERTEX_BUFFER_VIEW vertexBufferView = new D3D12_VERTEX_BUFFER_VIEW
            {
                SizeInBytes = buffer.SizeInBytes - offset,
                StrideInBytes = (uint)m_Dx12GraphicsPipeline.VertexStrides[slot],
                BufferLocation = dx12Buffer.NativeResource->GetGPUVirtualAddress() + offset
            };
            m_Dx12CommandBuffer.NativeCommandList->IASetVertexBuffers(slot, 1, &vertexBufferView);
        }

        public override void SetIndexBuffer(RHIBuffer buffer, in EIndexFormat format, in uint offset = 0)
        {
            Dx12Buffer dx12Buffer = buffer as Dx12Buffer;
            D3D12_INDEX_BUFFER_VIEW indexBufferView = new D3D12_INDEX_BUFFER_VIEW
            {
                Format = Dx12Utility.ConvertToDx12IndexFormat(format),
                SizeInBytes = buffer.SizeInBytes - offset,
                BufferLocation = dx12Buffer.NativeResource->GetGPUVirtualAddress() + offset
            };
            m_Dx12CommandBuffer.NativeCommandList->IASetIndexBuffer(&indexBufferView);
        }

        public override void Draw(in uint vertexCount, in uint instanceCount, in uint firstVertex, in uint firstInstance)
        {
            m_Dx12CommandBuffer.NativeCommandList->DrawInstanced(vertexCount, instanceCount, firstVertex, firstInstance);
        }

        public override void DrawIndexed(in uint indexCount, in uint instanceCount, in uint firstIndex, in uint baseVertex, in uint firstInstance)
        {
            m_Dx12CommandBuffer.NativeCommandList->DrawIndexedInstanced(indexCount, instanceCount, firstIndex, (int)baseVertex, firstInstance);
        }

        public override void DrawIndirect(RHIBuffer argsBuffer, in uint offset)
        {
            Dx12Buffer dx12Buffer = argsBuffer as Dx12Buffer;
            Dx12Device dx12Device = ((Dx12Queue)m_Dx12CommandBuffer.CommandPool.Queue).Dx12Device;
            m_Dx12CommandBuffer.NativeCommandList->ExecuteIndirect(dx12Device.DrawIndirectSignature, 1, dx12Buffer.NativeResource, offset, null, 0);
        }

        public override void DrawIndexedIndirect(RHIBuffer argsBuffer, in uint offset)
        {
            Dx12Buffer dx12Buffer = argsBuffer as Dx12Buffer;
            Dx12Device dx12Device = ((Dx12Queue)m_Dx12CommandBuffer.CommandPool.Queue).Dx12Device;
            m_Dx12CommandBuffer.NativeCommandList->ExecuteIndirect(dx12Device.DrawIndexedIndirectSignature, 1, dx12Buffer.NativeResource, offset, null, 0);
        }

        /*public override void ExecuteBundles(RHIIndirectCommandBuffer indirectCommandBuffer)
        {
            //Dx12IndirectCommandBuffer dx12IndirectCommandBuffer = indirectCommandBuffer as Dx12IndirectCommandBuffer;
            //m_Dx12CommandBuffer.NativeCommandList->ExecuteIndirect(null, indirectCommandBuffer.Count, dx12IndirectCommandBuffer.NativeResource, indirectCommandBuffer.Offset, null, 0);
        }*/

        public override void PushDebugGroup(string name)
        {
            IntPtr namePtr = Marshal.StringToHGlobalUni(name);
            m_Dx12CommandBuffer.NativeCommandList->BeginEvent(0, namePtr.ToPointer(), (uint)name.Length * 2);
            Marshal.FreeHGlobal(namePtr);
        }

        public override void PopDebugGroup()
        {
            m_Dx12CommandBuffer.NativeCommandList->EndEvent();
        }

        public override void EndPass()
        {
            PopDebugGroup();
            //m_Dx12CachePipelineLayout = null;
        }

        protected override void Release()
        {

        }
    }
#pragma warning restore CS0414, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
}
