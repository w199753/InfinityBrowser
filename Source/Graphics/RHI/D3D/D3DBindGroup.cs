using System.Diagnostics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct D3DBindGroupParameter
    {
        public int slot;
        public EBindingType bindType;
        public D3D12_GPU_DESCRIPTOR_HANDLE dx12GpuDescriptorHandle;
    }

    internal unsafe class D3DBindGroup : RHIBindGroup
    {
        public D3DBindGroupLayout BindGroupLayout
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
        public List<D3DBindGroupParameter> BindingParameters
        {
            get
            {
                return m_BindingParameters;
            }
        }

        private D3DBindGroupLayout m_BindGroupLayout;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private List<D3DBindGroupParameter> m_BindingParameters;

        public D3DBindGroup(in RHIBindGroupCreateInfo createInfo)
        {
            D3DBindGroupLayout bindGroupLayout = createInfo.layout as D3DBindGroupLayout;
            Debug.Assert(bindGroupLayout != null);

            m_BindGroupLayout = bindGroupLayout;
            m_BindingParameters = new List<D3DBindGroupParameter>(32);

            for (int i = 0; i < createInfo.elementCount; ++i)
            {
                ref RHIBindGroupElement element = ref createInfo.elements.Span[i];
                D3DRootParameterKeyInfo keyInfo = bindGroupLayout.RootParameterKeyInfos[i];

                D3D12_GPU_DESCRIPTOR_HANDLE handle = default;
                GetDescriptorHandleAndHeap(ref handle, ref m_NativeDescriptorHeap, keyInfo.bindType, element);

                D3DBindGroupParameter bidning;
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
                D3DSampler sampler = element.sampler as D3DSampler;
                heap = sampler.NativeDescriptorHeap;
                handle = sampler.NativeGpuDescriptorHandle;
            }
            else if (bindType == EBindingType.Texture || bindType == EBindingType.StorageTexture)
            {
                D3DTextureView textureView = element.textureView as D3DTextureView;
                heap = textureView.NativeDescriptorHeap;
                handle = textureView.NativeGpuDescriptorHandle;
            }
            else if (bindType == EBindingType.UniformBuffer || bindType == EBindingType.StorageBuffer)
            {
                D3DBufferView bufferView = element.bufferView as D3DBufferView;
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
