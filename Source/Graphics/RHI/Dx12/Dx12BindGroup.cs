using System.Diagnostics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindGroupParameter
    {
        public int slot;
        public EBindingType bindType;
        public D3D12_GPU_DESCRIPTOR_HANDLE dx12GpuDescriptorHandle;
    }

    internal unsafe class Dx12BindGroup : RHIBindGroup
    {
        public Dx12BindGroupLayout BindGroupLayout
        {
            get
            {
                return m_BindGroupLayout;
            }
        }
        public ID3D12DescriptorHeap* NativeDescriptorHeap
        {
            get
            {
                return m_NativeDescriptorHeap;
            }
        }
        public List<Dx12BindGroupParameter> BindingParameters
        {
            get
            {
                return m_BindingParameters;
            }
        }

        private Dx12BindGroupLayout m_BindGroupLayout;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private List<Dx12BindGroupParameter> m_BindingParameters;

        public Dx12BindGroup(in RHIBindGroupCreateInfo createInfo)
        {
            Dx12BindGroupLayout bindGroupLayout = createInfo.layout as Dx12BindGroupLayout;
            Debug.Assert(bindGroupLayout != null);

            m_BindGroupLayout = bindGroupLayout;
            m_BindingParameters = new List<Dx12BindGroupParameter>(32);

            for (int i = 0; i < createInfo.elementCount; ++i)
            {
                ref RHIBindGroupElement element = ref createInfo.elements.Span[i];
                Dx12RootParameterKeyInfo keyInfo = bindGroupLayout.RootParameterKeyInfos[i];

                D3D12_GPU_DESCRIPTOR_HANDLE handle = default;
                GetDescriptorHandleAndHeap(ref handle, ref m_NativeDescriptorHeap, keyInfo.bindType, element);

                Dx12BindGroupParameter bidning;
                //bidning.slot = element.slot;
                bidning.slot = keyInfo.slot;
                //bidning.bindType = element.type;
                bidning.bindType = keyInfo.bindType;
                bidning.dx12GpuDescriptorHandle = handle;
                m_BindingParameters.Add(bidning);
            }
        }

        internal unsafe static void GetDescriptorHandleAndHeap(ref D3D12_GPU_DESCRIPTOR_HANDLE handle, ref ID3D12DescriptorHeap* heap, in EBindingType bindType, in RHIBindGroupElement element)
        {
            if (bindType == EBindingType.Sampler)
            {
                Dx12Sampler sampler = element.sampler as Dx12Sampler;
                heap = sampler.NativeDescriptorHeap;
                handle = sampler.NativeGpuDescriptorHandle;
            }
            else if (bindType == EBindingType.Texture || bindType == EBindingType.StorageTexture)
            {
                Dx12TextureView textureView = element.textureView as Dx12TextureView;
                heap = textureView.NativeDescriptorHeap;
                handle = textureView.NativeGpuDescriptorHandle;
            }
            else if (bindType == EBindingType.Buffer || bindType == EBindingType.UniformBuffer || bindType == EBindingType.StorageBuffer)
            {
                Dx12BufferView bufferView = element.bufferView as Dx12BufferView;
                heap = bufferView.NativeDescriptorHeap;
                handle = bufferView.NativeGpuDescriptorHandle;
            }
        }

        protected override void Release()
        {
            m_NativeDescriptorHeap = null;
            m_BindingParameters.Clear();
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
