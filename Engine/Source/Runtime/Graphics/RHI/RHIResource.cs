﻿using Vortice.DXGI;
using Vortice.Direct3D12;
using InfinityEngine.Core.Object;
using InfinityEngine.Core.Memory;
using InfinityEngine.Core.Mathmatics;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
    public enum EUsageType
    {
        Static = 0x1,
        Dynamic = 0x2,
        Staging = 0x4,
        Default = 0x8
    };

    public enum EBufferType
    {
        Index = 0,
        Vertex = 1,
        Append = 2,
        Consume = 3,
        Counter = 4,
        Constant = 5,
        Argument = 6,
        Structured = 7,
        ByteAddress = 8
    };

    public enum ETextureType
    {
        Tex2D = 0,
        Tex2DArray = 1,
        Tex2DSparse = 2,
        TexCube = 3,
        TexCubeArray = 4,
        Tex3D = 5,
        Tex3DSparse = 6,
    };

    public enum EResourceType
    {
        Buffer = 0,
        Texture = 1
    };

    public enum EDepthBits
    {
        None = 0,
        Depth8 = 8,
        Depth16 = 16,
        Depth24 = 24,
        Depth32 = 32
    }

    public enum EWrapMode
    {
        Clamp = 0,
        Repeat = 1,
        Mirror = 2,
        MirrorOnce = 3
    }

    public enum EFilterMode
    {
        Point = 0,
        Bilinear = 1,
        Trilinear = 2
    }

    public enum EMSAASample
    {
        None = 1,
        MSAA2x = 2,
        MSAA4x = 4,
        MSAA8x = 8
    }

    public enum EGraphicsFormat
    {
        None = 0,
        R8_SRGB = 1,
        R8G8_SRGB = 2,
        R8G8B8_SRGB = 3,
        R8G8B8A8_SRGB = 4,
        R8_UNorm = 5,
        R8G8_UNorm = 6,
        R8G8B8_UNorm = 7,
        R8G8B8A8_UNorm = 8,
        R8_SNorm = 9,
        R8G8_SNorm = 10,
        R8G8B8_SNorm = 11,
        R8G8B8A8_SNorm = 12,
        R8_UInt = 13,
        R8G8_UInt = 14,
        R8G8B8_UInt = 15,
        R8G8B8A8_UInt = 16,
        R8_SInt = 17,
        R8G8_SInt = 18,
        R8G8B8_SInt = 19,
        R8G8B8A8_SInt = 20,
        R16_UNorm = 21,
        R16G16_UNorm = 22,
        R16G16B16_UNorm = 23,
        R16G16B16A16_UNorm = 24,
        R16_SNorm = 25,
        R16G16_SNorm = 26,
        R16G16B16_SNorm = 27,
        R16G16B16A16_SNorm = 28,
        R16_UInt = 29,
        R16G16_UInt = 30,
        R16G16B16_UInt = 31,
        R16G16B16A16_UInt = 32,
        R16_SInt = 33,
        R16G16_SInt = 34,
        R16G16B16_SInt = 35,
        R16G16B16A16_SInt = 36,
        R32_UInt = 37,
        R32G32_UInt = 38,
        R32G32B32_UInt = 39,
        R32G32B32A32_UInt = 40,
        R32_SInt = 41,
        R32G32_SInt = 42,
        R32G32B32_SInt = 43,
        R32G32B32A32_SInt = 44,
        R16_SFloat = 45,
        R16G16_SFloat = 46,
        R16G16B16_SFloat = 47,
        R16G16B16A16_SFloat = 48,
        R32_SFloat = 49,
        R32G32_SFloat = 50,
        R32G32B32_SFloat = 51,
        R32G32B32A32_SFloat = 52,
        B8G8R8_SRGB = 56,
        B8G8R8A8_SRGB = 57,
        B8G8R8_UNorm = 58,
        B8G8R8A8_UNorm = 59,
        B8G8R8_SNorm = 60,
        B8G8R8A8_SNorm = 61,
        B8G8R8_UInt = 62,
        B8G8R8A8_UInt = 63,
        B8G8R8_SInt = 64,
        B8G8R8A8_SInt = 65,
        R4G4B4A4_UNormPack16 = 66,
        B4G4R4A4_UNormPack16 = 67,
        R5G6B5_UNormPack16 = 68,
        B5G6R5_UNormPack16 = 69,
        R5G5B5A1_UNormPack16 = 70,
        B5G5R5A1_UNormPack16 = 71,
        A1R5G5B5_UNormPack16 = 72,
        E5B9G9R9_UFloatPack32 = 73,
        B10G11R11_UFloatPack32 = 74,
        A2B10G10R10_UNormPack32 = 75,
        A2B10G10R10_UIntPack32 = 76,
        A2B10G10R10_SIntPack32 = 77,
        A2R10G10B10_UNormPack32 = 78,
        A2R10G10B10_UIntPack32 = 79,
        A2R10G10B10_SIntPack32 = 80,
        RGB_DXT1_SRGB = 96,
        RGBA_DXT1_SRGB = 96,
        RGB_DXT1_UNorm = 97,
        RGBA_DXT1_UNorm = 97,
        RGBA_DXT3_SRGB = 98,
        RGBA_DXT3_UNorm = 99,
        RGBA_DXT5_SRGB = 100,
        RGBA_DXT5_UNorm = 101,
        R_BC4_UNorm = 102,
        R_BC4_SNorm = 103,
        RG_BC5_UNorm = 104,
        RG_BC5_SNorm = 105,
        RGB_BC6H_UFloat = 106,
        RGB_BC6H_SFloat = 107,
        RGBA_BC7_SRGB = 108,
        RGBA_BC7_UNorm = 109,
    }

    public class FRHIResource : FDisposable
    {
        public string name;
        internal EUsageType useFlag;
        internal EResourceType resourceType;
        internal EGraphicsFormat graphicsFormat;
        internal ID3D12Resource uploadResource;
        internal ID3D12Resource defaultResource;
        internal ID3D12Resource readbackResource;

        public FRHIResource(in EUsageType useFlag) : base()
        {
            this.useFlag = useFlag;
        }

        protected override void Release()
        {
            uploadResource?.Dispose();
            defaultResource?.Dispose();
            readbackResource?.Dispose();
        }
    }

    public struct FRHIBufferDescription
    {
        public ulong count;
        public ulong stride;
        public string name;
        public EBufferType type;

        public FRHIBufferDescription(in ulong count, in ulong stride) : this()
        {
            this.count = count;
            this.stride = stride;
            type = EBufferType.Structured;
        }

        public FRHIBufferDescription(in ulong count, in ulong stride, in EBufferType type) : this()
        {
            this.type = type;
            this.count = count;
            this.stride = stride;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode += (int)type;
            hashCode += count.GetHashCode();
            hashCode += stride.GetHashCode();

            return hashCode;
        }
    }

    public class FRHIBuffer : FRHIResource
    {
        internal ulong count;
        internal ulong stride;
        internal EBufferType bufferType;

        internal FRHIBuffer(ID3D12Device6 d3dDevice, in EUsageType useFlag, in FRHIBufferDescription bufferDescription) : base(useFlag)
        {
            this.count = bufferDescription.count;
            this.stride = bufferDescription.stride;
            this.bufferType = bufferDescription.type;
            this.resourceType = EResourceType.Buffer;

            // GPUMemory
            if ((useFlag & EUsageType.Static) == EUsageType.Static || (useFlag & EUsageType.Dynamic) == EUsageType.Dynamic || (useFlag & EUsageType.Default) == EUsageType.Default)
            {
                HeapProperties defaultHeapProperty;
                {
                    defaultHeapProperty.Type = HeapType.Default;
                    defaultHeapProperty.CPUPageProperty = CpuPageProperty.Unknown;
                    defaultHeapProperty.MemoryPoolPreference = MemoryPool.Unknown;
                    defaultHeapProperty.CreationNodeMask = 1;
                    defaultHeapProperty.VisibleNodeMask = 1;
                }
                ResourceDescription defaultResourceDesc;
                {
                    defaultResourceDesc.Dimension = ResourceDimension.Buffer;
                    defaultResourceDesc.Alignment = 0;
                    defaultResourceDesc.Width = stride * count;
                    defaultResourceDesc.Height = 1;
                    defaultResourceDesc.DepthOrArraySize = 1;
                    defaultResourceDesc.MipLevels = 1;
                    defaultResourceDesc.Format = Format.Unknown;
                    defaultResourceDesc.SampleDescription.Count = 1;
                    defaultResourceDesc.SampleDescription.Quality = 0;
                    defaultResourceDesc.Layout = TextureLayout.RowMajor;
                    defaultResourceDesc.Flags = ResourceFlags.None;
                }
                defaultResource = d3dDevice.CreateCommittedResource<ID3D12Resource>(defaultHeapProperty, HeapFlags.None, defaultResourceDesc, ResourceStates.Common, null);
            }

            // UploadMemory
            if ((useFlag & EUsageType.Static) == EUsageType.Static || (useFlag & EUsageType.Dynamic) == EUsageType.Dynamic)
            {
                HeapProperties uploadHeapProperty;
                {
                    uploadHeapProperty.Type = HeapType.Upload;
                    uploadHeapProperty.CPUPageProperty = CpuPageProperty.Unknown;
                    uploadHeapProperty.MemoryPoolPreference = MemoryPool.Unknown;
                    uploadHeapProperty.CreationNodeMask = 1;
                    uploadHeapProperty.VisibleNodeMask = 1;
                }
                ResourceDescription uploadResourceDesc;
                {
                    uploadResourceDesc.Dimension = ResourceDimension.Buffer;
                    uploadResourceDesc.Alignment = 0;
                    uploadResourceDesc.Width = stride * count;
                    uploadResourceDesc.Height = 1;
                    uploadResourceDesc.DepthOrArraySize = 1;
                    uploadResourceDesc.MipLevels = 1;
                    uploadResourceDesc.Format = Format.Unknown;
                    uploadResourceDesc.SampleDescription.Count = 1;
                    uploadResourceDesc.SampleDescription.Quality = 0;
                    uploadResourceDesc.Layout = TextureLayout.RowMajor;
                    uploadResourceDesc.Flags = ResourceFlags.None;
                }
                uploadResource = d3dDevice.CreateCommittedResource<ID3D12Resource>(uploadHeapProperty, HeapFlags.None, uploadResourceDesc, ResourceStates.GenericRead, null);
            }

            // ReadbackMemory
            if ((useFlag & EUsageType.Staging) == EUsageType.Staging)
            {
                HeapProperties readbackHeapProperties;
                {
                    readbackHeapProperties.Type = HeapType.Readback;
                    readbackHeapProperties.CPUPageProperty = CpuPageProperty.Unknown;
                    readbackHeapProperties.MemoryPoolPreference = MemoryPool.Unknown;
                    readbackHeapProperties.CreationNodeMask = 1;
                    readbackHeapProperties.VisibleNodeMask = 1;
                }
                ResourceDescription readbackResourceDesc;
                {
                    readbackResourceDesc.Dimension = ResourceDimension.Buffer;
                    readbackResourceDesc.Alignment = 0;
                    readbackResourceDesc.Width = stride * count;
                    readbackResourceDesc.Height = 1;
                    readbackResourceDesc.DepthOrArraySize = 1;
                    readbackResourceDesc.MipLevels = 1;
                    readbackResourceDesc.Format = Format.Unknown;
                    readbackResourceDesc.SampleDescription.Count = 1;
                    readbackResourceDesc.SampleDescription.Quality = 0;
                    readbackResourceDesc.Layout = TextureLayout.RowMajor;
                    readbackResourceDesc.Flags = ResourceFlags.None;
                }
                readbackResource = d3dDevice.CreateCommittedResource<ID3D12Resource>(readbackHeapProperties, HeapFlags.None, readbackResourceDesc, ResourceStates.CopyDestination, null);
            }
        }

        public void SetData<T>(params T[] data) where T : struct
        {
            if ((useFlag & EUsageType.Dynamic) == EUsageType.Dynamic)
            {
                IntPtr uploadResourcePtr = uploadResource.Map(0);
                data.AsSpan().CopyTo(uploadResourcePtr);
                uploadResource.Unmap(0);
            }
        }

        public void SetData<T>(ID3D12GraphicsCommandList5 d3dCmdList, params T[] data) where T : struct
        {
            if ((useFlag & EUsageType.Dynamic) == EUsageType.Dynamic)
            {
                IntPtr uploadResourcePtr = uploadResource.Map(0);
                data.AsSpan().CopyTo(uploadResourcePtr);
                uploadResource.Unmap(0);

                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.Common, ResourceStates.CopyDestination);
                d3dCmdList.CopyBufferRegion(defaultResource, 0, uploadResource, 0, count * (ulong)Unsafe.SizeOf<T>());
                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.CopyDestination, ResourceStates.Common);
            }
        }

        public void RequestUpload<T>(ID3D12GraphicsCommandList5 d3dCmdList) where T : struct
        {
            if ((useFlag & EUsageType.Dynamic) == EUsageType.Dynamic)
            {
                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.Common, ResourceStates.CopyDestination);
                d3dCmdList.CopyBufferRegion(defaultResource, 0, uploadResource, 0, count * (ulong)Unsafe.SizeOf<T>());
                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.CopyDestination, ResourceStates.Common);
            }
        }

        public void GetData<T>(T[] data) where T : struct
        {
            if ((useFlag & EUsageType.Staging) == EUsageType.Staging)
            {
                //Because current frame read-back copy cmd is not execute on GPU, so this will get last frame data
                IntPtr readbackResourcePtr = readbackResource.Map(0);
                readbackResourcePtr.CopyTo(data.AsSpan());
                readbackResource.Unmap(0);
            }
        }

        public void GetData<T>(ID3D12GraphicsCommandList5 d3dCmdList, T[] data) where T : struct
        {
            if ((useFlag & EUsageType.Staging) == EUsageType.Staging)
            {
                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.Common, ResourceStates.CopySource);
                d3dCmdList.CopyBufferRegion(readbackResource, 0, defaultResource, 0, count * (ulong)Unsafe.SizeOf<T>());
                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.CopySource, ResourceStates.Common);

                //Because current frame read-back copy cmd is not execute on GPU, so this will get last frame data
                IntPtr readbackResourcePtr = readbackResource.Map(0);
                readbackResourcePtr.CopyTo(data.AsSpan());
                readbackResource.Unmap(0);
            }
        }

        public void RequestReadback<T>(ID3D12GraphicsCommandList5 d3dCmdList) where T : struct
        {
            if ((useFlag & EUsageType.Staging) == EUsageType.Staging)
            {
                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.Common, ResourceStates.CopySource);
                d3dCmdList.CopyBufferRegion(readbackResource, 0, defaultResource, 0, count * (ulong)Unsafe.SizeOf<T>());
                d3dCmdList.ResourceBarrierTransition(defaultResource, ResourceStates.CopySource, ResourceStates.Common);
            }
        }

        protected override void Release()
        {
            base.Release();
        }
    }

    public struct FRHIBufferRef
    {
        internal int handle;
        public FRHIBuffer buffer;

        internal FRHIBufferRef(int handle, FRHIBuffer buffer) 
        { 
           this.handle = handle; 
           this.buffer = buffer; 
        }
    }

    public struct FRHITextureDescription
    {
        public int width;
        public int height;
        public int slices;
        public string name;
        public ETextureType type;
        public EGraphicsFormat colorFormat;
        public bool useMipMap;
        public int anisoLevel;
        public float mipMapBias;
        public bool enableMSAA;
        public EMSAASample msaaSample;

        public FRHITextureDescription(in int Width, in int Height) : this()
        {
            width = Width;
            height = Height;
            slices = 1;
            msaaSample = EMSAASample.None;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode += width;
            hashCode += height;
            hashCode += slices;
            hashCode += mipMapBias.GetHashCode();
            hashCode += (int)type;
            hashCode += anisoLevel;
            hashCode += (useMipMap ? 1 : 0);
            return hashCode;
        }
    }

    public class FRHITexture : FRHIResource
    {
        internal ETextureType textureType;

        public FRHITexture(ID3D12Device6 d3dDevice, in EUsageType useFlag, in FRHITextureDescription textureDescription) : base(useFlag)
        {
            this.textureType = textureDescription.type;
            this.resourceType = EResourceType.Texture;
        }

        protected override void Release()
        {
            base.Release();
        }
    }

    public struct FRHITextureRef
    {
        internal int handle;
        public FRHITexture texture;

        internal FRHITextureRef(int handle, FRHITexture texture) 
        { 
            this.handle = handle; 
            this.texture = texture; 
        }
    }
}
