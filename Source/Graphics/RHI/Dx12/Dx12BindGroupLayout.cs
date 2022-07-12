using System;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12RootParameterKeyInfo
    {
        public int slot;
        public int index;
        public int count;
        public EBindType bindType;
        public EShaderStage shaderStage;

        internal bool Bindless => count > 1;
    }

    internal unsafe class Dx12BindGroupLayout : RHIBindGroupLayout
    {
        public int Index
        {
            get
            {
                return m_Index;
            }
        }
        /*public D3D12_ROOT_PARAMETER1[] NativeRootParameters
        {
            get
            {
                return m_NativeRootParameters;
            }
        }*/
        public Dx12RootParameterKeyInfo[] RootParameterKeyInfos
        {
            get
            {
                return m_RootParameterKeyInfos;
            }
        }

        private int m_Index;
        //private D3D12_ROOT_PARAMETER1[] m_NativeRootParameters;
        private Dx12RootParameterKeyInfo[] m_RootParameterKeyInfos;

        public Dx12BindGroupLayout(in RHIBindGroupLayoutDescriptor descriptor)
        {
            m_Index = descriptor.index;
            //m_NativeRootParameters = new D3D12_ROOT_PARAMETER1[descriptor.elementCount];
            m_RootParameterKeyInfos = new Dx12RootParameterKeyInfo[descriptor.elements.Length];

            Span<RHIBindGroupLayoutElement> elements = descriptor.elements.Span;
            for (int i = 0; i < descriptor.elements.Length; ++i)
            {
                //ref RHIBindGroupLayoutElement element = ref elements[i];
                //D3D12_DESCRIPTOR_RANGE1 dx12DescriptorRange = new D3D12_DESCRIPTOR_RANGE1();
                //dx12DescriptorRange.Init(Dx12Utility.ConvertToDx12BindType(element.bindType), 1, (uint)element.slot, (uint)descriptor.layoutIndex, D3D12_DESCRIPTOR_RANGE_FLAGS.D3D12_DESCRIPTOR_RANGE_FLAG_DATA_STATIC);
                //m_NativeRootParameters[i].InitAsDescriptorTable(1, &dx12DescriptorRange, Dx12Utility.ConvertToDx12ShaderStage(element.shaderStage));

                ref RHIBindGroupLayoutElement element = ref elements[i];
                ref Dx12RootParameterKeyInfo keyInfo = ref m_RootParameterKeyInfos[i];
                keyInfo.slot = element.slot;
                keyInfo.index = descriptor.index;
                keyInfo.count = element.count;
                keyInfo.bindType = element.bindType;
                keyInfo.shaderStage = element.shaderStage;
            }
        }

        protected override void Release()
        {
            m_Index = 0;
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
