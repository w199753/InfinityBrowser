﻿using System;
using Vortice.DXGI;
using Vortice.Direct3D12;
using InfinityEngine.Core.Object;
using System.Runtime.CompilerServices;
using InfinityEngine.Core.Native.Utility;

namespace InfinityEngine.Graphics.RHI
{
    public enum EDescriptorType
    {
        DSV = 0,
        RTV = 1,
        CbvSrvUav = 2,
        Sample = 3
    };

    public struct FRHIIndexBufferView
    {
        internal IndexBufferView d3dView;
        internal ulong virtualAddressGPU;
    }

    public struct FRHIVertexBufferView
    {
        internal VertexBufferView d3dView;
        internal ulong virtualAddressGPU;
    }

    public struct FRHIDeptnStencilView
    {

    }

    public struct FRHIRenderTargetView
    {

    }

    public struct FRHIConstantBufferView
    {
        internal int descriptorSize;
        internal int descriptorIndex;
        internal CpuDescriptorHandle descriptorHandle;

        public FRHIConstantBufferView(int descriptorSize, int descriptorIndex, CpuDescriptorHandle descriptorHandle)
        {
            this.descriptorSize = descriptorSize;
            this.descriptorIndex = descriptorIndex;
            this.descriptorHandle = descriptorHandle;
        }

        public CpuDescriptorHandle GetDescriptorHandle()
        {
            return descriptorHandle + descriptorSize * descriptorIndex;
        }
    }

    public struct FRHIShaderResourceView
    {
        internal int descriptorSize;
        internal int descriptorIndex;
        internal CpuDescriptorHandle descriptorHandle;


        public FRHIShaderResourceView(int descriptorSize, int descriptorIndex, CpuDescriptorHandle descriptorHandle)
        {
            this.descriptorSize = descriptorSize;
            this.descriptorIndex = descriptorIndex;
            this.descriptorHandle = descriptorHandle;
        }

        public CpuDescriptorHandle GetDescriptorHandle()
        {
            return descriptorHandle + descriptorSize * descriptorIndex;
        }
    }

    public struct FRHIUnorderedAccessView
    {
        internal int descriptorSize;
        internal int descriptorIndex;
        internal CpuDescriptorHandle descriptorHandle;


        public FRHIUnorderedAccessView(int descriptorSize, int descriptorIndex, CpuDescriptorHandle descriptorHandle)
        {
            this.descriptorSize = descriptorSize;
            this.descriptorIndex = descriptorIndex;
            this.descriptorHandle = descriptorHandle;
        }

        public CpuDescriptorHandle GetDescriptorHandle()
        {
            return descriptorHandle + descriptorSize * descriptorIndex;
        }
    }

    public sealed class FRHIResourceViewRange : FDisposable
    {
        internal int length;
        internal int descriptorIndex;
        internal ID3D12Device6 d3dDevice;
        internal CpuDescriptorHandle descriptorHandle;

        internal FRHIResourceViewRange(ID3D12Device6 d3dDevice, FRHIDescriptorHeapFactory descriptorHeapFactory, in int length) : base()
        {
            this.length = length;
            this.d3dDevice = d3dDevice;
            this.descriptorIndex = descriptorHeapFactory.Allocator(length);
            this.descriptorHandle = descriptorHeapFactory.GetCPUHandleStart();
        }

        private CpuDescriptorHandle GetDescriptorHandle(in int offset)
        {
            return descriptorHandle + d3dDevice.GetDescriptorHandleIncrementSize(DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView) * (descriptorIndex + offset);
        }

        public void SetConstantBufferView(in int index, FRHIConstantBufferView constantBufferView)
        {
            d3dDevice.CopyDescriptorsSimple(1, GetDescriptorHandle(index), constantBufferView.GetDescriptorHandle(), DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
        }

        public void SetShaderResourceView(in int index, FRHIShaderResourceView shaderResourceView)
        {
            d3dDevice.CopyDescriptorsSimple(1, GetDescriptorHandle(index), shaderResourceView.GetDescriptorHandle(), DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
        }

        public void SetUnorderedAccessView(in int index, FRHIUnorderedAccessView unorderedAccessView)
        {
            d3dDevice.CopyDescriptorsSimple(1, GetDescriptorHandle(index), unorderedAccessView.GetDescriptorHandle(), DescriptorHeapType.ConstantBufferViewShaderResourceViewUnorderedAccessView);
        }

        protected override void Disposed()
        {

        }
    }
}
