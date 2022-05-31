using System;
using System.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal unsafe class D3DCommandBuffer : RHICommandBuffer
    {
        public D3DCommandPool? D3DCommandPool
        {
            get
            {
                return m_D3DCommandPool;
            }
        }
        public ID3D12GraphicsCommandList5* NativeCommandList
        {
            get
            {
                return m_NativeCommandList;
            }
        }

        private D3DCommandPool? m_D3DCommandPool;
        private ID3D12GraphicsCommandList5* m_NativeCommandList;

        public D3DCommandBuffer(D3DDevice device, D3DCommandPool cmdPool)
        {
            m_QueueType = cmdPool.Type;
            m_D3DCommandPool = cmdPool;

            ID3D12GraphicsCommandList5* commandList;
            bool success = SUCCEEDED(device.NativeDevice->CreateCommandList(0, D3DUtility.ConvertToNativeQueueType(m_D3DCommandPool.Type), m_D3DCommandPool.NativeCommandAllocator, null, __uuidof<ID3D12GraphicsCommandList5>(), (void**)&commandList));
            Debug.Assert(success);
            m_NativeCommandList = commandList;
        }

        public override void Begin()
        {
            m_NativeCommandList->Close();
            m_NativeCommandList->Reset(m_D3DCommandPool.NativeCommandAllocator, null);
        }

        public override RHIScopedCommandBufferRef BeginScoped()
        {
            Begin();
            return new RHIScopedCommandBufferRef(this);
        }

        public override RHIBlitEncoder GetBlitEncoder()
        {
            return new D3DBlitEncoder(this);
        }

        public override RHIComputeEncoder GetComputeEncoder()
        {
            throw new NotImplementedException();
        }

        public override RHIGraphicsEncoder GetGraphicsEncoder()
        {
            return new D3DGraphicsEncoder(this);
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
