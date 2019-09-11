using System;

// Used for the Path class.
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WMPLib;

[assembly: System.Reflection.AssemblyTitle("Globule")]
[assembly: System.Reflection.AssemblyVersion("0.1.0.0")]
[assembly: System.Reflection.AssemblyDescription("Windows Media Player plug-in that adds global hotkey support")]


// Guid is required for COM stuff.
[ComVisible(true)]
[Guid("D6CF1326-11A6-4c34-AA48-F48424B19BA7")]
public class Globule : IWMPPluginUI
{
	private static readonly string ClsidStr =
		typeof(Globule).GUID.ToString("B");

	public static readonly string RegPath = 
		Path.Combine(Registration.InstallationRegKey, ClsidStr);

	public static readonly string ComPath =
		Path.Combine(@"CLSID\" + ClsidStr, "InprocServer32");

	// A soon-to-be IWMPCore object.
	// PassedCore is also an IWMPCore object. 
	public static IWMPCore Core;
	public void SetCore(IWMPCore PassedCore)
	{
		lock (this)
		{
			Core = (PassedCore == null) ? null : PassedCore;
		}
	}

	// Created outside method scope because we use it
	// later on in Destroy(). (Only if you're
	// going to popup a form.)
	//		System.Windows.Forms.Form UponCreationForm;

	ListeningWindow listeningWindow;
	public IntPtr Create(IntPtr passedParentHandle)
	{
		// Create form, return handle, and set handle 
		// to WMP so WMP will display it. If you
		// return the parent window handle, no form
		// will pop up.

		//			UponCreationForm = new Form();
		//			SetParent(UponCreationForm.Handle, ParentWindowHandle);
		//			return UponCreationForm.Handle;

		listeningWindow = new ListeningWindow();
		listeningWindow.Initialize();

		NativeMethods.RegisterHotKey(listeningWindow.Handle, 50,
			NativeMethods.MOD_ALT | NativeMethods.MOD_SHIFT, 0x50);
		NativeMethods.SetParent(listeningWindow.Handle, passedParentHandle);
		return passedParentHandle;
	}

	public void Destroy()
	{
		NativeMethods.UnregisterHotKey(listeningWindow.Handle, 50);
		listeningWindow.Dispose();
	}

	// See the SDK documentation for more information
	// about the next few methods, which are required
	// by the interface we made earlier.

	// Do whatever you want with these methods. This is
	// the "plugin" part of the plugin.
	public void DisplayPropertyPage(IntPtr parentWindowHandle)
	{
		MessageBox.Show("This plugin has no property page yet.");
	}

	public object GetProperty(string propertyName)
	{
		return null;
	}

	// Generics used thanks to the magic of FxCop
	public void SetProperty<T>(string propertyName, [In] ref T propery)
	{
		// Globule.AlertBox(typeof(T).ToString());
	}

	public void TranslateAccelerator(IntPtr message)
	{
	}

	public static void AlertBox(string message)
	{
		System.Windows.Forms.MessageBox.Show(message);
	}
}