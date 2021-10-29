﻿using Vortice.DXGI;
using Vortice.Direct3D12;
using System.Collections.Generic;
using InfinityEngine.Core.Object;
using InfinityEngine.Core.Container;

namespace InfinityEngine.Graphics.RHI
{
    public enum EContextType
    {
        Copy = 3,
        Compute = 2,
        Graphics = 0
    }

    public class FRHIGraphicsContext : FDisposable
    {
        public ulong copyFrequency
        {
            get
            {
                return m_TransContext.nativeCmdQueue.TimestampFrequency;
            }
        }
        public ulong computeFrequency
        {
            get
            {
                return m_ComputeContext.nativeCmdQueue.TimestampFrequency;
            }
        }
        public ulong graphicsFrequency
        {
            get
            {
                return m_GraphicsContext.nativeCmdQueue.TimestampFrequency;
            }
        }
      
        private FRHIDevice m_Device;
        private FRHIFencePool m_FencePool;
        private FRHIResourcePool m_ResourcePool;
        private FRHIQueryContext[] m_QueryContext;
        private FRHICommandContext m_TransContext;
        private FRHICommandContext m_ComputeContext;
        private FRHICommandContext m_GraphicsContext;
        private TArray<FExecuteInfo> m_ExecuteGPUInfos;
        private FRHICommandListPool m_CopyCommandListPool;
        private FRHICommandListPool m_ComputeCommandListPool;
        private FRHICommandListPool m_GraphicsCommandListPool;
        private TArray<FRHICommandList> m_ManagedCommandList;
        private FRHIDescriptorHeapFactory m_DescriptorFactory;

        public FRHIGraphicsContext() : base()
        {
            m_Device = new FRHIDevice();
            m_FencePool = new FRHIFencePool(m_Device);
            m_ResourcePool = new FRHIResourcePool(m_Device);
            m_ExecuteGPUInfos = new TArray<FExecuteInfo>(32);
            m_ManagedCommandList = new TArray<FRHICommandList>(32);

            m_QueryContext = new FRHIQueryContext[2];
            m_QueryContext[0] = new FRHIQueryContext(m_Device, EQueryType.Timestamp, 64);
            m_QueryContext[1] = new FRHIQueryContext(m_Device, EQueryType.CopyTimestamp, 64);

            m_TransContext = new FRHICommandContext(m_Device, EContextType.Copy);
            m_ComputeContext = new FRHICommandContext(m_Device, EContextType.Compute);
            m_GraphicsContext = new FRHICommandContext(m_Device, EContextType.Graphics);
            m_CopyCommandListPool = new FRHICommandListPool(m_Device, EContextType.Copy);
            m_ComputeCommandListPool = new FRHICommandListPool(m_Device, EContextType.Compute);
            m_GraphicsCommandListPool = new FRHICommandListPool(m_Device, EContextType.Graphics);

            //TerraFX.Interop.D3D12MemAlloc.D3D12MA_CreateAllocator
            m_DescriptorFactory = new FRHIDescriptorHeapFactory(m_Device, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView, 32768);
        }

        // Context
        FRHICommandContext SelectContext(in EContextType contextType)
        {
            FRHICommandContext commandContext = m_GraphicsContext;

            switch (contextType)
            {
                case EContextType.Copy:
                    commandContext = m_TransContext;
                    break;

                case EContextType.Compute:
                    commandContext = m_ComputeContext;
                    break;
            }

            return commandContext;
        }
        
        public FRHICommandList CreateCommandList(in EContextType contextType, string name = null)
        {
            return new FRHICommandList(name, m_Device, contextType);
        }

        public FRHICommandList GetCommandList(in EContextType contextType, string name = null, bool bAutoRelease = false)
        {
            FRHICommandList cmdList = null;
            switch (contextType)
            {
                case EContextType.Copy:
                    cmdList = m_CopyCommandListPool.GetTemporary(name);
                    break;

                case EContextType.Compute:
                    cmdList = m_ComputeCommandListPool.GetTemporary(name);
                    break;

                case EContextType.Graphics:
                    cmdList = m_GraphicsCommandListPool.GetTemporary(name);
                    break;
            }

            if (bAutoRelease) { m_ManagedCommandList.Add(cmdList); }

            return cmdList;
        }

        public void ReleaseCommandList(FRHICommandList cmdList)
        {
            switch (cmdList.contextType)
            {
                case EContextType.Copy:
                    m_CopyCommandListPool.ReleaseTemporary(cmdList);
                    break;

                case EContextType.Compute:
                    m_ComputeCommandListPool.ReleaseTemporary(cmdList);
                    break;

                case EContextType.Graphics:
                    m_GraphicsCommandListPool.ReleaseTemporary(cmdList);
                    break;
            }
        }

