using System;

namespace Infinity.Graphics
{
    public abstract class RHIInstance : Disposal
    {
        public abstract int GpuCount
        {
            get;
        }
        public abstract ERHIBackend RHIType
        {
            get;
        }

        public abstract RHIGPU GetGpu(in int index);

        internal static ERHIBackend GetPlatformRHIBackend(in bool bForceVulkan)
        {
            ERHIBackend rhiType = bForceVulkan ? ERHIBackend.Vulkan : ERHIBackend.DirectX12;

            if (OperatingSystem.IsMacOS() || OperatingSystem.IsIOS())
            {
                rhiType = bForceVulkan ? ERHIBackend.Vulkan : ERHIBackend.Metal;
            }

            if (OperatingSystem.IsLinux() || OperatingSystem.IsAndroid())
            {
                rhiType = ERHIBackend.Vulkan;
            }

            return rhiType;
        }

        public static RHIInstance CreateByBackend(ERHIBackend type)
        {
            switch (type)
            {
                case ERHIBackend.Metal:
                    return new MtlInstance();

                case ERHIBackend.Vulkan:
                    return new VkInstance();

                case ERHIBackend.DirectX12:
                    return new D3DInstance();

                default:
                    return null;
            }
        }

        public static RHIInstance CreateByPlatform(in bool bForceVulkan = false)
        {
            return CreateByBackend(GetPlatformRHIBackend(bForceVulkan));
        }
    }
}
