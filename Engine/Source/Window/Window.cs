using System;
using Silk.NET.GLFW;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Windowing
{
    public sealed unsafe class Window : Disposal
	{
		/* Inputs relatives */
		event Action<int> KeyPressedEvent;
		event Action<int> KeyReleasedEvent;
		event Action<int> MouseButtonPressedEvent;
		event Action<int> MouseButtonReleasedEvent;

		/* Window events */
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

		public Window(int width, int height, string title)
        {
			m_Title = title;
			m_Width = width;
			m_Height = height;

			m_Glfw = GlfwProvider.GLFW.Value;
			m_Glfw.Init();

			m_Glfwindow = m_Glfw.CreateWindow(width, height, title, null, null);
			m_Glfw.MakeContextCurrent(m_Glfwindow);
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
	}
}
