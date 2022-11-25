namespace Infinity.Graphics
{
    public struct RHISamplerStateDescriptor
    {
        public int Anisotropy;
        public float LodMinClamp;
        public float LodMaxClamp;
        public EFilterMode MagFilter;
        public EFilterMode MinFilter;
        public EFilterMode MipFilter;
        public EAddressMode AddressModeU;
        public EAddressMode AddressModeV;
        public EAddressMode AddressModeW;
        public EComparisonMode ComparisonMode;
    }

    public abstract class RHISamplerState : Disposal
    {

    }
}
