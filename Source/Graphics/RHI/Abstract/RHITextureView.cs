namespace Infinity.Graphics
{
    public struct RHITextureViewCreateInfo
    {
        public int mipLevelNum;
        public int baseMipLevel;
        public int arrayLayerNum;
        public int baseArrayLayer;
        public EPixelFormat format;
        public ETextureViewType type;
        public ETextureViewDimension dimension;
    }

    public abstract class RHITextureView : Disposal
    {

    }
}
