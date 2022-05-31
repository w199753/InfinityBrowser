using System;
using System.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
    internal unsafe class D3DBuffer : RHIBuffer
    {
        public D3DDevice D3DDevice
        {
            get
            {
                return m_D3DDevice;
            }
        }
        public EBufferUsageFlags Usages
        {
            get
            {
                return m_Usages;
            }
        }
        public ID3D12Resource* NativeResource
        {
            get
            {
                return m_NativeResource;
            }
        }

        private EMapMode m_MapMode;
        private D3DDevice m_D3DDevice;
        private EBufferUsageFlags m_Usages;
        private ID3D12Resource* m_NativeResource;

        public D3DBuffer(D3DDevice device, in RHIBufferCreateInfo createInfo)
        {
            m_D3DDevice = device;
            m_Usages = createInfo.usages;
            m_MapMode = D3DUtility.GetMapModeByUsage(createInfo.usages);

            ID3D12Resource* dx12Resource;
            D3D12_RESOURCE_DESC resourceDesc = D3D12_RESOURCE_DESC.Buffer((ulong)createInfo.size);
            D3D12_HEAP_PROPERTIES heapProperties = new D3D12_HEAP_PROPERTIES(D3DUtility.GetDX12HeapTypeByUsage(createInfo.usages));
            bool success = SUCCEEDED(m_D3DDevice.NativeDevice->CreateCommittedResource(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &resourceDesc, D3DUtility.GetDX12ResourceStateByUsage(createInfo.usages), null, Windows.__uuidof<ID3D12Resource>(), (void**)&dx12Resource));
            Debug.Assert(success);
            m_NativeResource = dx12Resource;
        }

        public override IntPtr Map(in EMapMode mapMode, in int offset, in int length)
        {
            Debug.Assert(m_MapMode == mapMode);

            void* data;
            D3D12_RANGE range = new D3D12_RANGE((uint)offset, (uint)(offset + length));
            bool success = SUCCEEDED(m_NativeResource->Map(0, &range, &data));
            Debug.Assert(success);
            return new IntPtr(data);
        }

        public override void UnMap()
        {
            m_NativeResource->Unmap(0, null);
        }

        public override RHIBufferView CreateBufferView(in RHIBufferViewCreateInfo createInfo)
        {
            return new D3DBufferView(this, createInfo);
        }

        protected override void Release()
        {
            m_NativeResource->Release();
        }
    }
}
