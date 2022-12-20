using Infinity.Core;
using Infinity.Mathmatics;
using System.Runtime.InteropServices;

namespace Infinity.Graphics
{
    public enum EAccelerationStructureFlags : byte
    {
        None,
        AllowUpdate,
        PerformUpdate,
        MinimizeMemory,
        PreferFastTrace,
        PreferFastBuild,
        AllowCompactation
    }

    public enum EAccelerationStructureGeometryFlags : byte
    {
        None,
        Opaque,
        NoDuplicateAnyhitInverseOcation
    }

    public enum EAccelerationStructureInstanceFlags : byte
    {
        None,
        ForceOpaque,
        ForceNonOpaque,
        TriangleCullDisable,
        TriangleFrontCounterclockwise
    }

    public class RHIAccelerationStructureGeometry : Disposal
    {
        public EAccelerationStructureGeometryFlags Flags;

        protected RHIAccelerationStructureGeometry()
        {

        }
    }

    public class RHIAccelerationStructureAABBs : RHIAccelerationStructureGeometry
    {
        public uint Stride;
        public uint Offset;
        public ulong Count;
        public RHIBuffer? AABBs;
    }

    public class RHIAccelerationStructureTriangles : RHIAccelerationStructureGeometry
    {
        public uint IndexCount;
        public uint IndexOffset;
        public RHIBuffer? IndexBuffer;
        public EIndexFormat IndexFormat;
        public uint VertexCount;
        public uint VertexStride;
        public uint VertexOffset;
        public RHIBuffer? VertexBuffer;
        public EPixelFormat VertexFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RHIAccelerationStructureInstance
    {
        public EAccelerationStructureInstanceFlags Flags;
        public float4x4 TransformMatrix;
        public uint InstanceID;
        public byte InstanceMask;
        public uint InstanceContributionToHitGroupIndex;
        public RHIBottomLevelAccelerationStructure BottonLevelAS;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RHITopLevelAccelerationStructureDescriptor
    {
        public uint Offset;
        public EAccelerationStructureFlags Flags;
        public RHIAccelerationStructureInstance[] Instances;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RHIBottomLevelAccelerationStructureDescriptor
    {
        public RHIAccelerationStructureGeometry[] Geometries;
    }

    public abstract class RHITopLevelAccelerationStructure : Disposal
    {
        public RHITopLevelAccelerationStructureDescriptor Descriptor;

        protected RHITopLevelAccelerationStructure(RHIDevice device, ref RHITopLevelAccelerationStructureDescriptor descriptor)
        {
            Descriptor = descriptor;
        }
    }

    public abstract class RHIBottomLevelAccelerationStructure : Disposal
    {
        public RHIBottomLevelAccelerationStructureDescriptor Descriptor;

        protected RHIBottomLevelAccelerationStructure(RHIDevice device, in RHIBottomLevelAccelerationStructureDescriptor descriptor)
        {
            Descriptor = descriptor;
        }
    }
}
