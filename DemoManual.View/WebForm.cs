using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DemoManual.View
{
	public partial class WebForm : Form
	{
		public WebForm()
		{
			InitializeComponent();
		}

		private void WebForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
			}
		}
	}
}
