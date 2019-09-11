#define TEST

// TODO: Extend |FirewallManager|.
using System;
using Native.WindowsFirewall;

public class App {
	public static void Test() {
		IFirewallManager mgr = (IFirewallManager) new FirewallManager();
		Console.WriteLine("CurrentProfileType: " + mgr.CurrentProfileType);

		IFirewallProfile profile = mgr.LocalPolicy.CurrentProfile;
		Console.WriteLine("FirewallEnabled: " + profile.FirewallEnabled);

		/****************************************************/
		//IFirewallAllowedApplication newApp = (IFirewallAllowedApplication) new FirewallAllowedApplication();
		//   newApp.Enabled = true;
		//   newApp.Name = "FooTestApp";
		//   newApp.Scope = Scope.Subnet;
		//   newApp.ProcessImageFileName = "C:\\Documents and Settings\\Family\\my documents\\hao\\program Files\\colorPic.exe";
		//profile.AuthorizedApplications.Add(newApp);

		profile.AuthorizedApplications.Remove("C:\\Documents and Settings\\Family\\my documents\\hao\\program Files\\colorPic.exe");

		/****************************************************/
		System.Collections.IEnumerator e = profile.AuthorizedApplications._NewEnum;
		Console.WriteLine("\r\n-----  Applications  -----  ");
		while (e.MoveNext()) {
			IFirewallAllowedApplication app = e.Current as IFirewallAllowedApplication;
			Console.WriteLine("\t{0}\r\n\t\tImageFileName={1}\r\n\t\tEnabled={2}\r\n\t\tIpVersion={3}\r\n\t\tScope={4}\r\n\t\tRemoteAddresses={5}",
					app.Name, app.ProcessImageFileName, app.Enabled, app.IpVersion,
					app.Scope, app.RemoteAddresses);
		}

		/****************************************************/
		e = profile.Services._NewEnum;
		Console.WriteLine("\r\n-----  Services  -----  ");
		while (e.MoveNext()) {
			IFirewallService service = e.Current as IFirewallService;
			Console.WriteLine("\t{0}\r\n\t\tType={1}\r\n\t\tEnabled={2}\r\n\t\tIpVersion={3}" +
					"\r\n\t\tScope={4}\r\n\t\tCustomized={5}\r\n\t\tRemoteAddresses={6}",
					service.Name, service.Type, service.Enabled, service.IpVersion,
					service.Scope, service.Customized, service.RemoteAddresses);
			System.Collections.IEnumerator f = service.GloballyOpenPorts._NewEnum;
			while (f.MoveNext()) {
				IFirewallOpenPort x = f.Current as IFirewallOpenPort;
				Console.WriteLine("\tPortage: [Name: {0}] [Protocol: {1}] [Port: {2}]",
					x.Name, x.Protocol, x.Port);
			}
		}

		e = profile.GloballyOpenPorts._NewEnum;
		Console.WriteLine("\r\n-----  Globally open ports  -----  ");
		while (e.MoveNext()) {
			IFirewallOpenPort port = e.Current as IFirewallOpenPort;
			Console.WriteLine("\t{0}\r\n\t\tIsBuiltIn={1}\r\n\t\tEnabled={2}\r\n\t\tIpVersion={3}" +
					"\r\n\t\tScope={4}\r\n\t\tProtocol={5}\r\n\t\tRemoteAddresses={6}\r\n\t\tPort={7}",
					port.Name, port.BuiltIn, port.Enabled, port.IpVersion,
					port.Scope, port.Protocol, port.RemoteAddresses, port.Port
					);
		}


		Console.ReadLine();
	} // Test()

#if TEST
	public static void Main() {
		App.Test();
	}
#endif
}