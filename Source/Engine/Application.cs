using System;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;
using System.Threading;
using Infinity.Windowing;
using Infinity.Rendering;
using Infinity.Analytics;
using System.Collections.Generic;

namespace Infinity.Engine
{
    public partial class Application : Disposal
    {
        public static uint GTargetFrameRate = 60;

        public bool IsRunning 
        { 
            get 
            { 
                return m_Window.IsRunning; 
            } 
        }
        public GameWorld GameWorld
        {
            get
            {
                return m_GameWorld;
            }
        }
        
        private PlatformWindow m_Window;
        private GameWorld m_GameWorld;
        private float m_DeltaTime;
        private TimeProfiler m_TimeCounter;
        private List<float> m_LastDeltaTimes;
        private PhysicsModule m_PhysicsModule;
        private GraphicsModule m_GraphicsModule;

        public Application(in int width, in int height, string name)
        {
            Thread.CurrentThread.Name = "GameThread";
            m_Window = new PlatformWindow(width, height, name, OnResize, OnFocus, OnMouseUp, OnMouseDown, OnMouseMove, OnMouseClick, OnMouseDoubleClick, OnMouseScroll, OnKeyUp, OnKeyDown, OnKeyChar);
            m_GameWorld = new GameWorld();
            m_TimeCounter = new TimeProfiler();
            m_LastDeltaTimes = new List<float>(64);
            m_PhysicsModule = new PhysicsModule();
            m_GraphicsModule = new GraphicsModule(m_Window);
        }

        protected void OnFocus(bool state)
        {
            if (state)
            {
                //Console.WriteLine("GetFocus");
            }
            else
            {
                //Console.WriteLine("LossFocus");
            }
        }

        protected void OnResize(Vector2D<int> size)
        {
            GraphicsUtility.AddTask((RenderContext renderContext) =>
            {
                Console.WriteLine("Resize Window");
                renderContext.ResizeWindow(new Mathmatics.int2(size.X, size.Y));
            });
        }

        protected void OnKeyUp(IKeyboard keyboar, Key key, int arg3)
        {

        }

        protected void OnKeyDown(IKeyboard keyboar, Key key, int arg3)
        {
            if (key == Key.Escape)
            {
                m_Window.Close();
            }

            if (keyboar.IsKeyPressed(Key.Number1) && keyboar.IsKeyPressed(Key.Number2))
            {
                Console.WriteLine("Fuck");
            }
        }

        protected void OnKeyChar(IKeyboard keyboar, char character)
        {

        }

        protected void OnMouseUp(IMouse mouse, MouseButton button)
        {

        }

        protected void OnMouseDown(IMouse mouse, MouseButton button)
        {

        }

        protected void OnMouseMove(IMouse mouse, Vector2 pos)
        {

        }

        protected void OnMouseClick(IMouse mouse, MouseButton button, Vector2 pos)
        {

        }

        protected void OnMouseDoubleClick(IMouse mouse, MouseButton button, Vector2 pos)
        {

        }

        protected void OnMouseScroll(IMouse mouse, ScrollWheel wheel)
        {

        }

        public void Run()
        {
            m_TimeCounter.Reset();
            m_TimeCounter.Begin();

            m_GameWorld.Start();
            m_PhysicsModule.Start();
            m_GraphicsModule.Start();

            while (IsRunning)
            {
                m_TimeCounter.Start();
                Timer.Tick(m_DeltaTime);

                m_Window.Update(m_DeltaTime);
                m_GameWorld.Update(m_DeltaTime);
                m_PhysicsModule.Update(m_DeltaTime);
                m_GraphicsModule.Update(m_DeltaTime);
                WaitForTargetFPS();
            }

            m_TimeCounter.Stop();
            m_GameWorld.Exit();
            m_PhysicsModule.Exit();
            m_GraphicsModule.Exit();
        }

        protected override void Release()
        {
            m_Window.Dispose();
            m_PhysicsModule.Dispose();
            m_GraphicsModule.Dispose();
        }

        void WaitForTargetFPS()
        {
            long elapsed;
            const int deltaTimeSmoothing = 2;

            if (GTargetFrameRate > 0)
            {
                long targetMax = 1000000L / GTargetFrameRate;

                while (true)
                {
                    elapsed = m_TimeCounter.microseconds;
                    if (elapsed >= targetMax)
                        break;

                    // Sleep if 1 ms or more off the frame limiting goal
                    if (targetMax - elapsed >= 1000L)
                    {
                        int sleepTime = (int)((targetMax - elapsed) / 1000L);
                        Thread.Sleep(sleepTime);
                    }
                }
            }

            elapsed = m_TimeCounter.microseconds;
            m_TimeCounter.Start();

            // Perform timestep smoothing
            m_DeltaTime = 0.0f;
            m_LastDeltaTimes.Add(elapsed / 1000000.0f);

            if (m_LastDeltaTimes.Count > deltaTimeSmoothing)
            {
                // If the smoothing configuration was changed, ensure correct amount of samples
                m_LastDeltaTimes.RemoveRange(0, m_LastDeltaTimes.Count - deltaTimeSmoothing);
                for (int i = 0; i < m_LastDeltaTimes.Count; ++i)
                {
                    m_DeltaTime += m_LastDeltaTimes[i];
                }
                m_DeltaTime /= m_LastDeltaTimes.Count;
            }
            else
            {
                m_DeltaTime = m_LastDeltaTimes[m_LastDeltaTimes.Count - 1];
            }
        }
    }
}
