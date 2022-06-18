using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHITextureCreateInfo
    {
        public int3 extent;
        public int2 samples;
        public int mipLevels;
        public EPixelFormat format;
        public ETextureState state;
        public ETextureDimension dimension;
        public ETextureUsage usages;
    }

    public abstract class RHITexture : Disposal
    {
        public abstract RHITextureView CreateTextureView(in RHITextureViewCreateInfo createInfo);
    }
}
