using System;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal static class Dx12Utility
    {
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

        internal static EMapMode GetMapModeByUsage(in EBufferUsageFlags bufferUsages)
        {
            Dictionary<EBufferUsageFlags, EMapMode> mapRules = new Dictionary<EBufferUsageFlags, EMapMode>();
            mapRules.Add(EBufferUsageFlags.MapRead, EMapMode.Read);
            mapRules.Add(EBufferUsageFlags.MapWrite, EMapMode.Write);

            foreach (KeyValuePair<EBufferUsageFlags, EMapMode> rule in mapRules)
            {
                if ((bufferUsages & rule.Key) == rule.Key)
                {
                    return rule.Value;
                }
            }

            return EMapMode.Read;
        }

        internal static D3D12_HEAP_TYPE GetDX12HeapTypeByUsage(in EBufferUsageFlags bufferUsages)
        {
            D3D12_HEAP_TYPE fallback = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT;
            Dictionary<EBufferUsageFlags, D3D12_HEAP_TYPE> heapRules = new Dictionary<EBufferUsageFlags, D3D12_HEAP_TYPE>();
            heapRules.Add(EBufferUsageFlags.MapWrite | EBufferUsageFlags.CopySrc, D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_UPLOAD);
            heapRules.Add(EBufferUsageFlags.MapRead | EBufferUsageFlags.CopyDst, D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_READBACK);

            foreach (KeyValuePair<EBufferUsageFlags, D3D12_HEAP_TYPE> rule in heapRules)
            {
                if ((bufferUsages & rule.Key) == rule.Key)
                {
                    return rule.Value;
                }
            }

            return fallback;
        }

        internal static D3D12_RESOURCE_STATES GetDX12ResourceStateByUsage(in EBufferUsageFlags bufferUsages)
        {
            Dictionary<EBufferUsageFlags, D3D12_RESOURCE_STATES> stateRules = new Dictionary<EBufferUsageFlags, D3D12_RESOURCE_STATES>();
            stateRules.Add(EBufferUsageFlags.CopySrc, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_SOURCE);
            stateRules.Add(EBufferUsageFlags.CopyDst, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST);
            stateRules.Add(EBufferUsageFlags.Index, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);
            stateRules.Add(EBufferUsageFlags.Vertex, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);
            stateRules.Add(EBufferUsageFlags.Uniform, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);
            stateRules.Add(EBufferUsageFlags.Storage, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_UNORDERED_ACCESS);
            stateRules.Add(EBufferUsageFlags.Indirect, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ);

            D3D12_RESOURCE_STATES result = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;
            foreach (KeyValuePair<EBufferUsageFlags, D3D12_RESOURCE_STATES> rule in stateRules)
            {
                if ((bufferUsages & rule.Key) == rule.Key)
                {
                    result |= rule.Value;
                }
            }

            return result;
        }

        internal static D3D12_RESOURCE_STATES GetDX12ResourceStateByUsage(in ETextureUsageFlags textureUsages)
        {
            Dictionary<ETextureUsageFlags, D3D12_RESOURCE_STATES> stateRules = new Dictionary<ETextureUsageFlags, D3D12_RESOURCE_STATES>();
            stateRules.Add(ETextureUsageFlags.CopySrc, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_SOURCE);
            stateRules.Add(ETextureUsageFlags.CopyDst, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST);
            stateRules.Add(ETextureUsageFlags.DepthAttachment, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_DEPTH_WRITE);
            stateRules.Add(ETextureUsageFlags.ColorAttachment, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET);
            stateRules.Add(ETextureUsageFlags.TextureBinding, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON);
            stateRules.Add(ETextureUsageFlags.StorageBinding, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_UNORDERED_ACCESS);

            D3D12_RESOURCE_STATES result = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;
            foreach (KeyValuePair<ETextureUsageFlags, D3D12_RESOURCE_STATES> rule in stateRules)
            {
                if ((textureUsages & rule.Key) == rule.Key)
                {
                    result |= rule.Value;
                }
            }

            return result;
        }

        internal static D3D12_RESOURCE_DIMENSION ConvertToDX12ResourceDimension(in ETextureDimension dimension)
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

        internal static D3D12_RESOURCE_STATES ConvertToNativeBufferState(in EBufferState state)
        {
            /*switch (state)
            {
                case ETextureState.Present:
                    return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT;

                case ETextureState.RnederTarget:
                    return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET;

                default:
                    return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;
            }*/
            throw new NotImplementedException();
        }

        internal static D3D12_RESOURCE_STATES ConvertToNativeTextureState(in ETextureState state)
        {
            switch (state)
            {
                case ETextureState.Present:
                    return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT;

                case ETextureState.RenderTarget:
                    return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET;

                default:
                    return D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COMMON;
            }
        }

        internal static D3D_PRIMITIVE_TOPOLOGY ConvertToNativePrimitiveTopology(in EPrimitiveTopology primitiveTopology)
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

        internal static D3D12_DESCRIPTOR_RANGE_TYPE ConvertToDX12BindType(in EBindingType bindType)
        {
            switch (bindType)
            {
                case EBindingType.Buffer:
                case EBindingType.Texture:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV;

                case EBindingType.Sampler:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SAMPLER;

                case EBindingType.UniformBuffer:
                    return D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_CBV;

                case EBindingType.StorageBuffer:
                case EBindingType.StorageTexture:
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

        internal static bool IsIndexBuffer(in EBufferUsageFlags bufferUsages)
        {
            return (bufferUsages & EBufferUsageFlags.Index) == EBufferUsageFlags.Index;
        }

        internal static bool IsVertexBuffer(in EBufferUsageFlags bufferUsages)
        {
            return (bufferUsages & EBufferUsageFlags.Vertex) == EBufferUsageFlags.Vertex;
        }

        internal static bool IsConstantBuffer(in EBufferUsageFlags bufferUsages)
        {
            return (bufferUsages & EBufferUsageFlags.Uniform) == EBufferUsageFlags.Uniform;
        }

        internal static bool IsUnorderedAccessBuffer(in EBufferUsageFlags bufferUsages)
        {
            return (bufferUsages & EBufferUsageFlags.Storage) == EBufferUsageFlags.Storage;
        }

        internal static bool IsDepthStencilTexture(in ETextureUsageFlags textureUsages)
        {
            return (textureUsages & ETextureUsageFlags.DepthAttachment) == ETextureUsageFlags.DepthAttachment;
        }

        internal static bool IsRenderTargetTexture(in ETextureUsageFlags textureUsages)
        {
            return (textureUsages & ETextureUsageFlags.ColorAttachment) == ETextureUsageFlags.ColorAttachment;
        }

        internal static bool IsShaderResourceTexture(in ETextureUsageFlags textureUsages)
        {
            return (textureUsages & ETextureUsageFlags.TextureBinding) == ETextureUsageFlags.TextureBinding;
        }

        internal static bool IsUnorderedAccessTexture(in ETextureUsageFlags textureUsages)
        {
            return (textureUsages & ETextureUsageFlags.StorageBinding) == ETextureUsageFlags.StorageBinding;
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
