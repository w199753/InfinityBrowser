using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindGroupParameter
    {
        //public int Slot;
        //public int Count;
        //public EBindType BindType;
        public D3D12_GPU_DESCRIPTOR_HANDLE Dx12GpuDescriptorHandle;
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
        public Dx12BindGroupParameter[] BindParameters
        {
            get
            {
                return m_BindParameters;
            }
        }

        private Dx12BindGroupLayout m_BindGroupLayout;
        private Dx12BindGroupParameter[] m_BindParameters;

        public Dx12BindGroup(in RHIBindGroupDescriptor descriptor)
        {
            Dx12BindGroupLayout bindGroupLayout = descriptor.Layout as Dx12BindGroupLayout;
            Debug.Assert(bindGroupLayout != null);

            m_BindGroupLayout = bindGroupLayout;
            m_BindParameters = new Dx12BindGroupParameter[descriptor.Elements.Length];

            for (int i = 0; i < descriptor.Elements.Length; ++i)
            {
                ref Dx12BindInfo bindInfo = ref bindGroupLayout.BindInfos[i];
                ref RHIBindGroupElement element = ref descriptor.Elements.Span[i];

                ref Dx12BindGroupParameter bindParameter = ref m_BindParameters[i];
                switch (bindInfo.BindType)
                {
                    case EBindType.Buffer:
                    case EBindType.UniformBuffer:
                    case EBindType.StorageBuffer:
                        Dx12BufferView bufferView = element.BufferView as Dx12BufferView;
                        bindParameter.Dx12GpuDescriptorHandle = bufferView.NativeGpuDescriptorHandle;
                        break;

                    case EBindType.Texture:
                    case EBindType.StorageTexture:
                        Dx12TextureView textureView = element.TextureView as Dx12TextureView;
                        bindParameter.Dx12GpuDescriptorHandle = textureView.NativeGpuDescriptorHandle;
                        break;

                    case EBindType.SamplerState:
                        Dx12SamplerState samplerState = element.SamplerState as Dx12SamplerState;
                        bindParameter.Dx12GpuDescriptorHandle = samplerState.NativeGpuDescriptorHandle;
                        break;

                    case EBindType.Bindless:
                        //Todo Bindless
                        break;
                }
            }
        }

        public override void SetBindElement(in RHIBindGroupElement element, in EBindType bindType, in int slot)
        {
            ref Dx12BindGroupParameter bindParameter = ref m_BindParameters[slot];

            switch (bindType)
            {
                case EBindType.Buffer:
                case EBindType.UniformBuffer:
                case EBindType.StorageBuffer:
                    Dx12BufferView bufferView = element.BufferView as Dx12BufferView;
                    bindParameter.Dx12GpuDescriptorHandle = bufferView.NativeGpuDescriptorHandle;
                    break;

                case EBindType.Texture:
                case EBindType.StorageTexture:
                    Dx12TextureView textureView = element.TextureView as Dx12TextureView;
                    bindParameter.Dx12GpuDescriptorHandle = textureView.NativeGpuDescriptorHandle;
                    break;

                case EBindType.SamplerState:
                    Dx12SamplerState samplerState = element.SamplerState as Dx12SamplerState;
                    bindParameter.Dx12GpuDescriptorHandle = samplerState.NativeGpuDescriptorHandle;
                    break;

                case EBindType.Bindless:
                    //Todo Bindless
                    break;
            }
        }

        protected override void Release()
        {

        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
