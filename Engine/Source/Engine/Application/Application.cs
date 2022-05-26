using System;
using Infinity.Threading;
using Infinity.Windowing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Infinity
{
    public abstract partial class Application : Disposal
    {
        public static uint GTargetFrameRate = 60;

        public bool IsRunning 
        { 
            get 
            { 
                return m_Window.IsRunning; 
            } 
        }

        private PlatformWindow m_Window;
        private Semaphore m_SemaphoreG2R;
        private Semaphore m_SemaphoreR2G;
        private GameModule m_GameModule;
        //private PhysicsModule m_PhysicsModule;
        private GraphicsModule m_GraphicsModule;

        public Application(in int width, in int height, string name)
        {
            m_Window = new PlatformWindow(width, height, name);
            m_SemaphoreR2G = new Semaphore(true);
            m_SemaphoreG2R = new Semaphore(false);
            m_GameModule = new GameModule(End, Play, Tick, m_SemaphoreG2R, m_SemaphoreR2G);
            //m_PhysicsModule = new PhysicsModule();
            m_GraphicsModule = new GraphicsModule(m_Window, m_SemaphoreG2R, m_SemaphoreR2G);
        }

        protected abstract void Play();

        protected abstract void Tick();

        protected abstract void End();

        public void Run()
        {
            PlatformRun();
            PlatformExit();
            Dispose();
        }

        private void PlatformRun()
        {
            m_GameModule.Start();
            //m_PhysicsModule.Start();
            m_GraphicsModule.Start();

            while (IsRunning)
            {
                m_Window.Update();
                m_GameModule.Update();
            }
        }

        private void PlatformExit()
        {
            m_GameModule.Exit();
            //m_PhysicsModule.Exit();
            m_GraphicsModule.Exit();
        }

        protected override void Release()
        {
            m_GameModule.Dispose();
            //m_PhysicsModule.Dispose();
            m_GraphicsModule.Dispose();
            m_SemaphoreR2G.Dispose();
            m_SemaphoreG2R.Dispose();
            m_Window.Dispose();
        }
    }
}
