using System;
using System.Windows.Forms;
using System.Timers;

// DLLImport
using System.Runtime.InteropServices;
using System.Diagnostics;
using WMPLib;


// A ListeningWindow object is a Window that intercepts Keyboard events.
public class ListeningWindow : NativeWindow, IDisposable
{
	System.Timers.Timer MonitorTimer;
	public IntPtr cachedHandle;
	ChangeVolumeForm changeVolumeForm;
	public ListeningWindow()
	{
		System.Threading.ThreadPool.QueueUserWorkItem(
			new System.Threading.WaitCallback(delegate
		{
			cachedHandle =
				Process.GetProcessesByName("wmplayer")[0].MainWindowHandle;
		}));
	}

	public void Initialize()
	{
		changeVolumeForm = new ChangeVolumeForm();
		MonitorTimer = new System.Timers.Timer(2000);

		MonitorTimer.AutoReset = true;
		MonitorTimer.Elapsed += new ElapsedEventHandler(delegate
		{
			MonitorTimer.Stop();
			NativeMethods.PostMessage(this.Handle, NativeMethods.WM_APP, IntPtr.Zero, IntPtr.Zero);
		});

		// Details for this window.
		CreateParams cp = new CreateParams();
		cp.Caption = "Hidden window for Globule plugin";
		cp.X = 0x7FFFFFFF;
		cp.Y = 0x7FFFFFFF;
		cp.Height = 0;
		cp.Width = 0;
		cp.Style = NativeMethods.WS_CLIPCHILDREN;
		base.CreateHandle(cp);
	}

	bool waitingForCloseDialog;
	protected override void WndProc(ref Message WndProcMessage)
	{
		if (WndProcMessage.Msg == NativeMethods.WM_HOTKEY)
		{
			if (WndProcMessage.WParam == (IntPtr)50)
			{
				if (MonitorTimer.Enabled == false) RegisterAllHotKeys();
				else MonitorTimer.Stop();

				// This happens either way.
				MonitorTimer.Start();
			}

			else if (MonitorTimer.Enabled)
			{
				switch (WndProcMessage.WParam.ToInt32())
				{
					// P
					case 0:
						/* 
						 * 0: Undefined						 * 1: Stopped
						 * 2: Paused						 * 3: Playing
						 * 4: Fast forwarding				 * 5: Fast rewinding
						 * 6: Buffering						 * 7: Waiting
						 * 8: Completed playback			 * 9: Preparing next item
						 * 10: Ready						 * 11: Reconnecting
						 */
						if ((int)(Globule.Core.playState) == 1 ||
							(int)(Globule.Core.playState) == 2 ||
							(int)(Globule.Core.playState) == 8 ||
							(int)(Globule.Core.playState) == 10)
						{ Globule.Core.controls.play(); }

						else { Globule.Core.controls.pause(); }
						break;

					// K
					case 1: Globule.Core.controls.previous(); break;
					// L
					case 2: Globule.Core.controls.next(); break;

					// V
					case 3:
						// Need to unregister so that the volume form hotkeys can work.
						UnregisterAllHotKeys();

						// MyChangeVolumeForm.ShowDialog(new Win32Window(GetDesktopWindow()));
						changeVolumeForm.UpdateVolume();
						changeVolumeForm.Show();
						break;

					// X
					case 4:
						if (waitingForCloseDialog) break;
						waitingForCloseDialog = true;
						UnregisterAllHotKeys();
						if (MessageBox.Show(new Win32Window(NativeMethods.GetForegroundWindow()),
							"Are you sure you want to close Windows Media Player?",
							"Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							IntPtr wmpHandle1 = Process.GetProcessesByName("wmplayer")[0].MainWindowHandle;
							if (wmpHandle1 == IntPtr.Zero && cachedHandle == IntPtr.Zero)
							{
								MessageBox.Show(new Win32Window(NativeMethods.GetForegroundWindow()),
									"Sorry, Globule is not able to detect Windows Media Player, and it cannot minimize WMP.");
								break;
							}

							else if (wmpHandle1 == IntPtr.Zero) wmpHandle1 = cachedHandle;
							
							NativeMethods.ShowWindow(wmpHandle1, 7);
							Dispose();
							Process.GetProcessesByName("wmplayer")[0].CloseMainWindow();
						}
						waitingForCloseDialog = false;
						break;

					// A
					case 5:
						IntPtr wmpHandle2;
						if (cachedHandle == IntPtr.Zero)
						{
							wmpHandle2 = Process.GetProcessesByName("wmplayer")[0].MainWindowHandle;
							if (wmpHandle2 == IntPtr.Zero)
							{
								MessageBox.Show(new Win32Window(NativeMethods.GetForegroundWindow()),
									"Sorry, Globule is not able to detect Windows Media Player, and it cannot minimize WMP.");
								break;
							}
							else cachedHandle = wmpHandle2;
						}

						NativeMethods.ShowWindow(cachedHandle, 6);
						break;

					// S
					case 6:
						IntPtr wmpHandle3;
						if (cachedHandle == IntPtr.Zero)
						{
							wmpHandle3 = Process.GetProcessesByName("wmplayer")[0].MainWindowHandle;
							if (wmpHandle3 == IntPtr.Zero)
							{
								MessageBox.Show(new Win32Window(NativeMethods.GetForegroundWindow()),
									"Sorry, Globule is not able to detect Windows Media Player, and it cannot maximize WMP.");
								break;
							}
							else cachedHandle = wmpHandle3;
						}

						NativeMethods.ShowWindow(cachedHandle, 3);
						NativeMethods.SetForegroundWindow(cachedHandle);
						break;
				}

				MonitorTimer.Stop();
				UnregisterAllHotKeys();
			}
		}

		// Upon the monitoring thread being aborted.
		else if (WndProcMessage.Msg == NativeMethods.WM_APP)
		{
			UnregisterAllHotKeys();
		}

		// In case you forget this you will suffer a horrible death
		base.WndProc(ref WndProcMessage);
	}

	readonly uint[] VirtualKeyCodeArray = new uint[] {
			0x50, // P (Play/Pause)
			0x4B, // K (Previous)
			0x4C, // L (Next)
			0x56, // V (Volume)
			0x58, // X (Close)
			0x41, // A (Minimize)
			0x53 // S (Maximize)
		};

	void RegisterAllHotKeys()
	{
		int i = 0;
		foreach (uint VirtualKeyCode in VirtualKeyCodeArray)
		{
			NativeMethods.RegisterHotKey(this.Handle, i++, 0x0, VirtualKeyCode);
		}
	}

	void UnregisterAllHotKeys()
	{
		int VirtualKeyCodeArrayLength = VirtualKeyCodeArray.Length;
		for (int i = 0; i < VirtualKeyCodeArrayLength; i++)
		{
			NativeMethods.UnregisterHotKey(this.Handle, i);
		}
	}

	public void Dispose()
	{
		MonitorTimer.Close();
		UnregisterAllHotKeys();
		changeVolumeForm.Dispose();
		changeVolumeForm.Close();
		base.DestroyHandle();
	}
}

class Win32Window : IWin32Window
{
	IntPtr _Handle;
	public Win32Window(IntPtr PassedHandle)
	{
		_Handle = PassedHandle;
	}

	IntPtr IWin32Window.Handle
	{
		get { return _Handle; }
	}
}