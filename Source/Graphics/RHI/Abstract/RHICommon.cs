﻿namespace Infinity.Graphics
{
    public enum ERHIBackend : byte
    {
        Metal,
        Vulkan,
        DirectX12,
        Undefined
    }

    public enum EGpuType : byte
    {
        Hardware,
        Software,
        Undefined
    }

    public enum EQueueType : byte
    {
        Blit,
        Compute,
        Graphics,
        Undefined
    }

    public enum EQueryType : byte
    {
        Timestamp,
        Occlusion,
        BinaryOcclusion,
        Undefined
    }

    public enum EPixelFormat : byte
    {
        // 8-Bits
        R8_UNorm,
        R8_SNorm,
        R8_UInt,
        R8_SInt,
        // 16-Bits
        R16_UInt,
        R16_SInt,
        R16_Float,
        RG8_UNorm,
        RG8_SNorm,
        RG8_UInt,
        RG8_SInt,
        // 32-Bits
        R32_UInt,
        R32_SInt,
        R32_Float,
        RG16_UInt,
        RG16_SInt,
        RG16_Float,
        RGBA8_UNorm,
        RGBA8_UNorm_Srgb,
        RGBA8_SNorm,
        RGBA8_UInt,
        RGBA8_SInt,
        BGRA8_UNorm,
        BGRA8_UNorm_Srgb,
        RGB9_E5_Float,
        RGB10A2_UNorm,
        RG11B10_Float,
        // 64-Bits
        RG32_UInt,
        RG32_SInt,
        RG32_Float,
        RGBA16_UInt,
        RGBA16_SInt,
        RGBA16_Float,
        // 128-Bits
        RGBA32_UInt,
        RGBA32_SInt,
        RGBA32_Float,
        // Depth-Stencil
        D16_UNorm,
        D24_UNorm_S8_UInt,
        D32_Float,
        // Block-Compressed
        RGBA_DXT1_SRGB,
        RGB_DXT1_UNorm,
        RGBA_DXT1_UNorm,
        RGBA_DXT3_SRGB,
        RGBA_DXT3_UNorm,
        RGBA_DXT5_SRGB,
        RGBA_DXT5_UNorm,
        R_BC4_UNorm,
        R_BC4_SNorm,
        RG_BC5_UNorm,
        RG_BC5_SNorm,
        RGB_BC6H_UFloat,
        RGB_BC6H_SFloat,
        RGBA_BC7_SRGB,
        RGBA_BC7_UNorm,
        // AdaptiveScalable-Compressed
        RGBA_ASTC4X4_SRGB,
        RGBA_ASTC4X4_UNorm,
        RGBA_ASTC4X4_UFloat,
        RGBA_ASTC5X5_SRGB,
        RGBA_ASTC5X5_UNorm,
        RGBA_ASTC5X5_UFloat,
        RGBA_ASTC6X6_SRGB,
        RGBA_ASTC6X6_UNorm,
        RGBA_ASTC6X6_UFloat,
        RGBA_ASTC8X8_SRGB,
        RGBA_ASTC8X8_UNorm,
        RGBA_ASTC8X8_UFloat,
        RGBA_ASTC10X10_SRGB,
        RGBA_ASTC10X10_UNorm,
        RGBA_ASTC10X10_UFloat,
        RGBA_ASTC12X12_SRGB,
        RGBA_ASTC12X12_UNorm,
        RGBA_ASTC12X12_UFloat,
        // YUV 4:2:2 Video resource format.
        YUV2,
        Undefined
    }

    public enum ESemanticFormat : byte
    {
        UByte,
        UByte2,
        UByte3,
        UByte4,
        Byte,
        Byte2,
        Byte3,
        Byte4,
        UByteNormalized,
        UByte2Normalized,
        UByte3Normalized,
        UByte4Normalized,
        ByteNormalized,
        Byte2Normalized,
        Byte3Normalized,
        Byte4Normalized,
        UShort,
        UShort2,
        UShort3,
        UShort4,
        Short,
        Short2,
        Short3,
        Short4,
        UShortNormalized,
        UShort2Normalized,
        UShort3Normalized,
        UShort4Normalized,
        ShortNormalized,
        Short2Normalized,
        Short3Normalized,
        Short4Normalized,
        Half,
        Half2,
        Half3,
        Half4,
        Float,
        Float2,
        Float3,
        Float4,
        UInt,
        UInt2,
        UInt3,
        UInt4,
        Int,
        Int2,
        Int3,
        Int4,
        Undefined
    }

    public enum ESemanticType : byte
    {
        Position = 0,
        TexCoord = 1,
        Normal = 2,
        Tangent = 3,
        Binormal = 4,
        Color = 5,
        BlendIndices = 6,
        BlendWeight = 7,
        ShadingRate = 8,
        Undefined
    }

    public enum EShadingRate : byte
    {
        Rate1x1 = 0,
        Rate1x2 = 1,
        Rate2x1 = 4,
        Rate2x2 = 5,
        Rate2x4 = 6,
        Rate4x2 = 9,
        Rate4x4 = 10,
        Undefined
    }

    public enum EShadingRateCombiner : byte
    {
        Min = 0,
        Max = 1,
        Sum = 2,
        Override = 3,
        Passthrough = 4,
        Undefined
    }

    public enum ETextureDimension : byte
    {
        Texture2D,
        Texture3D,
        Undefined
    }

    public enum ETextureViewDimension : byte
    {
        Texture2D,
        Texture2DArray,
        TextureCube,
        TextureCubeArray,
        Texture3D,
        Undefined
    }

    public enum EAddressMode : byte
    {
        Repeat,
        ClampToEdge,
        MirrorRepeat,
        Undefined
    }

    public enum EFilterMode : byte
    {
        Point,
        Linear,
        Anisotropic,
        Undefined
    }

    public enum EBindType : byte
    {
        Buffer,
        Texture,
        Bindless,
        SamplerState,
        UniformBuffer,
        StorageBuffer,
        StorageTexture,
        AccelerationStructure,
        Undefined
    }

    public enum EVertexStepMode : byte
    {
        PerVertex,
        PerInstance,
        Undefined
    }

    public enum EPrimitiveTopology : byte
    {
        PointList,
        LineList,
        LineStrip,
        TriangleList,
        TriangleStrip,
        LineListAdj,
        LineStripAdj,
        TriangleListAdj,
        TriangleStripAdj,
        Undefined
    }

    public enum EPrimitiveTopologyType : byte
    {
        Point,
        Line,
        Triangle,
        Undefined
    }

    public enum EIndexFormat : byte
    {
        UInt16,
        UInt32,
        Undefined
    }

    public enum ESampleCount : byte
    {
        None = 1,
        Count2 = 2,
        Count4 = 4,
        Count8 = 8,
        Undefined
    }

    public enum EBlendMode : byte
    {
        Zero = 1,
        One = 2,
        SourceColor = 3,
        InverseSourceColor = 4,
        SourceAlpha = 5,
        InverseSourceAlpha = 6,
        DestinationAlpha = 7,
        InverseDestinationAlpha = 8,
        DestinationColor = 9,
        InverseDestinationColor = 10,
        SourceAlphaSaturate = 11,
        BlendFactor = 14,
        InverseBlendFactor = 15,
        SecondarySourceColor = 0x10,
        InverseSecondarySourceColor = 0x11,
        SecondarySourceAlpha = 0x12,
        InverseSecondarySourceAlpha = 0x13,
        Undefined
    }

    public enum EBlendOp : byte
    {
        Add = 1,
        Substract = 2,
        ReverseSubstract = 3,
        Min = 4,
        Max = 5,
        Undefined
    }

    public enum EColorWriteChannel : byte
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
        Alpha = 8,
        All = 15,
        Undefined
    }

    public enum EComparisonMode : byte
    {
        Never = 0,
        Less = 1,
        Equal = 2,
        LessEqual = 3,
        Greater = 4,
        NotEqual = 5,
        GreaterEqual = 6,
        Always = 7,
        Undefined
    }

    public enum EStencilOp : byte
    {
        Keep = 1,
        Zero = 2,
        Replace = 3,
        IncrementSaturation = 4,
        DecrementSaturation = 5,
        Invert = 6,
        Increment = 7,
        Decrement = 8,
        Undefined
    }

    public enum EFillMode : byte
    {
        Solid = 3,
        Wireframe = 2,
        Undefined
    }

    public enum ECullMode : byte
    {
        None = 1,
        Back = 3,
        Front = 2,
        Undefined
    }

    public enum ELoadOp : byte
    {
        Load,
        Clear,
        DontCare,
        Undefined
    }

    public enum EStoreOp : byte
    {
        Store,
        Resolve,
        StoreAndResolve,
        DontCare,
        Undefined
    }

    public enum EPresentMode : byte
    {
        // ToDo
        // 1. DirectX SwapEffect #see https://docs.microsoft.com/en-us/windows/win32/api/dxgi/ne-dxgi-dxgi_swap_effect
        // 2. Vulkan VkPresentModeKHR #see https://www.khronos.org/registry/vulkan/specs/1.3-extensions/man/html/VkPresentModeKHR.html
        VSync,
        Immediately,
        Undefined
    }

    public enum EResourceType : byte
    {
        Buffer,
        Texture,
        Undefined
    }

    public enum EStorageMode : byte
    {
        Default = 0,
        Static = 1,
        Dynamic = 2,
        Staging = 3,
        Undefined
    }

    public enum EBufferState
    {
        Common = 0x00000001,
        StreamOut = 0x00000002,
        CopyDest = 0x00000004,
        CopySource = 0x00000008,
        IndexBuffer = 0x00000010,
        VertexBuffer = 0x00000020,
        ConstantBuffer = 0x00000040,
        IndirectArgument = 0x00000080,
        ShaderResource = 0x00000100,
        UnorderedAccess = 0x00000200,
        AccelStructRead = 0x00000400,
        AccelStructWrite = 0x00000800,
        AccelStructBuildInput = 0x00001000,
        AccelStructBuildBlast = 0x00002000,
        Undefined
    }

    public enum ETextureState
    {
        Unknown = 0,
        Common = 0x00000001,
        Present = 0x00000002,
        CopyDest = 0x00000004,
        CopySource = 0x00000008,
        ResolveDest = 0x00000010,
        ResolveSource = 0x00000020,
        DepthRead = 0x00000040,
        DepthWrite = 0x00000080,
        RenderTarget = 0x00000100,
        ShaderResource = 0x00000200,
        UnorderedAccess = 0x00000400,
        ShadingRateSurface = 0x00000800,
        Undefined
    }

    public enum EBufferUsage
    {
        IndexBuffer = 0x0020,
        VertexBuffer = 0x0001,
        UniformBuffer = 0x0002,
        IndirectBuffer = 0x0004,
        ShaderResource = 0x0008,
        UnorderedAccess = 0x0010,
        AccelerationStructure = 0x0040,
        Undefined
    }

    public enum EBufferViewType : byte
    {
        UniformBuffer,
        ShaderResource,
        UnorderedAccess,
        Undefined
    }

    public enum ETextureUsage
    {
        DepthStencil = 0x0020,
        RenderTarget = 0x0001,
        ShaderResource = 0x0002,
        UnorderedAccess = 0x0004,
        Undefined
    }

    public enum ETextureViewType : byte
    {
        DepthStencil,
        RenderTarget,
        ShaderResource,
        UnorderedAccess,
        Undefined
    }

    public enum EFunctionStage
    {
        Compute = 0x0020,
        Vertex = 0x0001,
        Fragment = 0x0002,
        Task = 0x0004,
        Mesh = 0x0008,
        //AllGraphics = 0x0010,
        Miss = 0x0040,
        AnyHit = 0x0080,
        Callable = 0x00FE,
        ClosestHit = 0x0100,
        Intersection = 0x0200,
        RayGeneration = 0x0400,
        //AllRayTracing = 0x0800,
        All = 0x1000,
        Undefined
    }

    public enum EHitGroupType : byte
    {
        Triangles,
        Procedural,
        Undefined
    }
}
