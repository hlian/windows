using System;
using System.Drawing;
using System.Windows.Forms;

public class MainForm : Form
{
	RichTextBox ChatBox;
	Splitter ChatBoxSplitter;
	ListBox Userlist;
	TextBox InputBox;

	Font SystemFont;

	float ChatBoxProportion;

	[System.Runtime.InteropServices.DllImport("gdi32.dll")]
	public static extern System.IntPtr GetStockObject(int fnObject);
	/* #define SYSTEM_FONT      13
	#define DEVICE_DEFAULT_FONT 14
	#define DEFAULT_PALETTE     15
	#define SYSTEM_FIXED_FONT   16
	#define DEFAULT_GUI_FONT    17 */

	// Constructor
	MainForm()
	{
		SystemFont = SystemInformation.MenuFont;
		ChatBox = new RichTextBox();
		ChatBoxSplitter = new Splitter();
		Userlist = new ListBox();
		InputBox = new TextBox();
		this.SuspendLayout();

		ChatBox.Location = new Point(0, 0);
			ChatBox.Dock = DockStyle.Left | DockStyle.Top | DockStyle.Bottom;
			ChatBox.Font = new Font("Courier New", 10);
		ChatBoxSplitter.SplitterMoved += new SplitterEventHandler(ChatBoxSplitter_SplitterMoved);
			ChatBoxSplitter.Location = new Point(100, 0);
		Userlist.Dock = DockStyle.Fill;
			Userlist.Font = SystemFont;
			Userlist.IntegralHeight = false;
		InputBox.Dock = DockStyle.Bottom;
			InputBox.Font = SystemFont;

		this.WindowState = FormWindowState.Maximized;
		this.Controls.AddRange(new Control[] { Userlist, ChatBoxSplitter, ChatBox });
		this.Controls.Add(InputBox);
		ChatBox.Width = (int)(.8 * this.Width);
		
		this.ResumeLayout(false);

		Userlist.Left = ChatBoxSplitter.Right;
		Userlist.Items.Add("Properties");

		this.Resize += new EventHandler(ReLayout);
		ChatBoxProportion = (float)ChatBoxSplitter.Left / (float)this.Width;
	}

	void ChatBoxSplitter_SplitterMoved(object sender, SplitterEventArgs e)
	{
		ChatBoxProportion = (float)ChatBoxSplitter.Left / (float)this.Width;
	}

	static void Main() { Application.Run(new MainForm()); }

	void ReLayout(object sender, EventArgs e) { ReLayout(); }
	void ReLayout()
	{
		ChatBox.Width = (int)(ChatBoxProportion * this.Width);
		ChatBoxProportion = (float)ChatBoxSplitter.Left / (float)this.Width;
		Userlist.Left = ChatBoxSplitter.Right;
	}

	void DebugBox(string Message) { MessageBox.Show(Message); }
}