﻿using System.Threading;
using InfinityEngine.Core.Object;
using InfinityEngine.Graphics.RHI;
using System.Collections.Concurrent;
using InfinityEngine.Core.Container;
using InfinityEngine.Core.Thread.Sync;
using InfinityEngine.Rendering.RenderLoop;
using InfinityEngine.Rendering.RenderPipeline;

namespace InfinityEngine.Game.System
{
    public delegate void FGraphicsTask(FRenderContext renderContext, FRHIGraphicsContext graphicsContext);

    public static class FGraphics
    {
        internal static TArray<FGraphicsTask> GraphicsTasks = new TArray<FGraphicsTask>(64);

        public static void EnqueueTask(FGraphicsTask graphicsTask)
        {
            GraphicsTasks.Add(graphicsTask);
        }
    }

    internal class FGraphicsSystem : FDisposable
    {
        private bool bLoopExit;
        internal Thread renderThread;
        internal FSemaphore semaphoreG2R;
        internal FSemaphore semaphoreR2G;
        internal FRenderContext renderContext;
        internal FRenderPipeline renderPipeline;
        internal FRHIGraphicsContext graphicsContext;

        public FGraphicsSystem(FSemaphore semaphoreG2R, FSemaphore semaphoreR2G)
        {
            this.bLoopExit = false;
            this.semaphoreG2R = semaphoreG2R;
            this.semaphoreR2G = semaphoreR2G;
            this.renderThread = new Thread(GraphicsFunc);
            this.renderThread.Name = "RenderThread";

            this.renderContext = new FRenderContext();
            this.graphicsContext = new FRHIGraphicsContext();
            this.renderPipeline = new FUniversalRenderPipeline("UniversalRP");
        }

        public void Start()
        {
            renderThread.Start();
        }

        public void Exit()
        {
            bLoopExit = true;
            semaphoreG2R.Signal();
            renderThread.Join();
        }

        public void GraphicsFunc()
        {
            bool isInit = true;

            while (!bLoopExit)
            {
                semaphoreG2R.Wait();
                if (isInit) 
                {
                    isInit = false;
                    renderPipeline.Init(renderContext, graphicsContext); 
                }
                ProcessGraphicsTasks();
                renderPipeline.Render(renderContext, graphicsContext);
                graphicsContext.Flush();
                semaphoreR2G.Signal();
            }
        }

        public void ProcessGraphicsTasks()
        {
            if (FGraphics.GraphicsTasks.length == 0) { return; }

            for (int i = 0; i < FGraphics.GraphicsTasks.length; ++i)
            {
                FGraphics.GraphicsTasks[i](renderContext, graphicsContext);
                FGraphics.GraphicsTasks[i] = null;
            }
            FGraphics.GraphicsTasks.Clear();
        }

        protected override void Release()
        {
            ProcessGraphicsTasks();
            renderPipeline?.Destroy(renderContext, graphicsContext);

            renderContext?.Dispose();
            renderPipeline?.Dispose();
            graphicsContext?.Dispose();
        }
    }
}
