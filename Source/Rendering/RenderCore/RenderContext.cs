﻿using System;
using Infinity.Core;
using Infinity.Graphics;
using Infinity.Mathmatics;
using Infinity.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Infinity.Rendering
{
    public enum ECommandType
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
        public RHITextureView BackBufferView => m_SwapChain.GetTextureView(m_SwapChain.BackBufferIndex);

        private int2 m_ScreenSize;
        private RHIInstance m_Instance;
        private RHIGPU m_Gpu;
        private RHIDevice m_Device;
        private RHIQueue[] m_Queues;
        private RHIFence m_FrameFence;
        private RHISwapChain m_SwapChain;
        private RHICommandPool[] m_CommandPools;
        private CommandBufferPool[] m_CommandBufferPools;
        private TArray<RHICommandBuffer> m_CommandBufferAutoRelease;

        public RenderContext(in int width, in int height, in IntPtr surface)
        {
            m_ScreenSize = new int2(width, height);

            // Create Instance
            RHIInstanceDescriptor descriptor;
            {
                //descriptor.Backend = ERHIBackend.DirectX12;
                descriptor.Backend = RHIInstance.GetBackendByPlatform(false);
                descriptor.EnableDebugLayer = false;
                descriptor.EnableGpuValidatior = false;
            }
            m_Instance = RHIInstance.Create(descriptor);

            // Select Gpu
            m_Gpu = m_Instance.GetGpu(0);

            // Create Device
            RHIQueueDescriptor[] queueInfos = new RHIQueueDescriptor[3];
            {
                queueInfos[0].Count = 1;
                queueInfos[0].Type = EQueueType.Blit;
                queueInfos[1].Count = 1;
                queueInfos[1].Type = EQueueType.Compute;
                queueInfos[2].Count = 1;
                queueInfos[2].Type = EQueueType.Graphics;
            }
            Memory<RHIQueueDescriptor> queueInfosView = new Memory<RHIQueueDescriptor>(queueInfos);
            RHIDeviceDescriptor deviceDescriptor = new RHIDeviceDescriptor();
            deviceDescriptor.QueueInfos = queueInfosView;
            m_Device = m_Gpu.CreateDevice(deviceDescriptor);

            // Get GpuQueue
            m_Queues = new RHIQueue[3];
            m_Queues[0] = m_Device.GetQueue(EQueueType.Blit, 0);
            m_Queues[1] = m_Device.GetQueue(EQueueType.Compute, 0);
            m_Queues[2] = m_Device.GetQueue(EQueueType.Graphics, 0);

            // Create SwapChain
            RHISwapChainDescriptor swapChainDescriptor = new RHISwapChainDescriptor();
            swapChainDescriptor.Count = 3;
            swapChainDescriptor.Extent = m_ScreenSize;
            swapChainDescriptor.Format = EPixelFormat.RGBA8_UNorm;
            swapChainDescriptor.Surface = surface;
            swapChainDescriptor.PresentQueue = m_Queues[(int)EQueueType.Graphics];
            swapChainDescriptor.FrameBufferOnly = true;
            m_SwapChain = m_Device.CreateSwapChain(swapChainDescriptor);

            // Create FrameFence
            m_FrameFence = m_Device.CreateFence();

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
            m_Queues[(int)EQueueType.Graphics].Submit(null, m_FrameFence);
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
            m_SwapChain.Present(EPresentMode.VSync);
            m_Queues[(int)EQueueType.Graphics].Submit(null, m_FrameFence);
            m_FrameFence.Wait();
            GCFrameCommandBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeWindow(in int2 size)
        {
            m_SwapChain.Resize(size);
            m_ScreenSize = new int2(size.x, size.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHICommandBuffer GetCommandBuffer(in ECommandType commandType)
        {
            RHICommandBuffer cmdBuffer = m_CommandBufferPools[(int)commandType].Pull();
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
        public RHISamplerState CreateSamplerState(in RHISamplerStateDescriptor descriptor)
        {
            return m_Device.CreateSamplerState(descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFunction CreateFunction(in RHIFunctionDescriptor descriptor)
        {
            return m_Device.CreateFunction(descriptor);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIPipelineLayout CreatePipelineLayout(in RHIPipelineLayoutDescriptor descriptor)
        {
            return m_Device.CreatePipelineLayout(descriptor);
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
