using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS0169, CS0649, CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct D3DBindingTypeAndRootParameterIndex
    {
        public int index;
        public EBindingType bindType;
    }

    internal struct D3DRootParameterIndexMap
    {
        public Dictionary<int, D3DBindingTypeAndRootParameterIndex> slot;
    }

    internal unsafe class D3DPipelineLayout : RHIPipelineLayout
    {
        public ID3D12RootSignature* NativeRootSignature
        {
            get
            {
                return m_NativeRootSignature;
            }
        }

        private ID3D12RootSignature* m_NativeRootSignature;
        private Dictionary<EShaderStageFlags, D3DRootParameterIndexMap> m_RootDescriptorParameterIndexMap;

        public D3DPipelineLayout(D3DDevice device, in RHIBindGroupLayoutCreateInfo createInfo)
        {

        }

        public D3DBindingTypeAndRootParameterIndex QueryRootDescriptorParameterIndex(in EShaderStageFlags shaderStage, in int layoutIndex, in int binding)
        {
            return default;
        }

        protected override void Release()
        {

        }
    }
#pragma warning restore CS0169, CS0649, CS8600, CS8602, CS8604, CS8618, CA1416
}
