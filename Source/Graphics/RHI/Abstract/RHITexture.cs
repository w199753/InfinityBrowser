using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHITextureCreateInfo
    {
        public int3 extent;
        public int2 samples;
        public int mipLevels;
        public EPixelFormat format;
        public ETextureDimension dimension;
        public ETextureState state;
        public ETextureUsage usage;
        public EStorageMode storageMode;
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
