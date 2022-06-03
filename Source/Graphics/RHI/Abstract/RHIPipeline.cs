using System;

namespace Infinity.Graphics
{
    public struct RHIVertexAttribute
    {
        public EVertexFormat format;
        public int offset;
        // for Vulkan
        public uint location;
        // for DirectX 12
        public uint semanticIndex;
        public string semanticName;
    }

    public struct RHIVertexBufferLayout
    {
        public int stride;
        public EVertexStepMode stepMode;
        public int attributeCount => attributes.Length;
        public Memory<RHIVertexAttribute> attributes;
    }

    public struct RHIVertexState
    {
        public int bufferLayoutCount => bufferLayouts.Length;
        public Memory<RHIVertexBufferLayout> bufferLayouts;
    }

    public struct RHIPrimitiveState
    {
        public bool depthClip;
        // TODO fill mode ?
        public ECullMode cullMode;
        public EFrontFace frontFace;
        public EIndexFormat indexFormat;
        public EPrimitiveTopologyType topologyType;
    }

    public struct RHIStencilFaceState
    {
        public EStencilOp failOp;
        public EStencilOp passOp;
        public EStencilOp depthFailOp;
        public EComparisonFunc comparisonFunc;
    }

    public struct RHIDepthStencilState
    {
        public bool depthEnable;
        public bool stencilEnable;
        public EPixelFormat format;
        public EComparisonFunc depthComparisonFunc;
        public RHIStencilFaceState stencilBack;
        public RHIStencilFaceState stencilFront;
        public uint stencilReadMask;
        public uint stencilWriteMask;
        public int depthBias;
        public float depthBiasClamp;
        public float depthBiasSlopeScale;
    }

    public struct RHIMultiSampleState
    {
        public uint mask;
        public uint count;
        public bool alphaToCoverage;
    }

    public struct RHIBlendComponent
    {
        public EBlendOp op;
        public EBlendFactor srcFactor;
        public EBlendFactor dstFactor;
    }

    public struct RHIBlendState
    {
        public RHIBlendComponent color;
        public RHIBlendComponent alpha;
    }

    public struct RHIColorTargetState
    {
        public RHIBlendState blend;
        public EPixelFormat format;
        public EColorWriteFlags writeFlag;
    }

    public struct RHIFragmentState
    {
        public int colorTargetCount => colorTargets.Length;
        public Memory<RHIColorTargetState> colorTargets;
    }

    public struct RHIComputePipelineCreateInfo
    {
        public RHIShader shader;
        public RHIPipelineLayout layout;
    }

    public struct RHIGraphicsPipelineCreateInfo
    {
        public RHIShader vertexShader;
        public RHIShader fragmentShader;
        public RHIPipelineLayout layout;
        public RHIVertexState vertex;
        public RHIFragmentState fragment;
        public RHIPrimitiveState primitive;
        public RHIMultiSampleState multiSample;
        public RHIDepthStencilState depthStencil;
    }

    public abstract class RHIComputePipeline : Disposal
    {

    }

    public abstract class RHIGraphicsPipeline : Disposal
    {

    }
}
