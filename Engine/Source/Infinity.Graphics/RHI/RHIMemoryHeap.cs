﻿using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using InfinityEngine.Core.Object;
using System.Runtime.CompilerServices;
using InfinityEngine.Core.Native.Utility;

namespace InfinityEngine.Graphics.RHI
{
    internal sealed class FRHIMemoryHeapFactory : UObject
    {
        internal FRHIMemoryHeapFactory(ID3D12Device6 d3D12Device, in int heapCount) : base()
        {

        }

        protected override void Disposed()
        {

        }
    }
}
