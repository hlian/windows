using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using WMPLib;

namespace Globule
{
	// A ListeningWindow object is a Window that intercepts Keyboard events.
	class ListeningWindow : NativeWindow
	{
		public delegate void OnHotKeyDelegate(bool ThreadWanted);
		private OnHotKeyDelegate PassedHotKeyHandler;
		
		private const int 
			WM_HOTKEY = 0x0312,
			WS_CLIPCHILDREN = 0x02000000;
		public bool HotkeyPActivated = false;

		public const int
			MOD_ALT = 0x0001,
			MOD_CONTROL = 0x0002,
			MOD_SHIFT = 0x0004,
			MOD_WIN = 0x0008;

		public ListeningWindow(OnHotKeyDelegate OnHotKeyHandler)
		{
			PassedHotKeyHandler = OnHotKeyHandler;

			// Think of a stripped down form control.
			// From System.Windows.Forms namespace.
			CreateParams HiddenControl = new CreateParams();

			// Fill in the CreateParams details.
			HiddenControl.Caption = "Hidden window";
			HiddenControl.ClassName = null;
			HiddenControl.X = 0x7FFFFFFF;
			HiddenControl.Y = 0x7FFFFFFF;
			HiddenControl.Height = 0;
			HiddenControl.Width = 0;
			HiddenControl.Parent = Globule.ParentHandle;
			HiddenControl.Style = WS_CLIPCHILDREN;

			// Create the actual invisible window
			this.CreateHandle(HiddenControl);
		}

		/*[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		static extern bool PostMessage(IntPtr WindowHandle, int Message,
			int WParam, int LParam);*/

		protected override void WndProc(ref Message WndProcMessage)
		{
			if (WndProcMessage.Msg == WM_HOTKEY)
			{
				MessageBox.Show(WndProcMessage.WParam.ToString());
				// MessageBox.Show(WndProcMessage.ToString());
				if (WndProcMessage.WParam.ToInt32() == 265 && HotkeyPActivated == false)
				{
					PassedHotKeyHandler(true);
				}

				else if (HotkeyPActivated == true)
				{
					MessageBox.Show("P.");
					switch (WndProcMessage.WParam.ToInt32())
					{
						// P
						case 0:
							/* 
							 * 0: Undefined
							 * 1: Stopped
							 * 2: Paused
							 * 3: Playing
							 * 4: Fast forwarding
							 * 5: Fast rewinding
							 * 6: Buffering
							 * 7: Waiting
							 * 8: Completed playback
							 * 9: Preparing next item
							 * 10: Ready
							 * 11: Reconnecting
							 */
							if ((int)(Globule.MyCore.playState) == 1 || 
								(int)(Globule.MyCore.playState) == 2 || 
								(int)(Globule.MyCore.playState) == 8 ||
								(int)(Globule.MyCore.playState) == 10)
							{
								Globule.MyCore.controls.play();
							}

							else
							{
								Globule.MyCore.controls.pause();
							}
							break;

						// K
						case 1:
							Globule.MyCore.controls.previous();
							break;

						// L
						case 2:
							Globule.MyCore.controls.next();
							break;
					}

					HotkeyPActivated = false;
				}

				//	WndProcMessage.WParam.ToInt32() - 185, 0x10);
				/* PostMessage(GetForegroundWindow(), WM_KEYDOWN, 0x50, 0);
				PostMessage(GetForegroundWindow(), WM_KEYUP, 0x50, 0); */

				// MessageBox.Show(WndProcMessage.ToString());
			}
			
			// In case you forget this you will run into problems
			base.WndProc(ref WndProcMessage);
		}
	}
}