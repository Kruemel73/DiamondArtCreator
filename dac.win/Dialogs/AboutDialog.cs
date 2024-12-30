using System;
using System.Windows.Forms;
using Ravlyk.SAE5.WinForms.Properties;
//using TreeksLicensingLibrary2.EasyIntegration;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class AboutDialog : Form
	{
		public AboutDialog()
		{
			InitializeComponent();

			var toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };
			toolTip.SetToolTip(buttonWeb, Resources.HintHomeButtonWeb);
			toolTip.SetToolTip(buttonFeedback, Resources.HintHomeButtonFeedback);

			//labelVersion.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion);

			//if (tlli.MyLicense == null)
			//{
				string newLine = Environment.NewLine;
				textBox1.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + newLine +
					"(c) 2021 by Ralf Kostka" + newLine + newLine +
					"" + newLine + newLine +
					"Dank geht an:" + newLine +
					"Andrea, Birthe, Judith, Katrin, Marie, Patzi, Sandi, Sasii, Tatjana, Vivi und Yvonne" + newLine + newLine +
					"Spezieller Dank geht an:" + newLine +
					"Franziska, Birthe, Boo, Katrin, Manja und Sandra" + newLine + newLine;
			//}
			/*else if (tlli.MyLicense.IsDemo == true && tlli.IsLicenseValid() == true)
			{
				string newLine = Environment.NewLine;
				textBox1.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + newLine +
					"(c) 2021 - 2024 by Ralf Kostka" + newLine + newLine +
					"Trial-Version" + newLine + newLine +
					"Dank geht an:" + newLine +
					"Andrea, Birthe, Judith, Katrin, Marie, Patzi, Sandi, Sasii, Tatjana, Vivi und Yvonne" + newLine + newLine +
					"Spezieller Dank geht an:" + newLine +
					"Franziska, Birthe, Boo, Katrin, Manja und Sandra" + newLine + newLine;
			}
			else if (tlli.MyLicense.IsDemo == false && tlli.IsLicenseValid() == true)
			{
				string newLine = Environment.NewLine;
				textBox1.Text = string.Format(Resources.LabelVersion, AppInfo.AppVersion) + newLine +
					"(c) 2021 - 2024 by Ralf Kostka" + newLine + newLine +
					"Registriert für: " + tlli.MyLicense.OwnerName + newLine + newLine +
					"Dank geht an:" + newLine +
					"Andrea, Birthe, Judith, Katrin, Marie, Patzi, Sandi, Sasii, Tatjana, Vivi und Yvonne" + newLine + newLine +
					"Spezieller Dank geht an:" + newLine +
					"Franziska, Birthe, Boo, Katrin, Manja und Sandra" + newLine + newLine;
			}*/

		}

		//public static TLLInterface tlli = new TLLInterface(SAE5.WinForms.Properties.Resources.chunk, "");

		void buttonWeb_Click(object sender, EventArgs e)
		{
			AppInfo.GoToWebsite();
		}

		void buttonFeedback_Click(object sender, EventArgs e)
		{
			AppInfo.EmailToSupport();
		}

        private void buttonFacebook_Click(object sender, EventArgs e)
        {
			System.Diagnostics.Process.Start("http://facebook.com/diamondartcreator");
		}
    }
}
