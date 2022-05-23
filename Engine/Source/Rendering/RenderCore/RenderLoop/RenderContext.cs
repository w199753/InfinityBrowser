using System;
using InfinityEngine.Graphics;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Rendering
{
    public sealed class RenderContext : Disposal
    {
        public ulong computeFrequency => m_Instance.computeFrequency;
        public ulong graphicsFrequency => m_Instance.graphicsFrequency;
        public RHIContext Instance => m_Instance;
        public RHISwapChain SwapChain => m_SwapChain;

        internal RHITexture backBuffer => m_SwapChain.backBuffer;
        internal RHIRenderTargetView backBufferView => m_SwapChain.backBufferView;

        private RHIContext m_Instance;
        private RHISwapChain m_SwapChain;

        public RenderContext(in uint width, in uint height, in IntPtr window)
        {
            m_Instance = new D3DContext();
            m_SwapChain = m_Instance.CreateSwapChain("SwapChain", width, height, window);
            m_SwapChain.InitResourceView(m_Instance);
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
            return m_Instance.CreateCommandBuffer(contextType, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer GetCommandBuffer(string name = null, in EContextType contextType = EContextType.Graphics)
        {
            return m_Instance.GetCommandBuffer(contextType, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseCommandBuffer(RHICommandBuffer cmdBuffer)
        {
            m_Instance.ReleaseCommandBuffer(cmdBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteToFence(in EContextType contextType, RHIFence fence)
        {
            m_Instance.WriteToFence(contextType, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WaitForFence(in EContextType contextType, RHIFence fence)
        {
            m_Instance.WaitForFence(contextType, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandBuffer(RHICommandBuffer cmdBuffer)
        {
            m_Instance.ExecuteCommandBuffer(cmdBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHISwapChain CreateSwapChain(in uint width, in uint height, in IntPtr windowPtr, string name)
        {
            return m_Instance.CreateSwapChain(name, width, height, windowPtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence CreateFence(string name)
        {
            return m_Instance.CreateFence(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence GetFence(string name)
        {
            return m_Instance.GetFence(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseFence(RHIFence fence)
        {
            m_Instance.ReleaseFence(fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIQuery CreateQuery(in EQueryType queryType, string name)
        {
            return m_Instance.CreateQuery(queryType, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIQuery GetQuery(in EQueryType queryType, string name)
        {
            return m_Instance.GetQuery(queryType, name);;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseQuery(RHIQuery query)
        {
            m_Instance.ReleaseQuery(query);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIComputePipelineState CreateComputePipelineState(in RHIComputePipelineDescriptor descriptor)
        {
            return m_Instance.CreateComputePipelineState(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIRayTracePipelineState CreateRayTracePipelineState(in RHIRayTracePipelineDescriptor descriptor)
        {
            return m_Instance.CreateRayTracePipelineState(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIGraphicsPipelineState CreateGraphicsPipelineState(in RHIGraphicsPipelineDescriptor descriptor)
        {
            return m_Instance.CreateGraphicsPipelineState(descriptor);
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
            return m_Instance.CreateBuffer(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIBufferRef GetBuffer(in BufferDescriptor descriptor)
        {
            return m_Instance.GetBuffer(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseBuffer(in RHIBufferRef bufferRef)
        {
            m_Instance.ReleaseBuffer(bufferRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHITexture CreateTexture(in TextureDescriptor descriptor)
        {
            return m_Instance.CreateTexture(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHITextureRef GetTexture(in TextureDescriptor descriptor)
        {
            return m_Instance.GetTexture(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseTexture(RHITextureRef textureRef)
        {
            m_Instance.ReleaseTexture(textureRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIMemoryReadback CreateMemoryReadback(string requestName, bool bProfiler = false)
        {
            return m_Instance.CreateMemoryReadback(requestName, bProfiler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIIndexBufferView CreateIndexBufferView(RHIBuffer buffer)
        {
            return m_Instance.CreateIndexBufferView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIVertexBufferView CreateVertexBufferView(RHIBuffer buffer)
        {
            return m_Instance.CreateVertexBufferView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIDeptnStencilView CreateDepthStencilView(RHITexture texture)
        {
            return m_Instance.CreateDepthStencilView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIRenderTargetView CreateRenderTargetView(RHITexture texture)
        {
            return m_Instance.CreateRenderTargetView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIConstantBufferView CreateConstantBufferView(RHIBuffer buffer)
        {
            return m_Instance.CreateConstantBufferView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIShaderResourceView CreateShaderResourceView(RHIBuffer buffer)
        {
            return m_Instance.CreateShaderResourceView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIShaderResourceView CreateShaderResourceView(RHITexture texture)
        {
            return m_Instance.CreateShaderResourceView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIUnorderedAccessView CreateUnorderedAccessView(RHIBuffer buffer)
        {
            return m_Instance.CreateUnorderedAccessView(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIUnorderedAccessView CreateUnorderedAccessView(RHITexture texture)
        {
            return m_Instance.CreateUnorderedAccessView(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIResourceSet CreateResourceSet(in uint count)
        {
            return m_Instance.CreateResourceSet(count);
        }

        protected override void Release()
        {
            m_SwapChain.Dispose();
            m_Instance.Dispose();
        }

        public static implicit operator RHIContext(RenderContext renderContext) { return renderContext.m_Instance; }
    }
}
