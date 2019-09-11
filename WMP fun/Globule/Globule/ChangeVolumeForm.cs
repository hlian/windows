using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

class ChangeVolumeForm : Form
{
	Label VolumeLabel;
	int volume;
	public ChangeVolumeForm()
	{
		this.Text = "Globule Volume Changer";
		this.FormBorderStyle = FormBorderStyle.None;
		this.KeyPreview = true;
		this.Location = new Point(0, 0);
		this.KeyDown += new KeyEventHandler(ChangeVolumeForm_KeyDown);

		// Slider thingy.
		VolumeLabel = new Label();
		VolumeLabel.Dock = DockStyle.Top;
		VolumeLabel.Font = new Font("Georgia", 48);

		this.Size = CreateGraphics().MeasureString("100", VolumeLabel.Font, 400).ToSize();
		this.Width += 10;

		Rectangle ScreenBounds = Screen.GetBounds(this.Location);
		this.Left = (int)((ScreenBounds.Width - this.Width) * 0.5);
		this.Top = (int)((ScreenBounds.Height - this.Height) * 0.5);

		VolumeLabel.Size = new Size(this.Width, this.Height - 5);
		volume = Globule.Core.settings.volume;
		VolumeLabel.Text = volume.ToString();
		VolumeLabel.TextAlign = ContentAlignment.MiddleCenter;

		this.Opacity = 0.75;
		this.Controls.Add(VolumeLabel);
		this.TopMost = true;

		NativeMethods.SetForegroundWindow(this.Handle);
	}

	void ChangeVolumeForm_KeyDown(object sender, KeyEventArgs e)
	{
		// MessageBox.Show(e.KeyValue.ToString());
		switch (e.KeyCode)
		{
			case Keys.Enter:
			case Keys.Escape:
				this.Hide();
				break;

			case Keys.U:
				ChangeVolume(-10);
				break;

			case Keys.I:
				ChangeVolume(-1);
				break;

			case Keys.O:
				ChangeVolume(1);
				break;

			case Keys.P:
				ChangeVolume(10);
				break;
		}
	}

	void ChangeVolume(int Change)
	{
		SetVolume(volume + Change);
	}

	public void UpdateVolume()
	{
		SetVolume(Globule.Core.settings.volume);
	}

	void SetVolume(int volume)
	{
		if (volume > -1 && volume < 101)
		{
			this.volume = volume;
			VolumeLabel.Text = volume.ToString();
			Globule.Core.settings.volume = volume;
		}
	}
}