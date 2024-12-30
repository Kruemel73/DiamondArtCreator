using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;
using Ravlyk.SAE.Drawing.Properties;
using SAE5.Win;
using Size = Ravlyk.Common.Size;
//using TreeksLicensingLibrary2.EasyIntegration;
using System.Collections.Generic;
//using TreeksLicensingLibrary2;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

namespace Ravlyk.SAE5.WinForms.UserControls
{
	public partial class HomeUserControl : UserControl
	{
		public HomeUserControl(MainForm mainForm)
		{
			InitializeComponent();

			this.mainForm = mainForm;
		}


		readonly MainForm mainForm;

        //public static TLLInterface tlli = new TLLInterface(SAE5.WinForms.Properties.Resources.chunk, "");
        
        //private const string myPublicKey = "";
		
        //private LicenseVerification verification = new LicenseVerification(myPublicKey);
		
        public bool isLizenzValid;
		public bool isLizenzDemo;
		public bool isLizenzCommerc;
		public bool TestVersion = false;
        public string MyDocumentPath;

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			var toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };

			toolTip.SetToolTip(buttonWeb, Resources.HintHomeButtonWeb);
			toolTip.SetToolTip(buttonFeedback, Resources.HintHomeButtonFeedback);
			toolTip.SetToolTip(buttonFacebook, Resources.HintHomeButtonFacebook);
			toolTip.SetToolTip(buttonTwitter, Resources.HintHomeButtonTwitter);
			toolTip.SetToolTip(buttonLinkedIn, Resources.HintHomeButtonLinkedIn);
			toolTip.SetToolTip(buttonRegister, Resources.HintHomeButtonRegister);

			toolTip.SetToolTip(buttonEnglish, "English language");
			toolTip.SetToolTip(buttonGerman, "German language");
			toolTip.SetToolTip(buttonFrench, "French language");

            CheckRegistration();

            InitializeRecentFilesButtons(toolTip);

            if (TestVersion == false)
            {
				btnLizenceChange.Visible = false;
            }
			else
            {
				btnLizenceChange.Visible = true;
            }

