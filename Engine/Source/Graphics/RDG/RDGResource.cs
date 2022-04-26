using System;
using InfinityEngine.Graphics.RHI;
using InfinityEngine.Core.Container;

namespace InfinityEngine.Graphics.RDG
{
    public enum EDepthAccess
    {
        Read = 1 << 0,
        Write = 1 << 1,
        ReadWrite = Read | Write,
    }

    internal struct RDGResourceRef
    {
        internal bool IsValid;
        public int index { get; private set; }
        public EResourceType type { get; private set; }
        public int iType { get { return (int)type; } }


        internal RDGResourceRef(int value, EResourceType type)
        {
            this.type = type;
            this.index = value;
            this.IsValid = true;
        }

        public static implicit operator int(RDGResourceRef handle) => handle.index;
    }

    public struct RDGBufferRef
    {
        internal RDGResourceRef handle;

        internal RDGBufferRef(int handle) 
        { 
            this.handle = new RDGResourceRef(handle, EResourceType.Buffer); 
        }

        public static implicit operator RHIBuffer(RDGBufferRef bufferRef) => bufferRef.handle.IsValid ? RDGResourceFactory.current.GetBuffer(bufferRef) : null;
    }

    public struct RDGTextureRef
    {
        internal RDGResourceRef handle;

        internal RDGTextureRef(int handle) 
        { 
            this.handle = new RDGResourceRef(handle, EResourceType.Texture); 
        }

        public static implicit operator RHITexture(RDGTextureRef textureRef) => textureRef.handle.IsValid ? RDGResourceFactory.current.GetTexture(textureRef) : null;
    }

    internal class IRDGResource
    {
        public bool imported;
        public bool wasReleased;
        public int cachedHash;
        public int shaderProperty;
        public int temporalPassIndex;

        public virtual void Reset()
        {
            imported = false;
            wasReleased = false;
            cachedHash = -1;
            shaderProperty = 0;
            temporalPassIndex = -1;
        }

        public virtual string GetName()
        {
            return "";
        }
    }

    internal class RDGResource<DescType, ResType> : IRDGResource where DescType : struct where ResType : class
    {
        public DescType desc;
        public ResType resource;

        protected RDGResource()
        {

        }

        public override void Reset()
        {
            base.Reset();
            resource = null;
        }
    }

    internal class RDGBuffer : RDGResource<BufferDescriptor, RHIBuffer>
    {
        public override string GetName()
        {
            return desc.name;
        }
    }

    internal class RDGTexture : RDGResource<TextureDescriptor, RHITexture>
    {
        public override string GetName()
        {
            return desc.name;
        }
    }

    internal class RDGResourceFactory : Disposal
    {
        static RDGResourceFactory m_CurrentRegistry;
        internal static RDGResourceFactory current
        {
            get
            {
                return m_CurrentRegistry;
            } set {
                m_CurrentRegistry = value;
            }
        }

        RHIBufferCache m_BufferPool = new RHIBufferCache();
        RHITextureCache m_TexturePool = new RHITextureCache();
        TArray<IRDGResource>[] m_Resources = new TArray<IRDGResource>[2];

        internal RDGResourceFactory()
        {
            for (int i = 0; i < 2; ++i)
                m_Resources[i] = new TArray<IRDGResource>();
        }

        internal RHIBuffer GetBuffer(in RDGBufferRef bufferRef)
        {
            return GetBufferResource(bufferRef.handle).resource;
        }

        internal RHITexture GetTexture(in RDGTextureRef textureRef)
        {
            return GetTextureResource(textureRef.handle).resource;
        }

        #region Internal Interface
        ResType GetResource<DescType, ResType>(TArray<IRDGResource> resourceArray, int index) where DescType : struct where ResType : class
        {
            var rdgResource = resourceArray[index] as RDGResource<DescType, ResType>;
            return rdgResource.resource;
        }

        internal void BeginRender()
        {
            current = this;
        }

        internal void EndRender()
        {
            current = null;
        }

        internal string GetResourceName(in RDGResourceRef resourceRef)
        {
            return m_Resources[resourceRef.iType][resourceRef.index].GetName();
        }

        internal bool IsResourceImported(in RDGResourceRef resourceRef)
        {
            return m_Resources[resourceRef.iType][resourceRef.index].imported;
        }

        internal int GetResourceTemporalIndex(in RDGResourceRef resourceRef)
        {
            return m_Resources[resourceRef.iType][resourceRef.index].temporalPassIndex;
        }

        int AddNewResource<ResType>(TArray<IRDGResource> resourceArray, out ResType outRes) where ResType : IRDGResource, new()
        {
            int result = resourceArray.length;
            resourceArray.Resize(resourceArray.length + 1, true);
            if (resourceArray[result] == null)
                resourceArray[result] = new ResType();

            outRes = resourceArray[result] as ResType;
            outRes.Reset();
            return result;
        }

        internal RDGTextureRef ImportTexture(RHITexture rhiTexture, in int shaderProperty = 0)
        {
            int newHandle = AddNewResource(m_Resources[(int)EResourceType.Texture], out RDGTexture rdgTexture);
            rdgTexture.resource = rhiTexture;
            rdgTexture.imported = true;
            rdgTexture.shaderProperty = shaderProperty;

            return new RDGTextureRef(newHandle);
        }

