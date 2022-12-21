﻿using System;
using Infinity.Core;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHIOutputAttachmentDescriptor
    {
        public bool ResolveMSAA;
        public EPixelFormat Format;
    }

    public struct RHIOutputStateDescriptor
    {
        //public uint SliceCount;
        public ESampleCount SampleCount;
        public RHIOutputAttachmentDescriptor? OutputDepthAttachmentDescriptor;
        public Memory<RHIOutputAttachmentDescriptor> OutputColorAttachmentDescriptors;
    }

    public struct RHIVertexElementDescriptor
    {
        public uint Offset;
        public uint Index;
        public ESemanticType Type;
        public ESemanticFormat Format;
    }

    public struct RHIVertexLayoutDescriptor
    {
        public uint Stride;
        public uint StepRate;
        public EVertexStepMode StepMode;
        public Memory<RHIVertexElementDescriptor> VertexElementDescriptors;
    }

    public struct RHIBlendDescriptor
    {
        public bool BlendEnable;
        public EBlendOp BlendOpColor;
        public EBlendMode SrcBlendColor;
        public EBlendMode DstBlendColor;
        public EBlendOp BlendOpAlpha;
        public EBlendMode SrcBlendAlpha;
        public EBlendMode DstBlendAlpha;
        public EColorWriteChannel ColorWriteChannel;
    }

    public struct RHIBlendStateDescriptor
    {
        public bool AlphaToCoverage;
        public bool IndependentBlend;
        public RHIBlendDescriptor BlendDescriptor0;
        public RHIBlendDescriptor BlendDescriptor1;
        public RHIBlendDescriptor BlendDescriptor2;
        public RHIBlendDescriptor BlendDescriptor3;
        public RHIBlendDescriptor BlendDescriptor4;
        public RHIBlendDescriptor BlendDescriptor5;
        public RHIBlendDescriptor BlendDescriptor6;
        public RHIBlendDescriptor BlendDescriptor7;
    }

    public struct RHIRasterizerStateDescriptor
    {
        public EFillMode FillMode;
        public ECullMode CullMode;
        public bool DepthClipEnable;
        public bool ConservativeRaster;
        public bool AntialiasedLineEnable;
        public bool FrontCounterClockwise;
        public uint DepthBias;
        public float DepthBiasClamp;
        public float SlopeScaledDepthBias;
    }

    public struct RHIStencilStateDescriptor
    {
        public EStencilOp StencilPassOp;
        public EStencilOp StencilFailOp;
        public EStencilOp StencilDepthFailOp;
        public EComparisonMode ComparisonMode;
    }

    public struct RHIDepthStencilStateDescriptor
    {
        public bool DepthEnable;
        public bool DepthWriteMask;
        public bool StencilEnable;
        public byte StencilReference;
        public byte StencilReadMask;
        public byte StencilWriteMask;
        public EComparisonMode ComparisonMode;
        public RHIStencilStateDescriptor BackFaceDescriptor;
        public RHIStencilStateDescriptor FrontFaceDescriptor;
    }

    public struct RHIRenderStateDescriptor
    {
        public uint? SampleMask;
        public RHIBlendStateDescriptor BlendStateDescriptor;
        public RHIRasterizerStateDescriptor RasterizerStateDescriptor;
        public RHIDepthStencilStateDescriptor DepthStencilStateDescriptor;
    }

    public struct RHIRayHitGroupDescriptor
    {
        public string Name;
        public EHitGroupType Type;
        public RHIPipelineLayout LocalPipelineLayout;
        public RHIFunctionDescriptor? AnyHitDescriptor;
        public RHIFunctionDescriptor? IntersectDescriptor;
        public RHIFunctionDescriptor? ClosestHitDescriptor;
    }

    public struct RHIRayGeneralGroupDescriptor
    {
        public string Name;
        public RHIPipelineLayout LocalPipelineLayout;
        public RHIFunctionDescriptor GeneralDescriptor;
    }

    public struct RHIComputePipelineDescriptor
    {
        public uint3 ThreadSize;
        public RHIFunction ComputeFunction;
        public RHIPipelineLayout PipelineLayout;
    }

    public struct RHIRaytracingPipelineDescriptor
    {
        public uint MaxPayloadSize;
        public uint MaxAttributeSize;
        public uint MaxRecursionDepth;
        public RHIFunction FunctionLibrary;
        public RHIPipelineLayout GlobalPipelineLayout;
        public RHIRayGeneralGroupDescriptor RayGenerationDescriptor;
        public Memory<RHIRayHitGroupDescriptor> RayHitGroupDescriptors;
        public Memory<RHIRayGeneralGroupDescriptor> RayMissGroupDescriptors;
    }

    public struct RHIMeshletPipelineDescriptor
    {
        public RHIFunction TaskFunction;
        public RHIFunction MeshFunction;
        public RHIFunction FragmentFunction;
        public RHIPipelineLayout PipelineLayout;
        public EPrimitiveTopology PrimitiveTopology;
        public RHIOutputStateDescriptor OutputStateDescriptor;
        public RHIRenderStateDescriptor RenderStateDescriptor;
    }

    public struct RHIGraphicsPipelineDescriptor
    {
        public RHIFunction VertexFunction;
        public RHIFunction FragmentFunction;
        public RHIPipelineLayout PipelineLayout;
        public EPrimitiveTopology PrimitiveTopology;
        public RHIOutputStateDescriptor OutputStateDescriptor;
        public RHIRenderStateDescriptor RenderStateDescriptor;
        public Memory<RHIVertexLayoutDescriptor> VertexLayoutDescriptors;
    }

    public struct RHIPipelineLayoutDescriptor
    {
        public bool IsLocalSignature;
        public bool AllowVertexLayout;
        public RHIBindGroupLayout[] BindGroupLayouts;
        //public RHIPipelineConstantLayout[] PipelineConstantLayouts;
        public Memory<RHIStaticSamplerStateDescriptor>? StaticSamplerStates;
    };

    public abstract class RHIPipelineLayout : Disposal
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
