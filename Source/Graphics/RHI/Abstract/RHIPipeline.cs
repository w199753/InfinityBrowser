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
        public int stride;
        public int stepRate;
        public EVertexStepMode stepMode;
        public Memory<RHIVertexAttribute> attributes;
    }

    public struct RHIVertexState
    {
        public EPrimitiveTopology primitiveTopology;
        public Memory<RHIVertexLayout> vertexLayouts;
    }

    public struct RHIAttachmentBlendDescriptor
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

    public struct RHIBlendStateDescriptor
    {
        public bool alphaToCoverage;
        public bool independentBlend;
        public RHIAttachmentBlendDescriptor attachment0;
        public RHIAttachmentBlendDescriptor attachment1;
        public RHIAttachmentBlendDescriptor attachment2;
        public RHIAttachmentBlendDescriptor attachment3;
        public RHIAttachmentBlendDescriptor attachment4;
        public RHIAttachmentBlendDescriptor attachment5;
        public RHIAttachmentBlendDescriptor attachment6;
        public RHIAttachmentBlendDescriptor attachment7;
    }

    public struct RHIRasterizerStateDescriptor
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
        public RHIBlendStateDescriptor blendState;
        public RHIRasterizerStateDescriptor rasterizerState;
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
        public RHIRenderState renderState;
        public RHIVertexState vertexState;
        public RHIPipelineLayout pipelineLayout;
    }

    public abstract class RHIComputePipeline : Disposal
    {

    }

    public abstract class RHIGraphicsPipeline : Disposal
    {

    }
}
