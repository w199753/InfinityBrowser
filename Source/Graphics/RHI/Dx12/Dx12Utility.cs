using System;
using NUnit.Framework;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TerraFX.Interop.Windows.Windows;
using System.Text;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal static unsafe class Dx12Utility
    {
        public static void CHECK_BOOL(bool cond, [CallerFilePath] string __FILE__ = "", [CallerLineNumber] int __LINE__ = 0, [CallerArgumentExpression("cond")] string expr = "")
            => Assert.False(!cond, $"{__FILE__}({__LINE__}): !({(string.IsNullOrEmpty(expr) ? cond : expr)})");

        public static void CHECK_HR(int hr, [CallerFilePath] string __FILE__ = "", [CallerLineNumber] int __LINE__ = 0, [CallerArgumentExpression("hr")] string expr = "")
            => Assert.False(FAILED(hr), $"{__FILE__}({__LINE__}): FAILED({(string.IsNullOrEmpty(expr) ? hr.ToString("X8") : expr)})");

        internal static D3D12_COMMAND_LIST_TYPE ConvertToNativeQueueType(in EQueueType queueType)
        {
            switch (queueType)
            {
                case EQueueType.Compute:
                    return D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_COMPUTE;

                case EQueueType.Graphics:
                    return D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT;

                default:
                    return D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_COPY;
            }
        }

        internal static uint ConvertToDx12SyncInterval(in EPresentMode presentMode)
        {
            return (uint)(presentMode == EPresentMode.VSync ? 1 : 0);
        }

        internal static D3D12_FILTER ConvertToDx12Filter(in RHISamplerCreateInfo createInfo)
        {
            EFilterMode minFilter = createInfo.minFilter;
            EFilterMode magFilter = createInfo.magFilter;
            EFilterMode mipFilter = createInfo.mipFilter;

            if (minFilter == EFilterMode.Nearset && magFilter == EFilterMode.Nearset && mipFilter == EFilterMode.Nearset) { return D3D12_FILTER.D3D12_FILTER_MIN_MAG_MIP_POINT; }
            if (minFilter == EFilterMode.Nearset && magFilter == EFilterMode.Nearset && mipFilter == EFilterMode.Linear)  { return D3D12_FILTER.D3D12_FILTER_MIN_MAG_POINT_MIP_LINEAR; }
            if (minFilter == EFilterMode.Nearset && magFilter == EFilterMode.Linear  && mipFilter == EFilterMode.Nearset) { return D3D12_FILTER.D3D12_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT; }
            if (minFilter == EFilterMode.Nearset && magFilter == EFilterMode.Linear  && mipFilter == EFilterMode.Linear)  { return D3D12_FILTER.D3D12_FILTER_MIN_POINT_MAG_MIP_LINEAR; }
            if (minFilter == EFilterMode.Linear  && magFilter == EFilterMode.Nearset && mipFilter == EFilterMode.Nearset) { return D3D12_FILTER.D3D12_FILTER_MIN_LINEAR_MAG_MIP_POINT; }
            if (minFilter == EFilterMode.Linear  && magFilter == EFilterMode.Nearset && mipFilter == EFilterMode.Linear)  { return D3D12_FILTER.D3D12_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR; }
            if (minFilter == EFilterMode.Linear  && magFilter == EFilterMode.Linear  && mipFilter == EFilterMode.Nearset) { return D3D12_FILTER.D3D12_FILTER_MIN_MAG_LINEAR_MIP_POINT; }
            if (minFilter == EFilterMode.Linear  && magFilter == EFilterMode.Linear  && mipFilter == EFilterMode.Linear)  { return D3D12_FILTER.D3D12_FILTER_MIN_MAG_MIP_LINEAR; }
            return D3D12_FILTER.D3D12_FILTER_MIN_MAG_MIP_POINT;
        }

        internal static EMapMode GetMapModeByUsage(in EBufferUsage bufferUsages)
        {
            Dictionary<EBufferUsage, EMapMode> mapRules = new Dictionary<EBufferUsage, EMapMode>();
            mapRules.Add(EBufferUsage.MapRead, EMapMode.Read);
            mapRules.Add(EBufferUsage.MapWrite, EMapMode.Write);

            foreach (KeyValuePair<EBufferUsage, EMapMode> rule in mapRules)
            {
                if ((bufferUsages & rule.Key) == rule.Key)
                {
                    return rule.Value;
                }
            }

            return EMapMode.Read;
        }

        internal static D3D12_HEAP_TYPE GetDx12HeapTypeByUsage(in EBufferUsage bufferUsages)
        {
            D3D12_HEAP_TYPE fallback = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT;
            Dictionary<EBufferUsage, D3D12_HEAP_TYPE> heapRules = new Dictionary<EBufferUsage, D3D12_HEAP_TYPE>();
            heapRules.Add(EBufferUsage.MapWrite | EBufferUsage.CopySrc, D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_UPLOAD);
            heapRules.Add(EBufferUsage.MapRead | EBufferUsage.CopyDst, D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_READBACK);

            foreach (KeyValuePair<EBufferUsage, D3D12_HEAP_TYPE> rule in heapRules)
            {
                if ((bufferUsages & rule.Key) == rule.Key)
                {
                    return rule.Value;
                }
            }

            return fallback;
        }

        internal static D3D12_RESOURCE_STATES ConvertToDx12BufferStateByUsage(in EBufferUsage bufferUsages)
        {
            Dictionary<EBufferUsage, D3D12_RESOURCE_STATES> stateRules = new Dictionary<EBufferUsage, D3D12_RESOURCE_STATES>();
            stateRules.Add(EBufferUsage.CopySrc, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_SOURCE);
            stateRules.Add(EBufferUsage.CopyDst, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST);
            stateRules.Add(EBufferUsage.Index, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);
            stateRules.Add(EBufferUsage.Vertex, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);
            stateRules.Add(EBufferUsage.Uniform, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);
            stateRules.Add(EBufferUsage.Indirect, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);
            stateRules.Add(EBufferUsage.StorageResource, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_UNORDERED_ACCESS);

            D3D12_RESOURCE_STATES result = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;
            foreach (KeyValuePair<EBufferUsage, D3D12_RESOURCE_STATES> rule in stateRules)
            {
                if ((bufferUsages & rule.Key) == rule.Key)
                {
                    result |= rule.Value;
                }
            }

            return result;
        }

        internal static D3D12_RESOURCE_STATES ConvertToDx12TextureStateByUsage(in ETextureUsage textureUsages)
        {
            Dictionary<ETextureUsage, D3D12_RESOURCE_STATES> stateRules = new Dictionary<ETextureUsage, D3D12_RESOURCE_STATES>();
            stateRules.Add(ETextureUsage.CopySrc, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_SOURCE);
            stateRules.Add(ETextureUsage.CopyDst, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST);
            stateRules.Add(ETextureUsage.DepthAttachment, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_DEPTH_WRITE);
            stateRules.Add(ETextureUsage.ColorAttachment, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET);
            stateRules.Add(ETextureUsage.ShaderResource, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON);
            stateRules.Add(ETextureUsage.StorageResource, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_UNORDERED_ACCESS);

            D3D12_RESOURCE_STATES result = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;
            foreach (KeyValuePair<ETextureUsage, D3D12_RESOURCE_STATES> rule in stateRules)
            {
                if ((textureUsages & rule.Key) == rule.Key)
                {
                    result |= rule.Value;
                }
            }

            return result;
        }

        internal static D3D12_RESOURCE_FLAGS ConvertToDx12ResourceFlagByUsage(in EBufferUsage bufferUsages)
        {
            Dictionary<EBufferUsage, D3D12_RESOURCE_FLAGS> stateRules = new Dictionary<EBufferUsage, D3D12_RESOURCE_FLAGS>();
            stateRules.Add(EBufferUsage.CopySrc, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(EBufferUsage.CopyDst, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(EBufferUsage.Index, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(EBufferUsage.Vertex, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(EBufferUsage.Uniform, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(EBufferUsage.Indirect, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(EBufferUsage.StorageResource, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS);

            D3D12_RESOURCE_FLAGS result = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE;
            foreach (KeyValuePair<EBufferUsage, D3D12_RESOURCE_FLAGS> rule in stateRules)
            {
                if ((bufferUsages & rule.Key) == rule.Key)
                {
                    result |= rule.Value;
                }
            }

            return result;
        }

        internal static D3D12_RESOURCE_FLAGS ConvertToDx12ResourceFlagByUsage(in ETextureUsage textureUsages)
        {
            Dictionary<ETextureUsage, D3D12_RESOURCE_FLAGS> stateRules = new Dictionary<ETextureUsage, D3D12_RESOURCE_FLAGS>();
            stateRules.Add(ETextureUsage.CopySrc, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(ETextureUsage.CopyDst, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(ETextureUsage.DepthAttachment, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL);
            stateRules.Add(ETextureUsage.ColorAttachment, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET);
            stateRules.Add(ETextureUsage.ShaderResource, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE);
            stateRules.Add(ETextureUsage.StorageResource, D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS);

            D3D12_RESOURCE_FLAGS result = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE;
            foreach (KeyValuePair<ETextureUsage, D3D12_RESOURCE_FLAGS> rule in stateRules)
            {
                if ((textureUsages & rule.Key) == rule.Key)
                {
                    result |= rule.Value;
                }
            }

            return result;
        }

        internal static D3D12_RESOURCE_DIMENSION ConvertToDx12TextureDimension(in ETextureDimension dimension)
        {
            switch (dimension)
            {
                case ETextureDimension.Tex2D:
                    return D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D;

                case ETextureDimension.Tex3D:
                    return D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE3D;

                default:
                    return D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE1D;
            }
        }

        internal static D3D12_RESOURCE_STATES ConvertToDx12BufferState(in EBufferState state)
        {
            if (state == EBufferState.Common)
                return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;

            D3D12_RESOURCE_STATES result = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON; // also 0

            if ((state & EBufferState.StreamOut) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_STREAM_OUT;
            if ((state & EBufferState.CopyDest) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST;
            if ((state & EBufferState.CopySource) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_SOURCE;
            if ((state & EBufferState.IndexBuffer) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_INDEX_BUFFER;
            if ((state & EBufferState.VertexBuffer) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_VERTEX_AND_CONSTANT_BUFFER;
            if ((state & EBufferState.ConstantBuffer) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_VERTEX_AND_CONSTANT_BUFFER;
            if ((state & EBufferState.IndirectArgument) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_INDIRECT_ARGUMENT;
            if ((state & EBufferState.ShaderResource) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE | D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE;
            if ((state & EBufferState.UnorderedAccess) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_UNORDERED_ACCESS;
            if ((state & EBufferState.AccelStructRead) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE;
            if ((state & EBufferState.AccelStructWrite) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE;
            if ((state & EBufferState.AccelStructBuildInput) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE;
            if ((state & EBufferState.AccelStructBuildBlast) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE;

            return result;
        }

        internal static D3D12_RESOURCE_STATES ConvertToDx12TextureState(in ETextureState state)
        {
            if (state == ETextureState.Common)
                return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;

            D3D12_RESOURCE_STATES result = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON; // also 0

            if ((state & ETextureState.Present) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT;
            if ((state & ETextureState.CopyDest) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST;
            if ((state & ETextureState.CopySource) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_SOURCE;
            if ((state & ETextureState.ResolveDest) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RESOLVE_DEST;
            if ((state & ETextureState.ResolveSource) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RESOLVE_SOURCE;
            if ((state & ETextureState.DepthRead) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_DEPTH_READ;
            if ((state & ETextureState.DepthWrite) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_DEPTH_WRITE;
            if ((state & ETextureState.RenderTarget) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET;
            if ((state & ETextureState.ShaderResource) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE | D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE;
            if ((state & ETextureState.UnorderedAccess) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_UNORDERED_ACCESS;
            if ((state & ETextureState.ShadingRateSurface) != 0) result |= D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_SHADING_RATE_SOURCE;

            return result;
        }

        internal static D3D_PRIMITIVE_TOPOLOGY ConvertToDx12PrimitiveTopology(in EPrimitiveTopology primitiveTopology)
        {
            switch (primitiveTopology)
            {
                case EPrimitiveTopology.PointList:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_POINTLIST;

                case EPrimitiveTopology.LineList:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_LINELIST;

                case EPrimitiveTopology.LineStrip:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_LINESTRIP;

                case EPrimitiveTopology.TriangleList:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST;

                case EPrimitiveTopology.TriangleStrip:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP;

                case EPrimitiveTopology.LineListAdj:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_LINELIST_ADJ;

                case EPrimitiveTopology.LineStripAdj:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ;

                case EPrimitiveTopology.TriangleListAdj:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ;

                case EPrimitiveTopology.TriangleStripAdj:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ;

                default:
                    return D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_UNDEFINED;
            }
        }

        internal static D3D12_PRIMITIVE_TOPOLOGY_TYPE ConvertToDx12PrimitiveTopologyType(in EPrimitiveTopology primitiveTopology)
        {
            switch (primitiveTopology)
            {
                case EPrimitiveTopology.PointList:
                    return D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_POINT;

                case EPrimitiveTopology.LineList:
                case EPrimitiveTopology.LineStrip:
                case EPrimitiveTopology.LineListAdj:
                case EPrimitiveTopology.LineStripAdj:
                    return D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_LINE;

                case EPrimitiveTopology.TriangleList:
                case EPrimitiveTopology.TriangleStrip:
                case EPrimitiveTopology.TriangleListAdj:
                case EPrimitiveTopology.TriangleStripAdj:
                    return D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE;

                default:
                    return D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_UNDEFINED;
            }
        }

        internal static unsafe D3D12_BLEND_DESC CreateDx12BlendState(in RHIBlendStateDescription blendState)
        {
            D3D12_BLEND_DESC blendDescription = new D3D12_BLEND_DESC();
            blendDescription.AlphaToCoverageEnable = blendState.alphaToCoverage;
            blendDescription.IndependentBlendEnable = blendState.independentBlend;
            fixed (RHIAttachmentBlendDescription* attachmentPtr = &blendState.attachment0)
            {
                for (int i = 0; i < 8; i++)
                {
                    blendDescription.RenderTarget[i].BlendEnable = attachmentPtr[i].blendEnable;
                    blendDescription.RenderTarget[i].SrcBlend = (D3D12_BLEND)attachmentPtr[i].sourceBlendColor;
                    blendDescription.RenderTarget[i].DestBlend = (D3D12_BLEND)attachmentPtr[i].destinationBlendColor;
                    blendDescription.RenderTarget[i].BlendOp = (D3D12_BLEND_OP)attachmentPtr[i].blendOperationColor;
                    blendDescription.RenderTarget[i].SrcBlendAlpha = (D3D12_BLEND)attachmentPtr[i].sourceBlendAlpha;
                    blendDescription.RenderTarget[i].DestBlendAlpha = (D3D12_BLEND)attachmentPtr[i].destinationBlendAlpha;
                    blendDescription.RenderTarget[i].BlendOpAlpha = (D3D12_BLEND_OP)attachmentPtr[i].blendOperationAlpha;
                    blendDescription.RenderTarget[i].RenderTargetWriteMask = (byte)attachmentPtr[i].colorWriteChannel;
                }
            }
            return blendDescription;
        }

        internal static D3D12_RASTERIZER_DESC CreateDx12RasterizerState(in RHIRasterizerStateDescription description, bool bMultisample)
        {
            D3D12_RASTERIZER_DESC rasterDescription;
            rasterDescription.FillMode = (D3D12_FILL_MODE)description.FillMode;
            rasterDescription.CullMode = (D3D12_CULL_MODE)description.CullMode;
            rasterDescription.ForcedSampleCount = 0;
            rasterDescription.MultisampleEnable = bMultisample;
            rasterDescription.DepthBias = description.depthBias;
            rasterDescription.DepthBiasClamp = description.depthBiasClamp;
            rasterDescription.DepthClipEnable = description.depthClipEnable;
            rasterDescription.AntialiasedLineEnable = description.antialiasedLineEnable;
            rasterDescription.FrontCounterClockwise = description.frontCounterClockwise;
            rasterDescription.SlopeScaledDepthBias = description.slopeScaledDepthBias;
            rasterDescription.ConservativeRaster = (D3D12_CONSERVATIVE_RASTERIZATION_MODE)description.conservativeState;
            return rasterDescription;
        }

        internal static D3D12_COMPARISON_FUNC ConvertToDx12Comparison(in EComparison comparison)
        {
            switch (comparison)
            {
                case EComparison.Never:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_NEVER;

                case EComparison.Less:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_LESS;

                case EComparison.Equal:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_EQUAL;

                case EComparison.LessEqual:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_LESS_EQUAL;

                case EComparison.Greater:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_GREATER;

                case EComparison.NotEqual:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_NOT_EQUAL;

                case EComparison.GreaterEqual:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_GREATER_EQUAL;

                case EComparison.Always:
                    return D3D12_COMPARISON_FUNC.D3D12_COMPARISON_FUNC_ALWAYS;
            }
            return 0;
        }

        internal static D3D12_DEPTH_STENCIL_DESC CreateDx12DepthStencilState(in RHIDepthStencilStateDescription depthStencilState)
        {
            D3D12_DEPTH_STENCIL_DESC depthStencilDescription = new D3D12_DEPTH_STENCIL_DESC
            {
                DepthEnable = depthStencilState.depthEnable,
                DepthFunc = ConvertToDx12Comparison(depthStencilState.depthComparison),
                DepthWriteMask = depthStencilState.depthWriteMask ? D3D12_DEPTH_WRITE_MASK.D3D12_DEPTH_WRITE_MASK_ALL : D3D12_DEPTH_WRITE_MASK.D3D12_DEPTH_WRITE_MASK_ZERO,
                StencilEnable = depthStencilState.stencilEnable,
                StencilReadMask = depthStencilState.stencilReadMask,
                StencilWriteMask = depthStencilState.stencilWriteMask
            };
            D3D12_DEPTH_STENCILOP_DESC frontFaceDescription = new D3D12_DEPTH_STENCILOP_DESC
            {
                StencilFailOp = (D3D12_STENCIL_OP)depthStencilState.frontFace.stencilFailOperation,
                StencilPassOp = (D3D12_STENCIL_OP)depthStencilState.frontFace.stencilPassOperation,
                StencilDepthFailOp = (D3D12_STENCIL_OP)depthStencilState.frontFace.stencilDepthFailOperation,
                StencilFunc = ConvertToDx12Comparison(depthStencilState.frontFace.stencilComparison)
            };
            depthStencilDescription.FrontFace = frontFaceDescription;

            D3D12_DEPTH_STENCILOP_DESC backFaceDescription = new D3D12_DEPTH_STENCILOP_DESC
            {
                StencilFailOp = (D3D12_STENCIL_OP)depthStencilState.backFace.stencilFailOperation,
                StencilPassOp = (D3D12_STENCIL_OP)depthStencilState.backFace.stencilPassOperation,
                StencilDepthFailOp = (D3D12_STENCIL_OP)depthStencilState.backFace.stencilDepthFailOperation,
                StencilFunc = ConvertToDx12Comparison(depthStencilState.backFace.stencilComparison)
            };
            depthStencilDescription.BackFace = backFaceDescription;
            return depthStencilDescription;
        }

        internal static DXGI_FORMAT ConvertToDx12SemanticFormat(in ESemanticFormat format)
        {
            switch (format)
            {
                case ESemanticFormat.UByte:
                    return DXGI_FORMAT.DXGI_FORMAT_R8_UINT;

                case ESemanticFormat.UByte2:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8_UINT;

                case ESemanticFormat.UByte4:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UINT;

                case ESemanticFormat.Byte:
                    return DXGI_FORMAT.DXGI_FORMAT_R8_SINT;

                case ESemanticFormat.Byte2:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8_SINT;

                case ESemanticFormat.Byte4:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SINT;

                case ESemanticFormat.UByteNormalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R8_UNORM;

                case ESemanticFormat.UByte2Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM;

                case ESemanticFormat.UByte4Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;

                case ESemanticFormat.ByteNormalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R8_SNORM;

                case ESemanticFormat.Byte2Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8_SNORM;

                case ESemanticFormat.Byte4Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SNORM;

                case ESemanticFormat.UShort:
                    return DXGI_FORMAT.DXGI_FORMAT_R16_UINT;

                case ESemanticFormat.UShort2:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16_UINT;

                case ESemanticFormat.UShort4:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UINT;

                case ESemanticFormat.Short:
                    return DXGI_FORMAT.DXGI_FORMAT_R16_SINT;

                case ESemanticFormat.Short2:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16_SINT;

                case ESemanticFormat.Short4:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SINT;

                case ESemanticFormat.UShortNormalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R16_UNORM;

                case ESemanticFormat.UShort2Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16_UNORM;

                case ESemanticFormat.UShort4Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM;

                case ESemanticFormat.ShortNormalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R16_SNORM;

                case ESemanticFormat.Short2Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16_SNORM;

                case ESemanticFormat.Short4Normalized:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SNORM;

                case ESemanticFormat.Half:
                    return DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT;

                case ESemanticFormat.Half2:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16_FLOAT;

                case ESemanticFormat.Half4:
                    return DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT;

                case ESemanticFormat.Float:
                    return DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT;

                case ESemanticFormat.Float2:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT;

                case ESemanticFormat.Float3:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT;

                case ESemanticFormat.Float4:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;

                case ESemanticFormat.UInt:
                    return DXGI_FORMAT.DXGI_FORMAT_R32_UINT;

                case ESemanticFormat.UInt2:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32_UINT;

                case ESemanticFormat.UInt3:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32B32_UINT;

                case ESemanticFormat.UInt4:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_UINT;

                case ESemanticFormat.Int:
                    return DXGI_FORMAT.DXGI_FORMAT_R32_SINT;

                case ESemanticFormat.Int2:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32_SINT;

                case ESemanticFormat.Int3:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32B32_SINT;

                case ESemanticFormat.Int4:
                    return DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_SINT;
            }
            return DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
        }

        internal static DXGI_FORMAT ConvertToDx12Format(in EPixelFormat pixelFormat)
        {
            throw new NotImplementedException();
        }

        internal static DXGI_FORMAT ConvertToDx12ViewFormat(in EPixelFormat pixelFormat)
        {
            throw new NotImplementedException();
        }

        internal static DXGI_FORMAT ConvertToDx12IndexFormat(in EIndexFormat format)
        {
            return (format == EIndexFormat.UInt16) ? DXGI_FORMAT.DXGI_FORMAT_R16_UINT : ((format != EIndexFormat.UInt32) ? DXGI_FORMAT.DXGI_FORMAT_UNKNOWN : DXGI_FORMAT.DXGI_FORMAT_R32_UINT);
        }

        internal static DXGI_SAMPLE_DESC ConvertToDx12SampleCount(in ESampleCount sampleCount)
        {
            switch (sampleCount)
            {
                case ESampleCount.None:
                    return new DXGI_SAMPLE_DESC(1, 0);

                case ESampleCount.Count2:
                    return new DXGI_SAMPLE_DESC(2, 0);

                case ESampleCount.Count4:
                    return new DXGI_SAMPLE_DESC(4, 0);

                case ESampleCount.Count8:
                    return new DXGI_SAMPLE_DESC(8, 0);
            }
            return new DXGI_SAMPLE_DESC(0, 0);
        }

        internal static sbyte* ConvertToDx12SemanticName(this ESemanticType type)
        {
            string semanticName = string.Empty;

            switch (type)
            {
                case ESemanticType.Position:
                    semanticName = "POSITION";
                    break;

                case ESemanticType.TexCoord:
                    semanticName = "TEXCOORD";
                    break;

                case ESemanticType.Normal:
                    semanticName = "NORMAL";
                    break;

                case ESemanticType.Tangent:
                    semanticName = "TANGENT";
                    break;

                case ESemanticType.Binormal:
                    semanticName = "BINORMAL";
                    break;

                case ESemanticType.Color:
                    semanticName = "COLOR";
                    break;

                case ESemanticType.BlendIndices:
                    semanticName = "BLENDINDICES";
                    break;

                case ESemanticType.BlendWeight:
                    semanticName = "BLENDWEIGHT";
                    break;
            }

            byte[] bytes = Encoding.ASCII.GetBytes(semanticName);
            return (sbyte*)Convert.ToSByte(bytes);
        }

        internal static D3D12_INPUT_CLASSIFICATION ConvertToDx12InputSlotClass(this EVertexStepMode stepMode)
        {
            return ((stepMode == EVertexStepMode.PerVertex) || (stepMode != EVertexStepMode.PerInstance)) ? D3D12_INPUT_CLASSIFICATION.D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA : D3D12_INPUT_CLASSIFICATION.D3D12_INPUT_CLASSIFICATION_PER_INSTANCE_DATA;
        }

        internal static D3D12_INPUT_LAYOUT_DESC ConvertToDx12VertexLayout(in Span<RHIVertexLayout> vertexLayouts)
        {
            int num = 0;
            for (int i = 0; i < vertexLayouts.Length; ++i)
            {
                num += vertexLayouts[i].attributes.Length;
            }

            int slot = 0;
            int index = 0;

            D3D12_INPUT_ELEMENT_DESC[] elements = new D3D12_INPUT_ELEMENT_DESC[num];

            while (slot < vertexLayouts.Length)
            {
                ref RHIVertexLayout vertexLayout = ref vertexLayouts[slot];
                Span<RHIVertexAttribute> vertexAttributes = vertexLayout.attributes.Span;

                int num6 = 0;
                int stepRate = vertexLayout.stepRate;
                EVertexStepMode stepMode = vertexLayout.stepMode;

                while (true)
                {
                    if (num6 >= vertexAttributes.Length)
                    {
                        slot++;
                        break;
                    }
                    RHIVertexAttribute attribute = vertexAttributes[num6];

                    ref D3D12_INPUT_ELEMENT_DESC element = ref elements[index];
                    element.Format = ConvertToDx12SemanticFormat(attribute.format);
                    element.InputSlot = (uint)slot;
                    element.SemanticName = ConvertToDx12SemanticName(attribute.type);
                    element.SemanticIndex = attribute.index;
                    element.InputSlotClass = ConvertToDx12InputSlotClass(stepMode);
                    element.AlignedByteOffset = (uint)attribute.offset;
                    element.InstanceDataStepRate = (uint)stepRate;

                    index++;
                    num6++;
                }
            }

            D3D12_INPUT_LAYOUT_DESC outputLayout = new D3D12_INPUT_LAYOUT_DESC();
            fixed (D3D12_INPUT_ELEMENT_DESC* elementPtr = elements)
            {
                outputLayout.NumElements = (uint)num;
                outputLayout.pInputElementDescs = elementPtr;
            }
            return outputLayout;
        }

        internal static D3D12_DESCRIPTOR_RANGE_TYPE ConvertToDx12BindType(in EBindType bindType)
        {
            switch (bindType)
            {
                case EBindType.Buffer:
                case EBindType.Texture:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV;

                case EBindType.Sampler:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SAMPLER;

                case EBindType.Uniform:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_CBV;

                case EBindType.StorageBuffer:
                case EBindType.StorageTexture:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_UAV;

                default:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV;
            }
        }

        internal static D3D12_SHADER_VISIBILITY ConvertToDx12ShaderStage(in EShaderStageFlags shaderStage)
        {
            switch (shaderStage)
            {
                case EShaderStageFlags.Vertex:
                    return D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_VERTEX;

                case EShaderStageFlags.Fragment:
                    return D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_PIXEL;

                default:
                    return D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL;
            }
        }

        internal static D3D12_CLEAR_FLAGS GetDx12ClearFlagByDSA(in RHIGraphicsPassDepthStencilAttachment depthStencilAttachment)
        {
            D3D12_CLEAR_FLAGS result = new D3D12_CLEAR_FLAGS();

            if (depthStencilAttachment.depthLoadOp == ELoadOp.Clear) 
            {
                result |= D3D12_CLEAR_FLAGS.D3D12_CLEAR_FLAG_DEPTH;
            }

            if (depthStencilAttachment.stencilLoadOp == ELoadOp.Clear) 
            {
                result |= D3D12_CLEAR_FLAGS.D3D12_CLEAR_FLAG_STENCIL;
            }
            return result;
        }

        internal static bool IsIndexBuffer(in EBufferUsage bufferUsages)
        {
            return (bufferUsages & EBufferUsage.Index) == EBufferUsage.Index;
        }

        internal static bool IsVertexBuffer(in EBufferUsage bufferUsages)
        {
            return (bufferUsages & EBufferUsage.Vertex) == EBufferUsage.Vertex;
        }

        internal static bool IsConstantBuffer(in EBufferUsage bufferUsages)
        {
            return (bufferUsages & EBufferUsage.Uniform) == EBufferUsage.Uniform;
        }

        internal static bool IsShaderResourceBuffer(in EBufferUsage bufferUsages)
        {
            return (bufferUsages & EBufferUsage.ShaderResource) == EBufferUsage.ShaderResource;
        }

        internal static bool IsUnorderedAccessBuffer(in EBufferUsage bufferUsages)
        {
            return (bufferUsages & EBufferUsage.StorageResource) == EBufferUsage.StorageResource;
        }

        internal static bool IsDepthStencilTexture(in ETextureUsage textureUsages)
        {
            return (textureUsages & ETextureUsage.DepthAttachment) == ETextureUsage.DepthAttachment;
        }

        internal static bool IsRenderTargetTexture(in ETextureUsage textureUsages)
        {
            return (textureUsages & ETextureUsage.ColorAttachment) == ETextureUsage.ColorAttachment;
        }

        internal static bool IsShaderResourceTexture(in ETextureUsage textureUsages)
        {
            return (textureUsages & ETextureUsage.ShaderResource) == ETextureUsage.ShaderResource;
        }

        internal static bool IsUnorderedAccessTexture(in ETextureUsage textureUsages)
        {
            return (textureUsages & ETextureUsage.StorageResource) == ETextureUsage.StorageResource;
        }

        internal static void FillTexture2DSRV(ref D3D12_TEX2D_SRV srv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex2D) == ETextureViewDimension.Tex2D)) {
                return;
            }
            srv.MostDetailedMip = (uint)createInfo.baseMipLevel;
            srv.MipLevels = (uint)createInfo.mipLevelNum;
            srv.PlaneSlice = 0;
            srv.ResourceMinLODClamp = createInfo.baseMipLevel;
        }

        internal static void FillTexture2DArraySRV(ref D3D12_TEX2D_ARRAY_SRV srv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex2DArray) == ETextureViewDimension.Tex2DArray)) {
                return;
            }
            srv.MostDetailedMip = (uint)createInfo.baseMipLevel;
            srv.MipLevels = (uint)createInfo.mipLevelNum;
            srv.FirstArraySlice = (uint)createInfo.baseArrayLayer;
            srv.ArraySize = (uint)createInfo.arrayLayerNum;
            srv.PlaneSlice = 0;
            srv.ResourceMinLODClamp = createInfo.baseMipLevel;
        }

        internal static void FillTextureCubeSRV(ref D3D12_TEXCUBE_SRV srv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.TexCube) == ETextureViewDimension.TexCube)) {
                return;
            }
            srv.MipLevels = (uint)createInfo.mipLevelNum;
            srv.MostDetailedMip = (uint)createInfo.baseMipLevel;
            srv.ResourceMinLODClamp = createInfo.baseMipLevel;
        }

        internal static void FillTextureCubeArraySRV(ref D3D12_TEXCUBE_ARRAY_SRV srv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.TexCubeArray) == ETextureViewDimension.TexCubeArray)) {
                return;
            }
            srv.MostDetailedMip = (uint)createInfo.baseMipLevel;
            srv.MipLevels = (uint)createInfo.mipLevelNum;
            srv.NumCubes = (uint)createInfo.arrayLayerNum;
            srv.First2DArrayFace = (uint)createInfo.baseArrayLayer;
            srv.ResourceMinLODClamp = createInfo.baseMipLevel;
        }

        internal static void FillTexture3DSRV(ref D3D12_TEX3D_SRV srv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex3D) == ETextureViewDimension.Tex3D)) {
                return;
            }
            srv.MipLevels = (uint)createInfo.mipLevelNum;
            srv.MostDetailedMip = (uint)createInfo.baseMipLevel;
            srv.ResourceMinLODClamp = createInfo.baseMipLevel;
        }

        internal static void FillTexture2DUAV(ref D3D12_TEX2D_UAV uav, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex2D) == ETextureViewDimension.Tex2D)) {
                return;
            }
            uav.MipSlice = (uint)createInfo.baseMipLevel;
            uav.PlaneSlice = 0;
        }

        internal static void FillTexture2DArrayUAV(ref D3D12_TEX2D_ARRAY_UAV uav, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex2DArray) == ETextureViewDimension.Tex2DArray)) {
                return;
            }
            uav.MipSlice = (uint)createInfo.baseMipLevel;
            uav.FirstArraySlice = (uint)createInfo.baseArrayLayer;
            uav.ArraySize = (uint)createInfo.arrayLayerNum;
            uav.PlaneSlice = 0;
        }

        internal static void FillTexture3DUAV(ref D3D12_TEX3D_UAV uav, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex3D) == ETextureViewDimension.Tex3D)) {
                return;
            }
            uav.WSize = (uint)createInfo.arrayLayerNum;
            uav.MipSlice = (uint)createInfo.baseMipLevel;
            uav.FirstWSlice = (uint)createInfo.baseArrayLayer;
        }

        internal static void FillTexture2DRTV(ref D3D12_TEX2D_RTV rtv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex2D) == ETextureViewDimension.Tex2D)) {
                return;
            }
            rtv.MipSlice = (uint)createInfo.baseMipLevel;
            rtv.PlaneSlice = 0;
        }

        internal static void FillTexture2DArrayRTV(ref D3D12_TEX2D_ARRAY_RTV rtv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex2DArray) == ETextureViewDimension.Tex2DArray)) {
                return;
            }
            rtv.MipSlice = (uint)createInfo.baseMipLevel;
            rtv.FirstArraySlice = (uint)createInfo.baseArrayLayer;
            rtv.ArraySize = (uint)createInfo.arrayLayerNum;
            rtv.PlaneSlice = 0;
        }

        internal static void FillTexture3DRTV(ref D3D12_TEX3D_RTV rtv, in RHITextureViewCreateInfo createInfo)
        {
            if (!((createInfo.dimension & ETextureViewDimension.Tex3D) == ETextureViewDimension.Tex3D)) {
                return;
            }
            rtv.WSize = (uint)createInfo.arrayLayerNum;
            rtv.MipSlice = (uint)createInfo.baseMipLevel;
            rtv.FirstWSlice = (uint)createInfo.baseArrayLayer;
        }
    }
#pragma warning restore CS8600, CS8602
}
