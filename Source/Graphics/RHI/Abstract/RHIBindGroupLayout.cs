using System;

namespace Infinity.Graphics
{
    public struct RHIBindGroupLayoutEntry
    {
        public int slot;
        public EBindingType type;
        public EShaderStageFlags shaderVisibility;
    }

    public struct RHIBindGroupLayoutCreateInfo
    {
        public int entryCount;
        public int layoutIndex;
        public Memory<RHIBindGroupLayoutEntry> entries;
    }

    public abstract class RHIBindGroupLayout : Disposal
    {

    }
}
