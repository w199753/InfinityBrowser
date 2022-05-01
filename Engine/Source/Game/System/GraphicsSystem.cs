using System.Threading;
using InfinityEngine.Graphics;
using InfinityEngine.Rendering;
using InfinityEngine.Windowing;
using System.Runtime.CompilerServices;
using Semaphore = InfinityEngine.Threading.Semaphore;

namespace InfinityEngine.System
{
    internal class GraphicsSystem : Disposal
    {
        private bool IsLoopExit;
        private Thread m_RenderThread;
        private Semaphore m_SemaphoreG2R;
        private Semaphore m_SemaphoreR2G;
        private RHIContext m_Context;
        private RHISwapChain m_SwapChain;
        private RenderContext m_RenderContext;
        private RenderPipeline m_RenderPipeline;

        public GraphicsSystem(Window window, Semaphore semaphoreG2R, Semaphore semaphoreR2G)
        {
            IsLoopExit = false;
            m_SemaphoreG2R = semaphoreG2R;
            m_SemaphoreR2G = semaphoreR2G;
            m_RenderThread = new Thread(GraphicsFunc);
            m_RenderThread.Name = "m_RenderThread";
            m_Context = new D3DContext();
            m_RenderPipeline = new UniversalRenderPipeline("UniversalRP");
            m_SwapChain = m_Context.CreateSwapChain("SwapChain", (uint)window.width, (uint)window.height, window.windowPtr);
            m_SwapChain.InitResourceView(m_Context);
            m_RenderContext = new RenderContext(m_Context, m_SwapChain);
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
                    m_RenderPipeline.Init(m_RenderContext); 
                }
                m_RenderPipeline.Render(m_RenderContext);
                RHIContext.SubmitContext(m_Context);
                m_SwapChain.Present();
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
            m_RenderPipeline?.Release(m_RenderContext);

            m_SwapChain?.Dispose();
            m_Context?.Dispose();
            m_RenderPipeline?.Dispose();
            m_RenderContext?.Dispose();
        }
    }
}
