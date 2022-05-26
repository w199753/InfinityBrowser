using System.Threading;
using Infinity.Graphics;
using Infinity.Rendering;
using Infinity.Windowing;
using System.Runtime.CompilerServices;
using Semaphore = Infinity.Threading.Semaphore;

namespace Infinity.System
{
    internal class GraphicsSystem : Disposal
    {
        private bool IsLoopExit;
        private Thread m_RenderThread;
        private Semaphore m_SemaphoreG2R;
        private Semaphore m_SemaphoreR2G;
        private RenderContext m_RenderContext;
        private SceneRenderer m_SceneRenderer;

        public GraphicsSystem(PlatformWindow window, Semaphore semaphoreG2R, Semaphore semaphoreR2G)
        {
            IsLoopExit = false;
            m_SemaphoreG2R = semaphoreG2R;
            m_SemaphoreR2G = semaphoreR2G;
            m_RenderThread = new Thread(GraphicsFunc);
            m_RenderThread.Name = "RenderThread";
            m_RenderContext = new RenderContext((uint)window.width, (uint)window.height, window.windowPtr);
            m_SceneRenderer = new SceneRenderer(m_RenderContext);
        }

        public void Start()
        {
            m_RenderThread.Start();
        }

        public void Exit()
        {
            IsLoopExit = true;
            m_SemaphoreG2R.Signal();
            m_RenderThread.Join();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GraphicsFunc()
        {
            bool isInit = true;

            while (!IsLoopExit)
            {
                m_SemaphoreG2R.Wait();
                ProcessGraphicsTasks();
                if (isInit) {
                    isInit = false;
                    m_SceneRenderer.Init(); 
                }
                m_SceneRenderer.Render();
                RHIContext.SubmitContext(m_RenderContext.Instance);
                m_RenderContext.SwapChain.Present();
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
