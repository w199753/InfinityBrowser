namespace Infinity.Graphics
{
    public struct RHISamplerCreateInfo
    {
        public int maxAnisotropy;
        public float lodMinClamp;
        public float lodMaxClamp;
        public EFilterMode magFilter;
        public EFilterMode minFilter;
        // TODO remove mip filter ?
        public EFilterMode mipFilter;
        public EAddressMode addressModeU;
        public EAddressMode addressModeV;
        public EAddressMode addressModeW;
        public EComparisonFunc comparisonFunc;
    }

    public abstract class RHISampler : Disposal
    {

    }
}
