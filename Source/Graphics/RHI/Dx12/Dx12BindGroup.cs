using System.Diagnostics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindGroupParameter
    {
        //public int slot;
        //public int count;
        //public EBindType bindType;
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
        public Dx12BindGroupParameter[] BindParameters
        {
            get
            {
                return m_BindParameters;
            }
        }

        private Dx12BindGroupLayout m_BindGroupLayout;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private Dx12BindGroupParameter[] m_BindParameters;

        public Dx12BindGroup(in RHIBindGroupDescriptor descriptor)
        {
            Dx12BindGroupLayout bindGroupLayout = descriptor.layout as Dx12BindGroupLayout;
            Debug.Assert(bindGroupLayout != null);

            m_BindGroupLayout = bindGroupLayout;
            m_BindParameters = new Dx12BindGroupParameter[descriptor.elementCount];

            for (int i = 0; i < descriptor.elementCount; ++i)
            {
                ref RHIBindGroupElement element = ref descriptor.elements.Span[i];
                ref Dx12RootParameterKeyInfo keyInfo = ref bindGroupLayout.RootParameterKeyInfos[i];

                //D3D12_GPU_DESCRIPTOR_HANDLE handle = default;
                ref Dx12BindGroupParameter bindParameter = ref m_BindParameters[i];
                GetDescriptorHandleAndHeap(ref bindParameter.dx12GpuDescriptorHandle, ref m_NativeDescriptorHeap, keyInfo, element);

                //ref Dx12BindGroupParameter bindParameter = ref m_BindParameters[i];
                //bindParameter.slot = element.slot;
                //bindParameter.slot = keyInfo.slot;
                //bindParameter.count = keyInfo.count;
                //bindParameter.bindType = element.bindType;
                //bindParameter.bindType = keyInfo.bindType;
                //bindParameter.dx12GpuDescriptorHandle = handle;
            }
        }

        internal unsafe static void GetDescriptorHandleAndHeap(ref D3D12_GPU_DESCRIPTOR_HANDLE handle, ref ID3D12DescriptorHeap* heap, in Dx12RootParameterKeyInfo keyInfo, in RHIBindGroupElement element)
        {
            if (keyInfo.bindType == EBindType.Sampler)
            {
                if (keyInfo.Bindless)
                {

                }
                else
                {
                    Dx12Sampler textureSampler = element.textureSampler as Dx12Sampler;
                    heap = textureSampler.NativeDescriptorHeap;
                    handle = textureSampler.NativeGpuDescriptorHandle;
                }
            }
            else if (keyInfo.bindType == EBindType.Texture || keyInfo.bindType == EBindType.StorageTexture)
            {
                if (keyInfo.Bindless)
                {

                }
                else
                {
                    Dx12TextureView textureView = element.textureView as Dx12TextureView;
                    heap = textureView.NativeDescriptorHeap;
                    handle = textureView.NativeGpuDescriptorHandle;
                }
            }
            else if (keyInfo.bindType == EBindType.Uniform || keyInfo.bindType == EBindType.Buffer || keyInfo.bindType == EBindType.StorageBuffer)
            {
                if (keyInfo.Bindless)
                {

                }
                else
                {
                    Dx12BufferView bufferView = element.bufferView as Dx12BufferView;
                    heap = bufferView.NativeDescriptorHeap;
                    handle = bufferView.NativeGpuDescriptorHandle;
                }
            }
        }

        protected override void Release()
        {
            m_NativeDescriptorHeap = null;
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
