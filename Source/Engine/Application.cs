using System;
using Infinity.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;
using System.Threading;
using Infinity.Utility;
using Infinity.Windowing;
using Infinity.Rendering;
using Infinity.Analytics;
using System.Collections.Generic;

namespace Infinity.Engine
{
    internal class SystemTimer
    {
        public float DeltaTime
        {
            get
            {
                return m_DeltaTime;
            }
            set
            {
                m_DeltaTime = value;
            }
        }
        public List<float> LastDeltaTimes
        {
            get
            {
                return m_LastDeltaTimes;
            }
            set
            {
                m_LastDeltaTimes = value;
            }
        }
        public TimeProfiler TimeCounter
        {
            get
            {
                return m_TimeCounter;
            }
            set
            {
                m_TimeCounter = value;
            }
        }

        private float m_DeltaTime;
        private List<float> m_LastDeltaTimes;
        private TimeProfiler m_TimeCounter;

        public SystemTimer()
        {
            m_TimeCounter = new TimeProfiler();
            m_LastDeltaTimes = new List<float>(32);
        }

        internal void StartCoutner()
        {
            m_TimeCounter.Reset();
            m_TimeCounter.Begin();
        }

        internal void TickCounter()
        {
            m_TimeCounter.Start();
            GTime.Update(m_DeltaTime);
        }

        internal void Exit()
        {
            m_TimeCounter.Stop();
        }
    }

    public partial class Application : Disposal
    {
        public static uint GTargetFrameRate = 60;

        public bool IsRunning 
        { 
            get 
            { 
                return m_CrossWindow.IsRunning; 
            } 
        }
        public GameWorld GameWorld
        {
            get
            {
                return m_GameWorld;
            }
        }
        
        private SystemTimer m_SystemTimer;
        private CrossWindow m_CrossWindow;
        private GameWorld m_GameWorld;
        private PhysicsModule m_PhysicsModule;
        private GraphicsModule m_GraphicsModule;

        public Application(in int width, in int height, string name)
        {
            Thread.CurrentThread.Name = "GameThread";
            m_CrossWindow = new CrossWindow(width, height, name, OnResize, OnFocus, OnMouseUp, OnMouseDown, OnMouseMove, OnMouseClick, OnMouseDoubleClick, OnMouseScroll, OnKeyUp, OnKeyDown, OnKeyChar);
            m_SystemTimer = new SystemTimer();
            m_GameWorld = new GameWorld();
            m_PhysicsModule = new PhysicsModule();
            m_GraphicsModule = new GraphicsModule(m_CrossWindow);
        }

        protected void OnFocus(bool state)
        {
            if (state)
            {
                Console.WriteLine("GetFocus");
            }
            else
            {
                Console.WriteLine("LossFocus");
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
                m_CrossWindow.Close();
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
            m_SystemTimer.StartCoutner();
            m_GameWorld.Start();
            m_PhysicsModule.Start();
            m_GraphicsModule.Start();

            while (IsRunning)
            {
                m_SystemTimer.TickCounter();
                m_CrossWindow.Update(GTime.DeltaTime);
                m_GameWorld.Update(GTime.DeltaTime);
                m_PhysicsModule.Update(GTime.DeltaTime);
                m_GraphicsModule.Update(GTime.DeltaTime);
                WaitForTargetFPS();
            }

            m_GameWorld.Exit();
            m_PhysicsModule.Exit();
            m_GraphicsModule.Exit();
            m_SystemTimer.Exit();
        }

        protected override void Release()
        {
            m_CrossWindow.Dispose();
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
                    elapsed = m_SystemTimer.TimeCounter.microseconds;
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

            elapsed = m_SystemTimer.TimeCounter.microseconds;
            m_SystemTimer.TimeCounter.Start();

            // Perform timestep smoothing
            m_SystemTimer.DeltaTime = 0.0f;
            m_SystemTimer.LastDeltaTimes.Add(elapsed / 1000000.0f);

            if (m_SystemTimer.LastDeltaTimes.Count > deltaTimeSmoothing)
            {
                // If the smoothing configuration was changed, ensure correct amount of samples
                m_SystemTimer.LastDeltaTimes.RemoveRange(0, m_SystemTimer.LastDeltaTimes.Count - deltaTimeSmoothing);
                for (int i = 0; i < m_SystemTimer.LastDeltaTimes.Count; ++i)
                {
                    m_SystemTimer.DeltaTime += m_SystemTimer.LastDeltaTimes[i];
                }
                m_SystemTimer.DeltaTime /= m_SystemTimer.LastDeltaTimes.Count;
            }
            else
            {
                m_SystemTimer.DeltaTime = m_SystemTimer.LastDeltaTimes[m_SystemTimer.LastDeltaTimes.Count - 1];
            }
        }
    }
}
