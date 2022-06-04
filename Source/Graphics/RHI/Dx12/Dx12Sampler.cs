using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class Dx12Sampler : RHISampler
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
        private Dx12Device m_Dx12Device;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private D3D12_CPU_DESCRIPTOR_HANDLE m_NativeCpuDescriptorHandle;
        private D3D12_GPU_DESCRIPTOR_HANDLE m_NativeGpuDescriptorHandle;

        public Dx12Sampler(Dx12Device device, in RHISamplerCreateInfo createInfo)
        {
            m_Dx12Device = device;

            D3D12_SAMPLER_DESC desc = new D3D12_SAMPLER_DESC();
            desc.AddressU = /*Dx12Utility.GetNativeAddressMode(createInfo->addressModeU)*/D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP;
            desc.AddressV = /*Dx12Utility.GetNativeAddressMode(createInfo->addressModeV)*/D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP;
            desc.AddressW = /*Dx12Utility.GetNativeAddressMode(createInfo->addressModeW)*/D3D12_TEXTURE_ADDRESS_MODE.D3D12_TEXTURE_ADDRESS_MODE_WRAP;
            desc.Filter = Dx12Utility.ConvertToNativeFilter(createInfo);
            desc.MinLOD = createInfo.lodMinClamp;
            desc.MaxLOD = createInfo.lodMaxClamp;
            desc.ComparisonFunc = /*Dx12Utility.GetNativeComparisonFunc(createInfo->comparisonFunc)*/D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_ALWAYS;
            desc.MaxAnisotropy = (uint)createInfo.maxAnisotropy;

            Dx12DescriptorInfo allocation = device.AllocateSamplerDescriptor(1);
            m_HeapIndex = allocation.index;
            m_NativeDescriptorHeap = allocation.descriptorHeap;
            m_NativeCpuDescriptorHandle = allocation.cpuHandle;
            m_NativeGpuDescriptorHandle = allocation.gpuHandle;
            device.NativeDevice->CreateSampler(&desc, m_NativeCpuDescriptorHandle);
        }

        protected override void Release()
        {
            m_Dx12Device.FreeSamplerDescriptor(m_HeapIndex);
        }
    }
}
