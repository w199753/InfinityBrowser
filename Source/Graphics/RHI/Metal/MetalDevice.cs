using System;
using Apple.Metal;
using System.Diagnostics;
using System.Collections.Generic;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal unsafe class MtlDevice : RHIDevice
    {
        public MtlGPU MtlGpu
        {
            get
            {
                return m_MtlGpu;
            }
        }
        public MTLDevice NativeDevice
        {
            get
            {
                return m_NativeDevice;
            }
        }

        private MtlGPU m_MtlGpu;
        private MTLDevice m_NativeDevice;
        private Dictionary<EQueueType, List<MtlQueue>> m_Queues;

        public MtlDevice(MtlGPU gpu, in RHIDeviceCreateInfo createInfo)
        {
            m_MtlGpu = gpu;
            CreateDevice();
            CreateQueues(createInfo);
        }

        public override int GetQueueCount(in EQueueType type)
        {
            bool hashValue = m_Queues.TryGetValue(type, out List<MtlQueue> queueArray);
            Debug.Assert(hashValue);
            return queueArray.Count;
        }

        public override RHIQueue GetQueue(in EQueueType type, in int index)
        {
            bool hashValue = m_Queues.TryGetValue(type, out List<MtlQueue> queueArray);
            Debug.Assert(hashValue);
            Debug.Assert(index >= 0 && index < queueArray?.Count);
            return queueArray[index];
        }

        public override RHIFence CreateFence()
        {
            throw new NotImplementedException();
        }

        public override RHISwapChain CreateSwapChain(in RHISwapChainCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHIBuffer CreateBuffer(in RHIBufferCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHITexture CreateTexture(in RHITextureCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHISampler CreateSampler(in RHISamplerCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHIShader CreateShader(in RHIShaderCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHIBindGroupLayout CreateBindGroupLayout(in RHIBindGroupLayoutCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHIBindGroup CreateBindGroup(in RHIBindGroupCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHIPipelineLayout CreatePipelineLayout(in RHIPipelineLayoutCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHIComputePipeline CreateComputePipeline(in RHIComputePipelineCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        public override RHIGraphicsPipeline CreateGraphicsPipeline(in RHIGraphicsPipelineCreateInfo createInfo)
        {
            throw new NotImplementedException();
        }

        private void CreateDevice()
        {
            Debug.Assert(m_MtlGpu.GpuPtr.ToPointer() != null);
            m_NativeDevice = new MTLDevice(m_MtlGpu.GpuPtr);
        }

        private void CreateQueues(in RHIDeviceCreateInfo createInfo)
        {
            Dictionary<EQueueType, int> queueCountMap = new Dictionary<EQueueType, int>(3);
            for (int i = 0; i < createInfo.queueInfoCount; ++i)
            {
                RHIQueueInfo queueInfo = createInfo.queueInfos.Span[i];
                if (queueCountMap.TryGetValue(queueInfo.type, out int value))
                {
                    queueCountMap[queueInfo.type] = 0;
                }

                queueCountMap.TryAdd(queueInfo.type, (int)queueInfo.count);
            }

            m_Queues = new Dictionary<EQueueType, List<MtlQueue>>(3);
            foreach (KeyValuePair<EQueueType, int> iter in queueCountMap)
            {
                List<MtlQueue> tempQueues = new List<MtlQueue>(iter.Value);

                for (int i = 0; i < iter.Value; ++i)
                {
                    MTLCommandQueue commandQueue = m_NativeDevice.newCommandQueue();
                    Debug.Assert(commandQueue.NativePtr.ToPointer() != null);

                    MtlCommandQueueCreateInfo queueCreateInfo;
                    queueCreateInfo.cmdQueue = commandQueue;
                    queueCreateInfo.queueType = iter.Key;
                    tempQueues.Add(new MtlQueue(this, queueCreateInfo));
                }

                m_Queues.TryAdd(iter.Key, tempQueues);
            }
        }

        protected override void Release()
        {
            ObjectiveCRuntime.release(m_NativeDevice.NativePtr);
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
