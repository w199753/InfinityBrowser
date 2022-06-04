using System;
using System.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal unsafe class Dx12CommandBuffer : RHICommandBuffer
    {
        public Dx12CommandPool Dx12CommandPool
        {
            get
            {
                return m_Dx12CommandPool;
            }
        }
        public ID3D12GraphicsCommandList5* NativeCommandList
        {
            get
            {
                return m_NativeCommandList;
            }
        }

        private Dx12CommandPool m_Dx12CommandPool;
        private ID3D12GraphicsCommandList5* m_NativeCommandList;

        public Dx12CommandBuffer(Dx12Device device, Dx12CommandPool cmdPool)
        {
            m_QueueType = cmdPool.Type;
            m_Dx12CommandPool = cmdPool;

            ID3D12GraphicsCommandList5* commandList;
            bool success = SUCCEEDED(device.NativeDevice->CreateCommandList(0, Dx12Utility.ConvertToNativeQueueType(m_Dx12CommandPool.Type), m_Dx12CommandPool.NativeCommandAllocator, null, __uuidof<ID3D12GraphicsCommandList5>(), (void**)&commandList));
            Debug.Assert(success);
            m_NativeCommandList = commandList;
        }

        public override void Begin()
        {
            m_NativeCommandList->Close();
            m_NativeCommandList->Reset(m_Dx12CommandPool.NativeCommandAllocator, null);

            ID3D12DescriptorHeap** resourceBarriers = stackalloc ID3D12DescriptorHeap*[2];
            resourceBarriers[0] = m_Dx12CommandPool.Dx12Device.SamplerHeap.DescriptorHeap;
            resourceBarriers[1] = m_Dx12CommandPool.Dx12Device.CbvSrvUavHeap.DescriptorHeap;
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

        protected override void Release()
        {
            m_NativeCommandList->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
