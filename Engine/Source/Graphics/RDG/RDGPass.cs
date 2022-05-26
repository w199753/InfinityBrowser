using System;
using Infinity.Graphics;
using System.Collections.Generic;

namespace Infinity.Graphics
{
    internal abstract class IRDGPass
    {
        public int index;
        public string name;
        public int refCount;
        public int colorBufferMaxIndex;
        public bool enablePassCulling;
        public bool enableAsyncCompute;
        public virtual bool hasExecuteFunc => false;
        public RDGTextureRef depthBuffer;
        public RDGTextureRef[] colorBuffers;

        public List<RDGResourceRef>[] resourceReadLists = new List<RDGResourceRef>[2];
        public List<RDGResourceRef>[] resourceWriteLists = new List<RDGResourceRef>[2];
        public List<RDGResourceRef>[] temporalResourceList = new List<RDGResourceRef>[2];

        public IRDGPass()
        {
            colorBuffers = new RDGTextureRef[8];
            colorBufferMaxIndex = -1;

            for (int i = 0; i < 2; ++i)
            {
                resourceReadLists[i] = new List<RDGResourceRef>();
                resourceWriteLists[i] = new List<RDGResourceRef>();
                temporalResourceList[i] = new List<RDGResourceRef>();
            }
        }

        public abstract void Execute(in RDGContext graphContext, RHICommandBuffer cmdBuffer);
        public abstract void Release(RDGObjectPool objectPool);

        public void AddResourceWrite(in RDGResourceRef res)
        {
            resourceWriteLists[res.iType].Add(res);
        }

        public void AddResourceRead(in RDGResourceRef res)
        {
            resourceReadLists[res.iType].Add(res);
        }

        public void AddTemporalResource(in RDGResourceRef res)
        {
            temporalResourceList[res.iType].Add(res);
        }

        public void SetColorBuffer(in RDGTextureRef resource, int index)
        {
            colorBufferMaxIndex = Math.Max(colorBufferMaxIndex, index);
            colorBuffers[index] = resource;
            AddResourceWrite(resource.handle);
        }

        public void SetDepthBuffer(in RDGTextureRef resource, in EDepthAccess flags)
        {
            depthBuffer = resource;
            if ((flags & EDepthAccess.Read) != 0)
                AddResourceRead(resource.handle);
            if ((flags & EDepthAccess.Write) != 0)
                AddResourceWrite(resource.handle);
        }

        public void EnablePassCulling(in bool value)
        {
            enablePassCulling = value;
        }

        public void EnableAsyncCompute(in bool value)
        {
            enableAsyncCompute = value;
        }

        public void Clear()
        {
            name = "";
            index = -1;

            for (int i = 0; i < 2; ++i)
            {
                resourceReadLists[i].Clear();
                resourceWriteLists[i].Clear();
                temporalResourceList[i].Clear();
            }

            refCount = 0;
            enablePassCulling = true;
            enableAsyncCompute = false;

            // Invalidate everything
            colorBufferMaxIndex = -1;
            depthBuffer = new RDGTextureRef();
            for (int i = 0; i < 8; ++i)
            {
                colorBuffers[i] = new RDGTextureRef();
            }
        }
    }

    public delegate void FRDGExecuteFunc<T>(in T passData, in RDGContext graphContext, RHICommandBuffer cmdBuffer) where T : struct;

    internal sealed class RDGPass<T> : IRDGPass where T : struct
    {
        public T passData;
        public FRDGExecuteFunc<T> m_ExcuteFunc;
        public override bool hasExecuteFunc { get { return m_ExcuteFunc != null; } }

        public override void Execute(in RDGContext graphContext, RHICommandBuffer cmdBuffer)
        {
            m_ExcuteFunc(passData, graphContext, cmdBuffer);
        }

        public override void Release(RDGObjectPool graphObjectPool)
        {
            Clear();
            m_ExcuteFunc = null;
            graphObjectPool.Release(this);
        }
    }

    public struct RDGPassRef : IDisposable
    {
        bool IsDisposed;
        IRDGPass m_RenderPass;
        RDGResourceFactory m_ResourceFactory;

        internal RDGPassRef(IRDGPass renderPass, RDGResourceFactory resourceFactory)
        {
            IsDisposed = false;
            m_RenderPass = renderPass;
            m_ResourceFactory = resourceFactory;
        }

        public ref T GetPassData<T>() where T : struct => ref ((RDGPass<T>)m_RenderPass).passData;

        public void EnablePassCulling(in bool value)
        {
            m_RenderPass.EnablePassCulling(value);
        }

        public void EnableAsyncCompute(in bool value)
        {
            m_RenderPass.EnableAsyncCompute(value);
        }

        public RDGTextureRef ReadTexture(in RDGTextureRef input)
        {
            m_RenderPass.AddResourceRead(input.handle);
            return input;
        }

        public RDGTextureRef WriteTexture(in RDGTextureRef input)
        {
            m_RenderPass.AddResourceWrite(input.handle);
            return input;
        }

        public RDGTextureRef CreateTemporalTexture(in TextureDescriptor descriptor)
        {
            var result = m_ResourceFactory.CreateTexture(descriptor, 0, m_RenderPass.index);
            m_RenderPass.AddTemporalResource(result.handle);
            return result;
        }

        public RDGBufferRef ReadBuffer(in RDGBufferRef input)
        {
            m_RenderPass.AddResourceRead(input.handle);
            return input;
        }

        public RDGBufferRef WriteBuffer(in RDGBufferRef input)
        {
            m_RenderPass.AddResourceWrite(input.handle);
            return input;
        }

        public RDGBufferRef CreateTemporalBuffer(in BufferDescriptor descriptor)
        {
            var result = m_ResourceFactory.CreateBuffer(descriptor, m_RenderPass.index);
            m_RenderPass.AddTemporalResource(result.handle);
            return result;
        }

        public RDGTextureRef UseDepthBuffer(in RDGTextureRef input, in EDepthAccess accessFlag)
        {
            m_RenderPass.SetDepthBuffer(input, accessFlag);
            return input;
        }

        public RDGTextureRef UseColorBuffer(in RDGTextureRef input, int index)
        {
            m_RenderPass.SetColorBuffer(input, index);
            return input;
        }

        public void SetRenderFunc<T>(FRDGExecuteFunc<T> excuteFunc) where T : struct
        {
            ((RDGPass<T>)m_RenderPass).m_ExcuteFunc = excuteFunc;
        }

        void Dispose(in bool disposing)
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
