using System;

namespace Infinity.Graphics
{
#pragma warning disable CA1416
    internal class MtlGPU : RHIGPU
    {
        public IntPtr GpuPtr
        {
            get
            {
                return m_GpuPtr;
            }
        }
        public MtlInstance MTLInstance
        {
            get
            {
                return m_MTLInstance;
            }
        }

        private IntPtr m_GpuPtr;
        private MtlInstance m_MTLInstance;

        public MtlGPU(MtlInstance instance, IntPtr gpu)
        {
            m_GpuPtr = gpu;
            m_MTLInstance = instance;
        }

        public override RHIGpuProperty GetProperty()
        {
            RHIGpuProperty gpuProperty;
            gpuProperty.deviceId = 0;
            gpuProperty.vendorId = 0;
            gpuProperty.type = EGpuType.Hardware;
            return gpuProperty;
        }

        public override RHIDevice CreateDevice(in RHIDeviceCreateInfo createInfo)
        {
            return new MtlDevice(this, createInfo);
        }

        protected override void Release()
        {

        }
    }
#pragma warning restore CA1416
}
