﻿using System;
using System.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal unsafe class Dx12CommandBuffer : RHICommandBuffer
    {
        public ID3D12GraphicsCommandList5* NativeCommandList
        {
            get
            {
                return m_NativeCommandList;
            }
        }

        private ID3D12GraphicsCommandList5* m_NativeCommandList;

        public Dx12CommandBuffer(Dx12CommandPool cmdPool)
        {
            m_CommandPool = cmdPool;
            Dx12Queue queue = cmdPool.Queue as Dx12Queue;
            Dx12CommandPool dx12CommandPool = m_CommandPool as Dx12CommandPool;

            ID3D12GraphicsCommandList5* commandList;
            bool success = SUCCEEDED(queue.Dx12Device.NativeDevice->CreateCommandList(0, Dx12Utility.ConvertToNativeQueueType(queue.Type), dx12CommandPool.NativeCommandAllocator, null, __uuidof<ID3D12GraphicsCommandList5>(), (void**)&commandList));
            Debug.Assert(success);
            m_NativeCommandList = commandList;
        }

        public override void Begin()
        {
            Dx12CommandPool dx12CommandPool = m_CommandPool as Dx12CommandPool;
            Dx12Queue queue = m_CommandPool.Queue as Dx12Queue;

            m_NativeCommandList->Close();
            m_NativeCommandList->Reset(dx12CommandPool.NativeCommandAllocator, null);

            ID3D12DescriptorHeap** resourceBarriers = stackalloc ID3D12DescriptorHeap*[2];
            resourceBarriers[0] = queue.Dx12Device.SamplerHeap.DescriptorHeap;
            resourceBarriers[1] = queue.Dx12Device.CbvSrvUavHeap.DescriptorHeap;
            m_NativeCommandList->SetDescriptorHeaps(2, &*resourceBarriers);
        }

        public override RHIBlitEncoder GetBlitEncoder()
        {
            return new Dx12BlitEncoder(this);
        }

        public override RHIComputeEncoder GetComputeEncoder()
        {
            return new Dx12ComputeEncoder(this);
        }

        public override RHIGraphicsEncoder GetGraphicsEncoder()
        {
            return new Dx12GraphicsEncoder(this);
        }

        public override void End()
        {
            m_NativeCommandList->Close();
        }

        public override void Commit(RHIFence? fence)
        {
            Dx12CommandPool dx12CommandPool = m_CommandPool as Dx12CommandPool;
            Dx12Queue queue = m_CommandPool.Queue as Dx12Queue;

            ID3D12CommandList** ppCommandLists = stackalloc ID3D12CommandList*[1] { (ID3D12CommandList*)m_NativeCommandList };
            queue.NativeCommandQueue->ExecuteCommandLists(1, ppCommandLists);

            if (fence != null)
            {
                Dx12Fence dx12Fence = fence as Dx12Fence;
                dx12Fence.Reset();
                queue.NativeCommandQueue->Signal(dx12Fence.NativeFence, 1);
            }
        }

        protected override void Release()
        {
            m_NativeCommandList->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}