namespace ToolTip
{
    partial class ToolTipContent
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.Link = new System.Windows.Forms.LinkLabel();
			//this.bannerTextProvider1 = new Syncfusion.Windows.Forms.BannerTextProvider(this.components);
			this.RichContent = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// Link
			// 
			this.Link.AutoSize = true;
			this.Link.Location = new System.Drawing.Point(3, 39);
			this.Link.Name = "Link";
			this.Link.Size = new System.Drawing.Size(47, 13);
			this.Link.TabIndex = 1;
			this.Link.TabStop = true;
			this.Link.Text = "Click me";
			// 
			// RichContent
			// 
			this.RichContent.BackColor = System.Drawing.SystemColors.Info;
			this.RichContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.RichContent.Location = new System.Drawing.Point(0, 0);
			this.RichContent.Name = "RichContent";
			this.RichContent.ReadOnly = true;
			this.RichContent.Size = new System.Drawing.Size(226, 36);
			this.RichContent.TabIndex = 2;
			this.RichContent.Text = "Rich content";
			// 
			// ToolTipContent
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Info;
			this.Controls.Add(this.RichContent);
			this.Controls.Add(this.Link);
			this.Name = "ToolTipContent";
			this.Size = new System.Drawing.Size(226, 63);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.LinkLabel Link;
		//private Syncfusion.Windows.Forms.BannerTextProvider bannerTextProvider1;
		public System.Windows.Forms.RichTextBox RichContent;
	}
}
