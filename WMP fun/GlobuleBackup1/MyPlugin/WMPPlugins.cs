using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WMPLib;

namespace WMPPlugins
{
	// The two following uint enums are from
	// WMP 10's SDK. See SDK.
    [ComVisible(true)]
    public enum PluginType : uint
    {
		// Background flag is passed in this example.
		// See PluginInstaller's Install().
        Background		= 0x1,
        SeparateWindow	= 0x2,
        DisplayArea		= 0x3,
        SettingsArea	= 0x4,
        MetaDataArea	= 0x5
    }

    [ComVisible(true)]
    [Flags]
    public enum PluginFlags : uint
    {
		// HasPropertyPage is passed in this example.
		// See PluginInstaller's Install().
        HasPropertyPage		= 0x80000000,
        AutoRunOnInstall	= 0x40000000,
        LaunchPropertyPage	= 0x20000000,
        AcceptsMedia		= 0x10000000,
        AcceptsPlayLists	= 0x8000000,
        HasPresets			= 0x4000000
    }

	// Duplicating the interface that would normally
	// be present in a C++ WMP SDK environment. The 
	// Guid is required for COM stuff.
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("4C5E8F9F-AD3E-4bf9-9753-FCD30D6D38DD")]
    public interface IWMPPluginUI
    {
        void SetCore(IWMPCore PassedCore);
		IntPtr Create(IntPtr hwndParent);
        void Destroy();
        void DisplayPropertyPage(IntPtr hwndParent);
        object GetProperty(string pwszName);
        void SetProperty(string pwszName, [In] ref object pvarProperty);
        void TranslateAccelerator(IntPtr lpmsg);
    }

    public class Registration
    {
        // A bunch of preset registry stuff. See SDK!
        public const string InstallationRegKey	= @"Software\Microsoft\MediaPlayer\UIPlugins";
        public const string FriendlyName		= "FriendlyName";
        public const string Description			= "Description";
        public const string Capabilities		= "Capabilities";
        public const string UninstallPath		= "UninstallPath";

        public const string MiscPresetCount		= "PresetCount";
        public const string MiscPresetNames		= "PresetNames";
        public const string MiscCurrentPreset	= "CurrentPreset";
		public const string MiscQueryDestroy	= "QueryDestroy";

        public const string SeparateWindowResizablility	= "Resizable";
        public const string SeparateWindowDefaultWidth	= "DefaultWidth";
        public const string SeparateWindowDefaultHeight	= "DefaultHeight";
        public const string SeparateWindowMinWidth		= "MinWidth";
        public const string SeparateWindowMinHeight		= "MinHeight";
        public const string SeparateWindowMaxWidth		= "MaxWidth";
        public const string SeparateWindowMaxHeight		= "MaxHeight";

		public const string AllMediaSentTo		= "MediaSendTo";
        public const string AllPlayListSendTo	= "PlaylistSendTo";
    }
}
