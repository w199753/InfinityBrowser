using System;
using Infinity.Threading;
using Infinity.Windowing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;
using Infinity.Rendering;

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

        // Window
        private PlatformWindow m_Window;

        // World
        private GameWorld m_GameWorld;

        // Thread
        private Semaphore m_SemaphoreG2R;
        private Semaphore m_SemaphoreR2G;

        // Module
        private GameModule m_GameModule;
        //private PhysicsModule m_PhysicsModule;
        private GraphicsModule m_GraphicsModule;

        public Application(in int width, in int height, string name)
        {
            m_Window = new PlatformWindow(width, height, name, OnResize, OnFocus, OnMouseUp, OnMouseDown, OnMouseMove, OnMouseClick, OnMouseDoubleClick, OnMouseScroll, OnKeyUp, OnKeyDown, OnKeyChar);

            m_GameWorld = new GameWorld();

            m_SemaphoreR2G = new Semaphore(true);
            m_SemaphoreG2R = new Semaphore(false);

            m_GameModule = new GameModule(End, Play, Tick, m_SemaphoreG2R, m_SemaphoreR2G);
            //m_PhysicsModule = new PhysicsModule();
            m_GraphicsModule = new GraphicsModule(m_Window, m_SemaphoreG2R, m_SemaphoreR2G);
        }

        protected void Play()
        {
            m_GameWorld.StartWorld();
        }

        protected void Tick()
        {
            m_GameWorld.TickWorld(Timer.DeltaTime);
        }

        protected void End()
        {
            m_GameWorld.EndWorld();
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
            Loop();
            Exit();
            Dispose();
        }

        private void Loop()
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

        private void Exit()
        {
            m_GameModule.Exit();
            //m_PhysicsModule.Exit();
            m_GraphicsModule.Exit();
        }

        protected override void Release()
        {
            m_Window.Dispose();
            m_SemaphoreR2G.Dispose();
            m_SemaphoreG2R.Dispose();
            m_GameModule.Dispose();
            //m_PhysicsModule.Dispose();
            m_GraphicsModule.Dispose();
        }
    }
}
