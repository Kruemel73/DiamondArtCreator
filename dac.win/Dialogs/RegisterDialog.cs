using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Ravlyk.SAE5.WinForms.Properties;
//using TreeksLicensingLibrary2;
//using TreeksLicensingLibrary2.EasyIntegration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Ravlyk.SAE5.WinForms.Dialogs
{ 
	public partial class RegisterDialog : Form
	{
		public RegisterDialog()
		{
			InitializeComponent();
			/*tlli.HWID.EnableMoreDistinctMode = true;
			textBoxHWID.Text = tlli.GetActualHardwareID(); ;
			if (tlli.MyLicense != null) { 
				
				isAlreadyRegistered = true; }
			*/
		}

		//TLLInterface tlli = new TLLInterface(Resources.chunk, "");
        private const string myPublicKey = "";
        //private LicenseVerification verification = new LicenseVerification(myPublicKey);

        bool isAlreadyRegistered = false;
        public string MyDocumentPath;

        protected override void OnClosed(EventArgs e)
		{
			if (!RegistrationHelper.IsRegistered && RegistrationHelper.TrialDaysLeft <= 0)
			{
				Application.Exit();
			}
			else
			{
				base.OnClosed(e);
			}
		}

		void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		void buttonBuy_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.diamondartcreator.de/diamond-art-creator/diamond-art-creator-6/");
		}

		void buttonRegister_Click(object sender, EventArgs e)
		{
			bool registrationSuccessfull = true;
			string LicenseFilePath;
            MyDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (string.IsNullOrEmpty(tbLicenseFile.Text))
			{ 
				MessageBox.Show("Bitte Lizenz-Datei auswählen!", "Fehlende Lizenz-Datei!", MessageBoxButtons.OK);
			}
			else
			{
                /*LicenseFilePath = tbLicenseFile.Text;
                var Result = IsLicenseValid();

				if (Result)
				{
					var strRegistrationResult = tlli.Register(tbLicenseFile.Text);

					if (strRegistrationResult == "OK")
					{
						if (tlli.MyLicense.HardwareID.Count > 0)
						{
							tlli.MyLicense.EnableHardwareIDMoreDistinctMode = true;
							var hwid = tlli.GetActualHardwareID();
							string hwidreg = "";
							bool first = true;
							foreach (string hw in tlli.MyLicense.HardwareID)
							{
								if (first)
								{
									hwidreg += hw;
									first = false;
								}

								if (hwid != hwidreg)
								{
									registrationSuccessfull = false;
								}

							}
						}

						string OwnerName;
						string OwnerEmail;
						OwnerEmail = textBoxEmail.Text;
						OwnerName = textBoxForname.Text + " " + textBoxSurname.Text;
						if (tlli.MyLicense.OwnerName != OwnerName && tlli.MyLicense.OwnerName != OwnerEmail)
						{
							registrationSuccessfull = false;
						}
						if (tlli.MyLicense.SerialNumber != textBoxSerial.Text)
						{
							registrationSuccessfull = false;
						}

						if (registrationSuccessfull)
						{
							MessageBox.Show("Vielen Dank dass Sie DAC registriert haben !" + Environment.NewLine + Environment.NewLine +
							"Der DAC wird neu gestartet um die Registrierung abzuschließen !!!");
							//Application.Restart();

							var reply = tlli.ActivateSerial(textBoxSerial.Text, textBoxForname.Text, textBoxSurname.Text, textBoxEmail.Text);
							if (reply == "OK")
							{
								string KeyFile = Application.StartupPath + "/DAC.key";
								if (File.Exists(KeyFile))
								{
									File.Delete(KeyFile);
								}
							}
						}
						else
						{
							tlli.DeleteLicenseInRegistry();
							File.Delete(MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic");
							MessageBox.Show("Die Lizenzdatei ist auf diesem Computer nicht gültig!" + Environment.NewLine + Environment.NewLine + "Bitte prüfen Sie Ihre Eingaben!", "Ungültige Lizenz-Datei", MessageBoxButtons.OK);
						}
					}
					//generate License-File with HardwareLock
					if (tlli.MyLicense.HardwareID.Count == 0)
					{
						GenerateLicenseFile();
					}
                    try
                    {
                        File.Copy(tbLicenseFile.Text, MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic", true);
                    }
                    catch { }
                    Application.Restart();
                }
				else 
				{
					MessageBox.Show("Beim Einlesen der Lizenz-Datei ist ein Fehler aufgetreten!", "Fehlerhafte Lizenz-Datei", MessageBoxButtons.OK);
				}*/
            }
        }


        private void GenerateLicenseFile()
        {            
			/*string ErrorMessage = "";
			License LicenseObject = null;
            var hwid = tlli.GetActualHardwareID();

            License lic = new License();
            LicenseSigningKey key = new LicenseSigningKey();
            key.LoadFile(Application.StartupPath + "\\DAC6.lkey", "r*cJ@fV%nYzAG29QdwCFs$6X76");

            //license settings
            lic.ProductName = tlli.MyLicense.ProductName;
            lic.OwnerName = tlli.MyLicense.OwnerName;
            lic.Info2 = tlli.MyLicense.Info2;
            lic.EnableHardwareIDMoreDistinctMode = true;
			//lic.HardwareID = tlli.MyLicense.HardwareID;
			lic.HardwareID.Add(hwid);
            lic.SerialNumber = tlli.MyLicense.SerialNumber;
			lic.AllowedFeatures = tlli.MyLicense.AllowedFeatures;
			if (lic.AllowedFeatures.Count > 3)
			{
				lic.Info2 = "Commercial";
			}
            string strLicense = lic.GenerateLicenseString(key);
            lic.GenerateLicenseFile(key, MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic");
            var result = IsLicenseValid2();
			if (result)
			{
				try
				{
					File.Copy(MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic", tbLicenseFile.Text, true);
				}
				catch { }
			}*/
        }

        /*public bool IsLicenseValid(string ErrorMessage = "", License LicenseObject = null)
        {

            string LicenseFilePath = tbLicenseFile.Text;

            if (File.Exists(LicenseFilePath))
            {
                //var strLicense = File.ReadAllText(LicenseFilePath);
                if (verification.VerifyLicenseFile(LicenseFilePath, ref ErrorMessage, ref LicenseObject))
                {
                    return true;
                }
                else
                {
					if (ErrorMessage == "Invalid license data.")
					{
						ErrorMessage = "";
						string LicenseText = File.ReadAllText(LicenseFilePath);
						if (verification.VerifyLicenseString(LicenseText, ref ErrorMessage, ref LicenseObject))
						{
							return true;
						}
					}
					if (ErrorMessage == "This license is not valid on this computer.")
					{
						MessageBox.Show("Lizenz-Datei ist auf diesem Computer nicht gültig!", "Ungültige Lizenz-Datei", MessageBoxButtons.OK);
						return false;
					}
					return false;
                }
            }
            else
            {
                return false;
            }

        }*/

        /*public bool IsLicenseValid2(string ErrorMessage = "", License LicenseObject = null)
        {

            string LicenseFilePath = MyDocumentPath + "\\DiamondArtCreator\\DACLicense.lic";

            if (File.Exists(LicenseFilePath))
            {
                //var strLicense = File.ReadAllText(LicenseFilePath);
                if (verification.VerifyLicenseFile(LicenseFilePath, ref ErrorMessage, ref LicenseObject))

                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
		*/
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

			void textBoxEmail_TextChanged(object sender, EventArgs e)
		{
			buttonRegister.Enabled = !string.IsNullOrWhiteSpace(textBoxForname.Text) && !string.IsNullOrWhiteSpace(textBoxSerial.Text);
		}

		void buttonWeb_Click(object sender, EventArgs e)
		{
			AppInfo.GoToWebsite();
		}

		void buttonFeedback_Click(object sender, EventArgs e)
		{
			AppInfo.EmailToSupport();
		}

        private void flatButton1_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
				
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tbLicenseFile.Text = openFileDialog.FileName;
                }
            }
        }
    }
}
