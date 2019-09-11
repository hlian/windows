using System;

// Used for the Path class.
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// See WMPPlugins.cs
using WMPPlugins;

namespace MyPlugin
{
	// Guid is required for COM stuff.
    [ComVisible(true)]
    [Guid("D6CF1326-11A6-4c34-AA48-F48424B19BA7")]
    public class MyPlugin : IWMPPluginUI
    {
        public static string RegPath { 
			get {return Path.Combine(Registration.InstallationRegKey, ClsidStr);} 
		}
        public static string ComPath { 
			get {return Path.Combine(@"CLSID\" + ClsidStr, "InprocServer32");} 
		}
        private static string ClsidStr { 
			get {return typeof(MyPlugin).GUID.ToString("B");} 
		}

        // A soon-to-be IWMPCore object.
		// PassedCore is also an IWMPCore object. 
		private object MyCore;
		public void SetCore([MarshalAs(UnmanagedType.IDispatch)] object PassedCore)
        {
			MyCore = PassedCore;
        }

		// Created outside method scope because we use it
		// later on in Destroy(). (Only if you're
		// going to popup a form.)
//		System.Windows.Forms.Form UponCreationForm;

		[DllImport("user32.dll")]
		private static extern IntPtr SetParent(IntPtr ChildWindowHandle, IntPtr ParentWindowHandle);
        public IntPtr Create(IntPtr ParentWindowHandle)
        {
			// Create form, return handle, and set handle 
			// to WMP so WMP will display it. If you
			// return the parent window handle, no form
			// will pop up.

//			UponCreationForm = new Form();
//			SetParent(UponCreationForm.Handle, ParentWindowHandle);
//			return UponCreationForm.Handle;

			// Since we have no form handle created,
			// we return the parent handle for fun and
			// glee.
			return ParentWindowHandle;
        }

        public void Destroy()
        {
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