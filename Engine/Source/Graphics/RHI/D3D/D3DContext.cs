using System;
using InfinityEngine.Container;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics
{
    public unsafe class D3DContext : RHIContext
    {
        public override bool copyQueueState
        {
            get
            {
                return m_CopyContext.IsReady;
            }
        }
        public override ulong copyFrequency
        {
            get
            {
                ulong frequency;
                m_CopyContext.nativeCmdQueue->GetTimestampFrequency(&frequency);
                return frequency;
            }
        }
        public override ulong computeFrequency
        {
            get
            {
                ulong frequency;
                m_ComputeContext.nativeCmdQueue->GetTimestampFrequency(&frequency);
                return frequency;
            }
        }
        public override ulong graphicsFrequency
        {
            get
            {
                ulong frequency;
                m_GraphicsContext.nativeCmdQueue->GetTimestampFrequency(&frequency);
                return frequency;
            }
        }

        internal D3DDevice m_Device;
        internal RHIFencePool m_FencePool;
        internal RHIResourcePool m_ResourcePool;
        internal D3DQueryContext[] m_QueryContext;
        internal D3DCommandContext m_CopyContext;
        internal D3DCommandContext m_ComputeContext;
        internal D3DCommandContext m_GraphicsContext;
        internal TArray<FExecuteInfo> m_ExecuteInfos;
        internal RHICommandBufferPool m_CopyBufferPool;
        internal RHICommandBufferPool m_ComputeBufferPool;
        internal RHICommandBufferPool m_GraphicsBufferPool;
        internal TArray<RHICommandBuffer> m_ManagedBuffers;
        internal D3DDescriptorHeapFactory m_DSVDescriptorFactory;
        internal D3DDescriptorHeapFactory m_RTVDescriptorFactory;
        internal D3DDescriptorHeapFactory m_SamplerDescriptorFactory;
        internal D3DDescriptorHeapFactory m_CbvSrvUavDescriptorFactory;

        public D3DContext()
        {
            m_Device = new D3DDevice();
            m_FencePool = new RHIFencePool(this);
            m_ResourcePool = new RHIResourcePool(this);
            m_ExecuteInfos = new TArray<FExecuteInfo>(32);
            m_ManagedBuffers = new TArray<RHICommandBuffer>(32);

            m_CopyContext = new D3DCommandContext(m_Device, EContextType.Copy, "Copy");
            m_ComputeContext = new D3DCommandContext(m_Device, EContextType.Compute, "Compute");
            m_GraphicsContext = new D3DCommandContext(m_Device, EContextType.Graphics, "Graphics");

            m_QueryContext = new D3DQueryContext[2];
            m_QueryContext[0] = new D3DQueryContext(m_Device, SelectContext(EContextType.Copy), EQueryType.CopyTimestamp, 128, "CopyTimestamp");
            m_QueryContext[1] = new D3DQueryContext(m_Device, SelectContext(EContextType.Graphics), EQueryType.GenericTimestamp, 128, "GenericTimestamp");

            m_CopyBufferPool = new RHICommandBufferPool(this, EContextType.Copy);
            m_ComputeBufferPool = new RHICommandBufferPool(this, EContextType.Compute);
            m_GraphicsBufferPool = new RHICommandBufferPool(this, EContextType.Graphics);

            //TerraFX.Interop.D3D12MemAlloc.D3D12MA_CreateAllocator
            m_DSVDescriptorFactory = new D3DDescriptorHeapFactory(m_Device, EDescriptorType.DSV, 256, "DSVDescriptorHeap");
            m_RTVDescriptorFactory = new D3DDescriptorHeapFactory(m_Device, EDescriptorType.RTV, 256, "RTVDescriptorHeap");
            m_SamplerDescriptorFactory = new D3DDescriptorHeapFactory(m_Device, EDescriptorType.Sampler, 32768, "SamplerDescriptorHeap");
            m_CbvSrvUavDescriptorFactory = new D3DDescriptorHeapFactory(m_Device, EDescriptorType.CbvSrvUav, 32768, "CbvSrvUavDescriptorHeap");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override RHICommandContext SelectContext(in EContextType contextType)
        {
            RHICommandContext commandContext = m_GraphicsContext;

            switch (contextType)
            {
                case EContextType.Copy:
                    commandContext = m_CopyContext;
                    break;

                case EContextType.Compute:
                    commandContext = m_ComputeContext;
                    break;
            }

            return commandContext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHICommandBuffer CreateCommandBuffer(in EContextType contextType, string name)
        {
            return new D3DCommandBuffer(name, m_Device, SelectContext(contextType), contextType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHICommandBuffer GetCommandBuffer(in EContextType contextType, string name)
        {
            RHICommandBuffer cmdBuffer = null;
            switch (contextType)
            {
                case EContextType.Copy:
                    cmdBuffer = m_CopyBufferPool.GetTemporary(name);
                    break;

                case EContextType.Compute:
                    cmdBuffer = m_ComputeBufferPool.GetTemporary(name);
                    break;

                case EContextType.Graphics:
                    cmdBuffer = m_GraphicsBufferPool.GetTemporary(name);
                    break;
            }

            cmdBuffer.poolIndex = m_ManagedBuffers.Add(cmdBuffer);

            return cmdBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ReleaseCommandBuffer(RHICommandBuffer cmdBuffer)
        {
            switch (cmdBuffer.contextType)
            {
                case EContextType.Copy:
                    m_CopyBufferPool.ReleaseTemporary(cmdBuffer);
                    break;

                case EContextType.Compute:
                    m_ComputeBufferPool.ReleaseTemporary(cmdBuffer);
                    break;

                case EContextType.Graphics:
                    m_GraphicsBufferPool.ReleaseTemporary(cmdBuffer);
                    break;
            }

            m_ManagedBuffers.RemoveSwapAtIndex(cmdBuffer.poolIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteToFence(in EContextType contextType, RHIFence fence)
        {
#if RHI_DEFERREDEXECUTE
                FExecuteInfo executeInfo;
                executeInfo.fence = fence;
                executeInfo.cmdBuffer = null;
                executeInfo.executeType = EExecuteType.Signal;
                executeInfo.cmdContext = SelectContext(contextType);
                m_ExecuteInfos.Add(executeInfo);
#else
            SelectContext(contextType).SignalQueue(fence);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WaitForFence(in EContextType contextType, RHIFence fence)
        {
#if RHI_DEFERREDEXECUTE
                FExecuteInfo executeInfo;
                executeInfo.fence = fence;
                executeInfo.cmdBuffer = null;
                executeInfo.executeType = EExecuteType.Wait;
                executeInfo.cmdContext = SelectContext(contextType);
                m_ExecuteInfos.Add(executeInfo);
#else
            SelectContext(contextType).WaitQueue(fence);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ExecuteCommandBuffer(RHICommandBuffer cmdBuffer)
        {
#if RHI_DEFERREDEXECUTE
                cmdBuffer.Close();
                FExecuteInfo executeInfo;
                executeInfo.fence = null;
                executeInfo.cmdBuffer = cmdBuffer;
                executeInfo.executeType = EExecuteType.Execute;
                executeInfo.cmdContext = SelectContext(cmdBuffer.contextType);
                m_ExecuteInfos.Add(executeInfo);
#else
            SelectContext(cmdBuffer.contextType).ExecuteQueue(cmdBuffer);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void Submit()
        {
#if RHI_DEFERREDEXECUTE
            for (int i = 0; i < m_ExecuteInfos.length; ++i)
            {
                ref FExecuteInfo executeInfo = ref m_ExecuteInfos[i];
                D3DCommandContext cmdContext = (D3DCommandContext)executeInfo.cmdContext;

                switch (executeInfo.executeType)
                {
                    case EExecuteType.Signal:
                        cmdContext.SignalQueue(executeInfo.fence);
                        break;

                    case EExecuteType.Wait:
                        cmdContext.WaitQueue(executeInfo.fence);
                        break;

                    case EExecuteType.Execute:
                        cmdContext.ExecuteQueue(executeInfo.cmdBuffer);
                        break;
                }
            }
            m_ExecuteInfos.Clear();
#endif

            #region release temporal resource
            for (int i = 0; i < m_ManagedBuffers.length; ++i) 
            {
                ReleaseCommandBuffer(m_ManagedBuffers[i]);
                m_ManagedBuffers[i] = null;
            }
            m_ManagedBuffers.Clear();
            #endregion release temporal resource

            m_QueryContext[0].Submit(m_CopyContext);
            m_QueryContext[1].Submit(m_GraphicsContext);

            m_ComputeContext.Flush();
            m_GraphicsContext.Flush();

            m_CopyContext.AsyncFlush();

            m_QueryContext[0].ResolveData();
            m_QueryContext[1].ResolveData();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHISwapChain CreateSwapChain(string name, in uint width, in uint height, in IntPtr windowPtr)
        {
            return new D3DSwapChain(m_Device, m_GraphicsContext, windowPtr.ToPointer(), width, height, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIFence CreateFence(string name)
        {
            return new D3DFence(m_Device, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIFence GetFence(string name)
        {
            return m_FencePool.GetTemporary(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ReleaseFence(RHIFence fence)
        {
            m_FencePool.ReleaseTemporary((D3DFence)fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIQuery CreateQuery(in EQueryType queryType, string name)
        {
            D3DQuery outQuery = null;
            switch (queryType)
            {
                case EQueryType.Occlusion:
                    outQuery = null;
                    break;

                case EQueryType.Statistic:
                    outQuery = null;
                    break;

                case EQueryType.CopyTimestamp:
                    outQuery = new D3DQuery(m_QueryContext[0]);
                    break;

                case EQueryType.GenericTimestamp:
                    outQuery = new D3DQuery(m_QueryContext[1]);
                    break;
            }
            return outQuery;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIQuery GetQuery(in EQueryType queryType, string name)
        {
            RHIQuery outQuery = null;
            switch (queryType)
            {
                case EQueryType.Occlusion:
                    outQuery = null;
                    break;

                case EQueryType.Statistic:
                    outQuery = null;
                    break;

                case EQueryType.CopyTimestamp:
                    outQuery = m_QueryContext[0].GetTemporary(name);
                    break;

                case EQueryType.GenericTimestamp:
                    outQuery = m_QueryContext[1].GetTemporary(name);
                    break;
            }
            return outQuery;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ReleaseQuery(RHIQuery query)
        {
            D3DQuery d3dQuery = (D3DQuery)query;

            switch (d3dQuery.queryContext.queryType)
            {
                case EQueryType.Occlusion:
                    break;

                case EQueryType.Statistic:
                    break;

                case EQueryType.CopyTimestamp:
                    m_QueryContext[0].ReleaseTemporary(query);
                    break;

                case EQueryType.GenericTimestamp:
                    m_QueryContext[1].ReleaseTemporary(query);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIComputePipelineState CreateComputePipelineState(in RHIComputePipelineDescriptor descriptor)
        {
            return new RHIComputePipelineState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIRayTracePipelineState CreateRayTracePipelineState(in RHIRayTracePipelineDescriptor descriptor)
        {
            return new RHIRayTracePipelineState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIGraphicsPipelineState CreateGraphicsPipelineState(in RHIGraphicsPipelineDescriptor descriptor)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CreateSamplerState()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CreateVertexInputLayout()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CreateResourceInputLayout()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIBuffer CreateBuffer(in BufferDescriptor descriptor)
        {
            return new D3DBuffer(m_Device, descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIBufferRef GetBuffer(in BufferDescriptor descriptor)
        {
            return m_ResourcePool.GetBuffer(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ReleaseBuffer(in RHIBufferRef bufferRef)
        {
            m_ResourcePool.ReleaseBuffer(bufferRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHITexture CreateTexture(in TextureDescriptor descriptor)
        {
            return new D3DTexture(m_Device, descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHITextureRef GetTexture(in TextureDescriptor descriptor)
        {
            return m_ResourcePool.GetTexture(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void ReleaseTexture(RHITextureRef textureRef)
        {
            m_ResourcePool.ReleaseTexture(textureRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIMemoryReadback CreateMemoryReadback(string requestName, bool bProfiler)
        {
            return new D3DMemoryReadback(this, requestName, bProfiler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIIndexBufferView CreateIndexBufferView(RHIBuffer buffer)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIVertexBufferView CreateVertexBufferView(RHIBuffer buffer)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIDeptnStencilView CreateDepthStencilView(RHITexture texture)
        {
            return new D3DDeptnStencilView(m_Device, m_DSVDescriptorFactory, texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIRenderTargetView CreateRenderTargetView(RHITexture texture)
        {
            return new D3DRenderTargetView(m_Device, m_RTVDescriptorFactory, texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIConstantBufferView CreateConstantBufferView(RHIBuffer buffer)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIShaderResourceView CreateShaderResourceView(RHIBuffer buffer)
        {
            return null;

            /*D3DBuffer d3dBuffer = (D3DBuffer)buffer;
            ShaderResourceViewDescription srvDescriptor = new ShaderResourceViewDescription
            {
                Format = Format.Unknown,
                ViewDimension = ShaderResourceViewDimension.Buffer,
                Shader4ComponentMapping = 256,
                Buffer = new BufferShaderResourceView { FirstElement = 0, NumElements = (int)d3dBuffer.descriptor.count, StructureByteStride = (int)d3dBuffer.descriptor.stride }
            };
            int descriptorIndex = m_DescriptorFactory.Allocator(1);
            CpuDescriptorHandle descriptorHandle = m_DescriptorFactory.GetCPUHandleStart() + m_DescriptorFactory.GetDescriptorSize() * descriptorIndex;
            m_Device.nativeDevice.CreateShaderResourceView(d3dBuffer.defaultResource, srvDescriptor, descriptorHandle);

            return new RHIShaderResourceView(m_DescriptorFactory.GetDescriptorSize(), descriptorIndex, descriptorHandle);*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIShaderResourceView CreateShaderResourceView(RHITexture texture)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIUnorderedAccessView CreateUnorderedAccessView(RHIBuffer buffer)
        {
            return null;

            /*D3DBuffer d3dBuffer = (D3DBuffer)buffer;
            UnorderedAccessViewDescription uavDescriptor = new UnorderedAccessViewDescription
            {
                Format = Format.Unknown,
                ViewDimension = UnorderedAccessViewDimension.Buffer,
                Buffer = new BufferUnorderedAccessView { NumElements = (int)d3dBuffer.descriptor.count, StructureByteStride = (int)d3dBuffer.descriptor.stride }
            };
            int descriptorIndex = m_DescriptorFactory.Allocator(1);
            CpuDescriptorHandle descriptorHandle = m_DescriptorFactory.GetCPUHandleStart() + m_DescriptorFactory.GetDescriptorSize() * descriptorIndex;
            m_Device.nativeDevice.CreateUnorderedAccessView(d3dBuffer.defaultResource, null, uavDescriptor, descriptorHandle);

            return new RHIUnorderedAccessView(m_DescriptorFactory.GetDescriptorSize(), descriptorIndex, descriptorHandle);*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIUnorderedAccessView CreateUnorderedAccessView(RHITexture texture)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RHIResourceSet CreateResourceSet(in uint count)
        {
            return null;
            //return new RHIResourceSet(m_Device, m_DescriptorFactory, count);
        }

        protected override void Release()
        {
            m_Device?.Dispose();
            m_FencePool?.Dispose();
            m_ResourcePool?.Dispose();
            m_CopyContext?.Dispose();
            m_ComputeContext?.Dispose();
            m_GraphicsContext?.Dispose();
            m_QueryContext[0]?.Dispose();
            m_QueryContext[1]?.Dispose();
            m_CopyBufferPool?.Dispose();
            m_ComputeBufferPool?.Dispose();
            m_GraphicsBufferPool?.Dispose();
            m_DSVDescriptorFactory?.Dispose();
            m_RTVDescriptorFactory?.Dispose();
            m_SamplerDescriptorFactory?.Dispose();
            m_CbvSrvUavDescriptorFactory?.Dispose();
        }
    }
}
