using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHITextureCreateInfo
    {
        public int3 extent;
        public int2 samples;
        public int mipLevels;
        public EPixelFormat format;
        public ETextureFlag flag;
        public ETextureState state;
        public EResourceUsage usage;
        public ETextureDimension dimension;
    }

    public abstract class RHITexture : Disposal
    {
        public RHITextureCreateInfo CreateInfo
        {
            get
            {
                return m_CreateInfo;
            }
        }

        protected RHITextureCreateInfo m_CreateInfo;

        public abstract RHITextureView CreateTextureView(in RHITextureViewCreateInfo createInfo);
    }
}
