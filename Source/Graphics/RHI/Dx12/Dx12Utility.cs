using System;
using NUnit.Framework;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal static class Dx12Utility
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

        internal static uint ConvertToNativeSyncInterval(in EPresentMode presentMode)
        {
            return (uint)(presentMode == EPresentMode.VSync ? 1 : 0);
        }

        internal static D3D12_FILTER ConvertToNativeFilter(in RHISamplerCreateInfo createInfo)
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

        internal static D3D12_HEAP_TYPE GetDX12HeapTypeByUsage(in EBufferUsage bufferUsages)
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

        internal static D3D12_RESOURCE_STATES ConvertToDX12BufferStateByUsage(in EBufferUsage bufferUsages)
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

        internal static D3D12_RESOURCE_STATES ConvertToDX12TextureStateByUsage(in ETextureUsage textureUsages)
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

        internal static D3D12_RESOURCE_FLAGS ConvertToDX12ResourceFlagByUsage(in EBufferUsage bufferUsages)
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

        internal static D3D12_RESOURCE_FLAGS ConvertToDX12ResourceFlagByUsage(in ETextureUsage textureUsages)
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

        internal static D3D12_RESOURCE_DIMENSION ConvertToDX12TextureDimension(in ETextureDimension dimension)
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

        internal static D3D12_RESOURCE_STATES ConvertToDX12BufferState(in EBufferState state)
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

        internal static D3D12_RESOURCE_STATES ConvertToDX12TextureState(in ETextureState state)
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

        internal static D3D_PRIMITIVE_TOPOLOGY ConvertToDX12PrimitiveTopology(in EPrimitiveTopology primitiveTopology)
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

        internal static D3D12_DESCRIPTOR_RANGE_TYPE ConvertToDX12BindType(in EBindType bindType)
        {
            switch (bindType)
            {
                case EBindType.Buffer:
                case EBindType.Texture:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV;

                case EBindType.Sampler:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SAMPLER;

                case EBindType.UniformBuffer:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_CBV;

                case EBindType.StorageBuffer:
                case EBindType.StorageTexture:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_UAV;

                default:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV;
            }
        }

        internal static D3D12_SHADER_VISIBILITY ConvertToDX12ShaderStage(in EShaderStageFlags shaderStage)
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

        internal static D3D12_CLEAR_FLAGS GetDX12ClearFlagByDSA(in RHIGraphicsPassDepthStencilAttachment depthStencilAttachment)
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
