using System.Threading;
using Infinity.Graphics;
using Infinity.Rendering;
using Infinity.Windowing;
using System.Runtime.CompilerServices;
using Semaphore = Infinity.Threading.Semaphore;

namespace Infinity
{
    internal class GraphicsModule : Disposal
    {
        private bool m_LoopExit;
        private Thread m_RenderThread;
        private Semaphore m_SemaphoreG2R;
        private Semaphore m_SemaphoreR2G;
        private RenderContext m_RenderContext;
        private SceneRenderer m_SceneRenderer;

        public GraphicsModule(PlatformWindow window, Semaphore semaphoreG2R, Semaphore semaphoreR2G)
        {
            m_LoopExit = false;
            m_SemaphoreG2R = semaphoreG2R;
            m_SemaphoreR2G = semaphoreR2G;
            m_RenderThread = new Thread(GraphicsFunc);
            m_RenderThread.Name = "RenderThread";
            m_RenderContext = new RenderContext(window.Width, window.Height, window.WindowPtr);
            m_SceneRenderer = new SceneRenderer(m_RenderContext);
        }

        public void Start()
        {
            m_RenderThread.Start();
        }

        public void Exit()
        {
            m_LoopExit = true;
            m_SemaphoreG2R.Signal();
            m_RenderThread.Join();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GraphicsFunc()
        {
            m_SemaphoreG2R.Wait();
            m_RenderContext.BeginFrame();
            ProcessGraphicsTasks();
            m_SceneRenderer.Init();
            m_RenderContext.EndFrame();
            m_SemaphoreG2R.Signal();

            while (!m_LoopExit)
            {
                m_SemaphoreG2R.Wait();
                m_RenderContext.BeginFrame();
                ProcessGraphicsTasks();
                m_SceneRenderer.Render();
                m_RenderContext.EndFrame();
                m_SemaphoreR2G.Signal();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProcessGraphicsTasks()
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
            ProcessGraphicsTasks();
            m_SceneRenderer.Dispose();
            m_RenderContext.Dispose();
        }
    }
}
