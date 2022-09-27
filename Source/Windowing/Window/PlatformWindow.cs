using System;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;
using Silk.NET.Windowing;
using System.Runtime.CompilerServices;
using MouseButton = Silk.NET.Input.MouseButton;

namespace Infinity.Windowing
{
	public sealed class PlatformWindow : Disposal
	{
		int m_Width;
		int m_Height;
		string m_Title;
		IWindow m_Window;

		public bool IsRunning
		{
			get
			{
				return !m_Window.IsClosing;
			}
		}
		public int Width => m_Width;
		public int Height => m_Height;
		public string Title => m_Title;
		public IntPtr WindowPtr => m_Window.Native.Win32.Value.Hwnd;

		public PlatformWindow(in int width, in int height, string title, Action<Vector2D<int>> OnResize, Action<bool> OnFocus, Action<IMouse, MouseButton> OnMouseUp, Action<IMouse, MouseButton> OnMouseDown, Action<IMouse, Vector2> OnMouseMove, Action<IMouse, MouseButton, Vector2> OnMouseClick, Action<IMouse, MouseButton, Vector2> OnMouseDoubleClick, Action<IMouse, ScrollWheel> OnMouseScroll, Action<IKeyboard, Key, int> OnKeyUp, Action<IKeyboard, Key, int> OnKeyDown, Action<IKeyboard, char> OnKeyChar)
		{
			m_Title = title;
			m_Width = width;
			m_Height = height;

			//Window.PrioritizeSdl();
			WindowOptions options = new WindowOptions(true, new Vector2D<int>(256, 256), new Vector2D<int>(width, height), 60, 60, GraphicsAPI.None, title, WindowState.Normal, WindowBorder.Resizable, true, false, Silk.NET.Windowing.VideoMode.Default);
            m_Window = Window.Create(options);
            m_Window.Resize += OnResize;
            m_Window.FocusChanged += OnFocus;
            m_Window.Initialize();

            IInputContext inputContext = m_Window.CreateInput();
            for (int i = 0; i < inputContext.Keyboards.Count; ++i)
            {
                inputContext.Mice[i].MouseUp += OnMouseUp;
                inputContext.Mice[i].MouseDown += OnMouseDown;
                inputContext.Mice[i].MouseMove += OnMouseMove;
                inputContext.Mice[i].Click += OnMouseClick;
                inputContext.Mice[i].Scroll += OnMouseScroll;
                inputContext.Mice[i].DoubleClick += OnMouseDoubleClick;

                inputContext.Keyboards[i].KeyUp += OnKeyUp;
                inputContext.Keyboards[i].KeyDown += OnKeyDown;
                inputContext.Keyboards[i].KeyChar += OnKeyChar;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update()
		{
            m_Window.DoEvents();
		}

       
        public void Close()
        {
            m_Window.Close();
        }

        protected override void Release()
		{
            m_Window.Dispose();
		}
	}
}
