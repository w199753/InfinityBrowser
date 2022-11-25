﻿using System;
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
    internal unsafe struct Dx12DescriptorInfo
    {
        public int Index;
        public ID3D12DescriptorHeap* DescriptorHeap;
        public D3D12_CPU_DESCRIPTOR_HANDLE CpuHandle;
        public D3D12_GPU_DESCRIPTOR_HANDLE GpuHandle;
    };

    internal unsafe class Dx12DescriptorHeap : Disposal
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

        public Dx12DescriptorHeap(ID3D12Device6* device, in D3D12_DESCRIPTOR_HEAP_TYPE type, in D3D12_DESCRIPTOR_HEAP_FLAGS flag, in uint count)
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

    internal unsafe class Dx12Device : RHIDevice
    {
        public Dx12GPU Dx12Gpu
        {
            get
            {
                return m_Dx12Gpu;
            }
        }
        public Dx12DescriptorHeap DsvHeap
        {
            get
            {
                return m_DsvHeap;
            }
        }
        public Dx12DescriptorHeap RtvHeap
        {
            get
            {
                return m_RtvHeap;
            }
        }
        public Dx12DescriptorHeap SamplerHeap
        {
            get
            {
                return m_SamplerHeap;
            }
        }
        public Dx12DescriptorHeap CbvSrvUavHeap
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
        public ID3D12CommandSignature* DrawIndirectSignature
        {
            get
            {
                return m_DrawIndirectSignature;
            }
        }
        public ID3D12CommandSignature* DispatchIndirectSignature
        {
            get
            {
                return m_DispatchIndirectSignature;
            }
        }
        public ID3D12CommandSignature* DrawIndexedIndirectSignature
        {
            get
            {
                return m_DrawIndexedIndirectSignature;
            }
        }

        private Dx12GPU m_Dx12Gpu;
        private ID3D12Device6* m_NativeDevice;
        private Dx12DescriptorHeap m_DsvHeap;
        private Dx12DescriptorHeap m_RtvHeap;
        private Dx12DescriptorHeap m_SamplerHeap;
        private Dx12DescriptorHeap m_CbvSrvUavHeap;
        private ID3D12CommandSignature* m_DrawIndirectSignature;
        private ID3D12CommandSignature* m_DispatchIndirectSignature;
        private ID3D12CommandSignature* m_DrawIndexedIndirectSignature;
        private Dictionary<EQueueType, List<Dx12Queue>> m_GpuQueues;

        public Dx12Device(Dx12GPU gpu, in RHIDeviceDescriptor descriptor) 
        {
            m_Dx12Gpu = gpu;
            CreateDevice();
            CreateFeatureSet();
            CreateQueues(descriptor);
            CreateDescriptorHeaps();
            CreateCommandSignatures();
        }

        public override int GetQueueCount(in EQueueType type)
        {
            bool hashValue = m_GpuQueues.TryGetValue(type, out List<Dx12Queue> queueArray);
            Debug.Assert(hashValue);
            return queueArray.Count;
        }

        public override RHIQueue GetQueue(in EQueueType type, in int index)
        {
            bool hashValue = m_GpuQueues.TryGetValue(type, out List<Dx12Queue> queueArray);
            Debug.Assert(hashValue);
            Debug.Assert(index >= 0 && index < queueArray?.Count);
            return queueArray[index];
        }

        public override RHIFence CreateFence()
        {
            return new Dx12Fence(this);
        }

        public override RHISemaphore CreateSemaphore()
        {
            throw new NotImplementedException();
        }

        public override RHIQuery CreateQuery(in RHIQueryDescription descriptor)
        {
            throw new NotImplementedException();
        }

        public override RHIHeap CreateHeap(in RHIHeapDescription descriptor)
        {
            throw new NotImplementedException();
        }

        public override RHIBuffer CreateBuffer(in RHIBufferDescriptor descriptor)
        {
            return new Dx12Buffer(this, descriptor);
        }

        public override RHITexture CreateTexture(in RHITextureDescriptor descriptor)
        {
            return new Dx12Texture(this, descriptor);
        }

        public override RHISamplerState CreateSamplerState(in RHISamplerStateDescriptor descriptor)
        {
            return new Dx12SamplerState(this, descriptor);
        }

        public override RHIFunction CreateFunction(in RHIFunctionDescriptor descriptor)
        {
            return new Dx12Function(descriptor);
        }

        public override RHISwapChain CreateSwapChain(in RHISwapChainDescriptor descriptor)
        {
            return new Dx12SwapChain(this, descriptor);
        }

        public override RHIBindGroupLayout CreateBindGroupLayout(in RHIBindGroupLayoutDescriptor descriptor)
        {
            return new Dx12BindGroupLayout(descriptor);
        }

        public override RHIBindGroup CreateBindGroup(in RHIBindGroupDescriptor descriptor)
        {
            return new Dx12BindGroup(descriptor);
        }

        public override RHIPipelineLayout CreatePipelineLayout(in RHIPipelineLayoutDescriptor descriptor)
        {
            return new Dx12PipelineLayout(this, descriptor);
        }

        public override RHIComputePipeline CreateComputePipeline(in RHIComputePipelineDescriptor descriptor)
        {
            return new Dx12ComputePipeline(this, descriptor);
        }

        public override RHIRaytracingPipeline CreateRaytracingPipeline(in RHIRaytracingPipelineDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public override RHIMeshletPipeline CreateMeshletPipeline(in RHIMeshletPipelineDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public override RHIGraphicsPipeline CreateGraphicsPipeline(in RHIGraphicsPipelineDescriptor descriptor)
        {
            return new Dx12GraphicsPipeline(this, descriptor);
        }

        public Dx12DescriptorInfo AllocateDsvDescriptor(in int count)
        {
            int index = m_DsvHeap.Allocate();
            Dx12DescriptorInfo descriptorInfo;
            descriptorInfo.Index = index;
            descriptorInfo.CpuHandle = m_DsvHeap.CpuStartHandle.Offset(index, m_DsvHeap.DescriptorSize);
            descriptorInfo.GpuHandle = m_DsvHeap.GpuStartHandle.Offset(index, m_DsvHeap.DescriptorSize);
            descriptorInfo.DescriptorHeap = m_DsvHeap.DescriptorHeap;
            return descriptorInfo;
        }

        public Dx12DescriptorInfo AllocateRtvDescriptor(in int count)
        {
            int index = m_RtvHeap.Allocate();
            Dx12DescriptorInfo descriptorInfo;
            descriptorInfo.Index = index;
            descriptorInfo.CpuHandle = m_RtvHeap.CpuStartHandle.Offset(index, m_RtvHeap.DescriptorSize);
            descriptorInfo.GpuHandle = m_RtvHeap.GpuStartHandle.Offset(index, m_RtvHeap.DescriptorSize);
            descriptorInfo.DescriptorHeap = m_RtvHeap.DescriptorHeap;
            return descriptorInfo;
        }

        public Dx12DescriptorInfo AllocateSamplerDescriptor(in int count)
        {
            int index = m_SamplerHeap.Allocate();
            Dx12DescriptorInfo descriptorInfo;
            descriptorInfo.Index = index;
            descriptorInfo.CpuHandle = m_SamplerHeap.CpuStartHandle.Offset(index, m_SamplerHeap.DescriptorSize);
            descriptorInfo.GpuHandle = m_SamplerHeap.GpuStartHandle.Offset(index, m_SamplerHeap.DescriptorSize);
            descriptorInfo.DescriptorHeap = m_SamplerHeap.DescriptorHeap;
            return descriptorInfo;
        }

        public Dx12DescriptorInfo AllocateCbvSrvUavDescriptor(in int count)
        {
            int index = m_CbvSrvUavHeap.Allocate();
            Dx12DescriptorInfo descriptorInfo;
            descriptorInfo.Index = index;
            descriptorInfo.CpuHandle = m_CbvSrvUavHeap.CpuStartHandle.Offset(index, m_CbvSrvUavHeap.DescriptorSize);
            descriptorInfo.GpuHandle = m_CbvSrvUavHeap.GpuStartHandle.Offset(index, m_CbvSrvUavHeap.DescriptorSize);
            descriptorInfo.DescriptorHeap = m_CbvSrvUavHeap.DescriptorHeap;
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

        private void CreateDevice()
        {
            ID3D12Device6* device;
            bool success = SUCCEEDED(DirectX.D3D12CreateDevice((IUnknown*)m_Dx12Gpu.DXGIAdapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_1, __uuidof<ID3D12Device6>(), (void**)&device));
            Debug.Assert(success);
            m_NativeDevice = device;
        }

        private void CreateFeatureSet()
        {
            D3D12_FEATURE_DATA_D3D12_OPTIONS6 options;
            m_NativeDevice->CheckFeatureSupport(D3D12_FEATURE.D3D12_FEATURE_D3D12_OPTIONS6, &options, (uint)sizeof(D3D12_FEATURE_DATA_D3D12_OPTIONS6));
        }

        private void CreateQueues(in RHIDeviceDescriptor descriptor)
        {
            Dictionary<EQueueType, int> queueCountMap = new Dictionary<EQueueType, int>(3);
            for (int i = 0; i < descriptor.QueueInfoCount; ++i)
            {
                RHIQueueDescriptor queueInfo = descriptor.QueueInfos.Span[i];
                if (queueCountMap.TryGetValue(queueInfo.Type, out int value))
                {
                    queueCountMap[queueInfo.Type] = 0;
                }

                queueCountMap.TryAdd(queueInfo.Type, (int)queueInfo.Count);
            }

            m_GpuQueues = new Dictionary<EQueueType, List<Dx12Queue>>(3);
            foreach (KeyValuePair<EQueueType, int> iter in queueCountMap)
            {
                List<Dx12Queue> tempQueues = new List<Dx12Queue>(iter.Value);

                D3D12_COMMAND_QUEUE_DESC queueDesc = new D3D12_COMMAND_QUEUE_DESC();
                queueDesc.Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE;
                queueDesc.Type = Dx12Utility.ConvertToNativeQueueType(iter.Key);
                for (int i = 0; i < iter.Value; ++i)
                {
                    ID3D12CommandQueue* commandQueue;
                    bool success = SUCCEEDED(m_NativeDevice->CreateCommandQueue(&queueDesc, __uuidof<ID3D12CommandQueue>(), (void**)&commandQueue));
                    Debug.Assert(success);

                    Dx12CommandQueueDescriptor queueDescriptor;
                    queueDescriptor.Type = iter.Key;
                    queueDescriptor.Queue = commandQueue;
                    tempQueues.Add(new Dx12Queue(this, queueDescriptor));
                }

                m_GpuQueues.TryAdd(iter.Key, tempQueues);
            }
        }

        private void CreateDescriptorHeaps()
        {
            m_DsvHeap = new Dx12DescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_DSV, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_NONE, 1024);
            m_RtvHeap = new Dx12DescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_NONE, 1024);
            m_SamplerHeap = new Dx12DescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_SAMPLER, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE, 2048);
            m_CbvSrvUavHeap = new Dx12DescriptorHeap(m_NativeDevice, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV, D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE, 32768);
        }

        private void CreateCommandSignatures()
        {
            bool success = false;
            ID3D12CommandSignature* commandSignature;
            D3D12_INDIRECT_ARGUMENT_DESC indirectArgDesc;
            D3D12_COMMAND_SIGNATURE_DESC commandSignatureDesc;

            indirectArgDesc.Type = D3D12_INDIRECT_ARGUMENT_TYPE.D3D12_INDIRECT_ARGUMENT_TYPE_DRAW;
            //commandSignatureDesc.NodeMask = nodeMask;
            commandSignatureDesc.pArgumentDescs = &indirectArgDesc;
            commandSignatureDesc.ByteStride = (uint)sizeof(D3D12_DRAW_ARGUMENTS);
            commandSignatureDesc.NumArgumentDescs = 1;
            success = SUCCEEDED(m_NativeDevice->CreateCommandSignature(&commandSignatureDesc, null, __uuidof<ID3D12CommandSignature>(), (void**)&commandSignature));
            Debug.Assert(success);
            m_DrawIndirectSignature = commandSignature;

            indirectArgDesc.Type = D3D12_INDIRECT_ARGUMENT_TYPE.D3D12_INDIRECT_ARGUMENT_TYPE_DISPATCH;
            //commandSignatureDesc.NodeMask = nodeMask;
            commandSignatureDesc.pArgumentDescs = &indirectArgDesc;
            commandSignatureDesc.ByteStride = (uint)sizeof(D3D12_DISPATCH_ARGUMENTS);
            commandSignatureDesc.NumArgumentDescs = 1;
            success = SUCCEEDED(m_NativeDevice->CreateCommandSignature(&commandSignatureDesc, null, __uuidof<ID3D12CommandSignature>(), (void**)&commandSignature));
            Debug.Assert(success);
            m_DispatchIndirectSignature = commandSignature;

            indirectArgDesc.Type = D3D12_INDIRECT_ARGUMENT_TYPE.D3D12_INDIRECT_ARGUMENT_TYPE_DRAW_INDEXED;
            //commandSignatureDesc.NodeMask = nodeMask;
            commandSignatureDesc.pArgumentDescs = &indirectArgDesc;
            commandSignatureDesc.ByteStride = (uint)sizeof(D3D12_DRAW_INDEXED_ARGUMENTS);
            commandSignatureDesc.NumArgumentDescs = 1;
            success = SUCCEEDED(m_NativeDevice->CreateCommandSignature(&commandSignatureDesc, null, __uuidof<ID3D12CommandSignature>(), (void**)&commandSignature));
            Debug.Assert(success);
            m_DrawIndexedIndirectSignature = commandSignature;
        }

        protected override void Release()
        {
            m_DsvHeap.Dispose();
            m_RtvHeap.Dispose();
            m_SamplerHeap.Dispose();
            m_CbvSrvUavHeap.Dispose();
            foreach (KeyValuePair<EQueueType, List<Dx12Queue>> gpuQueue in m_GpuQueues)
            {
                for (int i = 0; i < gpuQueue.Value.Count; ++i)
                {
                    gpuQueue.Value[i].Dispose();
                }
            }
            m_DrawIndirectSignature->Release();
            m_DispatchIndirectSignature->Release();
            m_DrawIndexedIndirectSignature->Release();
            m_NativeDevice->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
