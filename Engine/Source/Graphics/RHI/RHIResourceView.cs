using Vortice.Direct3D12;

namespace InfinityEngine.Graphics.RHI
{
    public class RHIResourceView : Disposal
    {
        internal int descriptorIndex;
        internal ulong virtualAddressGPU;
    }

    public class RHIIndexBufferView : RHIResourceView
    {

    }

    public class RHIVertexBufferView : RHIResourceView
    {

    }

    public class RHIDeptnStencilView : RHIResourceView
    {

    }

    public class RHIRenderTargetView : RHIResourceView
    {

    }

    public class RHIConstantBufferView : RHIResourceView
    {
        internal int descriptorSize;
        
        internal CpuDescriptorHandle descriptorHandle
        {
            get
            {
                return m_DescriptorHandle + descriptorSize * descriptorIndex;
            }
        }

        private CpuDescriptorHandle m_DescriptorHandle;

        public RHIConstantBufferView(int descriptorSize, int descriptorIndex, CpuDescriptorHandle descriptorHandle)
        {
            this.descriptorSize = descriptorSize;
            this.descriptorIndex = descriptorIndex;
            this.m_DescriptorHandle = descriptorHandle;
        }
    }

    public class RHIShaderResourceView : RHIResourceView
    {
        internal int descriptorSize;

        internal CpuDescriptorHandle descriptorHandle
        {
            get
            {
                return m_DescriptorHandle + descriptorSize * descriptorIndex;
            }
        }

        private CpuDescriptorHandle m_DescriptorHandle;

        public RHIShaderResourceView(int descriptorSize, int descriptorIndex, CpuDescriptorHandle descriptorHandle)
        {
            this.descriptorSize = descriptorSize;
            this.descriptorIndex = descriptorIndex;
            this.m_DescriptorHandle = descriptorHandle;
        }
    }

    public class RHIUnorderedAccessView : RHIResourceView
    {
        internal int descriptorSize;

        internal CpuDescriptorHandle descriptorHandle
        {
            get
            {
                return m_DescriptorHandle + descriptorSize * descriptorIndex;
            }
        }

        private CpuDescriptorHandle m_DescriptorHandle;

        public RHIUnorderedAccessView(int descriptorSize, int descriptorIndex, CpuDescriptorHandle descriptorHandle)
        {
            this.descriptorSize = descriptorSize;
            this.descriptorIndex = descriptorIndex;
            this.m_DescriptorHandle = descriptorHandle;
        }
    }

    public sealed class RHIResourceSet : Disposal
    {
        /*internal int length;
        internal int descriptorIndex;
        internal ID3D12Device6 nativeDevice;
        internal CpuDescriptorHandle descriptorHandle;*/

        internal RHIResourceSet(RHIDevice device, RHIDescriptorHeapFactory descriptorHeapFactory, in int length)
        {
            /*this.length = length;
            this.nativeDevice = device.nativeDevice;
            this.descriptorIndex = descriptorHeapFactory.Allocator(length);
            this.descriptorHandle = descriptorHeapFactory.GetCPUHandleStart();*/
        }

        private CpuDescriptorHandle GetDescriptorHandle(in int offset)
        {
            return default;
            //return descriptorHandle + nativeDevice.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView) * (descriptorIndex + offset);
        }

        public void SetShaderResourceView(in int slot, RHIShaderResourceView shaderResourceView)
        {
            //nativeDevice.CopyDescriptorsSimple(1, GetDescriptorHandle(slot), shaderResourceView.descriptorHandle, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
        }

        public void SetUnorderedAccessView(in int slot, RHIUnorderedAccessView unorderedAccessView)
        {
            //nativeDevice.CopyDescriptorsSimple(1, GetDescriptorHandle(slot), unorderedAccessView.descriptorHandle, DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
        }

        protected override void Release()
        {

        }
    }
}
