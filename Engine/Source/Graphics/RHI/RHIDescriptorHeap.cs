using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
    internal enum EDescriptorType
    {
        DSV = 0,
        RTV = 1,
        CbvSrvUav = 2,
        Sampler = 3
    };

    internal abstract class RHIDescriptorHeapFactory : Disposal
    {
        protected EDescriptorType m_Type;

        public RHIDescriptorHeapFactory(RHIDevice device, in EDescriptorType type, in uint count, string name)
        {
            m_Type = type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract int Allocate();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract int Allocate(in int count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Free(in int index);
    }
}
