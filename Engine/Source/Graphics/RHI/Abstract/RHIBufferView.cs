namespace Infinity.Graphics
{
    public struct RHIIndexBufferViewInfo
    {
        public EIndexFormat format;
    }

    public struct RHIVertexBufferViewInfo
    {
        public int stride;
    }

    public struct RHIBufferViewCreateInfo
    {
        public int size;
        public int offset;
        public RHIIndexBufferViewInfo index;
        public RHIVertexBufferViewInfo vertex;
    }

    public abstract class RHIBufferView : Disposal
    {

    }
}
