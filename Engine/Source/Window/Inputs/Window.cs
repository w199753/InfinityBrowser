using System;

namespace InfinityEngine.Window.Inputs
{
    public class Window
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
	}
}
