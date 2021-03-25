﻿using Vortice.Direct3D;
using Vortice.Direct3D12;
using InfinityEngine.Core.Object;

namespace InfinityEngine.Graphics.RHI
{
    public enum EExecuteType
    {
        Signal = 0,
        Wait = 1,
        Execute = 2
    }

    internal struct FExecuteInfo
    {
        internal FRHIFence rhiFence;
        internal EExecuteType executeType;
        internal FRHICommandList rhiCmdList;
        internal FRHICommandContext rhiCmdContext;
    }

    public class FRHICommandList : UObject
    {
        public string name;
        internal ID3D12GraphicsCommandList5 d3D12CmdList;
        internal ID3D12CommandAllocator d3D12CmdAllocator;

        public FRHICommandList(string Name, ID3D12Device6 NativeDevice, CommandListType CommandBufferType)
        {
            name = Name;
            d3D12CmdAllocator = NativeDevice.CreateCommandAllocator<ID3D12CommandAllocator>(CommandBufferType);
            d3D12CmdList = NativeDevice.CreateCommandList<ID3D12GraphicsCommandList5>(0, CommandBufferType, d3D12CmdAllocator, null);
            d3D12CmdList.QueryInterface<ID3D12GraphicsCommandList5>();
        }

        public static implicit operator ID3D12GraphicsCommandList5(FRHICommandList rhiCmdBuffer) { return rhiCmdBuffer.d3D12CmdList; }

        public void Clear()
        {
            d3D12CmdAllocator.Reset();
            d3D12CmdList.Reset(d3D12CmdAllocator, null);
        }

        internal void Close()
        {
            d3D12CmdList.Close();
        }

        public void ClearBuffer(FRHIBuffer buffer)
        {

        }

        public void ClearTexture(FRHITexture texture)
        {

        }

        public void CopyBufferToBuffer(FRHIBuffer srcBuffer, FRHIBuffer dscBuffer)
        {

        }

        public void CopyBufferToTexture(FRHIBuffer srcBuffer, FRHITexture dscTexture)
        {

        }

        public void CopyTextureToBuffer(FRHITexture srcTexture, FRHIBuffer dscBuffer)
        {

        }

        public void CopyTextureToTexture(FRHITexture srcTexture, FRHITexture dscTexture)
        {

        }

        public void GenerateMipmaps(FRHITexture texture)
        {

        }

        public void TransitionResource()
        {

        }

        public void BeginTimeQuery(FRHITimeQuery timeQuery)
        {
            timeQuery.Begin(d3D12CmdList);
        }

        public void EndTimeQuery(FRHITimeQuery timeQuery)
        {
            timeQuery.End(d3D12CmdList);
        }

        public void SetComputePipelineState(FRHIComputeShader computeShader, FRHIComputePipelineState computeState)
        {

        }

        public void SetComputeSamplerState(FRHIComputeShader computeShader)
        {

        }

        public void SetComputeBuffer(FRHIComputeShader computeShader, FRHIBuffer buffer)
        {

        }

        public void SetComputeTexture(FRHIComputeShader computeShader, FRHITexture texture)
        {

        }

        public void DispatchCompute(FRHIComputeShader computeShader, uint sizeX, uint sizeY, uint sizeZ)
        {

        }

        public void DispatchComputeIndirect(FRHIComputeShader computeShader, FRHIBuffer argsBuffer, uint argsOffset)
        {

        }

        public void BuildAccelerationStructure()
        {

        }

        public void CopyAccelerationStructure()
        {

        }

        public void SetAccelerationStructure(FRHIRayGenShader rayGenShader)
        {

        }

        public void SetRayTracePipelineState(FRHIRayGenShader rayGenShader, FRHIRayTracePipelineState rayTraceState)
        {

        }

        public void SetRayTraceSamplerState(FRHIRayGenShader rayGenShader)
        {

        }

        public void SetRayTraceBuffer(FRHIRayGenShader rayGenShader, FRHIBuffer buffer)
        {

        }

        public void SetRayTraceTexture(FRHIRayGenShader rayGenShader, FRHITexture texture)
        {

        }

        public void DispatchRay(FRHIRayGenShader rayGenShader, uint sizeX, uint sizeY, uint sizeZ)
        {

        }

        public void DispatchRayIndirect(FRHIRayGenShader rayGenShader, FRHIBuffer argsBuffer, uint argsOffset)
        {

        }

        public void BeginOcclusionQuery(FRHIOcclusionQuery occlusionQuery)
        {
            occlusionQuery.Begin(d3D12CmdList);
        }

        public void EndOcclusionQuery(FRHIOcclusionQuery occlusionQuery)
        {
            occlusionQuery.End(d3D12CmdList);
        }

        public void BeginStatisticsQuery(FRHIStatisticsQuery statisticsQuery)
        {

        }

        public void EndStatisticsQuery(FRHIStatisticsQuery statisticsQuery)
        {

        }

        public void SetViewport()
        {

        }

        public void SetScissorRect()
        {

        }

        public void BeginFrame()
        {

        }

        public void EndFrame()
        {

        }

        public void BeginEvent()
        {

        }

        public void EndEvent()
        {

        }

        public void BeginRenderPass(FRHITexture depthBuffer, params FRHITexture[] colorBuffer)
        {

        }

        public void EndRenderPass()
        {
            d3D12CmdList.EndRenderPass();
        }

        public void SetStencilRef()
        {

        }

        public void SetBlendFactor()
        {

        }

        public void SetDepthBounds(float min, float max)
        {

        }

        public void SetShadingRate(ShadingRate shadingRate, ShadingRateCombiner[] combineMathdo)
        {
            d3D12CmdList.RSSetShadingRate(shadingRate, combineMathdo);
        }

        public void SetShadingRateIndirect(FRHITexture indirectTexture)
        {
            d3D12CmdList.RSSetShadingRateImage(indirectTexture.defaultResource);
        }

        public void SetGraphicsPipelineState(FRHIGraphicsShader graphicsShader, FRHIGraphicsPipelineState graphcisState)
        {

        }

        public void SetGraphicsSamplerState(FRHIGraphicsShader graphicsShader, string propertyName)
        {

        }

        public void SetGraphicsBuffer(FRHIGraphicsShader graphicsShader, string propertyName, FRHIBuffer buffer)
        {

        }

        public void SetGraphicsTexture(FRHIGraphicsShader graphicsShader, string propertyName, FRHITexture texture)
        {

        }

        public void DrawPrimitiveInstance(FRHIBuffer indexBuffer, FRHIBuffer vertexBuffer, PrimitiveTopology topologyType, uint indexCount, uint instanceCount)
        {

        }

        public void DrawPrimitiveInstanceIndirect(FRHIBuffer indexBuffer, FRHIBuffer vertexBuffer, PrimitiveTopology topologyType, FRHIBuffer argsBuffer, uint argsOffset)
        {

        }

        public void DrawMultiPrimitiveInstance()
        {

        }

        public void DrawMultiPrimitiveInstanceIndirect()
        {

        }

        protected override void Disposed()
        {
            d3D12CmdList?.Dispose();
            d3D12CmdAllocator?.Dispose();
        }
    }
}