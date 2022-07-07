﻿using System;

namespace Infinity.Graphics
{
    public struct RHIInstanceDescriptor
    {
        public ERHIBackend backend;
        public bool enableDebugLayer;
        public bool enableGpuValidatior;
    }

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

        public static ERHIBackend GetPlatformBackend(in bool bForceVulkan)
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

        public static RHIInstance? Create(in RHIInstanceDescriptor descriptor)
        {
            switch (descriptor.backend)
            {
                case ERHIBackend.Metal:
                    return new MtlInstance(descriptor);

                case ERHIBackend.Vulkan:
                    return new VkInstance(descriptor);

                case ERHIBackend.DirectX12:
                    return new Dx12Instance(descriptor);

                default:
                    return null;
            }
        }
    }
}
