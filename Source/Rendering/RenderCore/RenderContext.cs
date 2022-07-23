using System;
using Infinity.Graphics;
using Infinity.Container;
using Infinity.Mathmatics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Infinity.Rendering
{
    public enum EContextType
    {
        Compute = 1,
        Graphics = 2,
        MAX
    }

    internal class CommandBufferPool : Disposal
    {
        RHICommandPool m_CommandPool;
        Stack<RHICommandBuffer> m_Pooled;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Pooled.Count; } }

        internal CommandBufferPool(RHICommandPool commandPool)
        {
            m_CommandPool = commandPool;
            m_Pooled = new Stack<RHICommandBuffer>(64);
        }

        public RHICommandBuffer Pull()
        {
            RHICommandBuffer cmdBuffer;
            if (m_Pooled.Count == 0)
            {
                ++countAll;
                cmdBuffer = m_CommandPool.CreateCommandBuffer();
            }
            else
            {
                cmdBuffer = m_Pooled.Pop();
            }

            return cmdBuffer;
        }

        public void Push(RHICommandBuffer cmdBuffer)
        {
            m_Pooled.Push(cmdBuffer);
        }

        protected override void Release()
        {
            foreach (RHICommandBuffer cmdBuffer in m_Pooled)
            {
                cmdBuffer.Dispose();
            }
        }
    }

    public sealed class RenderContext : Disposal
    {
        public int2 ScreenSize => m_ScreenSize;
        public ulong ComputeFrequency => 0;
        public ulong GraphicsFrequency => 0;
        public RHITexture BackBuffer => m_SwapChain.GetTexture(m_SwapChain.BackBufferIndex);
        public RHITextureView BackBufferView => m_SwapChainViews[m_SwapChain.BackBufferIndex];

        private int2 m_ScreenSize;
        private RHIInstance m_Instance;
        private RHIGPU m_Gpu;
        private RHIDevice m_Device;
        private RHIQueue[] m_Queues;
        private RHIFence m_FrameFence;
        private RHISwapChain m_SwapChain;
        private RHITextureView[] m_SwapChainViews;
        private RHICommandPool[] m_CommandPools;
        private CommandBufferPool[] m_CommandBufferPools;
        private TArray<RHICommandBuffer> m_CommandBufferAutoRelease;

        public RenderContext(in int width, in int height, in IntPtr surface)
        {
            m_ScreenSize = new int2(width, height);

            // Create Instance
            RHIInstanceDescriptor descriptor;
            {
                //descriptor.backend = ERHIBackend.DirectX12;
                descriptor.backend = RHIInstance.GetPlatformBackend(false);
                descriptor.enableDebugLayer = true;
                descriptor.enableGpuValidatior = false;
            }
            m_Instance = RHIInstance.Create(descriptor);

            // Select Gpu
            m_Gpu = m_Instance.GetGpu(0);

            // Create Device
            RHIQueueDescriptor[] queueInfos = new RHIQueueDescriptor[3];
            {
                queueInfos[0].count = 1;
                queueInfos[0].type = EQueueType.Blit;
                queueInfos[1].count = 1;
                queueInfos[1].type = EQueueType.Compute;
                queueInfos[2].count = 1;
                queueInfos[2].type = EQueueType.Graphics;
            }
            Memory<RHIQueueDescriptor> queueInfosView = new Memory<RHIQueueDescriptor>(queueInfos);
            RHIDeviceDescriptor deviceDescriptor = new RHIDeviceDescriptor();
            deviceDescriptor.queueInfos = queueInfosView;
            m_Device = m_Gpu.CreateDevice(deviceDescriptor);

            // Get GpuQueue
            m_Queues = new RHIQueue[3];
            m_Queues[0] = m_Device.GetQueue(EQueueType.Blit, 0);
            m_Queues[1] = m_Device.GetQueue(EQueueType.Compute, 0);
            m_Queues[2] = m_Device.GetQueue(EQueueType.Graphics, 0);

            // Create FrameFence
            m_FrameFence = m_Device.CreateFence();

            // Create SwapChain
            RHISwapChainDescriptor swapChainDescriptor = new RHISwapChainDescriptor();
            swapChainDescriptor.count = 3;
            swapChainDescriptor.window = surface;
            swapChainDescriptor.frameBufferOnly = true;
            swapChainDescriptor.extent = m_ScreenSize;
            swapChainDescriptor.format = EPixelFormat.RGBA8_UNorm;
            swapChainDescriptor.presentQueue = m_Queues[(int)EQueueType.Graphics];
            m_SwapChain = m_Device.CreateSwapChain(swapChainDescriptor);

            RHITextureViewDescriptor viewDescriptor = new RHITextureViewDescriptor();
            viewDescriptor.mipCount = 1;
            viewDescriptor.baseMipLevel = 0;
            viewDescriptor.arrayLayerCount = 1;
            viewDescriptor.baseArrayLayer = 0;
            viewDescriptor.format = EPixelFormat.RGBA8_UNorm;
            viewDescriptor.viewType = ETextureViewType.RenderTargetView;
            viewDescriptor.dimension = ETextureViewDimension.Texture2D;
            m_SwapChainViews = new RHITextureView[3];
            m_SwapChainViews[0] = m_SwapChain.GetTexture(0).CreateTextureView(viewDescriptor);
            m_SwapChainViews[1] = m_SwapChain.GetTexture(1).CreateTextureView(viewDescriptor);
            m_SwapChainViews[2] = m_SwapChain.GetTexture(2).CreateTextureView(viewDescriptor);

            //Create CommandPool
            m_CommandPools = new RHICommandPool[3];
            m_CommandPools[0] = m_Queues[(int)EQueueType.Blit].CreateCommandPool();
            m_CommandPools[1] = m_Queues[(int)EQueueType.Compute].CreateCommandPool();
            m_CommandPools[2] = m_Queues[(int)EQueueType.Graphics].CreateCommandPool();

            m_CommandBufferPools = new CommandBufferPool[3];
            m_CommandBufferPools[0] = new CommandBufferPool(m_CommandPools[0]);
            m_CommandBufferPools[1] = new CommandBufferPool(m_CommandPools[1]);
            m_CommandBufferPools[2] = new CommandBufferPool(m_CommandPools[2]);

            m_CommandBufferAutoRelease = new TArray<RHICommandBuffer>(16);
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
            RHICommandBuffer cmdBuffer = GetCommandBuffer(EContextType.Graphics);
            cmdBuffer.Begin();
            cmdBuffer.End();
            ExecuteCommandBuffer(cmdBuffer, m_FrameFence);
            m_FrameFence.Wait();
            GCFrameCommandBuffer();
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
            RHICommandBuffer cmdBuffer = GetCommandBuffer(EContextType.Graphics);
            cmdBuffer.Begin();
            cmdBuffer.End();
            ExecuteCommandBuffer(cmdBuffer, m_FrameFence);
            m_SwapChain.Present(EPresentMode.VSync);
            m_FrameFence.Wait();
            GCFrameCommandBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer GetCommandBuffer(in EContextType contextType)
        {
            RHICommandBuffer cmdBuffer = m_CommandBufferPools[(int)contextType].Pull();
            m_CommandBufferAutoRelease.Add(cmdBuffer);
            return cmdBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GCFrameCommandBuffer()
        {
            for(int i = 0; i < m_CommandBufferAutoRelease.length; ++i)
            {
                RHICommandBuffer cmdBuffer = m_CommandBufferAutoRelease[i];
                m_CommandBufferPools[(int)cmdBuffer.CommandPool.Queue.Type].Push(cmdBuffer);
                m_CommandBufferAutoRelease[i] = null;
            }

            m_CommandBufferAutoRelease.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandBuffer(RHICommandBuffer cmdBuffer, RHIFence fence = null)
        {
            m_Queues[(int)cmdBuffer.CommandPool.Queue.Type].Submit(cmdBuffer, fence);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence CreateFence()
        {
            return m_Device.CreateFence();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIBuffer CreateBuffer(in RHIBufferDescriptor descriptor)
        {
            return m_Device.CreateBuffer(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHITexture CreateTexture(in RHITextureDescriptor descriptor)
        {
            return m_Device.CreateTexture(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHISampler CreateSampler(in RHISamplerDescriptor descriptor)
        {
            return m_Device.CreateSampler(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIShader CreateShader(in RHIShaderDescriptor descriptor)
        {
            return m_Device.CreateShader(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIBindGroupLayout CreateBindGroupLayout(in RHIBindGroupLayoutDescriptor descriptor)
        {
            return m_Device.CreateBindGroupLayout(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIBindGroup CreateBindGroup(in RHIBindGroupDescriptor descriptor)
        {
            return m_Device.CreateBindGroup(descriptor);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIQuery GetQuery(in EQueryType queryType, string name)
        {
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseQuery(RHIQuery query)
        {

        }*/

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIComputePipeline CreateComputePipeline(in RHIComputePipelineDescriptor descriptor)
        {
            return m_Device.CreateComputePipeline(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIRaytracingPipeline CreateRaytracingPipeline(in RHIRaytracingPipelineDescriptor descriptor)
        {
            return m_Device.CreateRaytracingPipeline(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIMeshletPipeline CreateMeshletPipeline(in RHIMeshletPipelineDescriptor descriptor)
        {
            return m_Device.CreateMeshletPipeline(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIGraphicsPipeline CreateGraphicsPipeline(in RHIGraphicsPipelineDescriptor descriptor)
        {
            return m_Device.CreateGraphicsPipeline(descriptor);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIRayTracePipeline CreateRaytracePipeline(in RHIRaytracePipelineDescriptor descriptor)
        {

        }*/

        protected override void Release()
        {
            m_SwapChainViews[0].Dispose();
            m_SwapChainViews[1].Dispose();
            m_SwapChain.Dispose();
            m_FrameFence.Dispose();
            m_CommandBufferPools[0].Dispose();
            m_CommandBufferPools[1].Dispose();
            m_CommandBufferPools[2].Dispose();
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
