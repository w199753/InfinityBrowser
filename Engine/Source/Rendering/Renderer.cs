using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Rendering
{
    public abstract class Renderer : Disposal
    {
        protected RenderContext m_RenderContext;

        public Renderer(RenderContext renderContext)
        {
            m_RenderContext = renderContext;
        }

        public abstract void Init();

        public abstract void Render();
    }

    public class GUIRenderer : Renderer
    {
        public GUIRenderer(RenderContext renderContext) : base(renderContext)
        {

        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }
    }

    public class SceneRenderer : Renderer
    {
        private HybridRenderPipeline m_RenderPipeline;

        public SceneRenderer(RenderContext renderContext) : base(renderContext)
        {
            m_RenderPipeline = new HybridRenderPipeline("HybridRP");
        }

        public override void Init()
        {
            m_RenderPipeline.Init(m_RenderContext);
        }

        public override void Render()
        {
            m_RenderPipeline.Render(m_RenderContext);
        }

        protected override void Release()
        {
            m_RenderPipeline.Release(m_RenderContext);
        }
    }
}