        internal RDGTextureRef CreateTexture(in TextureDescriptor textureDescriptor, in int shaderProperty = 0, in int temporalPassIndex = -1)
        {
            int newHandle = AddNewResource(m_Resources[(int)EResourceType.Texture], out RDGTexture rdgTexture);
            rdgTexture.desc = textureDescriptor;
            rdgTexture.shaderProperty = shaderProperty;
            rdgTexture.temporalPassIndex = temporalPassIndex;
            return new RDGTextureRef(newHandle);
        }

        internal int GetTextureResourceCount()
        {
            return m_Resources[(int)EResourceType.Texture].length;
        }

        RDGTexture GetTextureResource(in RDGResourceRef resourceRef)
        {
            return m_Resources[(int)EResourceType.Texture][resourceRef] as RDGTexture;
        }

        internal TextureDescriptor GetTextureDescriptor(in RDGResourceRef resourceRef)
        {
            return (m_Resources[(int)EResourceType.Texture][resourceRef] as RDGTexture).desc;
        }

        internal RDGBufferRef ImportBuffer(RHIBuffer rhiBuffer)
        {
            int newHandle = AddNewResource(m_Resources[(int)EResourceType.Buffer], out RDGBuffer rdgBuffer);
            rdgBuffer.resource = rhiBuffer;
            rdgBuffer.imported = true;

            return new RDGBufferRef(newHandle);
        }

        internal RDGBufferRef CreateBuffer(in BufferDescriptor bufferDescriptor, in int temporalPassIndex = -1)
        {
            int newHandle = AddNewResource(m_Resources[(int)EResourceType.Buffer], out RDGBuffer bufferResource);
            bufferResource.desc = bufferDescriptor;
            bufferResource.temporalPassIndex = temporalPassIndex;

            return new RDGBufferRef(newHandle);
        }

        internal int GetBufferResourceCount()
        {
            return m_Resources[(int)EResourceType.Buffer].length;
        }

        RDGBuffer GetBufferResource(in RDGResourceRef resourceRef)
        {
            return m_Resources[(int)EResourceType.Buffer][resourceRef] as RDGBuffer;
        }

        internal BufferDescriptor GetBufferDescriptor(in RDGResourceRef handle)
        {
            return (m_Resources[(int)EResourceType.Buffer][handle] as RDGBuffer).desc;
        }

        internal void CreateRealBuffer(in int index)
        {
            var resource = m_Resources[(int)EResourceType.Buffer][index] as RDGBuffer;
            if (!resource.imported)
            {
                var desc = resource.desc;
                int hashCode = desc.GetHashCode();

                if (resource.resource != null)
                    throw new InvalidOperationException(string.Format("Trying to create an already created Compute Buffer ({0}). Buffer was probably declared for writing more than once in the same pass.", resource.desc.name));

                resource.resource = null;
                if (!m_BufferPool.Pull(hashCode, out resource.resource))
                {
                    //resource.resource = new ComputeBuffer(resource.desc.count, resource.desc.stride, resource.desc.type);
                }
                resource.cachedHash = hashCode;
            }
        }

        internal void ReleaseRealBuffer(in int index)
        {
            var resource = m_Resources[(int)EResourceType.Buffer][index] as RDGBuffer;

            if (!resource.imported)
            {
                if (resource.resource == null)
                    throw new InvalidOperationException($"Tried to release a compute buffer ({resource.desc.name}) that was never created. Check that there is at least one pass writing to it first.");

                m_BufferPool.Push(resource.cachedHash, resource.resource);
                resource.cachedHash = -1;
                resource.resource = null;
                resource.wasReleased = true;
            }
        }

        internal void CreateRealTexture(in int index)
        {
            var resource = m_Resources[(int)EResourceType.Texture][index] as RDGTexture;

            if (!resource.imported)
            {
                var desc = resource.desc;
                int hashCode = desc.GetHashCode();

                if (resource.resource != null)
                    throw new InvalidOperationException(string.Format("Trying to create an already created texture ({0}). Texture was probably declared for writing more than once in the same pass.", resource.desc.name));

                resource.resource = null;

                if (!m_TexturePool.Pull(hashCode, out resource.resource))
                {
                    //resource.resource = new Texture();
                }

                resource.cachedHash = hashCode;
            }
        }

        internal void ReleaseRealTexture(in int index)
        {
            var resource = m_Resources[(int)EResourceType.Texture][index] as RDGTexture;

            if (!resource.imported)
            {
                if (resource.resource == null)
                    throw new InvalidOperationException($"Tried to release a texture ({resource.desc.name}) that was never created. Check that there is at least one pass writing to it first.");

                m_TexturePool.Push(resource.cachedHash, resource.resource);
                resource.cachedHash = -1;
                resource.resource = null;
                resource.wasReleased = true;
            }
        }

        internal void Clear()
        {
            for (int i = 0; i < 2; ++i)
            {
                m_Resources[i].Clear();
            }
        }

        protected override void Release()
        {
            m_BufferPool.Dispose();
            m_TexturePool.Dispose();
        }
        #endregion
    }
}
