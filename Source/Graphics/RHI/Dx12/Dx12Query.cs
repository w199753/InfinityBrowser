using System;
using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602

    internal unsafe class Dx12Query : RHIQuery
    {
        private ID3D12Resource* m_QueryResult;
        private ID3D12QueryHeap* m_QueryHeap;

        public Dx12Query(Dx12Device device, in RHIQueryDescription descriptor)
        {

        }

        public override bool ResolveData(RHIBlitEncoder blotEncoder)
        {
            throw new NotImplementedException();
        }

        public override bool ReadData(in uint startIndex, in uint count, in Span<ulong> results)
        {
            throw new NotImplementedException();
        }

        protected override void Release()
        {
            m_QueryHeap->Release();
            m_QueryResult->Release();
        }
    }
#pragma warning restore CS8600, CS8602
}
