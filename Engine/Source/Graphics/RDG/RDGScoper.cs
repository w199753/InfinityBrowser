using System.Collections.Generic;
using InfinityEngine.Graphics.RHI;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RDG
{
    internal class RDGResourceMap<Type> where Type : struct
    {
        internal Dictionary<int, Type> m_ResourceMap;

        internal RDGResourceMap()
        {
            m_ResourceMap = new Dictionary<int, Type>(64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Set(in int key, in Type value)
        {
            m_ResourceMap.TryAdd(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Type Get(in int key)
        {
            Type output;
            m_ResourceMap.TryGetValue(key, out output);
            return output;
        }

        internal void Clear()
        {
            m_ResourceMap.Clear();
        }

        internal void Dispose()
        {
            m_ResourceMap = null;
        }
    }

    public class RDGScoper
    {
        RDGBuilder m_GraphBuilder;
        RDGResourceMap<RDGBufferRef> m_BufferMap;
        RDGResourceMap<RDGTextureRef> m_TextureMap;

        public RDGScoper(RDGBuilder graphBuilder)
        {
            m_GraphBuilder = graphBuilder;
            m_BufferMap = new RDGResourceMap<RDGBufferRef>();
            m_TextureMap = new RDGResourceMap<RDGTextureRef>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RDGBufferRef QueryBuffer(in int handle)
        {
            return m_BufferMap.Get(handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterBuffer(int handle, in RDGBufferRef bufferRef)
        {
            m_BufferMap.Set(handle, bufferRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RDGBufferRef CreateBuffer(in int handle, in BufferDescriptor descriptor)
        {
            RDGBufferRef bufferRef = m_GraphBuilder.CreateBuffer(descriptor);
            RegisterBuffer(handle, bufferRef);
            return bufferRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RDGTextureRef QueryTexture(in int handle)
        {
            return m_TextureMap.Get(handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterTexture(int handle, in RDGTextureRef textureRef)
        {
            m_TextureMap.Set(handle, textureRef);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RDGTextureRef CreateAndRegisterTexture(in int handle, in TextureDescriptor descriptor)
        {
            RDGTextureRef textureRef = m_GraphBuilder.CreateTexture(descriptor, handle);
            RegisterTexture(handle, textureRef);
            return textureRef;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_BufferMap.Clear();
            m_TextureMap.Clear();
        }

        public void Dispose()
        {
            m_BufferMap.Dispose();
            m_TextureMap.Dispose();
        }
    }
}
