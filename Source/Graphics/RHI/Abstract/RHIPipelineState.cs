﻿using System;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHIOutputAttachmentDescriptor
    {
        public bool resolveMSAA;
        public EPixelFormat format;
    }

    public struct RHIOutputStateDescriptor
    {
        //public uint sliceCount;
        public ESampleCount sampleCount;
        public RHIOutputAttachmentDescriptor? depthAttachmentDescriptor;
        public Memory<RHIOutputAttachmentDescriptor> colorAttachmentDescriptors;
    }

    public struct RHIVertexAttributeDescriptor
    {
        public int offset;
        public uint index;
        public ESemanticType type;
        public ESemanticFormat format;
    }

    public struct RHIVertexLayoutDescriptor
    {
        public int stride;
        public int stepRate;
        public EVertexStepMode stepMode;
        public Memory<RHIVertexAttributeDescriptor> attributeDescriptors;
    }

    public struct RHIVertexStateDescriptor
    {
        public EPrimitiveTopology primitiveTopology;
        public Memory<RHIVertexLayoutDescriptor> vertexLayoutDescriptors;
    }

    public struct RHIAttachmentBlendDescriptor
    {
        public bool blendEnable;
        public EBlendOp blendOpColor;
        public EBlendMode srcBlendColor;
        public EBlendMode dstBlendColor;
        public EBlendOp blendOpAlpha;
        public EBlendMode srcBlendAlpha;
        public EBlendMode dstBlendAlpha;
        public EColorWriteChannel colorWriteChannel;
    }

    public struct RHIBlendStateDescriptor
    {
        public bool alphaToCoverage;
        public bool independentBlend;
        public RHIAttachmentBlendDescriptor attachmentDescriptor0;
        public RHIAttachmentBlendDescriptor attachmentDescriptor1;
        public RHIAttachmentBlendDescriptor attachmentDescriptor2;
        public RHIAttachmentBlendDescriptor attachmentDescriptor3;
        public RHIAttachmentBlendDescriptor attachmentDescriptor4;
        public RHIAttachmentBlendDescriptor attachmentDescriptor5;
        public RHIAttachmentBlendDescriptor attachmentDescriptor6;
        public RHIAttachmentBlendDescriptor attachmentDescriptor7;
    }

    public struct RHIRasterizerStateDescriptor
    {
        public EFillMode FillMode;
        public ECullMode CullMode;
        public bool scissorEnable;
        public bool depthClipEnable;
        public bool conservativeRaster;
        public bool antialiasedLineEnable;
        public bool frontCounterClockwise;
        public int depthBias;
        public float depthBiasClamp;
        public float slopeScaledDepthBias;
    }

    public struct RHIStencilStateDescriptor
    {
        public EStencilOp stencilPassOp;
        public EStencilOp stencilFailOp;
        public EStencilOp stencilDepthFailOp;
        public EComparisonMode comparisonMode;
    }

    public struct RHIDepthStencilStateDescriptor
    {
        public bool depthEnable;
        public bool depthWriteMask;
        public bool stencilEnable;
        public byte stencilReference;
        public byte stencilReadMask;
        public byte stencilWriteMask;
        public EComparisonMode comparisonMode;
        public RHIStencilStateDescriptor backFaceDescriptor;
        public RHIStencilStateDescriptor frontFaceDescriptor;
    }

    public struct RHIRenderStateDescriptor
    {
        public int? sampleMask;
        public RHIBlendStateDescriptor blendStateDescriptor;
        public RHIRasterizerStateDescriptor rasterizerStateDescriptor;
        public RHIDepthStencilStateDescriptor depthStencilStateDescriptor;
    }

    public struct RHIComputePipelineDescriptor
    {
        public uint3 threadSize;
        public RHIShader computeShader;
        public RHIPipelineLayout pipelineLayout;
    }

    public struct RHIGraphicsPipelineDescriptor
    {
        public RHIShader vertexShader;
        public RHIShader fragmentShader;
        public RHIPipelineLayout pipelineLayout;
        public RHIOutputStateDescriptor outputStateDescriptor;
        public RHIRenderStateDescriptor renderStateDescriptor;
        public RHIVertexStateDescriptor vertexStateDescriptor;
    }

    public abstract class RHIComputePipelineState : Disposal
    {

    }

    public abstract class RHIGraphicsPipelineState : Disposal
    {

    }
}