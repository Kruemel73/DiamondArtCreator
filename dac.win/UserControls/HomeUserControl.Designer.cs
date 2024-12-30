namespace Ravlyk.SAE5.WinForms.UserControls
{
	partial class HomeUserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeUserControl));
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.pictureBoxTitle = new System.Windows.Forms.PictureBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelLanguages = new System.Windows.Forms.Panel();
            this.buttonFrench = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonGerman = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonEnglish = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonRegister = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonLinkedIn = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonTwitter = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonFacebook = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonFeedback = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonWeb = new Ravlyk.Drawing.WinForms.FlatButton();
            this.btnLizenceChange = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelLastOpenFiles = new System.Windows.Forms.FlowLayoutPanel();
            this.panelLastOpenFilesTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.flatButtonPalette = new Ravlyk.Drawing.WinForms.FlatButton();
            this.button1 = new System.Windows.Forms.Button();
            this.flatButtonAbout = new Ravlyk.Drawing.WinForms.FlatButton();
            this.flatButtonTrialTest = new Ravlyk.Drawing.WinForms.FlatButton();
            this.flatButtonRegister = new Ravlyk.Drawing.WinForms.FlatButton();
            this.flatButtonOptions = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonOpen = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonNew = new Ravlyk.Drawing.WinForms.FlatButton();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.panelLanguages.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelLastOpenFilesTop.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.labelVersion);
            this.panelTop.Controls.Add(this.pictureBoxTitle);
            this.panelTop.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            // 
            // labelVersion
            // 
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Name = "labelVersion";
            // 
            // pictureBoxTitle
            // 
            resources.ApplyResources(this.pictureBoxTitle, "pictureBoxTitle");
            this.pictureBoxTitle.Name = "pictureBoxTitle";
            this.pictureBoxTitle.TabStop = false;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.panelLanguages);
            this.panelBottom.Controls.Add(this.buttonRegister);
            this.panelBottom.Controls.Add(this.buttonLinkedIn);
            this.panelBottom.Controls.Add(this.buttonTwitter);
            this.panelBottom.Controls.Add(this.buttonFacebook);
            this.panelBottom.Controls.Add(this.buttonFeedback);
            this.panelBottom.Controls.Add(this.buttonWeb);
            resources.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            // 
            // panelLanguages
            // 
            this.panelLanguages.Controls.Add(this.buttonFrench);
            this.panelLanguages.Controls.Add(this.buttonGerman);
            this.panelLanguages.Controls.Add(this.buttonEnglish);
            resources.ApplyResources(this.panelLanguages, "panelLanguages");
            this.panelLanguages.Name = "panelLanguages";
            // 
            // buttonFrench
            // 
            this.buttonFrench.BackColor = System.Drawing.Color.White;
            this.buttonFrench.FlatAppearance.BorderSize = 0;
            this.buttonFrench.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonFrench, "buttonFrench");
            this.buttonFrench.IsSelected = false;
            this.buttonFrench.Name = "buttonFrench";
            this.buttonFrench.UseVisualStyleBackColor = true;
            this.buttonFrench.Click += new System.EventHandler(this.buttonFrench_Click);
            // 
            // buttonGerman
            // 
            this.buttonGerman.BackColor = System.Drawing.Color.White;
            this.buttonGerman.FlatAppearance.BorderSize = 0;
            this.buttonGerman.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonGerman, "buttonGerman");
            this.buttonGerman.IsSelected = false;
            this.buttonGerman.Name = "buttonGerman";
            this.buttonGerman.UseVisualStyleBackColor = true;
            this.buttonGerman.Click += new System.EventHandler(this.buttonGerman_Click);
            // 
            // buttonEnglish
            // 
            this.buttonEnglish.BackColor = System.Drawing.Color.White;
            this.buttonEnglish.FlatAppearance.BorderSize = 0;
            this.buttonEnglish.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonEnglish, "buttonEnglish");
            this.buttonEnglish.IsSelected = false;
            this.buttonEnglish.Name = "buttonEnglish";
            this.buttonEnglish.UseVisualStyleBackColor = true;
            this.buttonEnglish.Click += new System.EventHandler(this.buttonEnglish_Click);
            // 
            // buttonRegister
            // 
            this.buttonRegister.BackColor = System.Drawing.Color.White;
            this.buttonRegister.FlatAppearance.BorderSize = 0;
            this.buttonRegister.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonRegister, "buttonRegister");
            this.buttonRegister.IsSelected = false;
            this.buttonRegister.Name = "buttonRegister";
            this.buttonRegister.UseVisualStyleBackColor = true;
            this.buttonRegister.Click += new System.EventHandler(this.buttonRegister_Click);
            // 
            // buttonLinkedIn
            // 
            this.buttonLinkedIn.BackColor = System.Drawing.Color.White;
            this.buttonLinkedIn.FlatAppearance.BorderSize = 0;
            this.buttonLinkedIn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonLinkedIn, "buttonLinkedIn");
            this.buttonLinkedIn.IsSelected = false;
            this.buttonLinkedIn.Name = "buttonLinkedIn";
            this.buttonLinkedIn.UseVisualStyleBackColor = true;
            this.buttonLinkedIn.Click += new System.EventHandler(this.buttonLinkedIn_Click);
            // 
            // buttonTwitter
            // 
            this.buttonTwitter.BackColor = System.Drawing.Color.White;
            this.buttonTwitter.FlatAppearance.BorderSize = 0;
            this.buttonTwitter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonTwitter, "buttonTwitter");
            this.buttonTwitter.IsSelected = false;
            this.buttonTwitter.Name = "buttonTwitter";
            this.buttonTwitter.UseVisualStyleBackColor = true;
            this.buttonTwitter.Click += new System.EventHandler(this.buttonTwitter_Click);
            // 
            // buttonFacebook
            // 
            this.buttonFacebook.BackColor = System.Drawing.Color.White;
            this.buttonFacebook.FlatAppearance.BorderSize = 0;
            this.buttonFacebook.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonFacebook, "buttonFacebook");
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
            this.buttonWeb.IsSelected = false;
            this.buttonWeb.Name = "buttonWeb";
            this.buttonWeb.UseVisualStyleBackColor = true;
            this.buttonWeb.Click += new System.EventHandler(this.buttonWeb_Click);
            // 
            // btnLizenceChange
            // 
            resources.ApplyResources(this.btnLizenceChange, "btnLizenceChange");
            this.btnLizenceChange.Name = "btnLizenceChange";
            this.btnLizenceChange.UseVisualStyleBackColor = true;
            this.btnLizenceChange.Click += new System.EventHandler(this.btnLizenceChange_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelLastOpenFiles);
            this.panelMain.Controls.Add(this.panelLastOpenFilesTop);
            this.panelMain.Controls.Add(this.panelButtons);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // panelLastOpenFiles
            // 
            resources.ApplyResources(this.panelLastOpenFiles, "panelLastOpenFiles");
            this.panelLastOpenFiles.Name = "panelLastOpenFiles";
            // 
            // panelLastOpenFilesTop
            // 
            this.panelLastOpenFilesTop.Controls.Add(this.label1);
            resources.ApplyResources(this.panelLastOpenFilesTop, "panelLastOpenFilesTop");
            this.panelLastOpenFilesTop.Name = "panelLastOpenFilesTop";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.button2);
            this.panelButtons.Controls.Add(this.btnLizenceChange);
            this.panelButtons.Controls.Add(this.flatButtonPalette);
            this.panelButtons.Controls.Add(this.button1);
            this.panelButtons.Controls.Add(this.flatButtonAbout);
            this.panelButtons.Controls.Add(this.flatButtonTrialTest);
            this.panelButtons.Controls.Add(this.flatButtonRegister);
            this.panelButtons.Controls.Add(this.flatButtonOptions);
            this.panelButtons.Controls.Add(this.buttonOpen);
            this.panelButtons.Controls.Add(this.buttonNew);
            resources.ApplyResources(this.panelButtons, "panelButtons");
            this.panelButtons.Name = "panelButtons";
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            //this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // flatButtonPalette
            // 
            resources.ApplyResources(this.flatButtonPalette, "flatButtonPalette");
            this.flatButtonPalette.BackColor = System.Drawing.Color.White;
            this.flatButtonPalette.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.flatButtonPalette.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.color_icons_96;
            this.flatButtonPalette.IsSelected = false;
            this.flatButtonPalette.Name = "flatButtonPalette";
            this.flatButtonPalette.UseVisualStyleBackColor = true;
            this.flatButtonPalette.Click += new System.EventHandler(this.flatButton1_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            //this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // flatButtonAbout
            // 
            resources.ApplyResources(this.flatButtonAbout, "flatButtonAbout");
            this.flatButtonAbout.BackColor = System.Drawing.Color.White;
            this.flatButtonAbout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.flatButtonAbout.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.About_96;
            this.flatButtonAbout.IsSelected = false;
            this.flatButtonAbout.Name = "flatButtonAbout";
            this.flatButtonAbout.UseVisualStyleBackColor = true;
            this.flatButtonAbout.Click += new System.EventHandler(this.flatButtonAbout_Click);
            // 
            // flatButtonTrialTest
            // 
            resources.ApplyResources(this.flatButtonTrialTest, "flatButtonTrialTest");
            this.flatButtonTrialTest.BackColor = System.Drawing.Color.White;
            this.flatButtonTrialTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.flatButtonTrialTest.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.free_trial_icon;
            this.flatButtonTrialTest.IsSelected = false;
            this.flatButtonTrialTest.Name = "flatButtonTrialTest";
            this.flatButtonTrialTest.UseVisualStyleBackColor = true;
            //this.flatButtonTrialTest.Click += new System.EventHandler(this.flatButtonTrialTest_Click);
            // 
            // flatButtonRegister
            // 
            resources.ApplyResources(this.flatButtonRegister, "flatButtonRegister");
            this.flatButtonRegister.BackColor = System.Drawing.Color.White;
            this.flatButtonRegister.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.flatButtonRegister.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.register;
            this.flatButtonRegister.IsSelected = false;
            this.flatButtonRegister.Name = "flatButtonRegister";
            this.flatButtonRegister.UseVisualStyleBackColor = true;
            this.flatButtonRegister.Click += new System.EventHandler(this.flatButtonRegister_Click);
            // 
            // flatButtonOptions
            // 
            resources.ApplyResources(this.flatButtonOptions, "flatButtonOptions");
            this.flatButtonOptions.BackColor = System.Drawing.Color.White;
            this.flatButtonOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.flatButtonOptions.IsSelected = false;
            this.flatButtonOptions.Name = "flatButtonOptions";
            this.flatButtonOptions.UseVisualStyleBackColor = true;
            this.flatButtonOptions.Click += new System.EventHandler(this.ribbonOrbMenuItemOptions_Click);
            // 
            // buttonOpen
            // 
            resources.ApplyResources(this.buttonOpen, "buttonOpen");
            this.buttonOpen.BackColor = System.Drawing.Color.White;
            this.buttonOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.buttonOpen.IsSelected = false;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonNew
            // 
            resources.ApplyResources(this.buttonNew, "buttonNew");
            this.buttonNew.BackColor = System.Drawing.Color.White;
            this.buttonNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.buttonNew.IsSelected = false;
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // HomeUserControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "HomeUserControl";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTitle)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelLanguages.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelLastOpenFilesTop.ResumeLayout(false);
            this.panelLastOpenFilesTop.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.PictureBox pictureBoxTitle;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.Panel panelButtons;
		private Drawing.WinForms.FlatButton buttonOpen;
		private Drawing.WinForms.FlatButton buttonNew;
		private System.Windows.Forms.FlowLayoutPanel panelLastOpenFiles;
		private System.Windows.Forms.Panel panelLastOpenFilesTop;
		internal Drawing.WinForms.FlatButton buttonRegister;
		private Drawing.WinForms.FlatButton buttonLinkedIn;
		private Drawing.WinForms.FlatButton buttonTwitter;
		private Drawing.WinForms.FlatButton buttonFacebook;
		private Drawing.WinForms.FlatButton buttonFeedback;
		private Drawing.WinForms.FlatButton buttonWeb;
		private System.Windows.Forms.Panel panelLanguages;
		private Drawing.WinForms.FlatButton buttonEnglish;
        private Drawing.WinForms.FlatButton buttonGerman;
        private Drawing.WinForms.FlatButton flatButtonOptions;
        private System.Windows.Forms.Label label1;
        private Drawing.WinForms.FlatButton flatButtonRegister;
        private Drawing.WinForms.FlatButton flatButtonTrialTest;
        private Drawing.WinForms.FlatButton flatButtonAbout;
        private System.Windows.Forms.Button button1;
        private Drawing.WinForms.FlatButton flatButtonPalette;
        private System.Windows.Forms.Button btnLizenceChange;
        private Drawing.WinForms.FlatButton buttonFrench;
        private System.Windows.Forms.Button button2;
    }
}
