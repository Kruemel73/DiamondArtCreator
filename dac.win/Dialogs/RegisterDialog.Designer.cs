namespace Ravlyk.SAE5.WinForms.Dialogs
{
	partial class RegisterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterDialog));
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonBuy = new Ravlyk.Drawing.WinForms.FlatDialogButton();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.buttonRegister = new Ravlyk.Drawing.WinForms.FlatDialogButton();
            this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxHWID = new System.Windows.Forms.TextBox();
            this.labelHWID = new System.Windows.Forms.Label();
            this.textBoxEmail = new System.Windows.Forms.TextBox();
            this.labelEmail = new System.Windows.Forms.Label();
            this.textBoxSurname = new System.Windows.Forms.TextBox();
            this.labelSurname = new System.Windows.Forms.Label();
            this.textBoxSerial = new System.Windows.Forms.TextBox();
            this.labelSerial = new System.Windows.Forms.Label();
            this.textBoxForname = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.buttonFeedback = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonWeb = new Ravlyk.Drawing.WinForms.FlatButton();
            this.tbLicenseFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.flatButton1 = new Ravlyk.Drawing.WinForms.FlatButton();
            this.panelBottom.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonBuy);
            this.panelBottom.Controls.Add(this.panelButtons);
            resources.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            // 
            // buttonBuy
            // 
            this.buttonBuy.BackColor = System.Drawing.Color.White;
            this.buttonBuy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonBuy, "buttonBuy");
            this.buttonBuy.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.BankCard24;
            this.buttonBuy.IsSelected = false;
            this.buttonBuy.Name = "buttonBuy";
            this.buttonBuy.UseVisualStyleBackColor = true;
            this.buttonBuy.Click += new System.EventHandler(this.buttonBuy_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.buttonRegister);
            this.panelButtons.Controls.Add(this.buttonCancel);
            resources.ApplyResources(this.panelButtons, "panelButtons");
            this.panelButtons.Name = "panelButtons";
            // 
            // buttonRegister
            // 
            this.buttonRegister.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.buttonRegister, "buttonRegister");
            this.buttonRegister.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.buttonRegister.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Key24;
            this.buttonRegister.IsSelected = false;
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.White;
            this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.IsSelected = false;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flatButton1);
            this.groupBox1.Controls.Add(this.tbLicenseFile);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxHWID);
            this.groupBox1.Controls.Add(this.labelHWID);
            this.groupBox1.Controls.Add(this.textBoxEmail);
            this.groupBox1.Controls.Add(this.labelEmail);
            this.groupBox1.Controls.Add(this.textBoxSurname);
            this.groupBox1.Controls.Add(this.labelSurname);
            this.groupBox1.Controls.Add(this.textBoxSerial);
            this.groupBox1.Controls.Add(this.labelSerial);
            this.groupBox1.Controls.Add(this.textBoxForname);
            this.groupBox1.Controls.Add(this.labelName);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxHWID
            // 
            resources.ApplyResources(this.textBoxHWID, "textBoxHWID");
            this.textBoxHWID.Name = "textBoxHWID";
            // 
            // labelHWID
            // 
            resources.ApplyResources(this.labelHWID, "labelHWID");
            this.labelHWID.Name = "labelHWID";
            // 
            // textBoxEmail
            // 
            resources.ApplyResources(this.textBoxEmail, "textBoxEmail");
            this.textBoxEmail.Name = "textBoxEmail";
            // 
            // labelEmail
            // 
            resources.ApplyResources(this.labelEmail, "labelEmail");
            this.labelEmail.Name = "labelEmail";
            // 
            // textBoxSurname
            // 
            resources.ApplyResources(this.textBoxSurname, "textBoxSurname");
            this.textBoxSurname.Name = "textBoxSurname";
            // 
            // labelSurname
            // 
            resources.ApplyResources(this.labelSurname, "labelSurname");
            this.labelSurname.Name = "labelSurname";
            // 
            // textBoxSerial
            // 
            resources.ApplyResources(this.textBoxSerial, "textBoxSerial");
            this.textBoxSerial.Name = "textBoxSerial";
            this.textBoxSerial.TextChanged += new System.EventHandler(this.textBoxEmail_TextChanged);
            // 
            // labelSerial
            // 
            resources.ApplyResources(this.labelSerial, "labelSerial");
            this.labelSerial.Name = "labelSerial";
            // 
            // textBoxForname
            // 
            resources.ApplyResources(this.textBoxForname, "textBoxForname");
            this.textBoxForname.Name = "textBoxForname";
            this.textBoxForname.TextChanged += new System.EventHandler(this.textBoxEmail_TextChanged);
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
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
            // tbLicenseFile
            // 
            resources.ApplyResources(this.tbLicenseFile, "tbLicenseFile");
            this.tbLicenseFile.Name = "tbLicenseFile";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // flatButton1
            // 
            resources.ApplyResources(this.flatButton1, "flatButton1");
            this.flatButton1.BackColor = System.Drawing.Color.White;
            this.flatButton1.BackgroundImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.Open16;
            this.flatButton1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Coral;
            this.flatButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSlateGray;
            this.flatButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.flatButton1.IsSelected = false;
            this.flatButton1.Name = "flatButton1";
            this.flatButton1.UseVisualStyleBackColor = true;
            this.flatButton1.Click += new System.EventHandler(this.flatButton1_Click);
            // 
            // RegisterDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.buttonFeedback);
            this.Controls.Add(this.buttonWeb);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RegisterDialog";
            this.panelBottom.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelBottom;
		private Drawing.WinForms.FlatDialogButton buttonBuy;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatDialogButton buttonRegister;
		private Drawing.WinForms.FlatDialogButton buttonCancel;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxSerial;
		private System.Windows.Forms.Label labelSerial;
		private System.Windows.Forms.TextBox textBoxForname;
		private System.Windows.Forms.Label labelName;
		private Drawing.WinForms.FlatButton buttonFeedback;
		private Drawing.WinForms.FlatButton buttonWeb;
        private System.Windows.Forms.TextBox textBoxEmail;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.TextBox textBoxSurname;
        private System.Windows.Forms.Label labelSurname;
        private System.Windows.Forms.TextBox textBoxHWID;
        private System.Windows.Forms.Label labelHWID;
        private System.Windows.Forms.TextBox tbLicenseFile;
        private System.Windows.Forms.Label label2;
        private Drawing.WinForms.FlatButton flatButton1;
    }
}