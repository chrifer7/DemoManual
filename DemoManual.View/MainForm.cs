using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using DemoManual.View.Properties;
using ToolTip;

namespace DemoManual.View
{
	public partial class MainForm : Form
    {
        //public SuperToolTip SuperToolTipDemo { get; set; }

		private ToolTipContent toolTip;
		private ToolTipStatus toolTipStatus;

		private WebForm webForm;

		public MainForm()
        {
            InitializeComponent();
			InitializeInteractiveToolTip();
			webForm = new WebForm();
		}

        private async Task TogleToolTipAsync(Control control, string textContent = "", string linkText = "", string linkFileName = "") 
        {
			switch (toolTipStatus) 
			{
				case ToolTipStatus.Hide:
					SetAndShowToolTip(control, textContent, linkText, linkFileName);
					break;

				case ToolTipStatus.Waiting:
					interactiveToolTip1.Hide();
					SetAndShowToolTip(control, textContent, linkText, linkFileName);
					break;


				case ToolTipStatus.Showing:
					toolTipStatus = ToolTipStatus.Waiting;
					await Task.Delay(3000);
					if (toolTipStatus == ToolTipStatus.Waiting) {
						interactiveToolTip1.Hide();
						toolTipStatus = ToolTipStatus.Hide;
					}					
					break;
			}
		}

		private void SetAndShowToolTip(Control control, string textContent, string linkText, string linkFileName)
		{
			//Set content
			if (!string.IsNullOrWhiteSpace(textContent)) toolTip.RichContent.Text = textContent;
			if (!string.IsNullOrWhiteSpace(linkText)) toolTip.Link.Text = linkText;
			if (!string.IsNullOrWhiteSpace(linkFileName)) toolTip.LinkFileName = linkFileName;

			// position the tooltip with its stem towards the right end of the button
			interactiveToolTip1.Show(toolTip, control, control.Width - 16, 0);
			toolTipStatus = ToolTipStatus.Showing;
		}

		private void InitializeInteractiveToolTip()
		{
			toolTip = new ToolTipContent();

			// we want the tooltip's background colour to show through
			toolTip.BackColor = Color.Transparent;

			toolTip.Link.LinkClicked += delegate (object sender, LinkLabelLinkClickedEventArgs e)
			{
				var linkFileName = ((ToolTipContent)((Control)sender).Parent).LinkFileName;

				if (!string.IsNullOrWhiteSpace(linkFileName))
				{
					string curDir = Directory.GetCurrentDirectory();
					webForm.webBrowser.Url = new Uri(String.Format("file:///{0}/Manual/{1}", curDir, linkFileName));
					webForm.Show();
				}
			};
		}

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.htmlcontrol
        }

		private void button4_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button dummy", "Click me", "Interact.html").ConfigureAwait(false);
		}

		private void button4_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button dummy", "Click me", "Interact.html").ConfigureAwait(false);
		}

		private void button1_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button ADD", "Click me bro!", "Add.html").ConfigureAwait(false);
		}

		private void button1_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button ADD", "Click me bro!", "Add.html").ConfigureAwait(false);
		}

		private void button3_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button REMOVE", "Click me 3", "Delete.html").ConfigureAwait(false);
		}

		private void button3_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button REMOVE", "Click me 3", "Delete.html").ConfigureAwait(false);
		}

		private void button2_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button EDIT function", "Click me 2", "Edit.html").ConfigureAwait(false);
		}

		private void button2_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender, "Interactive button EDIT function", "Click me 2", "Edit.html").ConfigureAwait(false);
		}
	}
}
