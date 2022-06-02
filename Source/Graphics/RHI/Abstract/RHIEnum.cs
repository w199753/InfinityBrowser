namespace Infinity.Graphics
{
    public enum ERHIBackend
    {
        Metal,
        Vulkan,
        DirectX12,
        MAX
    }

    public enum EGpuType
    {
        Hardware,
        Software,
        MAX
    }

    public enum EQueueType
    {
        Blit,
        Compute,
        Graphics,
        MAX
    }

    public enum EMapMode
    {
        Read,
        Write,
        MAX
    }

    public enum EPixelFormat
    {
        // 8-Bits
        R8_UNORM,
        R8_SNORM,
        R8_UINT,
        R8_SINT,
        // 16-Bits
        R16_UINT,
        R16_SINT,
        R16_FLOAT,
        RG8_UNORM,
        RG8_SNORM,
        RG8_UINT,
        RG8_SINT,
        // 32-Bits
        R32_UINT,
        R32_SINT,
        R32_FLOAT,
        RG16_UINT,
        RG16_SINT,
        RG16_FLOAT,
        RGBA8_UNORM,
        RGBA8_UNORM_SRGB,
        RGBA8_SNORM,
        RGBA8_UINT,
        RGBA8_SINT,
        BGRA8_UNORM,
        BGRA8_UNORM_SRGB,
        RGB9_E5_FLOAT,
        RGB10A2_UNORM,
        RG11B10_FLOAT,
        // 64-Bits
        RG32_UINT,
        RG32_SINT,
        RG32_FLOAT,
        RGBA16_UINT,
        RGBA16_SINT,
        RGBA16_FLOAT,
        // 128-Bits
        RGBA32_UINT,
        RGBA32_SINT,
        RGBA32_FLOAT,
        // Depth-Stencil
        D16_UNORM,
        D24_UNORM_S8_UINT,
        D32_FLOAT,
        // TODO features / bc / etc / astc
        MAX
    }

    public enum EVertexFormat
    {
        // 8-Bits Channel
        UINT8_X2,
        UINT8_X4,
        SINT8_X2,
        SINT8_X4,
        UNORM8_X2,
        UNORM8_X4,
        SNORM8_X2,
        SNORM8_X4,
        // 16-Bits Channel
        UINT16_X2,
        UINT16_X4,
        SINT16_X2,
        SINT16_X4,
        UNORM16_X2,
        UNORM16_X4,
        SNORM16_X2,
        SNORM16_X4,
        FLOAT16_X2,
        FLOAT16_X4,
        // 32-Bits Channel
        FLOAT32_X1,
        FLOAT32_X2,
        FLOAT32_X3,
        FLOAT32_X4,
        UINT32_X1,
        UINT32_X2,
        UINT32_X3,
        UINT32_X4,
        SINT32_X1,
        SINT32_X2,
        SINT32_X3,
        SINT32_X4,
        MAX
    }

    public enum ETextureDimension
    {
        Tex2D,
        Tex3D,
        MAX
    }

    public enum ETextureViewDimension
    {
        Tex2D,
        Tex2DArray,
        TexCube,
        TexCubeArray,
        Tex3D,
        MAX
    }

    public enum ETextureAspect
    {
        Color,
        Depth,
        Stencil,
        DepthStencil,
        MAX
    }

    public enum EAddressMode
    {
        ClampToEdge,
        Repeat,
        Mirror_Repeat,
        MAX
    }

    public enum EFilterMode
    {
        Nearset,
        Linear,
        MAX
    }

    public enum EComparisonFunc
    {
        Never,
        Less,
        Equal,
        LessEqual,
        Greater,
        NotEqual,
        GreaterEqual,
        Always,
        MAX
    }

    public enum EBindingType
    {
        Buffer,
        Texture,
        Sampler,
        UniformBuffer,
        StorageBuffer,
        StorageTexture,
        MAX
    }

    public enum ESamplerBindingType
    {
        Filtering,
        NonFiltering,
        Comparison,
        MAX
    }

    public enum ETextureSampleType
    {
        Float,
        NonFilterableFloat,
        Depth,
        SInt,
        UInt,
        MAX
    }

    public enum EStorageTextureAccess
    {
        WriteOnly,
        MAX
    }

    public enum EVertexStepMode
    {
        PerVertex,
        PerInstance,
        MAX
    }

    public enum EPrimitiveTopologyType
    {
        Point,
        Line,
        Triangle,
        MAX
    }

    public enum EPrimitiveTopology
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

    public enum EIndexFormat
    {
        UInt16,
        UInt32,
        MAX
    }

    public enum EFrontFace
    {
        CW,
        CCW,
        MAX
    }

    public enum ECullMode
    {
        None,
        Back,
        Front,
        MAX
    }

    public enum EStencilOp
    {
        Keep,
        Zero,
        Invert,
        Replace,
        IncrementClamp,
        DecrementClamp,
        IncrementWarp,
        DecrementWarp,
        MAX
    }

    public enum EBlendFactor
    {
        Zero,
        One,
        Src,
        OneMinusSrc,
        SrcAlpha,
        OneMinusSrcAlpha,
        Dst,
        OneMinusDst,
        DstAlpha,
        OneMinusDstAlpha,
        // TODO check spec
        // SrcAlphaSaturated,
        // CONSTANT,
        // OneMinusConstan,
        MAX
    }

    public enum EBlendOp
    {
        Min,
        Max,
        Add,
        Subtract,
        ReverseSubtract,
        MAX
    }

    public enum ELoadOp
    {
        Load,
        Clear,
        DontCare,
        MAX
    }

    public enum EStoreOp
    {
        Store,
        Resolve,
        StoreAndResolve,
        DontCare,
        MAX
    }

    public enum EPresentMode
    {
        // TODO check this
        // 1. DirectX SwapEffect #see https://docs.microsoft.com/en-us/windows/win32/api/dxgi/ne-dxgi-dxgi_swap_effect
        // 2. Vulkan VkPresentModeKHR #see https://www.khronos.org/registry/vulkan/specs/1.3-extensions/man/html/VkPresentModeKHR.html
        VSync,
        Immediately,
        MAX
    }

    public enum EResourceType
    {
        Buffer,
        Texture,
        MAX
    }

    public enum EBufferState
    {
        // TODO check this ?
    }

    public enum ETextureState
    {
        Undefined,
        Present,
        RnederTarget,
        MAX
    }

    public enum EBufferUsageFlags
    {
        MapRead       = 0x1,
        MapWrite      = 0x2,
        CopySrc       = 0x4,
        CopyDst       = 0x8,
        Index         = 0x10,
        Vertex        = 0x20,
        Uniform       = 0x40,
        Storage       = 0x80,
        Indirect      = 0x100,
        QueryResolve  = 0x200,
        MAX
    }

    public enum ETextureUsageFlags
    {
        CopySrc          = 0x1,
        CopyDst          = 0x2,
        DepthAttachment  = 0x4,
        ColorAttachment  = 0x8,
        TextureBinding   = 0x10,
        StorageBinding   = 0x20,
        MAX
    }

    public enum EShaderStageFlags
    {
        Vertex   = 0x1,
        Fragment = 0x2,
        Compute  = 0x4,
        MAX
    }

    public enum EColorWriteFlags
    {
        Red   = 0x1,
        Green = 0x2,
        Blur  = 0x4,
        Alpha = 0x8,
        MAX
    }
}
