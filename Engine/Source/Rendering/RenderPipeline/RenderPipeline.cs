using InfinityEngine.Graphics;

namespace InfinityEngine.Rendering
{
    public abstract class RenderPipeline : Disposal
    {
        public string name;
        protected RDGBuilder m_GraphBuilder;

        public RenderPipeline(string name)
        {
            this.name = name;
            this.m_GraphBuilder = new RDGBuilder("GraphBuilder");
        }

        public abstract void Init(RenderContext renderContext);

        public abstract void Render(RenderContext renderContext);

        public abstract void Release(RenderContext renderContext);

        protected override void Release()
        {
            m_GraphBuilder?.Dispose();
        }
    }
}
