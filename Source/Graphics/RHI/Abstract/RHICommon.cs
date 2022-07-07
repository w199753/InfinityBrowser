namespace Infinity.Graphics
{
    public enum ERHIBackend : byte
    {
        Metal,
        Vulkan,
        DirectX12,
        MAX
    }

    public enum EGpuType : byte
    {
        Hardware,
        Software,
        MAX
    }

    public enum EQueueType : byte
    {
        Blit,
        Compute,
        Graphics,
        MAX
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
        // TODO features / bc / etc / astc
        MAX
    }

    public enum EShadingRate : byte
    {
        Rate1x1 = 0,
        Rate1x2 = 1,
        Rate2x1 = 4,
        Rate2x2 = 5,
        Rate2x4 = 6,
        Rate4x2 = 9,
        Rate4x4 = 10
    }

    public enum EShadingRateCombiner : byte
    {
        Min = 0,
        Max = 1,
        Sum = 2,
        Override = 3,
        Passthrough = 4
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
        MAX
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
        MAX
    }

    public enum ETextureDimension : byte
    {
        Texture2D,
        Texture3D,
        MAX
    }

    public enum ETextureViewDimension : byte
    {
        Texture2D,
        Texture2DArray,
        TextureCube,
        TextureCubeArray,
        Texture3D,
        MAX
    }

    public enum EAddressMode : byte
    {
        Repeat,
        ClampToEdge,
        Mirror_Repeat,
        MAX
    }

    public enum EFilterMode : byte
    {
        Nearset,
        Linear,
        MAX
    }

    public enum EBindType : byte
    {
        Buffer,
        Texture,
        Sampler,
        UniformBuffer,
        StorageBuffer,
        StorageTexture,
        MAX
    }

    public enum EVertexStepMode : byte
    {
        PerVertex,
        PerInstance,
        MAX
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
        MAX
    }

    public enum EPrimitiveTopologyType : byte
    {
        Point,
        Line,
        Triangle,
        MAX
    }

    public enum EIndexFormat : byte
    {
        UInt16,
        UInt32,
        MAX
    }

    public enum ESampleCount : byte
    {
        None = 1,
        Count2 = 2,
        Count4 = 4,
        Count8 = 8,
        MAX
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
        InverseSecondarySourceAlpha = 0x13
    }

    public enum EBlendOp : byte
    {
        Add = 1,
        Substract = 2,
        ReverseSubstract = 3,
        Min = 4,
        Max = 5
    }

    public enum EColorWriteChannel : byte
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
        Alpha = 8,
        All = 15
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
        Always = 7
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
        Decrement = 8
    }

    public enum EFillMode : byte
    {
        Solid = 3,
        Wireframe = 2
    }

    public enum ECullMode : byte
    {
        None = 1,
        Back = 3,
        Front = 2
    }

    public enum ELoadOp : byte
    {
        Load,
        Clear,
        DontCare,
        MAX
    }

    public enum EStoreOp : byte
    {
        Store,
        Resolve,
        StoreAndResolve,
        DontCare,
        MAX
    }

    public enum EPresentMode : byte
    {
        // TODO check this
        // 1. DirectX SwapEffect #see https://docs.microsoft.com/en-us/windows/win32/api/dxgi/ne-dxgi-dxgi_swap_effect
        // 2. Vulkan VkPresentModeKHR #see https://www.khronos.org/registry/vulkan/specs/1.3-extensions/man/html/VkPresentModeKHR.html
        VSync,
        Immediately,
        MAX
    }

    public enum EResourceType : byte
    {
        Buffer,
        Texture,
        MAX
    }

    public enum EStorageMode : byte
    {
        Default = 0,
        Static = 1,
        Dynamic = 2,
        Staging = 3
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
        MAX
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
        MAX
    }

    public enum EBufferUsage
    {
        IndexBuffer = 0x1,
        VertexBuffer = 0x2,
        UniformBuffer = 0x4,
        IndirectBuffer = 0x8,
        ShaderResource = 0x10,
        UnorderedAccess = 0x20,
        AccelerationStructure = 0x400,
        MAX
    }

    public enum EBufferViewType : byte
    {
        UniformBufferView,
        ShaderResourceView,
        UnorderedAccessView,
        MAX
    }

    public enum ETextureUsage
    {
        DepthStencil = 0x1,
        RenderTarget = 0x2,
        ShaderResource = 0x4,
        UnorderedAccess = 0x8,
        MAX
    }

    public enum ETextureViewType : byte
    {
        DepthStencilView,
        RenderTargetView,
        ShaderResourceView,
        UnorderedAccessView,
        MAX
    }

    public enum EShaderStage
    {
        Vertex = 0x1,
        Fragment = 0x2,
        Compute = 0x4,
        MAX
    }
}