        public void WriteFence(in EContextType contextType, FRHIFence fence)
        {
            FExecuteInfo executeInfo;
            executeInfo.fence = fence;
            executeInfo.cmdList = null;
            executeInfo.executeType = EExecuteType.Signal;
            executeInfo.cmdContext = SelectContext(contextType);
            m_ExecuteGPUInfos.Add(executeInfo);
        }

        public void WaitFence(in EContextType contextType, FRHIFence fence)
        {
            FExecuteInfo executeInfo;
            executeInfo.fence = fence;
            executeInfo.cmdList = null;
            executeInfo.executeType = EExecuteType.Wait;
            executeInfo.cmdContext = SelectContext(contextType);
            m_ExecuteGPUInfos.Add(executeInfo);
        }

        public void ExecuteCommandList(in EContextType contextType, FRHICommandList cmdList)
        {
            FExecuteInfo executeInfo;
            executeInfo.fence = null;
            executeInfo.cmdList = cmdList;
            executeInfo.executeType = EExecuteType.Execute;
            executeInfo.cmdContext = SelectContext(contextType);
            m_ExecuteGPUInfos.Add(executeInfo);
        }

        public void Flush()
        {
            for (int i = 0; i < m_ManagedCommandList.length; ++i)
            {
                ReleaseCommandList(m_ManagedCommandList[i]);
                m_ManagedCommandList[i] = null;
            }
            m_ManagedCommandList.Clear();

            m_QueryContext[1].Submit(m_TransContext);
            m_QueryContext[0].Submit(m_GraphicsContext);

            m_GraphicsContext.Flush();

            m_QueryContext[0].GetData();
            m_QueryContext[1].GetData();
        }

        public void Submit()
        {
            for (int i = 0; i < m_ExecuteGPUInfos.length; ++i)
            {
                FExecuteInfo executeInfo = m_ExecuteGPUInfos[i];
                FRHICommandContext cmdContext = executeInfo.cmdContext;

                switch (executeInfo.executeType)
                {
                    case EExecuteType.Signal:
                        cmdContext.SignalQueue(executeInfo.fence);
                        break;

                    case EExecuteType.Wait:
                        cmdContext.WaitQueue(executeInfo.fence);
                        break;

                    case EExecuteType.Execute:
                        cmdContext.ExecuteQueue(executeInfo.cmdList);
                        break;
                }
            }

            m_ExecuteGPUInfos.Clear();
        }

        // Resource
        public void CreateViewport()
        {

        }

        public FRHIFence CreateFence(string name = null)
        {
            return new FRHIFence(m_Device, name);
        }
        
        public FRHIFence GetFence(string name = null)
        {
            return m_FencePool.GetTemporary(name);
        }

        public void ReleaseFence(FRHIFence fence)
        {
            m_FencePool.ReleaseTemporary(fence);
        }

        public FRHIQuery CreateQuery(in EQueryType queryType, string name = null)
        {
            FRHIQuery outQuery = null;
            switch (queryType)
            {
                case EQueryType.Occlusion:
                    outQuery = null;
                    break;
                case EQueryType.Timestamp:
                    outQuery = new FRHIQuery(m_QueryContext[0]);
                    break;
                case EQueryType.Statistics:
                    outQuery = null;
                    break;
                case EQueryType.CopyTimestamp:
                    outQuery = new FRHIQuery(m_QueryContext[1]);
                    break;
            }
            return outQuery;
        }

        public FRHIQuery GetQuery(in EQueryType queryType, string name = null)
        {
            FRHIQuery outQuery = null;
            switch (queryType)
            {
                case EQueryType.Occlusion:
                    outQuery = null;
                    break;
                case EQueryType.Timestamp:
                    outQuery = m_QueryContext[0].GetTemporary(name);
                    break;
                case EQueryType.Statistics:
                    outQuery = null;
                    break;
                case EQueryType.CopyTimestamp:
                    outQuery = m_QueryContext[1].GetTemporary(name);
                    break;
            }
            return outQuery;
        }

        public void ReleaseQuery(FRHIQuery query)
        {
            switch (query.queryContext.queryType)
            {
                case EQueryType.Occlusion:
                    break;
                case EQueryType.Timestamp:
                    m_QueryContext[0].ReleaseTemporary(query);
                    break;
                case EQueryType.Statistics:
                    break;
                case EQueryType.CopyTimestamp:
                    m_QueryContext[1].ReleaseTemporary(query);
                    break;
            }
        }

        public FRHIComputePipelineState CreateComputePipelineState()
        {
            return new FRHIComputePipelineState();
        }

        public FRHIGraphicsPipelineState CreateGraphicsPipelineState()
        {
            return new FRHIGraphicsPipelineState();
        }

        public FRHIRayTracePipelineState CreateRayTracePipelineState()
        {
            return new FRHIRayTracePipelineState();
        }

        public void CreateSamplerState()
        {

        }

        public void CreateVertexInputLayout()
        {

        }

        public void CreateResourceInputLayout()
        {

        }

        public FRHIBuffer CreateBuffer(in FRHIBufferDescription description)
        {
            return new FRHIBuffer(m_Device, description);
        }

