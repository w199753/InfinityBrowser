namespace Infinity.Graphics
{
    public struct RHITextureViewDescriptor
    {
        public int mipCount;
        public int baseMipLevel;
        public int arrayLayerCount;
        public int baseArrayLayer;
        public EPixelFormat format;
        public ETextureViewType viewType;
        public ETextureViewDimension dimension;
    }

    public abstract class RHITextureView : Disposal
    {

    }
}
