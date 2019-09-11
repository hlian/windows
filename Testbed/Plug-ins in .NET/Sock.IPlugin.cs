// csc /t:library /o+ Sock.IPlugin.cs
using System;
using System.Windows.Forms;

namespace Sock {
	public interface IMainForm {
		Button GetFileButton();
		Form GetForm();
	}

	public interface IPlugin {
		void Init(IMainForm f);
		void End();
		string GetName();
	}

	[AttributeUsage(AttributeTargets.Assembly)]
	public class PluginLoadableAttribute : System.Attribute {
		public readonly string TypeName;
		public readonly string PluginName;

		public PluginLoadableAttribute(string typeName, string pluginName) {
			TypeName = typeName;
			PluginName = pluginName;
		}
	}
}