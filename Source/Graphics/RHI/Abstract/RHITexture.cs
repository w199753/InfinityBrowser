using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHITextureDescriptor
    {
        public int mipCount;
        public int3 extent;
        public int2 samples;
        public EPixelFormat format;
        public ETextureDimension dimension;
        public ETextureState state;
        public ETextureUsage usage;
        public EStorageMode storageMode;
    }

    public abstract class RHITexture : Disposal
    {
        public RHITextureDescriptor Descriptor
        {
            get
            {
                return m_Descriptor;
            }
        }

        protected RHITextureDescriptor m_Descriptor;

        public abstract RHITextureView CreateTextureView(in RHITextureViewDescriptor descriptor);
    }
}