			if (File.Exists(Settings.Default.UserPalettesLocationSafe + "\\croppedpic.jpg"))
            {
				File.Delete(Settings.Default.UserPalettesLocationSafe + "\\croppedpic.jpg");
            } 
			if (File.Exists(Settings.Default.UserPalettesLocationSafe + "\\editpic.jpg"))
            {
				File.Delete(Settings.Default.UserPalettesLocationSafe + "\\editpic.jpg");
            }
			if (File.Exists(Settings.Default.UserPalettesLocationSafe + "\\editpic1.jpg"))
            {
				File.Delete(Settings.Default.UserPalettesLocationSafe + "\\editpic1.jpg");
            }
			SetLanguageButtons();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}

		public void CheckRegistration()
		{
			Settings.Default.FT1 = true;
			Settings.Default.FT2 = true;
			Settings.Default.FT3 = true;
			Settings.Default.FT4 = true;
			Settings.Default.FT5 = true;
			Settings.Default.FT6 = true;
			Settings.Default.FT7 = true;
			Settings.Default.FT8 = true;
			Settings.Default.FT9 = true;
			Settings.Default.FT10 = true;

			SAEWizardSettings.Default.FT1 = true;
			SAEWizardSettings.Default.FT2 = true;
			SAEWizardSettings.Default.FT3 = true;
			SAEWizardSettings.Default.FT4 = true;
			SAEWizardSettings.Default.FT5 = true;
			SAEWizardSettings.Default.FT6 = true;
			SAEWizardSettings.Default.FT7 = true;
			SAEWizardSettings.Default.FT8 = true;
			SAEWizardSettings.Default.FT9 = true;
			SAEWizardSettings.Default.FT10 = true;

			Settings.Default.isLizenzCommerc = true;
			Settings.Default.isLizenzDemo = false;
			Settings.Default.isLizenzValid = true;

			SAEWizardSettings.Default.isLizenzCommerc = true;
			SAEWizardSettings.Default.isLizenzDemo = false;
			SAEWizardSettings.Default.isLizenzValid = true;

			isLizenzValid = true;
			isLizenzDemo = false;
			isLizenzCommerc = true;

            /*//set modules 2-4 active for all
            SAEWizardSettings.Default.isLizenzValid = true;
            Settings.Default.isLizenzValid = true;
            isLizenzValid = true;
            SAEWizardSettings.Default.FT2 = true;
            Settings.Default.FT2 = true;
            SAEWizardSettings.Default.FT3 = true;
            Settings.Default.FT3 = true;
            SAEWizardSettings.Default.FT4 = true; flatButtonPalette.Visible = true;
            Settings.Default.FT4 = true;
			*/

            MyDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			//check if serial-number is on blacklist or revoked on registration-server 
			/*if (tlli.MyLicense != null) {
				string filename = Application.StartupPath + @"\TreeksLicensing.dll";
				if (File.Exists(filename))
				{
					var lineCount = File.ReadAllLines(filename).Count();
					if (lineCount == 0)
					{
						//return strLicenseDump;

					}
					var lines = File.ReadAllLines(filename);

					foreach (var line in lines)
					{
						if (line != null)
						{
							var lineDecode = Base64Decode(line);
							if (lineDecode == tlli.MyLicense.SerialNumber)
							{
								tlli.DeleteLicenseInRegistry();
								//tlli.DeactivateLicense();
                                string License = MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic";
								File.Delete(License);
                                MessageBox.Show("Ihre Lizenz wurde aufgrund einer Stornierung oder einer unerlaubten Handlung deaktiviert.", "Lizenz zurückgesetzt!", MessageBoxButtons.OK);
							}
						}

					}
				}
				var RevoCheck = tlli.OnlineRevocationCheck();
				if (RevoCheck == true) 
				{
                    tlli.DeleteLicenseInRegistry();
                    tlli.DeactivateLicense();
                    string License = MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic";
					if (File.Exists(License)) 
					{ 
						File.Delete(License); 
					}
                    MessageBox.Show("Ihre Lizenz wurde aufgrund einer Stornierung oder einer unerlaubten Handlung deaktiviert.", "Lizenz zurückgesetzt!", MessageBoxButtons.OK);

                }
            }*/
            
			/*string LicenseFile = MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic";
            if (!File.Exists(LicenseFile) && tlli.MyLicense != null)
            {
                using (var CreateLicenseFile = new CreateRegister())
                {
                    CreateLicenseFile.ShowDialog(this);
                }
            }
			*/
            /*if (tlli.MyLicense == null)
			{
				labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + Environment.NewLine + "Free-Version";
				flatButtonRegister.Visible = true;
				flatButtonTrialTest.Visible = true;
			}
			*/
			/*if (tlli.MyLicense != null && tlli.MyLicense.IsDemo)
			{
				SAEWizardSettings.Default.isLizenzCommerc = true;
				Settings.Default.isLizenzCommerc = true;
				SAEWizardSettings.Default.isLizenzDemo = true;
				Settings.Default.isLizenzDemo = true;
				isLizenzCommerc = true;
				isLizenzDemo = true;

				labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + Environment.NewLine +
				"Trial-Version" + Environment.NewLine + "Aktiv bis: " + tlli.MyLicense.ExpirationDate;
				flatButtonRegister.Visible = true;
			}*/

			//if (tlli.MyLicense != null && tlli.MyLicense.Info2 == "Commercial")
             if (isLizenzCommerc == true)
                {
                //CreateRegFile();
                SAEWizardSettings.Default.isLizenzValid = true;
                Settings.Default.isLizenzValid = true;
                SAEWizardSettings.Default.isLizenzCommerc = true;
                Settings.Default.isLizenzCommerc = true;
                isLizenzCommerc = true;
                isLizenzValid = true;
                labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + Environment.NewLine + "Registered Version: Pro-Version" +
					Environment.NewLine; // + "Registriert für: " + tlli.MyLicense.OwnerName;
				btnLizenceChange.Text = "Free";
				flatButtonRegister.Visible = false;
				flatButtonRegister.Text = "Lizenz ändern...";
				flatButtonPalette.Visible = true;

			}

			/*if (tlli.MyLicense != null && tlli.MyLicense.AllowedFeatures.Count > 3 && tlli.MyLicense.Info2 != "Commvercial")
			{
				//CreateRegFile();
				SAEWizardSettings.Default.isLizenzValid = true;
				Settings.Default.isLizenzValid = true;
				SAEWizardSettings.Default.isLizenzCommerc = true;
				Settings.Default.isLizenzCommerc = true;
				isLizenzCommerc = true;
				isLizenzValid = true;
				labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + Environment.NewLine + "Registered Version: Pro-Version" +
					Environment.NewLine + "Registriert für: " + tlli.MyLicense.OwnerName;
				btnLizenceChange.Text = "Free";
				flatButtonRegister.Visible = false;
				flatButtonRegister.Text = "Lizenz ändern...";
				flatButtonPalette.Visible = true;
			}*/

			/*if (tlli.MyLicense != null && tlli.MyLicense.AllowedFeatures.Count > 0 && tlli.MyLicense.AllowedFeatures.Count < 3)
			{
				//CreateRegFile()
				labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + Environment.NewLine + "Registered Version: " + tlli.MyLicense.Info2 +
				Environment.NewLine + "Registriert für: " + tlli.MyLicense.OwnerName;
				btnLizenceChange.Text = "Free";
				flatButtonRegister.Visible = true;
				flatButtonRegister.Text = "Pro-Version freischalten";

				foreach (string af in tlli.MyLicense.AllowedFeatures)
				{
					if (af == "FT1") { Settings.Default.FT1 = true; SAEWizardSettings.Default.FT1 = true; }
					if (af == "FT2") { Settings.Default.FT2 = true; SAEWizardSettings.Default.FT2 = true; }
					if (af == "FT3") { Settings.Default.FT3 = true; SAEWizardSettings.Default.FT3 = true; }
					if (af == "FT4") { Settings.Default.FT4 = true; SAEWizardSettings.Default.FT4 = true; flatButtonPalette.Visible = true; }
					if (af == "FT5") { Settings.Default.FT5 = true; SAEWizardSettings.Default.FT5 = true; }
					if (af == "FT6") { Settings.Default.FT6 = true; SAEWizardSettings.Default.FT6 = true; }
					if (af == "FT7") { Settings.Default.FT7 = true; SAEWizardSettings.Default.FT7 = true; }
					if (af == "FT8") { Settings.Default.FT8 = true; SAEWizardSettings.Default.FT8 = true; }
					if (af == "FT9") { Settings.Default.FT9 = true; SAEWizardSettings.Default.FT9 = true; }
					if (af == "FT10") { Settings.Default.FT10 = true; SAEWizardSettings.Default.FT10 = true; }
				}
			}*/
		}

		/*private void GenerateLicenseForCustomer(string ProductName, string LicenseOwner, string HardwareID = "")
		{
			if (File.Exists(Settings.Default.UserPalettesLocation + "\\KEY\\trial.lkey"))
			{
				TLLIntegration.tlli.ActivateTrial();
                TLLIntegration.tlli.AsyncSendUsageInfo("Trial Startet: " + DateTime.Now);

                TreeksLicensingLibrary2.License lic = new TreeksLicensingLibrary2.License();

                TreeksLicensingLibrary2.LicenseSigningKey key = new TreeksLicensingLibrary2.LicenseSigningKey(Settings.Default.UserPalettesLocation + "\\KEY\\trial.lkey", "r*cJ@fV%nYzAG29QdwCFs$6X76");
				//trial license settings
				lic.ProductName = ProductName;
				lic.OwnerName = LicenseOwner;
				lic.IsDemo = true;
				lic.ExpirationDateEnabled = true;
				lic.ExpirationDate = DateTime.Today.AddDays(10);

				string strLicense = lic.GenerateLicenseString(key);

				File.WriteAllText(Settings.Default.UserPalettesLocation + "\\KEY\\trial.lic", strLicense);
				File.Delete(Settings.Default.UserPalettesLocation + "\\KEY\\trial.lkey");
			}
			else
			{
				MessageBox.Show(null, "Der 10-tätige Testzeitraum ist bereits abgelaufen und kann nicht verlängert werden!", "Testzeitraum abgelaufen !!!", MessageBoxButtons.OK);
			}
		}*/

		/*private string GenerateLicenseDump(TreeksLicensingLibrary2.License lic)
		{
			if (lic == null)
				return "No license is present.";

			string strLicenseDump = "Correct and working license present." + Environment.NewLine + Environment.NewLine + "Product: " + lic.ProductName + Environment.NewLine + "License owner: " + lic.OwnerName;
			strLicenseDump += Environment.NewLine + "Serial: " + lic.SerialNumber;

			strLicenseDump += Environment.NewLine + Environment.NewLine + "License expiration enabled: " + lic.ExpirationDateEnabled;

			if (lic.ExpirationDateEnabled)
			{
				strLicenseDump += Environment.NewLine + "Expiration date: " + lic.ExpirationDate;
				strLicenseDump += Environment.NewLine + "Expiration check time server: " + lic.ExpirationCheckTimeServer;
				strLicenseDump += Environment.NewLine + "Treat offline as expired: " + lic.ExpirationCheckOnlineOnly;
			}

			if (lic.HardwareID.Count > 0)
			{
				strLicenseDump += Environment.NewLine + Environment.NewLine + "Hardware locking enabled: True";

				strLicenseDump += Environment.NewLine + "Allowed hardware IDs: ";

				bool first = true;
				foreach (string hwid in lic.HardwareID)
				{
					if (first)
					{
						strLicenseDump += hwid;
						first = false;
					}
					else
					{
						strLicenseDump += ", " + hwid;
					}
				}
			}
			else
			{
				strLicenseDump += Environment.NewLine + Environment.NewLine + "Hardware locking enabled: False";
			}
			if (lic.AllowedFeatures.Count > 0)
			{
				strLicenseDump += Environment.NewLine + Environment.NewLine + "Allowed features: ";

				bool first = true;
				foreach (string af in lic.AllowedFeatures)
				{
					if (first)
					{
						strLicenseDump += af;
						first = false;
					}
					else
					{
						strLicenseDump += ", " + af;
					}
				}
			}
			else
			{
				strLicenseDump += Environment.NewLine + Environment.NewLine + "Allowed features: none";
			}

			if (lic.BlockedFeatures.Count > 0)
			{
				strLicenseDump += Environment.NewLine + Environment.NewLine + "Blocked features: ";

				bool first = true;
				foreach (string bf in lic.BlockedFeatures)
				{
					if (first)
					{
						strLicenseDump += bf;
						first = false;
					}
					else
					{
						strLicenseDump += ", " + bf;
					}
				}
			}
			else
			{
				strLicenseDump += Environment.NewLine + Environment.NewLine + "Blocked features: none";
			}

			strLicenseDump += Environment.NewLine + Environment.NewLine + "Custom fields:";

			strLicenseDump += Environment.NewLine + "Info 1: " + lic.Info1;
			strLicenseDump += Environment.NewLine + "Info 2: " + lic.Info2;
			strLicenseDump += Environment.NewLine + "Info 3: " + lic.Info3;
			strLicenseDump += Environment.NewLine + "Info 4: " + lic.Info4;
			strLicenseDump += Environment.NewLine + "Info 5: " + lic.Info5;
			strLicenseDump += Environment.NewLine + "Info 6: " + lic.Info6;
			strLicenseDump += Environment.NewLine + "Info 7: " + lic.Info7;
			strLicenseDump += Environment.NewLine + "Info 8: " + lic.Info8;
			strLicenseDump += Environment.NewLine + "Info 9: " + lic.Info9;
			strLicenseDump += Environment.NewLine + "Info 10: " + lic.Info10;

			return strLicenseDump;
		}*/

		/*private void LoadLicenseStatus()
		{
			if (tlli.MyLicense == null)
			{
				MessageBox.Show("License validation failed: " + tlli.LastVerificationResult);
			}
			else
			{
				MessageBox.Show(GenerateLicenseDump(tlli.MyLicense));
            }

		}*/


		#region Recent files

		void InitializeRecentFilesButtons(ToolTip toolTip)
		{
			if (string.IsNullOrEmpty(Settings.Default.LastOpenFiles))
			{
				var startupPath = Path.Combine(AppInfo.StartupPath, "Samples");
				if (!string.IsNullOrEmpty(startupPath) && Directory.Exists(startupPath))
				{
					var sampleFileNames = Directory.GetFiles(startupPath, "*.dac").Take(10);
					Settings.Default.LastOpenFiles = string.Join(Settings.FilesSeparator.ToString(), sampleFileNames);
					Settings.Default.Save();
				}
			}

			foreach (var fileName in Settings.Default.LastOpenFiles.Split(Settings.FilesSeparator))
			{
				if (File.Exists(fileName))
				{
					IndexedImage image;
					try
					{
						using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
						{
							image = ImageSerializer.LoadFromStream(stream, fileName);
						}
					}
					catch (IOException)
					{
						continue;
					}
					
					if (image != null && image.Size.Width > 0 && image.Size.Height > 0)
					{
						Bitmap bitmap;
						if (image.Size.Width > 200 || image.Size.Height > 200)
						{
							var maxLength = Math.Max(image.Size.Width, image.Size.Height);
							var newSize = new Size(image.Size.Width * 200 / maxLength, image.Size.Height * 200 / maxLength);
							bitmap = new ImageResampler().Resample(image, newSize, ImageResampler.FilterType.Box).ToBitmap();
						}
						else
						{
							bitmap = image.ToBitmap();
						}
						
						var imageButton = new FlatButton();
						imageButton.Size = new System.Drawing.Size(250, 250);
						imageButton.Image = bitmap;
						imageButton.Text = Environment.NewLine + Path.GetFileNameWithoutExtension(fileName);
						imageButton.Tag = fileName;
						imageButton.TextAlign = ContentAlignment.BottomCenter;
						imageButton.ImageAlign = ContentAlignment.MiddleCenter;
						imageButton.FlatAppearance.BorderSize = 0;
						imageButton.Click += ImageButton_Click;

						var tooltip = fileName + Environment.NewLine +
							string.Format(Resources.ImageInfoTooltip, image.Size.Width, image.Size.Height, image.Palette.Count);
						toolTip.SetToolTip(imageButton, tooltip);

						panelLastOpenFiles.Controls.Add(imageButton);
					}
				}
			}
		}

		#endregion

		#region Buttons clicked event handlers

		void buttonNew_Click(object sender, EventArgs e)
		{
			NewButtonClicked?.Invoke(this, e);
		}

		public event EventHandler NewButtonClicked;

		void buttonOpen_Click(object sender, EventArgs e)
		{
			using (var openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = Resources.FileFilterSAE;
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					OnOpenButtonClicked(openFileDialog.FileName);
				}
			}
		}

		void ImageButton_Click(object sender, EventArgs e)
		{
			OnOpenButtonClicked(((FlatButton)sender).Tag.ToString());
		}

		void OnOpenButtonClicked(string fileName)
		{
			OpenButtonClicked?.Invoke(this, new OpenFileEventArgs(fileName));
		}

		public event EventHandler<OpenFileEventArgs> OpenButtonClicked;

		public class OpenFileEventArgs : EventArgs
		{
			public OpenFileEventArgs(string fileName)
			{
				FileName = fileName;
			}

			public string FileName { get; }
		}

		#endregion

		#region Link buttons

		void buttonWeb_Click(object sender, EventArgs e)
		{
			AppInfo.GoToWebsite();
		}

		void buttonFeedback_Click(object sender, EventArgs e)
		{
			AppInfo.EmailToSupport();
		}

		void buttonFacebook_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.facebook.com/DiamondArtCreator/");
		}

		void buttonTwitter_Click(object sender, EventArgs e)
		{
			Process.Start("http://twitter.com/StitchArtEasy");
		}

		void buttonLinkedIn_Click(object sender, EventArgs e)
		{
			Process.Start("http://linkedin.com/in/mykolakovalchuk");
		}

		#endregion

		#region Updates

		internal void SetUpdatesLink()
		{
			if (string.IsNullOrEmpty(mainForm.UpdateDetails))
			{
				return;
			}

			var linkLabel = new LinkLabel
			{
				Text = Resources.LinkLabelNewVersionAvailable,
				AutoSize = true,
				Left = buttonRegister.Visible ? buttonRegister.Left + buttonRegister.Width + 16 : buttonRegister.Left,
				Top = buttonRegister.Top + 13
			};
			linkLabel.LinkClicked += (sender, e) => MainForm.ShowUpdateDialog(mainForm.UpdateDetails, this);
			panelBottom.Controls.Add(linkLabel);
		}

		#endregion

		#region Registration

		void buttonRegister_Click(object sender, EventArgs e)
		{
			RegistrationHelper.ShowRegisterDialog(this);
			if (RegistrationHelper.IsRegistered)
			{
				buttonRegister.Visible = false;
			}
		}

		#endregion

		#region Language

		void SetLanguageButtons()
		{
			switch (Settings.Default.Locale)
			{
				case "fr-FR":
					buttonFrench.IsSelected = true;
					break;
				case "en-US":
					buttonEnglish.IsSelected = true;
					break;
				default:
					buttonGerman.IsSelected = true;
					break;
			}
		}

		void buttonEnglish_Click(object sender, EventArgs e)
		{
			SetLanguage("en-US");
		}
		private void buttonGerman_Click(object sender, EventArgs e)
		{
			SetLanguage("de");
		}
		private void buttonFrench_Click(object sender, EventArgs e)
		{
			SetLanguage("fr-FR");
		}
		void SetLanguage(string cultureName)
		{
			AppInfo.SetSelectedLanguage(cultureName, false);
			Dispose();
		}

		#endregion

		private void ribbonOrbMenuItemOptions_Click(object sender, EventArgs e)
		{
			using (var optionsDialog = new OpitonsDialog())
			{
				if (optionsDialog.ShowDialog(this) == DialogResult.OK)
				{
					//maybe something to do here...
				}
			}
		}

		private void flatButtonThreadManagement_Click(object sender, EventArgs e)
		{
			using (var threadsManageDialog = new ThreadsManagementDialog())
			{
				threadsManageDialog.ShowDialog(this);
			}
		}

		private void flatButtonRegister_Click(object sender, EventArgs e)
		{
			//using own register form

			using (var registerDialog = new RegisterDialog())
			{
				registerDialog.ShowDialog(this);
				CheckRegistration();
			}
		}

		/*private void flatButtonTrialTest_Click(object sender, EventArgs e)
		{
            GenerateLicenseForCustomer("DAC", "Trial", "");
            // set Trial-License
            var  strRegistrationResult = tlli.Register(Settings.Default.UserPalettesLocation + "\\KEY\\trial.lic");
			
			if (strRegistrationResult == "OK")
			{
				File.Delete(Settings.Default.UserPalettesLocation + "\\KEY\\trial.lic");

				flatButtonRegister.Visible = true;
				flatButtonTrialTest.Visible = false;
				labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + Environment.NewLine +
					"Trial-Version" + Environment.NewLine + "Aktiv bis: " + tlli.MyLicense.ExpirationDate;

				DialogResult result = MessageBox.Show("Trial-Version für 10 Tage aktiviert." + Environment.NewLine + Environment.NewLine + "DAC muss neu gestartet werden !!!" +
					Environment.NewLine + Environment.NewLine + "Viel Spaß beim testen !", "Trial-Version aktiviert!", MessageBoxButtons.OK);
				Application.Restart();
			}
			else
			{
				//MessageBox.Show(strRegistrationResult);
			}
		}*/

		private void flatButtonAbout_Click(object sender, EventArgs e)
		{
			using (var aboutDialog = new AboutDialog())
			{
				aboutDialog.ShowDialog(this);
			}
		}

        /*private void button1_Click(object sender, EventArgs e)
        {
			LoadLicenseStatus();
		}*/

        private void flatButton1_Click(object sender, EventArgs e)
        {
			string filename;
			using (var symbolManager = new ThreadsManagementDialogSymbols())
			{
				symbolManager.ShowDialog(this);
				symbolManager.Dispose();
				filename = Settings.Default.UserPalettesLocation + @"\osc.txt";

				if (!Directory.Exists(Settings.Default.UserPalettesLocation))
				{
					Directory.CreateDirectory(Settings.Default.UserPalettesLocation);
				}
			}
		}

        private void btnLizenceChange_Click(object sender, EventArgs e)
        {
			if (btnLizenceChange.Text == "Free")
            {
				Settings.Default.isLizenzValid = false; isLizenzValid = false;
				Settings.Default.isLizenzCommerc = false; isLizenzCommerc = false;
				Settings.Default.isLizenzDemo = false; isLizenzDemo = false;
				btnLizenceChange.Text = "Kommerziell";
			} 
			else 
            {
				Settings.Default.isLizenzValid = true; isLizenzValid = true;
				Settings.Default.isLizenzCommerc = false; isLizenzCommerc = false;
				Settings.Default.isLizenzDemo = false; isLizenzDemo = false;
				btnLizenceChange.Text = "Free";
			}
        }

        /*private void button2_Click(object sender, EventArgs e)
        {

			//tlli.AsyncSendUsageInfo("Try to register: " + DateTime.Now);
			string serialnumber = tlli.MyLicense.SerialNumber;
			string ownermail = tlli.MyLicense.OwnerName;

			tlli.DeleteLicenseInRegistry();
            var reply = tlli.ActivateSerial(serialnumber, Settings.Default.ForeName, Settings.Default.SureName, ownermail);
            if (reply == "OK")
            {
                MessageBox.Show("Vielen Dank dass Sie DAC registriert haben !" + Environment.NewLine + Environment.NewLine +
                    "Der DAC wird neu gestartet um die Registrierung abzuschließen !!!");

                Application.Restart();
            }
            else
            {
                MessageBox.Show(reply, "Error registering", MessageBoxButtons.OK);
            }

			LoadLicenseStatus();
        }*/
    }
}
