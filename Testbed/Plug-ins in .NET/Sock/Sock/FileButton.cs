using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sock {
	class FileButton : Button {
		public delegate void FoundFileDel(string s);
		public event FoundFileDel FoundFile;

		protected override void OnClick(EventArgs e) {
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = @".\plugins";
			ofd.RestoreDirectory = true;
			ofd.Filter = "Plug-in files (*.dll)|*.dll|All files (*.*)|*.*";

			if (ofd.ShowDialog() != DialogResult.OK) {
				ofd.FileName = "";
			}
			base.OnClick(e);

			FoundFile(ofd.FileName);
		}
	}
}