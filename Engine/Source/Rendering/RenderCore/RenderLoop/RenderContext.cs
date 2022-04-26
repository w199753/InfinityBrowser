using System;
using InfinityEngine.Graphics.RHI;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Rendering.RenderLoop
{
    public sealed class RenderContext : Disposal
    {
        public ulong computeFrequency => m_Context.computeFrequency;
        public ulong graphicsFrequency => m_Context.graphicsFrequency;
        internal RHITexture backBuffer => m_SwapChain.backBuffer;
        internal RHIRenderTargetView backBufferView => m_SwapChain.backBufferView;

        private RHIContext m_Context;
        private RHISwapChain m_SwapChain;

        public RenderContext(RHIContext context, RHISwapChain swapChain)
        {
            m_Context = context;
            m_SwapChain = swapChain;
        }

        public void Cull()
        {
            CullLight();
            CullTerrain();
            CullFoliage();
            CullPrimitive();
            CullLightProbe();
        }

        private void CullLight()
        {

        }

        private void CullTerrain()
        {

        }

        private void CullFoliage()
        {

        }

        private void CullPrimitive()
        {

        }

        private void CullLightProbe()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer CreateCommandBuffer(string name = null, in EContextType contextType = EContextType.Graphics)
        {
            return m_Context.CreateCommandBuffer(contextType, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer GetCommandBuffer(string name = null, in EContextType contextType = EContextType.Graphics)
        {
            return m_Context.GetCommandBuffer(contextType, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseCommandBuffer(RHICommandBuffer cmdBuffer)
        {
            m_Context.ReleaseCommandBuffer(cmdBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteToFence(in EContextType contextType, RHIFence fence)
        {
            m_Context.WriteToFence(contextType, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitForFence(in EContextType contextType, RHIFence fence)
        {
            m_Context.WaitForFence(contextType, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandBuffer(RHICommandBuffer cmdBuffer)
        {
            m_Context.ExecuteCommandBuffer(cmdBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHISwapChain CreateSwapChain(in uint width, in uint height, in IntPtr windowPtr, string name)
        {
            return m_Context.CreateSwapChain(name, width, height, windowPtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence CreateFence(string name)
        {
            return m_Context.CreateFence(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence GetFence(string name)
        {
            return m_Context.GetFence(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseFence(RHIFence fence)
        {
            m_Context.ReleaseFence(fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIQuery CreateQuery(in EQueryType queryType, string name)
        {
            return m_Context.CreateQuery(queryType, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIQuery GetQuery(in EQueryType queryType, string name)
        {
            return m_Context.GetQuery(queryType, name);;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseQuery(RHIQuery query)
        {
            m_Context.ReleaseQuery(query);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIComputePipelineState CreateComputePipelineState(in RHIComputePipelineDescriptor descriptor)
        {
            return m_Context.CreateComputePipelineState(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIRayTracePipelineState CreateRayTracePipelineState(in RHIRayTracePipelineDescriptor descriptor)
        {
            return m_Context.CreateRayTracePipelineState(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIGraphicsPipelineState CreateGraphicsPipelineState(in RHIGraphicsPipelineDescriptor descriptor)
        {
            return m_Context.CreateGraphicsPipelineState(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateSamplerState()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateVertexInputLayout()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateResourceInputLayout()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIBuffer CreateBuffer(in BufferDescriptor descriptor)
        {
            return m_Context.CreateBuffer(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIBufferRef GetBuffer(in BufferDescriptor descriptor)
        {
            return m_Context.GetBuffer(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseBuffer(in RHIBufferRef bufferRef)
        {
            m_Context.ReleaseBuffer(bufferRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHITexture CreateTexture(in TextureDescriptor descriptor)
        {
            return m_Context.CreateTexture(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHITextureRef GetTexture(in TextureDescriptor descriptor)
        {
            return m_Context.GetTexture(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseTexture(RHITextureRef textureRef)
        {
            m_Context.ReleaseTexture(textureRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIMemoryReadback CreateMemoryReadback(string requestName, bool bProfiler = false)
        {
            return m_Context.CreateMemoryReadback(requestName, bProfiler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIIndexBufferView CreateIndexBufferView(RHIBuffer buffer)
        {
            return m_Context.CreateIndexBufferView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIVertexBufferView CreateVertexBufferView(RHIBuffer buffer)
        {
            return m_Context.CreateVertexBufferView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIDeptnStencilView CreateDepthStencilView(RHITexture texture)
        {
            return m_Context.CreateDepthStencilView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIRenderTargetView CreateRenderTargetView(RHITexture texture)
        {
            return m_Context.CreateRenderTargetView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIConstantBufferView CreateConstantBufferView(RHIBuffer buffer)
        {
            return m_Context.CreateConstantBufferView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIShaderResourceView CreateShaderResourceView(RHIBuffer buffer)
        {
            return m_Context.CreateShaderResourceView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIShaderResourceView CreateShaderResourceView(RHITexture texture)
        {
            return m_Context.CreateShaderResourceView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIUnorderedAccessView CreateUnorderedAccessView(RHIBuffer buffer)
        {
            return m_Context.CreateUnorderedAccessView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIUnorderedAccessView CreateUnorderedAccessView(RHITexture texture)
        {
            return m_Context.CreateUnorderedAccessView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIResourceSet CreateResourceSet(in uint count)
        {
            return m_Context.CreateResourceSet(count);
        }

        protected override void Release()
        {

        }

        public static implicit operator RHIContext(RenderContext renderContext) { return renderContext.m_Context; }
    }
}
