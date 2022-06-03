using System;
using System.Diagnostics;
using Infinity.Container;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal unsafe struct D3DDescriptorInfo
    {
        public int index;
        public ID3D12DescriptorHeap* descriptorHeap;
        public D3D12_CPU_DESCRIPTOR_HANDLE cpuHandle;
        public D3D12_GPU_DESCRIPTOR_HANDLE gpuHandle;
    };

    internal unsafe class D3DDescriptorHeap : Disposal
    {
        public uint DescriptorSize => m_DescriptorSize;
        public D3D12_DESCRIPTOR_HEAP_TYPE Type => m_Type;
        public ID3D12DescriptorHeap* DescriptorHeap => m_DescriptorHeap;
        public D3D12_CPU_DESCRIPTOR_HANDLE CpuStartHandle => m_DescriptorHeap->GetCPUDescriptorHandleForHeapStart();
        public D3D12_GPU_DESCRIPTOR_HANDLE GpuStartHandle => m_DescriptorHeap->GetGPUDescriptorHandleForHeapStart();

        private uint m_DescriptorSize;
        private TValueArray<int> m_CacheMap;
        private D3D12_DESCRIPTOR_HEAP_TYPE m_Type;
        private ID3D12DescriptorHeap* m_DescriptorHeap;

        public D3DDescriptorHeap(ID3D12Device6* device, in D3D12_DESCRIPTOR_HEAP_TYPE type, in D3D12_DESCRIPTOR_HEAP_FLAGS flag, in uint count)
        {
            m_CacheMap = new TValueArray<int>((int)count);
            for (int i = 0; i < (int)count; ++i)
            {
                m_CacheMap.Add(i);
            }

            m_Type = type;
            m_DescriptorSize = device->GetDescriptorHandleIncrementSize(type);

            D3D12_DESCRIPTOR_HEAP_DESC descriptorInfo;
            descriptorInfo.Type = type;
            descriptorInfo.Flags = flag;
            descriptorInfo.NumDescriptors = count;

            ID3D12DescriptorHeap* descriptorHeap;
            bool success = SUCCEEDED(device->CreateDescriptorHeap(&descriptorInfo, __uuidof<ID3D12DescriptorHeap>(), (void**)&descriptorHeap));
            Debug.Assert(success);
            m_DescriptorHeap = descriptorHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Allocate()
        {
            int index = m_CacheMap[m_CacheMap.length - 1];
            m_CacheMap.RemoveSwapAtIndex(m_CacheMap.length - 1);
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Allocate(in int count)
        {
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free(in int index)
        {
            m_CacheMap.Add(index);
        }

        protected override void Release()
        {
            m_CacheMap.Dispose();
            m_DescriptorHeap->Release();
        }
    }

    internal unsafe class D3DDevice : RHIDevice
    {
        public D3DGPU D3DGpu
        {
            get
            {
                return m_D3DGpu;
            }
        }
        public D3DDescriptorHeap DsvHeap
        {
            get
            {
                return m_DsvHeap;
            }
        }
        public D3DDescriptorHeap RtvHeap
        {
            get
            {
                return m_RtvHeap;
            }
        }
        public D3DDescriptorHeap SamplerHeap
        {
            get
            {
                return m_SamplerHeap;
            }
        }
        public D3DDescriptorHeap CbvSrvUavHeap
        {
            get
            {
                return m_CbvSrvUavHeap;
            }
        }
        public ID3D12Device6* NativeDevice
        {
            get
            {
                return m_NativeDevice;
            }
        }

        private D3DGPU m_D3DGpu;
        private ID3D12Device6* m_NativeDevice;
        private D3DDescriptorHeap m_DsvHeap;
        private D3DDescriptorHeap m_RtvHeap;
        private D3DDescriptorHeap m_SamplerHeap;
        private D3DDescriptorHeap m_CbvSrvUavHeap;
        private Dictionary<EQueueType, List<D3DQueue>> m_Queues;

        public D3DDevice(D3DGPU gpu, in RHIDeviceCreateInfo createInfo) 
        {
            m_D3DGpu = gpu;
            CreateDevice(gpu.DXGIAdapter);
            CreateDescriptorHeaps();
            CreateQueues(createInfo);
        }

        public override int GetQueueCount(in EQueueType type)
        {
            bool hashValue = m_Queues.TryGetValue(type, out List<D3DQueue> queueArray);
            Debug.Assert(hashValue);
            return queueArray.Count;
        }

        public override RHIQueue GetQueue(in EQueueType type, in int index)
        {
            bool hashValue = m_Queues.TryGetValue(type, out List<D3DQueue> queueArray);
            Debug.Assert(hashValue);
            Debug.Assert(index >= 0 && index < queueArray?.Count);
            return queueArray[index];
        }

        public override RHIFence CreateFence()
        {
            return new D3DFence(this);
        }

        public override RHISwapChain CreateSwapChain(in RHISwapChainCreateInfo createInfo)
        {
            return new D3DSwapChain(this, createInfo);
        }

        public override RHIBuffer CreateBuffer(in RHIBufferCreateInfo createInfo)
        {
            return new D3DBuffer(this, createInfo);
        }

        public override RHITexture CreateTexture(in RHITextureCreateInfo createInfo)
        {
            return new D3DTexture(this, createInfo);
        }

        public override RHISampler CreateSampler(in RHISamplerCreateInfo createInfo)
        {
            return new D3DSampler(this, createInfo);
        }

        public override RHIShader CreateShader(in RHIShaderCreateInfo createInfo)
        {
            return new D3DShader(createInfo);
        }

        public override RHIBindGroupLayout CreateBindGroupLayout(in RHIBindGroupLayoutCreateInfo createInfo)
        {
            return new D3DBindGroupLayout(createInfo);
        }

        public override RHIBindGroup CreateBindGroup(in RHIBindGroupCreateInfo createInfo)
        {
            return new D3DBindGroup(createInfo);
        }

        public override RHIPipelineLayout CreatePipelineLayout(in RHIPipelineLayoutCreateInfo createInfo)
        {
            return new D3DPipelineLayout(this, createInfo);
        }

        public override RHIComputePipeline CreateComputePipeline(in RHIComputePipelineCreateInfo createInfo)
        {
            return new D3DComputePipeline(this, createInfo);
        }

        public override RHIGraphicsPipeline CreateGraphicsPipeline(in RHIGraphicsPipelineCreateInfo createInfo)
        {
            return new D3DGraphicsPipeline(this, createInfo);
        }

        public D3DDescriptorInfo AllocateDsvDescriptor(in int count)
        {
            int index = m_DsvHeap.Allocate();
            D3DDescriptorInfo descriptorInfo;
            descriptorInfo.index = index;
            descriptorInfo.cpuHandle = m_DsvHeap.CpuStartHandle.Offset(index, m_DsvHeap.DescriptorSize);
            descriptorInfo.gpuHandle = m_DsvHeap.GpuStartHandle.Offset(index, m_DsvHeap.DescriptorSize);
            descriptorInfo.descriptorHeap = m_DsvHeap.DescriptorHeap;
            return descriptorInfo;
        }

        public D3DDescriptorInfo AllocateRtvDescriptor(in int count)
        {
            int index = m_RtvHeap.Allocate();
            D3DDescriptorInfo descriptorInfo;
            descriptorInfo.index = index;
            descriptorInfo.cpuHandle = m_RtvHeap.CpuStartHandle.Offset(index, m_RtvHeap.DescriptorSize);
            descriptorInfo.gpuHandle = m_RtvHeap.GpuStartHandle.Offset(index, m_RtvHeap.DescriptorSize);
            descriptorInfo.descriptorHeap = m_RtvHeap.DescriptorHeap;
            return descriptorInfo;
        }

        public D3DDescriptorInfo AllocateSamplerDescriptor(in int count)
        {
            int index = m_SamplerHeap.Allocate();
            D3DDescriptorInfo descriptorInfo;
            descriptorInfo.index = index;
            descriptorInfo.cpuHandle = m_SamplerHeap.CpuStartHandle.Offset(index, m_SamplerHeap.DescriptorSize);
            descriptorInfo.gpuHandle = m_SamplerHeap.GpuStartHandle.Offset(index, m_SamplerHeap.DescriptorSize);
            descriptorInfo.descriptorHeap = m_SamplerHeap.DescriptorHeap;
            return descriptorInfo;
        }

        public D3DDescriptorInfo AllocateCbvSrvUavDescriptor(in int count)
        {
            int index = m_CbvSrvUavHeap.Allocate();
            D3DDescriptorInfo descriptorInfo;
            descriptorInfo.index = index;
            descriptorInfo.cpuHandle = m_CbvSrvUavHeap.CpuStartHandle.Offset(index, m_CbvSrvUavHeap.DescriptorSize);
            descriptorInfo.gpuHandle = m_CbvSrvUavHeap.GpuStartHandle.Offset(index, m_CbvSrvUavHeap.DescriptorSize);
            descriptorInfo.descriptorHeap = m_CbvSrvUavHeap.DescriptorHeap;
            return descriptorInfo;
        }

        public void FreeDsvDescriptor(in int index)
        {
            m_DsvHeap.Free(index);
        }

        public void FreeRtvDescriptor(in int index)
        {
            m_RtvHeap.Free(index);
        }

        public void FreeSamplerDescriptor(in int index)
        {
            m_SamplerHeap.Free(index);
        }

        public void FreeCbvSrvUavDescriptor(in int index)
        {
            m_CbvSrvUavHeap.Free(index);
        }

        private void CreateDevice(in IDXGIAdapter1* adapter)
        {
            ID3D12Device6* device;
            bool success = SUCCEEDED(DirectX.D3D12CreateDevice((IUnknown*)adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_1, __uuidof<ID3D12Device6>(), (void**)&device));
            Debug.Assert(success);
            m_NativeDevice = device;
        }

        private void CreateDescriptorHeaps()
        {
            m_DsvHeap = new D3DDescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_DSV, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_NONE, 1024);
            m_RtvHeap = new D3DDescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_NONE, 1024);
            m_SamplerHeap = new D3DDescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_SAMPLER, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE, 2048);
            m_CbvSrvUavHeap = new D3DDescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE, 32768);
        }

        private void CreateQueues(in RHIDeviceCreateInfo createInfo)
        {
            Dictionary<EQueueType, int> queueCountMap = new Dictionary<EQueueType, int>(3);
            for (int i = 0; i < createInfo.queueInfoCount; ++i)
            {
                RHIQueueInfo queueInfo = createInfo.queueInfos.Span[i];
                if (queueCountMap.TryGetValue(queueInfo.type, out int value))
                {
                    queueCountMap[queueInfo.type] = 0;
                }

                queueCountMap.TryAdd(queueInfo.type, (int)queueInfo.count);
            }

            m_Queues = new Dictionary<EQueueType, List<D3DQueue>>(3);
            foreach (KeyValuePair<EQueueType, int> iter in queueCountMap)
            {
                List<D3DQueue> tempQueues = new List<D3DQueue>(iter.Value);

                D3D12_COMMAND_QUEUE_DESC queueDesc = new D3D12_COMMAND_QUEUE_DESC();
                queueDesc.Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE;
                queueDesc.Type = D3DUtility.ConvertToNativeQueueType(iter.Key);
                for (int i = 0; i < iter.Value; ++i)
                {
                    ID3D12CommandQueue* commandQueue;
                    bool success = SUCCEEDED(m_NativeDevice->CreateCommandQueue(&queueDesc, __uuidof<ID3D12CommandQueue>(), (void**)&commandQueue));
                    Debug.Assert(success);

                    D3DCommandQueueCreateInfo queueCreateInfo;
                    queueCreateInfo.cmdQueue = commandQueue;
                    queueCreateInfo.queueType = iter.Key;
                    tempQueues.Add(new D3DQueue(this, queueCreateInfo));
                }

                m_Queues.TryAdd(iter.Key, tempQueues);
            }
        }

        protected override void Release()
        {
            m_DsvHeap.Dispose();
            m_RtvHeap.Dispose();
            m_SamplerHeap.Dispose();
            m_CbvSrvUavHeap.Dispose();
            foreach (KeyValuePair<EQueueType, List<D3DQueue>> iter in m_Queues)
            {
                for (int i = 0; i < iter.Value.Count; ++i)
                {
                    iter.Value[i].Dispose();
                }
            }
            m_NativeDevice->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
