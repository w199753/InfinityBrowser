using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class D3DSampler : RHISampler
    {
        public ID3D12DescriptorHeap* NativeDescriptorHeap
        {
            get
            {
                return m_NativeDescriptorHeap;
            }
        }
        public D3D12_CPU_DESCRIPTOR_HANDLE NativeCpuDescriptorHandle
        {
            get
            {
                return m_NativeCpuDescriptorHandle;
            }
        }
        public D3D12_GPU_DESCRIPTOR_HANDLE NativeGpuDescriptorHandle
        {
            get
            {
                return m_NativeGpuDescriptorHandle;
            }
        }

        private int m_HeapIndex;
        private D3DDevice m_D3DDevice;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private D3D12_CPU_DESCRIPTOR_HANDLE m_NativeCpuDescriptorHandle;
        private D3D12_GPU_DESCRIPTOR_HANDLE m_NativeGpuDescriptorHandle;

        public D3DSampler(D3DDevice device, in RHISamplerCreateInfo createInfo)
        {
            m_D3DDevice = device;

            D3D12_SAMPLER_DESC desc = new D3D12_SAMPLER_DESC();
            desc.AddressU = /*D3DUtility.GetNativeAddressMode(createInfo->addressModeU)*/D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP;
            desc.AddressV = /*D3DUtility.GetNativeAddressMode(createInfo->addressModeV)*/D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP;
            desc.AddressW = /*D3DUtility.GetNativeAddressMode(createInfo->addressModeW)*/D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP;
            desc.Filter = D3DUtility.ConvertToNativeFilter(createInfo);
            desc.MinLOD = createInfo.lodMinClamp;
            desc.MaxLOD = createInfo.lodMaxClamp;
            desc.ComparisonFunc = /*D3DUtility.GetNativeComparisonFunc(createInfo->comparisonFunc)*/D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_ALWAYS;
            desc.MaxAnisotropy = (uint)createInfo.maxAnisotropy;

            D3DDescriptorInfo allocation = device.AllocateSamplerDescriptor(1);
            m_HeapIndex = allocation.index;
            m_NativeDescriptorHeap = allocation.descriptorHeap;
            m_NativeCpuDescriptorHandle = allocation.cpuHandle;
            m_NativeGpuDescriptorHandle = allocation.gpuHandle;
            device.NativeDevice->CreateSampler(&desc, m_NativeCpuDescriptorHandle);
        }

        protected override void Release()
        {
            m_D3DDevice.FreeSamplerDescriptor(m_HeapIndex);
        }
    }
}
