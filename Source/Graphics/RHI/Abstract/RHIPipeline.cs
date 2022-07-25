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

    public struct RHIBlendDescriptor
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
        public RHIBlendDescriptor blendDescriptor0;
        public RHIBlendDescriptor blendDescriptor1;
        public RHIBlendDescriptor blendDescriptor2;
        public RHIBlendDescriptor blendDescriptor3;
        public RHIBlendDescriptor blendDescriptor4;
        public RHIBlendDescriptor blendDescriptor5;
        public RHIBlendDescriptor blendDescriptor6;
        public RHIBlendDescriptor blendDescriptor7;
    }

    public struct RHIRasterizerStateDescriptor
    {
        public EFillMode FillMode;
        public ECullMode CullMode;
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

    public struct RHIHitGroupDescription
    {
        public string name;
        public EHitGroupType type;
        public RHIShader anyHitShader;
        public RHIShader closeHitShader;
        public RHIShader intersectShader;
        public RHIBindGroupLayout bindGroupLayout;
    }

    public struct RHIRayGeneralDescription
    {
        public string name;
        public RHIShader generalShader;
        public RHIBindGroupLayout bindGroupLayout;
    }

    public struct RHIComputePipelineDescriptor
    {
        public uint3 threadSize;
        public RHIShader computeShader;
        public RHIBindGroupLayout[] bindGroupLayouts;
    }

    public struct RHIRaytracingPipelineDescriptor
    {
        public uint payloadSize;
        public uint attributeSize;
        public uint recursionDepth;
        public RHIShader generateShader;
        public RHIBindGroupLayout[] bindGroupLayouts;
        public Memory<RHIHitGroupDescription> hitGroupDescriptors;
        public Memory<RHIRayGeneralDescription> rayGeneralDescriptors;
    }

    public struct RHIMeshletPipelineDescriptor
    {
        public RHIShader taskShader;
        public RHIShader meshShader;
        public RHIShader fragmentShader;
        public EPrimitiveTopology primitiveTopology;
        public RHIBindGroupLayout[] bindGroupLayouts;
        public RHIOutputStateDescriptor outputStateDescriptor;
        public RHIRenderStateDescriptor renderStateDescriptor;
    }

    public struct RHIGraphicsPipelineDescriptor
    {
        public RHIShader vertexShader;
        public RHIShader fragmentShader;
        public RHIBindGroupLayout[] bindGroupLayouts;
        public RHIOutputStateDescriptor outputStateDescriptor;
        public RHIRenderStateDescriptor renderStateDescriptor;
        public RHIVertexStateDescriptor vertexStateDescriptor;
    }

    internal struct RHIPipelineLayoutDescriptor
    {
        //public RHIConstantLayout[] constantLayouts;
        public RHIBindGroupLayout[] bindGroupLayouts;
    };

    internal abstract class RHIPipelineLayout : Disposal
    {

    }

    public abstract class RHIComputePipeline : Disposal
    {

    }

    public abstract class RHIRaytracingPipeline : Disposal
    {

    }

    public abstract class RHIMeshletPipeline : Disposal
    {

    }

    public abstract class RHIGraphicsPipeline : Disposal
    {

    }
}
