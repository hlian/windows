using System;
using System.Runtime.InteropServices;

static internal class NativeMethods
{
	[DllImport("user32.dll")]
	internal static extern void SetParent(IntPtr ChildWindowHandle,
		IntPtr ParentWindowHandle);

	[DllImport("user32.dll")]
	internal static extern void RegisterHotKey(IntPtr ReceivingWindowHandle,
		int HotKeyIdentifier, uint KeyModifiers, uint VirtualKeyCode);

	[DllImport("user32.dll", SetLastError = true)]
	internal static extern void UnregisterHotKey(IntPtr ReceivingWindowHandle,
		int HotKeyIdentifier);

	[DllImport("user32.dll")]
	internal static extern void ShowWindow(IntPtr hwnd, int cmd);

	[DllImport("user32.dll")]
	internal static extern void PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam,
	   IntPtr lParam);

	public const int
		WM_HOTKEY = 0x0312,
		WS_CLIPCHILDREN = 0x02000000,
		WM_APP = 0x8000;

	public const int
		MOD_ALT = 0x0001,
		MOD_CONTROL = 0x0002,
		MOD_SHIFT = 0x0004,
		MOD_WIN = 0x0008;

	[DllImport("user32.dll")]
	internal static extern void SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	internal static extern IntPtr GetDesktopWindow();

	public const int HWND_TOPMOST = -1,
		HWND_NOTOPMOST = -2;

	public const uint SWP_SHOWWINDOW = 0x40,
		SWP_NOSIZE = 0x1,
		SWP_NOMOVE = 0x2;

	[DllImport("kernel32.dll")]
	internal static extern int SearchPath(string lpPath,
		string lpFileName, string lpExtension, int nBufferLength,
		[Out] System.Text.StringBuilder lpBuffer, out IntPtr lpFilePart);
}