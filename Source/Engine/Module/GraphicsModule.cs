using Infinity.Core;
using Infinity.Rendering;
using Infinity.Windowing;
using System.Runtime.CompilerServices;

namespace Infinity.Engine
{
    internal class GraphicsModule : Disposal
    {
        private RenderContext m_RenderContext;
        private SceneRenderer m_SceneRenderer;

        public GraphicsModule(CrossWindow window)
        {
            m_RenderContext = new RenderContext(window.Width, window.Height, window.WindowPtr);
            m_SceneRenderer = new SceneRenderer(m_RenderContext);
        }

        public void Start()
        {
            m_RenderContext.BeginInit();
            ProcessingGraphicsTasks();
            m_SceneRenderer.Init();
            m_RenderContext.EndInit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(in float deltaTime)
        {
            m_RenderContext.BeginFrame();
            ProcessingGraphicsTasks();
            m_SceneRenderer.Render();
            m_RenderContext.EndFrame();
        }

        public void Exit()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProcessingGraphicsTasks()
        {
            if (GraphicsUtility.GraphicsTasks.length == 0) { return; }

            for (int i = 0; i < GraphicsUtility.GraphicsTasks.length; ++i) {
                GraphicsUtility.GraphicsTasks[i](m_RenderContext);
                GraphicsUtility.GraphicsTasks[i] = null;
            }
            GraphicsUtility.GraphicsTasks.Clear();
        }

        protected override void Release()
        {
            ProcessingGraphicsTasks();
            m_SceneRenderer.Dispose();
            m_RenderContext.Dispose();
        }
    }
}
