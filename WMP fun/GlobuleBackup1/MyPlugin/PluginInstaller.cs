using System;
using System.ComponentModel;
using System.Configuration.Install;

// Used for StringBuilder().
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

// See WMPPlugins.cs
using WMPPlugins;

namespace Globule
{
	// This class is apparently automatically initiated
	// by the installer (InstUtil?) somewhere.
	[RunInstaller(true)]
	public class MyPluginInstaller : System.Configuration.Install.Installer
	{
		// SearchPath() is used to find a "registered" file. It 
		// returns the full path through [Out] lpBuffer and 
		// also returns the string Length. StringBuilder() is
		// used to make a string (see MSDN on StringBuilder).

		// See also WinAPI documentation on SearchPath.
		[DllImport("kernel32.dll")]
		private static extern int SearchPath(string lpPath,
			string lpFileName, string lpExtension, int nBufferLength,
			[Out] StringBuilder lpBuffer, out IntPtr lpFilePart);
		private static string SearchPath(string FileName)
		{
			IntPtr DummyPointer;
			StringBuilder FullPath = new StringBuilder(256);

			// If you're wondering where FullPath came from,
			// it's an [Out] parameter of the method.
			FullPath.Length = SearchPath(null, FileName, null, 
				256, FullPath, out DummyPointer);
			return FullPath.ToString();
		}

		public override void Install(System.Collections.IDictionary stateSaver)
		{
			base.Install(stateSaver);

			// Sets (Default) to something like C:\Windows\system32\mscoree.dll
			using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(Globule.ComPath, true))
			{
				// "null" corresponds to the (Default) you see in 
				// regedit.exe.

				// The (Default) is naturally set to "mscoree.dll",
				string lib = (string)key.GetValue(null);

				// but we need to retrieve the full path.
				key.SetValue(null, SearchPath(lib));
			}

			// All installing a UI plugin in WMP is
			// just a matter of adding a few registry keys.
			using (RegistryKey key = Registry.LocalMachine.CreateSubKey(Globule.RegPath))
			{
				// Unchecked because it'll automatically go to checked
				// if you don't do it. Not even (uint) will fix it.

				// Add more flags with the bitwise OR (see SDK documentation)
				// to get more flag functions.
				key.SetValue(Registration.Capabilities,
					unchecked((int)PluginType.SeparateWindow | (int)PluginFlags.HasPropertyPage),
					RegistryValueKind.DWord);

				key.SetValue(Registration.FriendlyName, "Globule");
				key.SetValue(Registration.Description,
					"My plugin description.");
			}
		}

		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			base.Uninstall(savedState);

			// Uninstall UI plugin by deleting the registry
			// information.
			Registry.LocalMachine.DeleteSubKey(Globule.RegPath, false);
		}
	}
}