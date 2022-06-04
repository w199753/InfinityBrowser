using System;
using Infinity.Graphics;
using Infinity.Mathmatics;
using System.Runtime.CompilerServices;

namespace Infinity.Rendering
{
    public enum EContextType
    {
        Compute = 1,
        Graphics = 2,
        MAX
    }

    public sealed class RenderContext : Disposal
    {
        public ulong computeFrequency => 0;
        public ulong graphicsFrequency => 0;
        public RHITexture BackBuffer => m_SwapChain.GetTexture(m_SwapChain.BackBufferIndex);
        public RHITextureView BackBufferView => m_SwapChainViews[m_SwapChain.BackBufferIndex];

        private RHIInstance m_Instance;
        private RHIGPU m_Gpu;
        private RHIDevice m_Device;
        private RHIQueue[] m_Queues;
        private RHIFence m_FrameFence;
        private RHISwapChain m_SwapChain;
        private RHITextureView[] m_SwapChainViews;
        private RHICommandPool[] m_CommandPools;

        public RenderContext(in int width, in int height, in IntPtr window)
        {
            // Create Instance And SelectGPU
            m_Instance = RHIInstance.CreateByPlatform();
            m_Gpu = m_Instance.GetGpu(0);

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
            swapChainCreateInfo.count = 3;
            swapChainCreateInfo.frameBufferOnly = true;
            swapChainCreateInfo.extent = new int2(width, height);
            swapChainCreateInfo.format = EPixelFormat.RGBA8_UNORM;
            swapChainCreateInfo.presentMode = EPresentMode.VSync;
            swapChainCreateInfo.window = window;
            swapChainCreateInfo.presentQueue = m_Queues[(int)EQueueType.Graphics];
            m_SwapChain = m_Device.CreateSwapChain(swapChainCreateInfo);

            RHITextureViewCreateInfo viewCreateInfo = new RHITextureViewCreateInfo();
            viewCreateInfo.mipLevelNum = 1;
            viewCreateInfo.baseMipLevel = 0;
            viewCreateInfo.arrayLayerNum = 1;
            viewCreateInfo.baseArrayLayer = 0;
            viewCreateInfo.aspect = ETextureAspect.Color;
            viewCreateInfo.format = EPixelFormat.RGBA8_UNORM;
            viewCreateInfo.dimension = ETextureViewDimension.Tex2D;
            m_SwapChainViews = new RHITextureView[3];
            m_SwapChainViews[0] = m_SwapChain.GetTexture(0).CreateTextureView(viewCreateInfo);
            m_SwapChainViews[1] = m_SwapChain.GetTexture(1).CreateTextureView(viewCreateInfo);
            m_SwapChainViews[2] = m_SwapChain.GetTexture(2).CreateTextureView(viewCreateInfo);

            //Create CommandPool
            m_CommandPools = new RHICommandPool[3];
            m_CommandPools[0] = m_Queues[(int)EQueueType.Blit].CreateCommandPool();
            m_CommandPools[1] = m_Queues[(int)EQueueType.Compute].CreateCommandPool();
            m_CommandPools[2] = m_Queues[(int)EQueueType.Graphics].CreateCommandPool();
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
        public void BeginInit()
        {
            m_CommandPools[0].Reset();
            m_CommandPools[1].Reset();
            m_CommandPools[2].Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndInit()
        {
            m_Queues[(int)EQueueType.Graphics].Submit(null, m_FrameFence);
            m_FrameFence.Wait();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginFrame()
        {
            m_CommandPools[0].Reset();
            m_CommandPools[1].Reset();
            m_CommandPools[2].Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndFrame()
        {
            m_Queues[(int)EQueueType.Graphics].Submit(null, m_FrameFence);
            m_SwapChain.Present();
            m_FrameFence.Wait();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer CreateCommandBuffer(in EContextType contextType)
        {
            return m_CommandPools[(int)contextType].CreateCommandBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer GetCommandBuffer(in EContextType contextType)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandBuffer(RHICommandBuffer cmdBuffer, RHIFence fence = null)
        {
            m_Queues[(int)cmdBuffer.QueueType].Submit(cmdBuffer, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence CreateFence()
        {
            return m_Device.CreateFence();
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIQuery GetQuery(in EQueryType queryType, string name)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseQuery(RHIQuery query)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIComputePipelineState CreateComputePipelineState(in RHIComputePipelineDescriptor descriptor)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIRayTracePipelineState CreateRayTracePipelineState(in RHIRayTracePipelineDescriptor descriptor)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIGraphicsPipelineState CreateGraphicsPipelineState(in RHIGraphicsPipelineDescriptor descriptor)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CreateSampler()
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
        }*/

        protected override void Release()
        {
            m_SwapChainViews[0].Dispose();
            m_SwapChainViews[1].Dispose();
            m_SwapChain.Dispose();
            m_FrameFence.Dispose();
            m_CommandPools[0].Dispose();
            m_CommandPools[1].Dispose();
            m_CommandPools[2].Dispose();
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
