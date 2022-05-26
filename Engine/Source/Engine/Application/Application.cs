using System;
using Infinity.System;
using Infinity.Threading;
using Infinity.Windowing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Infinity
{
    public abstract partial class GameApplication : Disposal
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
        private GameSystem m_GameSystem;
        private PhysicsSystem m_PhysicsSystem;
        private GraphicsSystem m_GraphicsSystem;

        public GameApplication(in int width, in int height, string name)
        {
            m_Window = new PlatformWindow(width, height, name);
            m_SemaphoreR2G = new Semaphore(true);
            m_SemaphoreG2R = new Semaphore(false);
            m_GameSystem = new GameSystem(End, Play, Tick, m_SemaphoreG2R, m_SemaphoreR2G);
            m_PhysicsSystem = new PhysicsSystem();
            m_GraphicsSystem = new GraphicsSystem(m_Window, m_SemaphoreG2R, m_SemaphoreR2G);
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
            m_GameSystem.Start();
            m_PhysicsSystem.Start();
            m_GraphicsSystem.Start();

            while (IsRunning)
            {
                m_Window.Update();
                m_GameSystem.Update();
            }
        }

        private void PlatformExit()
        {
            m_GameSystem.Exit();
            m_PhysicsSystem.Exit();
            m_GraphicsSystem.Exit();
        }

        protected override void Release()
        {
            m_GameSystem.Dispose();
            m_PhysicsSystem.Dispose();
            m_GraphicsSystem.Dispose();

            m_Window.Dispose();
            m_SemaphoreR2G.Dispose();
            m_SemaphoreG2R.Dispose();
        }
    }
}
