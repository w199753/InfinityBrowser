using System;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindInfo
    {
        public int slot;
        public int index;
        public int count;
        public EBindType bindType;
        public EShaderStage shaderStage;

        internal bool Bindless => bindType == EBindType.ArrayTexture;
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
        public Dx12BindInfo[] BindInfos
        {
            get
            {
                return m_BindInfos;
            }
        }

        private int m_Index;
        private Dx12BindInfo[] m_BindInfos;

        public Dx12BindGroupLayout(in RHIBindGroupLayoutDescriptor descriptor)
        {
            m_Index = descriptor.index;
            m_BindInfos = new Dx12BindInfo[descriptor.elements.Length];

            Span<RHIBindGroupLayoutElement> elements = descriptor.elements.Span;
            for (int i = 0; i < descriptor.elements.Length; ++i)
            {
                ref RHIBindGroupLayoutElement element = ref elements[i];
                ref Dx12BindInfo bindInfo = ref m_BindInfos[i];
                bindInfo.slot = element.slot;
                bindInfo.index = descriptor.index;
                bindInfo.count = element.count;
                bindInfo.bindType = element.bindType;
                bindInfo.shaderStage = element.shaderStage;
            }
        }

        protected override void Release()
        {
            m_Index = 0;
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
