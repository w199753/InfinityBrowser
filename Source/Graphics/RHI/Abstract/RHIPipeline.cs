using System;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHIOutputAttachmentDescription
    {
        public bool resolveMSAA;
        public EPixelFormat format;
    }

    public struct RHIOutputState
    {
        public uint arraySliceCount;
        public ESampleCount sampleCount;
        public RHIOutputAttachmentDescription? depthAttachment;
        public Memory<RHIOutputAttachmentDescription> colorAttachments;
    }

    public struct RHIVertexAttribute
    {
        public int offset;
        public uint index;
        public ESemanticType type;
        public ESemanticFormat format;
    }

    public struct RHIVertexLayout
    {
        public uint stride;
        public int stepRate;
        public EVertexStepMode stepMode;
        public Memory<RHIVertexAttribute> attributes;
    }

    public struct RHIVertexState
    {
        public EPrimitiveTopology primitiveTopology;
        public Memory<RHIVertexLayout> vertexLayouts;
    }

    public struct RHIAttachmentBlendDescription
    {
        public bool blendEnable;
        public EBlend sourceBlendColor;
        public EBlend destinationBlendColor;
        public EBlendOperation blendOperationColor;
        public EBlend sourceBlendAlpha;
        public EBlend destinationBlendAlpha;
        public EBlendOperation blendOperationAlpha;
        public EColorWriteChannel colorWriteChannel;
    }

    public struct RHIBlendStateDescription
    {
        public bool alphaToCoverage;
        public bool independentBlend;
        public RHIAttachmentBlendDescription attachment0;
        public RHIAttachmentBlendDescription attachment1;
        public RHIAttachmentBlendDescription attachment2;
        public RHIAttachmentBlendDescription attachment3;
        public RHIAttachmentBlendDescription attachment4;
        public RHIAttachmentBlendDescription attachment5;
        public RHIAttachmentBlendDescription attachment6;
        public RHIAttachmentBlendDescription attachment7;
    }

    public struct RHIRasterizerStateDescription
    {
        public EFillMode FillMode;
        public ECullMode CullMode;
        public EConservativeState conservativeState;
        public bool scissorEnable;
        public bool depthClipEnable;
        public bool antialiasedLineEnable;
        public bool frontCounterClockwise;
        public int depthBias;
        public float depthBiasClamp;
        public float slopeScaledDepthBias;
    }

    public struct RHIDepthStencilOperation
    {
        public EComparison stencilComparison;
        public EStencilOperation stencilPassOperation;
        public EStencilOperation stencilFailOperation;
        public EStencilOperation stencilDepthFailOperation;
    }

    public struct RHIDepthStencilStateDescription
    {
        public bool depthEnable;
        public bool depthWriteMask;
        public EComparison depthComparison;
        public bool stencilEnable;
        public byte stencilReadMask;
        public byte stencilWriteMask;
        public RHIDepthStencilOperation backFace;
        public RHIDepthStencilOperation frontFace;
    }

    public struct RHIRenderState
    {
        public int stencilRef;
        public int? sampleMask;
        public float4? blendFactor;
        public RHIBlendStateDescription blendState;
        public RHIRasterizerStateDescription rasterizerState;
        public RHIDepthStencilStateDescription depthStencilState;
    }

    public struct RHIComputePipelineCreateInfo
    {
        public uint3 threadSize;
        public RHIShader computeShader;
        public RHIPipelineLayout pipelineLayout;
    }

    public struct RHIGraphicsPipelineCreateInfo
    {
        public RHIShader vertexShader;
        public RHIShader fragmentShader;
        public RHIOutputState outputState;
        public RHIVertexState vertexState;
        public RHIRenderState renderState;
        public RHIPipelineLayout pipelineLayout;
    }

    public abstract class RHIComputePipeline : Disposal
    {

    }

    public abstract class RHIGraphicsPipeline : Disposal
    {

    }
}
