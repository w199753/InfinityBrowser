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
}
