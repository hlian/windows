using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

static class UpNat {
	// http://www.java2s.com/Code/CSharp/Network/Udp-Client.htm
	// http://zbowling.com/projects/upnp/
	// http://svn.sourceforge.net/viewcvs.cgi/gaim/trunk/src/upnp.c
	public static void Main() {
		string response = ""; string address = "";
		UpNat.Discover(ref response, ref address);
		Console.WriteLine("Received from {0}: {1}", address, response);
		Console.ReadLine();
	}

	static void Discover(ref string response, ref string actualAddress) {
		byte[] ssdpRequest = Encoding.ASCII.GetBytes("M-SEARCH * HTTP/1.1\r\n" +
			"MX: 2\r\n" + "HOST: 239.255.255.250:1900\r\n" +
			"MAN: \"ssdp:discover\"\r\n" +
			"ST: urn:schemas-upnp-org:service:WANIPConnection:1\r\n" +
			"\r\n");
		IPAddress ip = IPAddress.Parse("239.255.255.250");
		int sendPort = 1900; int receivePort = 4900;

		// This is the port (4900) the router will send back the data.
		// From Ethereal, it looks like, otherwise, CLI chooses a random port for you.
		// That is /bad/ because that means you have to listen to every port later on
		// to receive the data, which I assume is a huge security hole.
		UdpClient sock = new UdpClient(receivePort);
		IPEndPoint iep = new IPEndPoint(ip, sendPort);
		sock.Send(ssdpRequest, ssdpRequest.Length, iep);

		sock.JoinMulticastGroup(ip, 50); // That's TTL, not port.
		iep = new IPEndPoint(IPAddress.Any, receivePort);

		byte[] data = { }; int tries = 0;
		while (data.Length == 0) {
			data = sock.Receive(ref iep); tries++;
			System.Threading.Thread.Sleep(1000);
			if (tries >= 5) throw new DiscoverHttpResponseException();
		}
		sock.Close();

		// I'm guessing sock.Receive or the CLI overwrites IPAddress.Any with the actual
		// data. Anyway, that's how we get the real IP address.
		response = Encoding.ASCII.GetString(data);
		actualAddress = iep.ToString();
	}

	public class DiscoverHttpResponseException : ApplicationException { }
}