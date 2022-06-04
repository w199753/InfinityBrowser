using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12RootParameterKeyInfo
    {
        public int slot;
        public int layoutIndex;
        public EBindingType bindType;
        public EShaderStageFlags shaderStage;
    }

    internal unsafe class Dx12BindGroupLayout : RHIBindGroupLayout
    {
        public int LayoutIndex
        {
            get
            {
                return m_LayoutIndex;
            }
        }
        public List<D3D12_ROOT_PARAMETER1> NativeRootParameters
        {
            get
            {
                return m_NativeRootParameters;
            }
        }
        public List<Dx12RootParameterKeyInfo> RootParameterKeyInfos
        {
            get
            {
                return m_RootParameterKeyInfos;
            }
        }

        private int m_LayoutIndex;
        private List<D3D12_ROOT_PARAMETER1> m_NativeRootParameters;
        private List<Dx12RootParameterKeyInfo> m_RootParameterKeyInfos;

        public Dx12BindGroupLayout(in RHIBindGroupLayoutCreateInfo createInfo)
        {
            m_LayoutIndex = createInfo.layoutIndex;
            m_NativeRootParameters = new List<D3D12_ROOT_PARAMETER1>(16);
            m_RootParameterKeyInfos = new List<Dx12RootParameterKeyInfo>(16);

            Dictionary<EShaderStageFlags, List<RHIBindGroupLayoutElement>> visiblityMap = new Dictionary<EShaderStageFlags, List<RHIBindGroupLayoutElement>>();
            visiblityMap.TryAdd(EShaderStageFlags.Vertex, new List<RHIBindGroupLayoutElement>(32));
            visiblityMap.TryAdd(EShaderStageFlags.Fragment, new List<RHIBindGroupLayoutElement>(32));
            visiblityMap.TryAdd(EShaderStageFlags.Compute, new List<RHIBindGroupLayoutElement>(32));
            for (int i = 0; i < createInfo.elementCount; ++i)
            {
                foreach (KeyValuePair<EShaderStageFlags, List<RHIBindGroupLayoutElement>> visiblity in visiblityMap)
                {
                    if ((createInfo.elements.Span[i].shaderVisibility & visiblity.Key) == visiblity.Key)
                    {
                        continue;
                    }
                    visiblity.Value.Add(createInfo.elements.Span[i]);
                }
            }

            foreach (KeyValuePair<EShaderStageFlags, List<RHIBindGroupLayoutElement>> visiblity in visiblityMap)
            {
                foreach (RHIBindGroupLayoutElement element in visiblity.Value)
                {
                    m_NativeRootParameters.Add(default);
                    {
                        D3D12_DESCRIPTOR_RANGE1 dx12DescriptorRange = new D3D12_DESCRIPTOR_RANGE1();
                        dx12DescriptorRange.Init(/*Dx12Utility.GetNativeBindingType(entry.type)*/D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV, 1, (uint)element.slot, (uint)createInfo.layoutIndex, D3D12_DESCRIPTOR_RANGE_FLAGS.D3D12_DESCRIPTOR_RANGE_FLAG_DATA_STATIC);
                        m_NativeRootParameters[m_NativeRootParameters.Count - 1].InitAsDescriptorTable(1, &dx12DescriptorRange, /*Dx12Utility.GetNativeShaderStage(visiblity.Key)*/D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL);
                    }

                    m_RootParameterKeyInfos.Add(default);
                    {
                        Dx12RootParameterKeyInfo keyInfo;
                        keyInfo.slot = element.slot;
                        keyInfo.layoutIndex = createInfo.layoutIndex;
                        keyInfo.bindType = element.bindType;
                        keyInfo.shaderStage = visiblity.Key;
                        m_RootParameterKeyInfos[m_RootParameterKeyInfos.Count - 1] = keyInfo;
                    }
                }
            }
        }

        protected override void Release()
        {
            m_LayoutIndex = 0;
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
