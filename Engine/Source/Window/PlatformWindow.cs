using System;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Windowing
{
	/*public sealed unsafe class PlatformWindow : Disposal
	{
		// Inputs relatives
		event Action<int> KeyPressedEvent;
		event Action<int> KeyReleasedEvent;
		event Action<int> MouseButtonPressedEvent;
		event Action<int> MouseButtonReleasedEvent;

		// Window events
		event Action<ushort, ushort> ResizeEvent;
		event Action<ushort, ushort> FramebufferResizeEvent;
		event Action<short, short> MoveEvent;
		event Action<short, short> CursorMoveEvent;
		event Action MinimizeEvent;
		event Action MaximizeEvent;
		event Action GainFocusEvent;
		event Action LostFocusEvent;
		event Action CloseEvent;

		Glfw m_Glfw;
		int m_Width;
		int m_Height;
		string m_Title;
		WindowHandle* m_Glfwindow;

		public bool IsRunning
		{
			get
			{
				return !m_Glfw.WindowShouldClose(m_Glfwindow);
			}
		}
		public int width => m_Width;
		public int height => m_Height;
		public string title => m_Title;
		public IntPtr windowPtr => new GlfwNativeWindow(m_Glfw, m_Glfwindow).Win32.Value.Hwnd;

		public PlatformWindow(int width, int height, string title)
        {
			m_Title = title;
			m_Width = width;
			m_Height = height;

			m_Glfw = GlfwProvider.GLFW.Value;
			m_Glfw.Init();
			m_Glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);

			//Monitor* monitor = m_Glfw.GetPrimaryMonitor();
			m_Glfwindow = m_Glfw.CreateWindow(width, height, title, null, null);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update()
		{
			m_Glfw.PollEvents();
		}

		protected override void Release()
		{
			base.Release();
			m_Glfw.Terminate();
			m_Glfw.Dispose();
		}
	}*/

	public sealed unsafe class PlatformWindow : Disposal
	{
		int m_Width;
		int m_Height;
		string m_Title;
		IWindow window;

		public bool IsRunning
		{
			get
			{
				return !window.IsClosing;
			}
		}
		public int width => m_Width;
		public int height => m_Height;
		public string title => m_Title;
		//public IntPtr windowPtr => new IntPtr(Silk.NET.Windowing.Sdl.SdlWindowing.GetSysWMInfo(window).Value.Info.Win.Hwnd);
		public IntPtr windowPtr => window.Native.Win32.Value.Hwnd;

		public PlatformWindow(int width, int height, string title)
		{
			m_Title = title;
			m_Width = width;
			m_Height = height;
			Window.PrioritizeSdl();
			WindowOptions options = new WindowOptions(true, new Vector2D<int>(50, 50), new Vector2D<int>(width, height), 60, 60, GraphicsAPI.None, title, WindowState.Normal, WindowBorder.Resizable, true, false, Silk.NET.Windowing.VideoMode.Default);
			window = Window.Create(options);
			window.Load += OnLoad;
			window.Initialize();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update()
		{
			window.DoEvents();
		}

		void KeyDown(IKeyboard arg1, Key arg2, int arg3)
		{
			//Check to close the window on escape.
			if (arg2 == Key.Escape)
			{
				window.Close();
			}

			if (arg1.IsKeyPressed(Key.Number1) && arg1.IsKeyPressed(Key.Number2))
			{
				Console.WriteLine("Fuck");
			}
		}

		void OnLoad()
		{
			//Set-up input context. 
			IInputContext input = window.CreateInput();
			for (int i = 0; i < input.Keyboards.Count; i++)
			{
				input.Keyboards[i].KeyDown += KeyDown;
			}
		}
		
		protected override void Release()
		{
			base.Release();
			window.Dispose();
		}
	}
}
