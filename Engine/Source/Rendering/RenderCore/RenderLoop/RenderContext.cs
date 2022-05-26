using System;
using Infinity.Graphics;
using Infinity.Mathmatics;
using System.Runtime.CompilerServices;

namespace Infinity.Rendering
{
    public sealed class RenderContext : Disposal
    {
        public ulong computeFrequency => 0;
        public ulong graphicsFrequency => 0;
        public RHITexture BackBuffer => m_SwapChain.GetTexture(m_SwapChain.GetBackBufferIndex());
        public RHITextureView BackBufferView => m_SwapChainViews[m_SwapChain.GetBackBufferIndex()];

        private RHIInstance m_Instance;
        private RHIGPU m_Gpu;
        private RHIDevice m_Device;
        private RHIQueue[] m_Queues;
        private RHIFence m_FrameFence;
        private RHISwapChain m_SwapChain;
        private RHITextureView[] m_SwapChainViews;

        public RenderContext(in uint width, in uint height, in IntPtr window)
        {
            // Create Instance And SelectGPU
            m_Instance = RHIInstance.CreateByPlatform();
            m_Gpu = m_Instance?.GetGpu(0);

            // Create Device
            RHIQueueInfo[] queueInfos = new RHIQueueInfo[3];
            {
                queueInfos[0].count = 1;
                queueInfos[0].type = EQueueType.Blit;
                queueInfos[1].count = 1;
                queueInfos[1].type = EQueueType.Compute;
                queueInfos[2].count = 1;
                queueInfos[2].type = EQueueType.Graphics;
            }
            Memory<RHIQueueInfo> queueInfosView = new Memory<RHIQueueInfo>(queueInfos);
            RHIDeviceCreateInfo deviceCreateInfo = new RHIDeviceCreateInfo();
            deviceCreateInfo.queueInfos = queueInfosView;
            m_Device = m_Gpu.CreateDevice(deviceCreateInfo);

            // Get GPUQueue
            m_Queues = new RHIQueue[3];
            m_Queues[0] = m_Device.GetQueue(EQueueType.Blit, 0);
            m_Queues[1] = m_Device.GetQueue(EQueueType.Compute, 0);
            m_Queues[2] = m_Device.GetQueue(EQueueType.Graphics, 0);

            // Create FrameFence
            m_FrameFence = m_Device.CreateFence();

            // Create SwapChain
            RHISwapChainCreateInfo swapChainCreateInfo = new RHISwapChainCreateInfo();
            swapChainCreateInfo.count = 2;
            swapChainCreateInfo.extent = new int2(1280, 720);
            swapChainCreateInfo.format = EPixelFormat.RGBA8_UNORM;
            swapChainCreateInfo.presentMode = EPresentMode.VSync;
            swapChainCreateInfo.window = window;
            swapChainCreateInfo.presentQueue = m_Queues[2];
            m_SwapChain = m_Device?.CreateSwapChain(swapChainCreateInfo);

            RHITextureViewCreateInfo viewCreateInfo = new RHITextureViewCreateInfo();
            viewCreateInfo.mipLevelNum = 1;
            viewCreateInfo.baseMipLevel = 0;
            viewCreateInfo.arrayLayerNum = 1;
            viewCreateInfo.baseArrayLayer = 0;
            viewCreateInfo.aspect = ETextureAspect.Color;
            viewCreateInfo.format = EPixelFormat.RGBA8_UNORM;
            viewCreateInfo.dimension = ETextureViewDimension.Tex2D;
            m_SwapChainViews = new RHITextureView[2];
            m_SwapChainViews[0] = m_SwapChain.GetTexture(0).CreateTextureView(viewCreateInfo);
            m_SwapChainViews[1] = m_SwapChain.GetTexture(1).CreateTextureView(viewCreateInfo);
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
        public void Submit()
        {
            m_Queues[(int)EQueueType.Graphics].Submit(null, m_FrameFence);
            m_SwapChain.Present();
            m_FrameFence.Wait();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandPool CreateCommandPool(in EQueueType queueType)
        {
            return m_Queues[(int)queueType].CreateCommandPool();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer GetCommandPool(in EQueueType queueType)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandBuffer(RHICommandBuffer cmdBuffer, RHIFence fence = null)
        {
            m_Queues[(int)EQueueType.Graphics].Submit(cmdBuffer, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandBufferAsync(RHICommandBuffer cmdBuffer, RHIFence fence = null)
        {
            m_Queues[(int)EQueueType.Compute].Submit(cmdBuffer, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence CreateFence()
        {
            return m_Device?.CreateFence();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence GetFence()
        {
            throw new NotImplementedException();
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        }*/

        protected override void Release()
        {
            m_SwapChainViews[0].Dispose();
            m_SwapChainViews[1].Dispose();
            m_SwapChain.Dispose();
            m_FrameFence.Dispose();
            m_Queues[0].Dispose();
            m_Queues[1].Dispose();
            m_Queues[2].Dispose();
            m_Device.Dispose();
            m_Gpu.Dispose();
            m_Instance.Dispose();
        }

        //public static implicit operator RHIContext(RenderContext renderContext) { return renderContext.m_Instance; }
    }
}
