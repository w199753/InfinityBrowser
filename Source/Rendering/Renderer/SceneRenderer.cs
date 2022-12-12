using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Infinity.Rendering
{
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
            m_RenderPipeline.Dispose();
        }
    }
}
