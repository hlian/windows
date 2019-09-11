using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace Sock {
	class MainForm : Form, IMainForm {
		[STAThread]
		static public void Main() {
			Application.Run(new MainForm());
		}

		static Type PluginLoadableAttributeType;
		static MainForm() {
			PluginLoadableAttributeType = typeof(PluginLoadableAttribute);
		}

		public MainForm() {
			SetUp();
		}

		TextBox fileNameBox;
		FileButton fileButton;
		Button loadButton;
		Panel filePanel;
		Label pluginLabel;
		void SetUp() {
			Width = 500;
			Height = 500;
			KeyPreview = true;
			SuspendLayout();

			filePanel = new Panel();
			filePanel.Dock = DockStyle.Top;
			

			fileNameBox = new TextBox();
			fileNameBox.Multiline = false;

			fileButton = new FileButton();
			fileButton.FoundFile += delegate(string s) {
				fileNameBox.Text = s;
			};
			fileButton.AutoSize = true;
			fileButton.Text = "Browse...";

			loadButton = new Button();
			loadButton.AutoSize = true;
			loadButton.Text = "Load";
			loadButton.Dock = DockStyle.Right;
			loadButton.Click += new EventHandler(loadButton_Click);

			//fileButton.Dock = DockStyle.Right;
			fileNameBox.Dock = DockStyle.Fill;
			fileButton.Dock = DockStyle.Right;
			filePanel.Height = fileNameBox.Height;

			filePanel.Controls.AddRange(new Control[] { fileNameBox, fileButton, loadButton });

			this.Padding = new Padding(5);

			pluginLabel = new Label();
			pluginLabel.Height = 100;
			pluginLabel.Width = 100;
			pluginLabel.Text = "Hello!";
			pluginLabel.Dock = DockStyle.Bottom;
			
			Controls.Add(filePanel);
			Controls.Add(pluginLabel);
			ResumeLayout();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			if (null != p) p.End();
			base.OnClosing(e);
		}

		IPlugin p;
		void loadButton_Click(object sender, EventArgs e) {
			if (System.IO.File.Exists(fileNameBox.Text) == false) {
				Alert("File does not exist.");
				return;
			}

			Assembly a = Assembly.LoadFile(fileNameBox.Text);
			PluginLoadableAttribute x;
			try {
				x = (PluginLoadableAttribute)
					a.GetCustomAttributes(PluginLoadableAttributeType, false)[0];
            Type t;
			}
			catch (MissingMethodException) {
				Alert("Could not find an attribute that matches PluginLoadableAttribute.\n" +
					"Check that arguments to PluginLoadableAttribute matches its parameters.");
				return;
			}
			pluginLabel.Text = x.PluginName;
			p = (IPlugin) a.CreateInstance(x.TypeName);
			p.Init(this);
		}

		public static void Alert(object o) {
			MessageBox.Show(o.ToString());
		}

		public Button GetFileButton() { return fileButton; }

		public Form GetForm() { return this; }
	}
}