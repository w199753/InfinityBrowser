﻿using System;
using System.Threading;
using System.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
    internal unsafe class D3DFence : RHIFence
    {
        public ID3D12Fence* NativeFence
        {
            get
            {
                return m_NativeFence;
            }
        }
        public override EFenceStatus Status
        {
            get
            {
                return Completed ? EFenceStatus.Success : EFenceStatus.NotReady;
            }
        }
        
        private bool Completed
        {
            get
            {
                return m_NativeFence->GetCompletedValue() < 1 ? false : true;
            }
        }

        private ID3D12Fence* m_NativeFence;
        private AutoResetEvent m_FenceEvent;

        public D3DFence(D3DDevice device)
        {
            ID3D12Fence* fence;
            Debug.Assert(SUCCEEDED(device.NativeDevice->CreateFence(0, D3D12_FENCE_FLAGS.D3D12_FENCE_FLAG_NONE, Windows.__uuidof<ID3D12Fence>(), (void**)&fence)));
            m_NativeFence = fence;

            m_FenceEvent = new AutoResetEvent(false);
            Debug.Assert(m_FenceEvent != null);
        }

        public override void Reset()
        {
            m_NativeFence->Signal(0);
        }

        public override void Wait()
        {
            IntPtr eventPtr = m_FenceEvent.SafeWaitHandle.DangerousGetHandle();
            HANDLE eventHandle = new HANDLE(eventPtr.ToPointer());
            m_NativeFence->SetEventOnCompletion(1, eventHandle);
            m_FenceEvent.WaitOne();
        }

        protected override void Release()
        {
            m_NativeFence->Release();
        }
    }
}
