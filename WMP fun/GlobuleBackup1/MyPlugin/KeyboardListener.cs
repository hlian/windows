using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Globule
{
	// The KeyboardListener is a static class that allows registering a number
	// of event handlers that you want to get called in case some keyboard key is pressed 
	// or released. The nice thing is that this KeyboardListener is also active in case
	// the parent application is running in the back.
	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "SkipVerification")]
	public class KeyboardListener
	{
		private static ListeningWindow s_Listener;

		// The function that will handle all keyboard activity signaled by the ListeningWindow.
		// In this context handling means calling all registered subscribers for every key pressed / released.

		// Inside this method the events could also be fired by calling
		// s_KeyEventHandler(null,new KeyEventArgs(key,msg)) However, in case one of the registered
		// subscribers throws an exception, execution of the non-executed subscribers is cancelled.
		private static void KeyHandler(ushort key, uint msg)
		{
			if (s_KeyEventHandler != null)
			{
				Delegate[] delegates = s_KeyEventHandler.GetInvocationList();
				foreach (Delegate del in delegates)
				{
					EventHandler EventHandlerDel = (EventHandler)del;

					try
					{
						// This is a static class, therefore null is passed as the object reference
						EventHandlerDel(null, new UniversalKeyEventArgs(key, msg));
					}

					// You can add some meaningful code to this catch block.
					catch {};
				}
			}
		}

		// An instance of this class is passed when Keyboard events 
		// are fired by the KeyboardListener.
		public class UniversalKeyEventArgs : KeyEventArgs
		{

			public readonly bool KeyUp;
			public readonly bool KeyDown;
			public readonly ushort Key;

			public UniversalKeyEventArgs(ushort aKey, uint aMsg)
				: base((Keys)aKey)
			{
				if (aMsg == 257)
				{
					KeyUp = true;
					KeyDown = false;
				}

				else if (aMsg == 256)
				{
					KeyUp = false;
					KeyDown = true;
				}

				Key = aKey;
			}
		}

		// For every application thread that is interested in keyboard events
		// an EventHandler can be added to this variable
		public static event EventHandler s_KeyEventHandler;

		// This is the constructor. Honestly, there needs to be a
		// constructor keyword.
		static KeyboardListener()
		{
			ListeningWindow.KeyDelegate aKeyDelegate = new ListeningWindow.KeyDelegate(KeyHandler);

			// Creates a window ... of sorts.
			s_Listener = new ListeningWindow(aKeyDelegate);
		}

		// A ListeningWindow object is a Window that intercepts Keyboard events.
		private class ListeningWindow : NativeWindow
		{
			public delegate void KeyDelegate(ushort key, uint msg);

			private const int
				WS_CLIPCHILDREN = 0x02000000,
				WM_INPUT = 0x00FF,
				RIDEV_INPUTSINK = 0x00000100,
				RID_INPUT = 0x10000003,
				RIM_TYPEKEYBOARD = 1;

			private uint m_PrevMessage = 0;
			private ushort m_PrevControlKey = 0;
			private KeyDelegate m_KeyHandler = null;

			internal unsafe struct RAWINPUTDEV
			{
				public ushort usUsagePage;
				public ushort usUsage;
				public uint dwFlags;
				public void* hwndTarget;
			};

			internal unsafe struct RAWINPUTHEADER
			{
				public uint dwType;
				public uint dwSize;
				public void* hDevice;
				public void* wParam;
			};

			internal unsafe struct RAWINPUTHKEYBOARD
			{
				public RAWINPUTHEADER header;
				public ushort MakeCode;
				public ushort Flags;
				public ushort Reserved;
				public ushort VKey;
				public uint Message;
				public uint ExtraInformation;

			};

			public ListeningWindow(KeyDelegate keyHandlerFunction)
			{
				m_KeyHandler = keyHandlerFunction;

				// Think of a stripped down form control.
				// From System.Windows.Forms namespace.
				CreateParams HiddenControls = new CreateParams();

				// Fill in the CreateParams details.
				HiddenControls.Caption = "Hidden window";
				HiddenControls.ClassName = null;
				HiddenControls.X = 0x7FFFFFFF;
				HiddenControls.Y = 0x7FFFFFFF;
				HiddenControls.Height = 0;
				HiddenControls.Width = 0;
				HiddenControls.Parent = Globule.ParentHandle;
				HiddenControls.Style = WS_CLIPCHILDREN;

				// Create the actual invisible window
				this.CreateHandle(HiddenControls);

				unsafe
				{
					RAWINPUTDEV myRawDevice = new RAWINPUTDEV();
					myRawDevice.usUsagePage = 0x01;
					myRawDevice.usUsage = 0x06;
					myRawDevice.dwFlags = RIDEV_INPUTSINK;
					myRawDevice.hwndTarget = this.Handle.ToPointer();

					// Register the hidden window as a receiever of
					// input device messages? See MSDN.
					if (RegisterRawInputDevices(&myRawDevice, 1, 
						(uint)sizeof(RAWINPUTDEV)) == false)
					{
						int err = Marshal.GetLastWin32Error();
						throw new Win32Exception(err, 
							"ListeningWindow::RegisterRawInputDevices");
					}
				}
			}


			protected override void WndProc(ref Message m)
			{
				if (m.Msg == WM_INPUT)
				{
					try
					{
						unsafe
						{
							uint dwSize, receivedBytes;
							uint sizeof_RAWINPUTHEADER = (uint)(sizeof(RAWINPUTHEADER));

							// Find out the size of the buffer we have to provide
							int res = GetRawInputData(m.LParam.ToPointer(),
								RID_INPUT, null, &dwSize, sizeof_RAWINPUTHEADER);

							if (res == 0)
							{
								// Allocate a buffer and ...
								byte* lpb = stackalloc byte[(int)dwSize];

								// ... get the data
								receivedBytes = (uint)GetRawInputData((RAWINPUTHKEYBOARD*)(m.LParam.ToPointer()),
									RID_INPUT, lpb, &dwSize, sizeof_RAWINPUTHEADER);
								if (receivedBytes == dwSize)
								{
									RAWINPUTHKEYBOARD* keybData = (RAWINPUTHKEYBOARD*)lpb;

									// Finally, analyze the data
									if (keybData->header.dwType == RIM_TYPEKEYBOARD)
									{
										/*if ((m_PrevControlKey != keybData->VKey) ||
											(m_PrevMessage != keybData->Message))
										{*/
											m_PrevControlKey = keybData->VKey;
											m_PrevMessage = keybData->Message;

											if (keybData->Message == 257)
												System.Windows.Forms.MessageBox.Show(keybData->VKey.ToString());

											// Call the delegate in case data satisfies
											m_KeyHandler(keybData->VKey, keybData->Message);
										// }
									}
								}
								else
								{
									string errMsg = string.Format("WndProc::GetRawInputData " +
										"(2) received {0} bytes while expected {1} bytes",
										receivedBytes, dwSize);
									throw new Exception(errMsg);
								}
							}
							else
							{
								string errMsg = string.Format("WndProc::GetRawInputData " +
									"(1) returned non zero value ({0})", res);
								throw new Exception(errMsg);
							}
						}
					}

					catch { throw; }
				}

				// In case you forget this you will run into problems
				base.WndProc(ref m);
			}

			// In case you want to have a comprehensive overview of calling conventions follow the next link:
			// http://www.codeproject.com/cpp/calling_conventions_demystified.asp

			[DllImport("User32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern unsafe bool RegisterRawInputDevices(
				RAWINPUTDEV* rawInputDevices,
				uint numDevices, 
				uint size
			);

			[DllImport("User32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.I4)]
			internal static extern unsafe int GetRawInputData(
				void* hRawInput,
				uint uiCommand,
				byte* pData,
				uint* pcbSize,
				uint cbSizeHeader
			);
		}
	}
}