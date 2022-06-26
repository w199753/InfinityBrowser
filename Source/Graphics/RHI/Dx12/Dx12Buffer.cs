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
        public ID3D12Resource* NativeResource
        {
            get
            {
                return m_NativeResource;
            }
        }

        private Dx12Device m_Dx12Device;
        private ID3D12Resource* m_NativeResource;

        public Dx12Buffer(Dx12Device device, in RHIBufferCreateInfo createInfo)
        {
            m_Dx12Device = device;
            m_CreateInfo = createInfo;
            m_SizeInBytes = (uint)createInfo.size;

            D3D12_RESOURCE_DESC resourceDesc = D3D12_RESOURCE_DESC.Buffer((ulong)createInfo.size, Dx12Utility.ConvertToDx12BufferFlag(createInfo.flag));
            D3D12_HEAP_PROPERTIES heapProperties = new D3D12_HEAP_PROPERTIES(Dx12Utility.ConvertToDx12ResourceFlagByUsage(createInfo.usage));

            D3D12_RESOURCE_STATES initialState = Dx12Utility.ConvertToDx12BufferState(createInfo.state);
            if (createInfo.usage == EResourceUsage.Static || createInfo.usage == EResourceUsage.Dynamic)
            {
                initialState = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ;
            }
            if (createInfo.usage == EResourceUsage.Staging)
            {
                initialState = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST;
            }

            ID3D12Resource* dx12Resource;
            bool success = SUCCEEDED(m_Dx12Device.NativeDevice->CreateCommittedResource(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &resourceDesc, initialState, null, __uuidof<ID3D12Resource>(), (void**)&dx12Resource));
            Debug.Assert(success);
            m_NativeResource = dx12Resource;
        }

        public override IntPtr Map(in int length, in int offset)
        {
            Debug.Assert(!(m_CreateInfo.usage == EResourceUsage.Default));

            void* data;
            D3D12_RANGE range = new D3D12_RANGE((uint)offset, (uint)(offset + length));
            bool success = SUCCEEDED(m_NativeResource->Map(0, &range, &data));
            Debug.Assert(success);
            return new IntPtr(data);
        }

        public override void UnMap()
        {
            Debug.Assert(!(m_CreateInfo.usage == EResourceUsage.Default));

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
