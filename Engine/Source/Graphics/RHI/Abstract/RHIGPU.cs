namespace Infinity.Graphics
{
    public struct RHIGpuProperty
    {
        public EGpuType type;
        public uint vendorId;
        public uint deviceId;
    }

    public abstract class RHIGPU : Disposal
    {
        public abstract RHIGpuProperty GetProperty();
        public abstract RHIDevice CreateDevice(in RHIDeviceCreateInfo createInfo);
    }
}
