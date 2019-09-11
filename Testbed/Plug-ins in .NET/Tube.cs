// csc /t:library /o+ /r:System.Windows.Forms.dll /r:Sock.IPlugin.dll Tube.cs

using System;
using System.Drawing;
using System.Windows.Forms;

using Sock;

[assembly: Sock.PluginLoadable("Tube.Amok", "The Amokilator 3000: \n Providing excellence!")]

namespace Tube {
	public class Amok : IPlugin {
		Form form;
		public void Init(IMainForm f) {
			form = f.GetForm();
			form.BackColor = Color.Yellow;
			form.Resize += delegate { form.BackColor = Color.Green; };

			f.GetFileButton().Click += delegate { form.BackColor = Color.Purple; };
		}
		
		public string GetName() {
			return "Amok 2.0";
		}

		public void End() {
			MessageBox.Show("Bye now!");
		}
	}
}
