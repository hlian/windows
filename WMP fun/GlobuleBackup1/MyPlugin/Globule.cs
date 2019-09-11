using System;

// Used for the Path class.
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// See WMPPlugins.cs
using WMPPlugins;
using WMPLib;

namespace Globule
{
	// Guid is required for COM stuff.
    [ComVisible(true)]
    [Guid("D6CF1326-11A6-4c34-AA48-F48424B19BA7")]
    public class Globule : IWMPPluginUI
    {
        public static string RegPath { 
			get {return Path.Combine(Registration.InstallationRegKey, ClsidStr);} 
		}
        public static string ComPath { 
			get {return Path.Combine(@"CLSID\" + ClsidStr, "InprocServer32");} 
		}
        private static string ClsidStr { 
			get {return typeof(Globule).GUID.ToString("B");} 
		}

        // A soon-to-be IWMPCore object.
		// PassedCore is also an IWMPCore object.
		public static IWMPCore MyCore;
		public void SetCore(IWMPCore PassedCore)
        {
			if (PassedCore == null)
			{
				MyCore = null;
				return;
			}

			MyCore = PassedCore;
        }

		[DllImport("user32.dll")]
		private static extern IntPtr SetParent(IntPtr ChildWindowHandle, IntPtr ParentWindowHandle);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool RegisterHotKey(IntPtr ReceivingWindowHandle, int HotKeyIdentifier,
			uint KeyModifiers, uint VirtualKeyCode);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr ReceivingWindowHandle, int HotKeyIdentifier);

		// ParentHandle used for assigning parent to the Listener control.
		public static IntPtr ParentHandle;
		ListeningWindow AListeningWindow;

        public IntPtr Create(IntPtr hwndParent)
        {
			//KeyboardListener.s_KeyEventHandler += new EventHandler(Globule.OnUniversalKeyEvent);
			ParentHandle = hwndParent;
			ListeningWindow.OnHotKeyDelegate AHotKeyHandler = new ListeningWindow.OnHotKeyDelegate(MyHotKeyHandlerMethod);
			AListeningWindow = new ListeningWindow(AHotKeyHandler);

			RegisterHotKey(AListeningWindow.Handle, 265, ListeningWindow.MOD_SHIFT |
				ListeningWindow.MOD_ALT, 0x50);

			WaitToUnregisterManualEvent = new System.Threading.ManualResetEvent(false);
				
			// Create form, return handle, and set handle 
			// to WMP so WMP will display it. If you
			// return the parent window handle, no form
			// will pop up.

			return hwndParent;
		}

		System.Threading.Thread WaitForPThread;
		System.Threading.Thread MonitorHotkeyPActivated;

		// Initialize in Create();
		System.Threading.ManualResetEvent WaitToUnregisterManualEvent;

		void MyHotKeyHandlerMethod() {
			MyHotKeyHandlerMethod(false);
		}

		System.Threading.Thread iThread;
		void MyHotKeyHandlerMethod(bool ThreadWanted)
		{
			if (ThreadWanted == true)
			{
				iThread = new System.Threading.Thread(new System.Threading.ThreadStart(MyHotKeyHandlerMethod));
				iThread.Start();
				MessageBox.Show("iThread");
			}

			else
			{
				AListeningWindow.HotkeyPActivated = true;
				WaitForPThread = new System.Threading.Thread(new
					System.Threading.ThreadStart(WaitForPThreadMethod));
				MonitorHotkeyPActivated = new System.Threading.Thread(new
					System.Threading.ThreadStart(MonitorHotkeyPActivatedMethod));

				RegisterAllHotKeys();

				// MessageBox.Show("P arrived.");
				WaitForPThread.Start();
				MonitorHotkeyPActivated.Start();

				WaitToUnregisterManualEvent.Reset();
				WaitToUnregisterManualEvent.WaitOne();
				MessageBox.Show("Reseted.");

				UnregisterAllHotKeys();
				AListeningWindow.HotkeyPActivated = false;
			}
		}


		void WaitForPThreadMethod()
		{
			System.Threading.Thread.Sleep(2000);
			MonitorHotkeyPActivated.Abort();
			WaitToUnregisterManualEvent.Set();
		}

		void MonitorHotkeyPActivatedMethod()
		{
			while (true)
			{
				if (AListeningWindow.HotkeyPActivated == false)
				{
					WaitForPThread.Abort();
					WaitToUnregisterManualEvent.Set();
					break;
				}
			}
		}

		// 0x51 = Q, and so on.
		uint[] VirtualKeyCodeArray = new uint[] {
			0x50, // P (Play/Pause)
			0x4B, // K (Previous)
			0x4C  // L (Next)
		};

		void RegisterAllHotKeys() {
			for (int i = 0; i < VirtualKeyCodeArray.Length; i++)
			{
				RegisterHotKey(AListeningWindow.Handle, i, 0x0, VirtualKeyCodeArray[i]);
				// Find GetLastError
			}
		}

		void UnregisterAllHotKeys()
		{
			for (int i = 0; i < VirtualKeyCodeArray.Length; i++)
			{
				MessageBox.Show(UnregisterHotKey(AListeningWindow.Handle, i).ToString());
			}
		}


        public void Destroy()
        {
			// If you make AListeningWindow null before you
			// unregister, its Handle will be gone too.
			WaitForPThread.Abort();
			MonitorHotkeyPActivated.Abort();
			UnregisterHotKey(AListeningWindow.Handle, 265);
			UnregisterAllHotKeys();

			AListeningWindow = null;
			// Only use if you have a UponCreationForm.
/*			
 *			if (MyControl != null)
 *				MyControl.Dispose();
 *			MyControl = null; 
 */
        }

		// See the SDK documentation for more information
		// about the next few methods, which are required
		// by the interface we made earlier.

		// Do whatever you want with these methods. This is
		// the "plugin" part of the plugin.
        public void DisplayPropertyPage(IntPtr ParentWindowHandle)
        {
            MessageBox.Show("This plugin has no property page.");
        }

        public object GetProperty(string pwszName)
        {
            return null;
        }

        public void SetProperty(string pwszName, [In] ref object pvarProperty)
        {
        }

        public void TranslateAccelerator(IntPtr lpmsg)
        {
        }
    }
}