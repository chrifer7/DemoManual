using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DemoManual.View.Properties;
using Syncfusion.Windows.Forms.Tools;
using ToolTip;

namespace DemoManual.View
{
	public partial class MainForm : Form
    {
        public SuperToolTip SuperToolTipDemo { get; set; }

		private ToolTipContent toolTip;
		private ToolTipStatus toolTipStatus;

		public MainForm()
        {
            InitializeComponent();
			InitializeInteractiveToolTip();

		}

        private async Task TogleToolTipAsync(Control control, string textContent = "", string linkText = "", string linkDestPath = "") 
        {
			switch (toolTipStatus) 
			{
				case ToolTipStatus.Hide:
					SetAndShowToolTip(control, textContent, linkText);
					break;

				case ToolTipStatus.Waiting:
					interactiveToolTip1.Hide();
					SetAndShowToolTip(control, textContent, linkText);
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

		private void SetAndShowToolTip(Control control, string textContent, string linkText)
		{
			//Set content
			if (!string.IsNullOrWhiteSpace(textContent)) toolTip.RichContent.Text = textContent;
			if (!string.IsNullOrWhiteSpace(linkText)) toolTip.Link.Text = linkText;
			//if (!string.IsNullOrWhiteSpace(linkDestPath))

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
                MessageBox.Show("Link clicked");
			};
		}

		//private void InitializeToolTips()
  //      {
  //          this.SuperToolTipDemo = new Syncfusion.Windows.Forms.Tools.SuperToolTip(this);

  //          SuperToolTipDemo.VisualStyle = Syncfusion.Windows.Forms.Tools.SuperToolTip.Appearance.Office2016Colorful;
  //          SuperToolTipDemo.Style = Syncfusion.Windows.Forms.Tools.SuperToolTip.SuperToolTipStyle.Balloon;
            
  //          SuperToolTipDemo.InitialDelay = 500;
  //          SuperToolTipDemo.ToolTipDuration = 30;

  //          //Adding ToolTip Header Item
  //          ToolTipInfo toolTipInfo1 = new Syncfusion.Windows.Forms.Tools.ToolTipInfo();
  //          toolTipInfo1.Header.Text = "Add demo text";
  //          toolTipInfo1.Header.TextAlign = System.Drawing.ContentAlignment.TopCenter;
  //          toolTipInfo1.Footer.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
  //          toolTipInfo1.Footer.ImageScalingSize = new System.Drawing.Size(16, 16);

  //          ToolTipInfo toolTipInfo2 = new Syncfusion.Windows.Forms.Tools.ToolTipInfo();
  //          toolTipInfo2.Header.TextAlign = System.Drawing.ContentAlignment.TopCenter;
  //          toolTipInfo2.Footer.Size = new System.Drawing.Size(400, 350);
  //          toolTipInfo2.Footer.RenderHtml = true;
  //          toolTipInfo2.Footer.Text = "<ul><li>List Item</li><li>Detail demo text for demo</li></ul><a href='https://www.google.com' >Click me</a>";
  //          //toolTipInfo2.Separator = true;

  //          toolTipInfo2.Footer.Image = ((System.Drawing.Image)(Resources.image1_jpg));

  //          //Associating SuperToolTip for ToolStripTabItem
  //          SuperToolTipDemo.SetToolTip(this.button1, toolTipInfo1);

            
  //          SuperToolTipDemo.SetToolTip(this.button3, toolTipInfo2);
  //      }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.htmlcontrol
        }

		private void button4_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}

		private void button4_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}

		private void button1_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}

		private void button1_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}

		private void button3_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}

		private void button3_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}

		private void button2_MouseEnter(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}

		private void button2_MouseLeave(object sender, EventArgs e)
		{
			TogleToolTipAsync((Control)sender).ConfigureAwait(false);
		}
	}
}
