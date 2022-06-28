﻿using System;
using Apple.Metal;
using System.Diagnostics;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal unsafe struct MtlCommandQueueDescriptor
    {
        public EQueueType queueType;
        public MTLCommandQueue cmdQueue;
    }

    internal unsafe class MtlQueue : RHIQueue
    {
        public MTLCommandQueue NativeQueue
        {
            get
            {
                return m_NativeQueue;
            }
        }

        private MtlDevice m_MtlDevice;
        private EQueueType m_QueueType;
        private MTLCommandQueue m_NativeQueue;

        public MtlQueue(MtlDevice device, in MtlCommandQueueDescriptor descriptor)
        {
            m_MtlDevice = device;
            m_QueueType = descriptor.queueType;
            m_NativeQueue = descriptor.cmdQueue;
        }

        public override RHICommandPool CreateCommandPool()
        {
            throw new NotImplementedException();
        }

        protected override void Release()
        {
            ObjectiveCRuntime.release(m_NativeQueue.NativePtr);
        }
    }
#pragma warning restore CS8600, CS8602
}
