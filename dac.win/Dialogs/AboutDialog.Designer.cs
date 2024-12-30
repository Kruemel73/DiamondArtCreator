namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBoxTitle = new System.Windows.Forms.PictureBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonFacebook = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonFeedback = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonWeb = new Ravlyk.Drawing.WinForms.FlatButton();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonOk = new Ravlyk.Drawing.WinForms.FlatDialogButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.pictureBoxTitle);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            // 
            // pictureBoxTitle
            // 
            this.pictureBoxTitle.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources._190840551_1425354761190463_3752218376746074022_n;
            resources.ApplyResources(this.pictureBoxTitle, "pictureBoxTitle");
            this.pictureBoxTitle.InitialImage = global::Ravlyk.SAE5.WinForms.Properties.Resources._190840551_1425354761190463_3752218376746074022_n;
            this.pictureBoxTitle.Name = "pictureBoxTitle";
            this.pictureBoxTitle.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonFacebook);
            this.panelBottom.Controls.Add(this.buttonFeedback);
            this.panelBottom.Controls.Add(this.buttonWeb);
            this.panelBottom.Controls.Add(this.panelButtons);
            resources.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            // 
            // buttonFacebook
            // 
            this.buttonFacebook.BackColor = System.Drawing.Color.White;
            this.buttonFacebook.FlatAppearance.CheckedBackColor = System.Drawing.Color.Coral;
            this.buttonFacebook.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSlateGray;
            this.buttonFacebook.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonFacebook, "buttonFacebook");
            this.buttonFacebook.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Facebook24;
            this.buttonFacebook.IsSelected = false;
            this.buttonFacebook.Name = "buttonFacebook";
            this.buttonFacebook.UseVisualStyleBackColor = true;
            this.buttonFacebook.Click += new System.EventHandler(this.buttonFacebook_Click);
            // 
            // buttonFeedback
            // 
            this.buttonFeedback.BackColor = System.Drawing.Color.White;
            this.buttonFeedback.FlatAppearance.BorderSize = 0;
            this.buttonFeedback.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonFeedback, "buttonFeedback");
            this.buttonFeedback.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Mail_48;
            this.buttonFeedback.IsSelected = false;
            this.buttonFeedback.Name = "buttonFeedback";
            this.buttonFeedback.UseVisualStyleBackColor = true;
            this.buttonFeedback.Click += new System.EventHandler(this.buttonFeedback_Click);
            // 
            // buttonWeb
            // 
            this.buttonWeb.BackColor = System.Drawing.Color.White;
            this.buttonWeb.FlatAppearance.BorderSize = 0;
            this.buttonWeb.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonWeb, "buttonWeb");
            this.buttonWeb.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.HTTP_48;
            this.buttonWeb.IsSelected = false;
            this.buttonWeb.Name = "buttonWeb";
            this.buttonWeb.UseVisualStyleBackColor = true;
            this.buttonWeb.Click += new System.EventHandler(this.buttonWeb_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonOk);
            resources.ApplyResources(this.panelButtons, "panelButtons");
            this.panelButtons.Name = "panelButtons";
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.White;
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Checkmark24;
            this.buttonOk.IsSelected = false;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // AboutDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.buttonOk;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AboutDialog";
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonOk;
		private System.Windows.Forms.PictureBox pictureBoxTitle;
		private Drawing.WinForms.FlatButton buttonFeedback;
		private Drawing.WinForms.FlatButton buttonWeb;
        private Drawing.WinForms.FlatButton buttonFacebook;
        private System.Windows.Forms.TextBox textBox1;
    }
}