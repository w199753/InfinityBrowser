namespace Infinity.Graphics
{
    public struct RHIBufferViewCreateInfo
    {
        public int count;
        public int offset;
        public int stride;
        public EBufferViewType type;
    }

    public abstract class RHIBufferView : Disposal
    {

    }
}
