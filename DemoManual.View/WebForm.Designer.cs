﻿namespace DemoManual.View
{
	partial class WebForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// webBrowser
			// 
			this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser.Location = new System.Drawing.Point(0, 0);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(794, 488);
			this.webBrowser.TabIndex = 0;
			this.webBrowser.Url = new System.Uri("http://ManualTest.html", System.UriKind.Absolute);
			// 
			// WebForm
			// 
			this.ClientSize = new System.Drawing.Size(794, 488);
			this.Controls.Add(this.webBrowser);
			this.Name = "WebForm";
			this.Text = "Web content";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebForm_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.WebBrowser webBrowser;
	}
}
