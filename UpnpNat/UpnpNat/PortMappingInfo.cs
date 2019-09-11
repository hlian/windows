using System.Net;

namespace NatTraversal {
	public class PortMappingInfo {

		private bool enabled;
		private string description;
		private string internalHostName;
		private int internalPort;
		private IPAddress externalIPAddress;
		private int externalPort;
		private string protocol;

		public PortMappingInfo(string description, string protocol, string internalHostName, int internalPort, IPAddress externalIPAddress, int externalPort, bool enabled) {
			// Initializes fields
			this.enabled = enabled;
			this.description = description;
			this.internalHostName = internalHostName;
			this.internalPort = internalPort;
			this.externalIPAddress = externalIPAddress;
			this.externalPort = externalPort;
			this.protocol = protocol;
		}

		public string InternalHostName {
			get {
				return internalHostName;
			}
		}

		public int InternalPort {
			get {
				return internalPort;
			}
		}

		public IPAddress ExternalIPAddress {
			get {
				return externalIPAddress;
			}
		}

		public int ExternalPort {
			get {
				return externalPort;
			}
		}

		public string Protocol {
			get {
				return protocol;
			}
		}

		public bool Enabled {
			get {
				return enabled;
			}
		}

		public string Description {
			get {
				return description;
			}
		}

	}
}