        public FRHIBufferRef GetBuffer(in FRHIBufferDescription description)
        {
            return m_ResourcePool.GetBuffer(description);
        }

        public void ReleaseBuffer(FRHIBufferRef bufferRef)
        {
            m_ResourcePool.ReleaseBuffer(bufferRef);
        }

        public FRHITexture CreateTexture(in FRHITextureDescription description)
        {
            return new FRHITexture(m_Device, description);
        }
        
        public FRHITextureRef GetTexture(in FRHITextureDescription description)
        {
            return m_ResourcePool.GetTexture(description);
        }

        public void ReleaseTexture(FRHITextureRef textureRef)
        {
            m_ResourcePool.ReleaseTexture(textureRef);
        }

        public FRHIIndexBufferView CreateIndexBufferView(FRHIBuffer buffer)
        {
            FRHIIndexBufferView ibv = new FRHIIndexBufferView();
            return ibv;
        }

        public FRHIVertexBufferView CreateVertexBufferView(FRHIBuffer buffer)
        {
            FRHIVertexBufferView vbo = new FRHIVertexBufferView();
            return vbo;
        }

        public FRHIDeptnStencilView CreateDepthStencilView(FRHITexture texture)
        {
            FRHIDeptnStencilView dsv = new FRHIDeptnStencilView();
            return dsv;
        }

        public FRHIRenderTargetView CreateRenderTargetView(FRHITexture texture)
        {
            FRHIRenderTargetView rtv = new FRHIRenderTargetView();
            return rtv;
        }

        public FRHIConstantBufferView CreateConstantBufferView(FRHIBuffer buffer)
        {
            FRHIConstantBufferView cbv = new FRHIConstantBufferView();
            return cbv;
        }

        public FRHIShaderResourceView CreateShaderResourceView(FRHIBuffer buffer)
        {
            ShaderResourceViewDescription srvDescriptor = new ShaderResourceViewDescription
            {
                Format = Format.Unknown,
                ViewDimension = ShaderResourceViewDimension.Buffer,
                Shader4ComponentMapping = 256,
                Buffer = new BufferShaderResourceView { FirstElement = 0, NumElements = (int)buffer.description.count, StructureByteStride = (int)buffer.description.stride }
            };
            int descriptorIndex = m_DescriptorFactory.Allocator(1);
            CpuDescriptorHandle descriptorHandle = m_DescriptorFactory.GetCPUHandleStart() + m_DescriptorFactory.GetDescriptorSize() * descriptorIndex;
            m_Device.nativeDevice.CreateShaderResourceView(buffer.defaultResource, srvDescriptor, descriptorHandle);

            return new FRHIShaderResourceView(m_DescriptorFactory.GetDescriptorSize(), descriptorIndex, descriptorHandle);
        }

        public FRHIShaderResourceView CreateShaderResourceView(FRHITexture texture)
        {
            FRHIShaderResourceView srv = new FRHIShaderResourceView();
            return srv;
        }

        public FRHIUnorderedAccessView CreateUnorderedAccessView(FRHIBuffer buffer)
        {
            UnorderedAccessViewDescription uavDescriptor = new UnorderedAccessViewDescription
            {
                Format = Format.Unknown,
                ViewDimension = UnorderedAccessViewDimension.Buffer,
                Buffer = new BufferUnorderedAccessView { NumElements = (int)buffer.description.count, StructureByteStride = (int)buffer.description.stride }
            };
            int descriptorIndex = m_DescriptorFactory.Allocator(1);
            CpuDescriptorHandle descriptorHandle = m_DescriptorFactory.GetCPUHandleStart() + m_DescriptorFactory.GetDescriptorSize() * descriptorIndex;
            m_Device.nativeDevice.CreateUnorderedAccessView(buffer.defaultResource, null, uavDescriptor, descriptorHandle);

            return new FRHIUnorderedAccessView(m_DescriptorFactory.GetDescriptorSize(), descriptorIndex, descriptorHandle);
        }

        public FRHIUnorderedAccessView CreateUnorderedAccessView(FRHITexture texture)
        {
            FRHIUnorderedAccessView uav = new FRHIUnorderedAccessView();
            return uav;
        }

        public FRHIResourceSet CreateResourceSet(in int count)
        {
            return new FRHIResourceSet(m_Device, m_DescriptorFactory, count);
        }

        protected override void Release()
        {
            m_Device?.Dispose();
            m_FencePool?.Dispose();
            m_ResourcePool?.Dispose();
            m_TransContext?.Dispose();
            m_ComputeContext?.Dispose();
            m_GraphicsContext?.Dispose();
            m_QueryContext[0]?.Dispose();
            m_QueryContext[1]?.Dispose();
            m_DescriptorFactory?.Dispose();
            m_CopyCommandListPool?.Dispose();
            m_ComputeCommandListPool?.Dispose();
            m_GraphicsCommandListPool?.Dispose();
        }
    }
}
