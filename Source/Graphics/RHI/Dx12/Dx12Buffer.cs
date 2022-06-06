using System;
using System.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
    internal unsafe class Dx12Buffer : RHIBuffer
    {
        public Dx12Device Dx12Device
        {
            get
            {
                return m_Dx12Device;
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
        private Dx12Device m_Dx12Device;
        private EBufferUsageFlags m_Usages;
        private ID3D12Resource* m_NativeResource;

        public Dx12Buffer(Dx12Device device, in RHIBufferCreateInfo createInfo)
        {
            m_Dx12Device = device;
            m_Usages = createInfo.usages;
            m_MapMode = Dx12Utility.GetMapModeByUsage(createInfo.usages);

            ID3D12Resource* dx12Resource;
            D3D12_RESOURCE_DESC resourceDesc = D3D12_RESOURCE_DESC.Buffer((ulong)createInfo.size, Dx12Utility.ConvertToDX12ResourceFlagByUsage(createInfo.usages));
            D3D12_HEAP_PROPERTIES heapProperties = new D3D12_HEAP_PROPERTIES(Dx12Utility.GetDX12HeapTypeByUsage(createInfo.usages));
            bool success = SUCCEEDED(m_Dx12Device.NativeDevice->CreateCommittedResource(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &resourceDesc, Dx12Utility.ConvertToDX12BufferState(createInfo.state), null, __uuidof<ID3D12Resource>(), (void**)&dx12Resource));
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
            return new Dx12BufferView(this, createInfo);
        }

        protected override void Release()
        {
            m_NativeResource->Release();
        }
    }
}
