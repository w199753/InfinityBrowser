﻿using System;
using System.Diagnostics;
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

        public Dx12Buffer(Dx12Device device, in RHIBufferDescriptor descriptor)
        {
            m_Dx12Device = device;
            m_Descriptor = descriptor;
            m_SizeInBytes = (uint)descriptor.Size;

            D3D12_RESOURCE_DESC resourceDesc = D3D12_RESOURCE_DESC.Buffer((ulong)descriptor.Size, Dx12Utility.ConvertToDx12BufferFlag(descriptor.Usage));
            D3D12_HEAP_PROPERTIES heapProperties = new D3D12_HEAP_PROPERTIES(Dx12Utility.ConvertToDx12ResourceFlagByUsage(descriptor.StorageMode));

            ID3D12Resource* dx12Resource;
            bool success = SUCCEEDED(m_Dx12Device.NativeDevice->CreateCommittedResource(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &resourceDesc, Dx12Utility.ConvertToDx12BufferState(descriptor.State), null, __uuidof<ID3D12Resource>(), (void**)&dx12Resource));
            Debug.Assert(success);
            m_NativeResource = dx12Resource;
        }

        public override IntPtr Map(in int length, in int offset)
        {
            Debug.Assert(!(m_Descriptor.StorageMode == EStorageMode.Default));

            void* data;
            D3D12_RANGE range = new D3D12_RANGE((uint)offset, (uint)(offset + length));
            bool success = SUCCEEDED(m_NativeResource->Map(0, &range, &data));
            Debug.Assert(success);
            return new IntPtr(data);
        }

        public override void UnMap()
        {
            Debug.Assert(!(m_Descriptor.StorageMode == EStorageMode.Default));

            m_NativeResource->Unmap(0, null);
        }

        public override RHIBufferView CreateBufferView(in RHIBufferViewDescriptor descriptor)
        {
            return new Dx12BufferView(this, descriptor);
        }

        protected override void Release()
        {
            m_NativeResource->Release();
        }
    }
}
