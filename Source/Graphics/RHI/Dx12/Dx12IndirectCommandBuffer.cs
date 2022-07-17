using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal unsafe class Dx12IndirectCommandBuffer : RHIIndirectCommandBuffer
    {
        public Dx12IndirectCommandBuffer(Dx12Device device, in RHIIndirectCommandBufferDescription descriptor)
        {

        }

        protected override void Release()
        {

        }
    }
#pragma warning restore CS8600, CS8602
}
