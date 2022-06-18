namespace Infinity.Graphics
{
    public struct RHIBufferViewCreateInfo
    {
        public int size;
        public int offset;
        public int stride;
        public EIndexFormat format;
        public EBufferViewType type;
    }

    public abstract class RHIBufferView : Disposal
    {

    }
}
