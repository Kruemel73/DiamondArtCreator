using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using UpdateMe;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class UpdateDialog : Form
	{
		public const string updaterPrefix = "";
		private static string processToEnd = "DAC";
		private static string DownloadUrl = "https://www.diamondartcreator.de/Download/";
		private string downloadFile = "update.zip";
		//private static string postProcess = Application.StartupPath + @"\" + processToEnd + ".exe";
		private static string postProcess = "";
		public static string updater = Application.StartupPath + @"\update.exe";

		public UpdateDialog(string details)
		{
			InitializeComponent();
			textBoxDetails.Text = details;
			textBoxDetails.SelectionLength = 0;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			textBoxDetails.SelectionStart = 0;
			textBoxDetails.SelectionLength = 0;
		}

		void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		void buttonDownload_Click(object sender, EventArgs e)
		{
			//OpenDownloadPage();
			string currentCulture = Thread.CurrentThread.CurrentUICulture.ToString();
			if (currentCulture == "de-DE")
            {
				MessageBox.Show("Um das Update zu installieren wird DAC beendet. Nach der Installation starten Sie bitte DAC neu!", "Update installieren", MessageBoxButtons.OK);
            }
			else if (currentCulture == "fr-FR")
			{
				MessageBox.Show("Pour installer la mise à jour, DAC est fermé. Après l'installation, veuillez redémarrer DAC!", "Installer les mises à jour", MessageBoxButtons.OK);
			}
			else if (currentCulture == "en-US")
			{
				MessageBox.Show("To install the update, DAC is closed. After the installation please restart DAC!", "Install update", MessageBoxButtons.OK);
			}
			update.installUpdateRestart(DownloadUrl, downloadFile, "\"" + Application.StartupPath + "\\", processToEnd, postProcess, "updated", updater);
			//Process.Start("update.exe", AppInfo.AppVersion);
			Application.Exit();
        }

		void OpenDownloadPage()
		{
			Process.Start("https://www.diamondartcreator.de");
		}
	}
}
