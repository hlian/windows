using System;
using System.Runtime.InteropServices;

namespace Native.WindowsFirewall {
	[ComImport, ComVisible(false), Guid("304CE942-6E39-40D8-943A-B913C40C9CD4")]
	public class FirewallManager { }

	[ComImport, ComVisible(false), Guid("F7898AF5-CAC4-4632-A2EC-DA06E5111AF2"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallManager {
		IFirewallPolicy LocalPolicy { get;}
		FirewallProfileType CurrentProfileType { get;}
		void RestoreDefaults();
		void IsPortAllowed(string imageFileName,
		 IPVersion ipVersion,
		 long portNumber,
		 string localAddress,
		 IPProtocol ipProtocol,
		 [Out] out bool allowed,
		 [Out] out bool restricted);

		void IsIcmpTypeAllowed(IPVersion ipVersion,
		  string localAddress,
		  byte type,
		  [Out] out bool allowed,
		  [Out] out bool restricted);
	}

	[ComImport, ComVisible(false), Guid("D46D2478-9AC9-4008-9DC7-5563CE5536CC"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallPolicy {
		IFirewallProfile CurrentProfile { get;}
		IFirewallProfile GetProfileByType(FirewallProfileType profileType);
	}

	[ComImport, ComVisible(false), Guid("174A0DDA-E9F9-449D-993B-21AB667CA456"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallProfile {
		FirewallProfileType Type { get;}
		bool FirewallEnabled { get;set;}
		bool ExceptionsNotAllowed { get;set;}
		bool NotificationsDisabled { get;set;}
		bool UnicastResponsesToMulticastBroadcastDisabled { get;set;}
		IFirewallRemoteAdminSettings RemoteAdminSettings { get;}
		IFirewallIcmpSettings IcmpSettings { get;}
		IFirewallOpenPorts GloballyOpenPorts { get;}
		IFirewallServices Services { get;}
		IFirewallAllowedApplications AuthorizedApplications { get;}
	}

	[ComImport, ComVisible(false), Guid("D4BECDDF-6F73-4A83-B832-9C66874CD20E"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallRemoteAdminSettings {
		IPVersion IpVersion { get;set;}
		Scope Scope { get;set;}
		string RemoteAddresses { get;set;}
		bool Enabled { get;set;}
	}

	[ComImport, ComVisible(false), Guid("A6207B2E-7CDD-426A-951E-5E1CBC5AFEAD"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallIcmpSettings {
		bool AllowOutboundDestinationUnreachable { get;set;}
		bool AllowRedirect { get;set;}
		bool AllowInboundEchoRequest { get;set;}
		bool AllowOutboundTimeExceeded { get;set;}
		bool AllowOutboundParameterProblem { get;set;}
		bool AllowOutboundSourceQuench { get;set;}
		bool AllowInboundRouterRequest { get;set;}
		bool AllowInboundTimestampRequest { get;set;}
		bool AllowInboundMaskRequest { get;set;}
		bool AllowOutboundPacketTooBig { get;set;}

	}

	[ComImport, ComVisible(false), Guid("C0E9D7FA-E07E-430A-B19A-090CE82D92E2"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallOpenPorts {
		long Count { get;}
		void Add(IFirewallOpenPort port);
		void Remove(long portNumber, IPProtocol ipProtocol);
		IFirewallOpenPort Item(long portNumber, IPProtocol ipProtocol);
		System.Collections.IEnumerator _NewEnum { get;}
	}

	[ComImport, ComVisible(false), Guid("E0483BA0-47FF-4D9C-A6D6-7741D0B195F7"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallOpenPort {
		string Name { get;set;}
		IPVersion IpVersion { get;set;}
		IPProtocol Protocol { get;set;}
		long Port { get;set;}
		Scope Scope { get;set;}
		string RemoteAddresses { get;set;}
		bool Enabled { get;set;}
		bool BuiltIn { get;}
	}

	[ComImport, ComVisible(false), Guid("79649BB4-903E-421B-94C9-79848E79F6EE"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallServices {
		long Count { get;}
		IFirewallService Item(ServiceType svcType);
		System.Collections.IEnumerator _NewEnum { get;}
	}

	[ComImport, ComVisible(false), Guid("79FD57C8-908E-4A36-9888-D5B3F0A444CF"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallService {
		string Name { get;}
		ServiceType Type { get;}
		bool Customized { get;}
		IPVersion IpVersion { get;set;}
		Scope Scope { get;set;}
		string RemoteAddresses { get;set;}
		bool Enabled { get;set;}
		IFirewallOpenPorts GloballyOpenPorts { get;}

	}

	[ComImport, ComVisible(false), Guid("644EFD52-CCF9-486C-97A2-39F352570B30"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallAllowedApplications {
		long Count { get;}
		void Add(IFirewallAllowedApplication port);
		void Remove(string imageFileName);
		IFirewallAllowedApplication Item(string imageFileName);
		System.Collections.IEnumerator _NewEnum { get;}
	}

	[ComImport, ComVisible(false), Guid("EC9846B3-2762-4A6B-A214-6ACB603462D2")]
	public class FirewallAllowedApplication { }

	[ComImport, ComVisible(false), Guid("B5E64FFA-C2C5-444E-A301-FB5E00018050"), System.Runtime.InteropServices.InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IFirewallAllowedApplication {
		string Name { get;set;}
		string ProcessImageFileName { get;set;}
		IPVersion IpVersion { get;set;}
		Scope Scope { get;set;}
		string RemoteAddresses { get;set;}
		bool Enabled { get;set;}
	}

	public enum FirewallProfileType { Domain = 0, Standard = 1, Current = 2, Max = 3 }
	public enum IPVersion { IPv4 = 0, IPv6 = 1, IPAny = 2, IPMax = 3 }
	public enum IPProtocol { Tcp = 6, Udp = 17 }
	public enum Scope { All = 0, Subnet = 1, Custom = 2, Max = 3 }
	public enum ServiceType { FileAndPrint = 0, UPnP = 1, RemoteDesktop = 2, None = 3, Max = 4 }
}