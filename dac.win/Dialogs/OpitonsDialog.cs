using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing.Properties;
using Ravlyk.SAE5.WinForms.Properties;
using GdiColor = System.Drawing.Color;
using Rectangle = Ravlyk.Common.Rectangle;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class OpitonsDialog : Form
	{
		public OpitonsDialog()
		{
			InitializeComponent();

			isLizenzValid = Settings.Default.isLizenzValid;
			isLizenzDemo = Settings.Default.isLizenzDemo;
			isLizenzCommerc = Settings.Default.isLizenzCommerc;

			checkBoxCheckForUpdates.Checked = Settings.Default.CheckForUpdatesAtStartup;

			if (isLizenzValid)
            {
				checkBoxOwnSymbolsPerColor.Visible = true;
			}

			switch (Settings.Default.Locale)
			{
				case "fr":
					comboBoxLanguage.SelectedIndex = 2;
					break;
				case "en-us":
					comboBoxLanguage.SelectedIndex = 1;
					break;
				case "de":
					comboBoxLanguage.SelectedIndex = 0;
					break;
				default:
					comboBoxLanguage.SelectedIndex = 0;
					break;
			}

			textBoxUserPalettesLocation.Text = Settings.Default.UserPalettesLocationSafe;
			SetButtonColor(buttonLineArgb, GridPainterSettings.Default.LineArgb.ToArgb());
			SetButtonColor(buttonLine5Argb, GridPainterSettings.Default.Line5Argb.ToArgb());
			SetButtonColor(buttonLine10Argb, GridPainterSettings.Default.Line10Argb.ToArgb());
			SetButtonColor(buttonNumbersArgb, GridPainterSettings.Default.NumbersArgb.ToArgb());
			SetButtonColor(buttonSelectionArgb1, GridPainterSettings.Default.SelectionArgb1.ToArgb());
			SetButtonColor(buttonSelectionArgb2, GridPainterSettings.Default.SelectionArgb2.ToArgb());
			checkBoxLine10Double.Checked = GridPainterSettings.Default.Line10DoubleWidth;
			checkBoxOwnSymbols.Checked = Settings.Default.OwnSymbols;
			checkBoxOwnSymbolsPerColor.Checked = Settings.Default.OwnSymbolsColor;
			textBoxBagsize.Text = SAEWizardSettings.Default.Bagsize.ToString();
			textBoxReserve.Text = SAEWizardSettings.Default.Reserve.ToString();
			
			var toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };

			toolTip.SetToolTip(buttonReset, Resources.HintOptionsReset);
		}

		public bool isLizenzValid;
		public bool isLizenzDemo;
		public bool isLizenzCommerc;

		string OldPalettesLocation = Settings.Default.UserPalettesLocationSafe;

		static void SetButtonColor(ButtonBase button, int argb)
		{
			const int ImageWidth = 20;

			var image = new IndexedImage { Size = new Size(ImageWidth, ImageWidth) };
			ImagePainter.FillRect(image, new Rectangle(0, 0, ImageWidth, ImageWidth), argb);

			var oldBitmap = button.Image;
			button.Image = image.ToBitmap();
			oldBitmap?.Dispose();

			button.Tag = argb;
		}

		void buttonGlidLinesColor_Click(object sender, EventArgs e)
		{
			var button = sender as ButtonBase;
			if (button != null)
			{
				using (var colorDialog = new ColorDialog())
				{
					var argb = button.Tag as int? ?? GdiColor.Black.ToArgb();
					var color = GdiColor.FromArgb(argb);
					colorDialog.Color = color;
					colorDialog.FullOpen = true;

					if (colorDialog.ShowDialog(this) == DialogResult.OK)
					{
						SetButtonColor(button, colorDialog.Color.ToArgb());
					}
				}
			}
		}

		void buttonOk_Click(object sender, EventArgs e)
		{
			Settings.Default.CheckForUpdatesAtStartup = checkBoxCheckForUpdates.Checked;
			Settings.Default.UserPalettesLocation = textBoxUserPalettesLocation.Text;
			
			// copy symbol lists to new palettes location
			string filetocopy =  OldPalettesLocation + "\\os.txt";
			string newlocation = Settings.Default.UserPalettesLocation + "\\os.txt";
			try
            {
				System.IO.File.Move(filetocopy, newlocation);
            }
            catch { }
			filetocopy = OldPalettesLocation + "\\osc.txt";
			newlocation = Settings.Default.UserPalettesLocation + "\\osc.txt";
			try
			{
				System.IO.File.Move(filetocopy, newlocation);
			}
			catch { }

			Settings.Default.OwnSymbols = checkBoxOwnSymbols.Checked;
			Settings.Default.OwnSymbolsColor = checkBoxOwnSymbolsPerColor.Checked;
			if (textBoxBagsize.Text == "") textBoxBagsize.Text = "190";
			if (textBoxReserve.Text == "") textBoxReserve.Text = "0";
			SAEWizardSettings.Default.Bagsize = Int32.Parse(textBoxBagsize.Text);
			SAEWizardSettings.Default.Reserve = Int32.Parse(textBoxReserve.Text);
			switch (comboBoxLanguage.SelectedIndex)
			{
				case 1:
					AppInfo.SetSelectedLanguage("en-US", true);
					break;
				case 2:
					AppInfo.SetSelectedLanguage("fr-FR", true);
					break;
				default:
					AppInfo.SetSelectedLanguage("de", true);
					break;
			}

			Settings.Default.Save();

			GridPainterSettings.Default.LineArgb = buttonLineArgb.Tag is int ? GdiColor.FromArgb((int)buttonLineArgb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.Line5Argb = buttonLine5Argb.Tag is int ? GdiColor.FromArgb((int)buttonLine5Argb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.Line10Argb = buttonLine10Argb.Tag is int ? GdiColor.FromArgb((int)buttonLine10Argb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.NumbersArgb = buttonNumbersArgb.Tag is int ? GdiColor.FromArgb((int)buttonNumbersArgb.Tag) : GdiColor.Black;
			GridPainterSettings.Default.SelectionArgb1 = buttonSelectionArgb1.Tag is int ? GdiColor.FromArgb((int)buttonSelectionArgb1.Tag) : GdiColor.Black;
			GridPainterSettings.Default.SelectionArgb2 = buttonSelectionArgb2.Tag is int ? GdiColor.FromArgb((int)buttonSelectionArgb2.Tag) : GdiColor.White;
			GridPainterSettings.Default.Line10DoubleWidth = checkBoxLine10Double.Checked;

			GridPainterSettings.Default.Save();
		}

		void buttonReset_Click(object sender, EventArgs e)
		{
			checkBoxCheckForUpdates.Checked = true;
			//comboBoxLanguage.SelectedIndex = 0;

			SetButtonColorWithDefaultValue(buttonLineArgb, GridPainterSettings.Default, nameof(GridPainterSettings.LineArgb));
			SetButtonColorWithDefaultValue(buttonLine5Argb, GridPainterSettings.Default, nameof(GridPainterSettings.Line5Argb));
			SetButtonColorWithDefaultValue(buttonLine10Argb, GridPainterSettings.Default, nameof(GridPainterSettings.Line10Argb));
			SetButtonColorWithDefaultValue(buttonNumbersArgb, GridPainterSettings.Default, nameof(GridPainterSettings.NumbersArgb));
			SetButtonColorWithDefaultValue(buttonSelectionArgb1, GridPainterSettings.Default, nameof(GridPainterSettings.SelectionArgb1));
			SetButtonColorWithDefaultValue(buttonSelectionArgb2, GridPainterSettings.Default, nameof(GridPainterSettings.SelectionArgb2));
			checkBoxLine10Double.Checked = true;

			System.Media.SystemSounds.Beep.Play();
		}

		void SetButtonColorWithDefaultValue(ButtonBase button, ApplicationSettingsBase settings, string propertyName)
		{
			var property = settings.Properties[propertyName];
			if (property?.DefaultValue != null)
			{
				try
				{
					var color = (GdiColor?)new ColorConverter().ConvertFrom(property.DefaultValue);
					if (color.HasValue)
					{
						SetButtonColor(button, color.Value.ToArgb());
					}
				}
				catch (InvalidCastException) { }
				catch (NotSupportedException) { }
			}
		}

		void buttonSelectUserPalettesLocation_Click(object sender, EventArgs e)
		{
			using (var folderDialog = new FolderBrowserDialog { ShowNewFolderButton = true })
			{
				folderDialog.SelectedPath = textBoxUserPalettesLocation.Text;
				if (folderDialog.ShowDialog(this) == DialogResult.OK)
				{
					textBoxUserPalettesLocation.Text = folderDialog.SelectedPath;
				}
			}
		}

        private void checkBoxOwnSymbolsPerColor_Click(object sender, EventArgs e)
        {
			checkBoxOwnSymbols.Checked = false;
        }

        private void checkBoxOwnSymbols_Click(object sender, EventArgs e)
        {
			checkBoxOwnSymbolsPerColor.Checked = false;
        }

        private void panelButtons_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panelBottom_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        private void tabPageGeneral_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabControlOptinos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBoxReserve_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void textBoxBagsize_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxOwnSymbols_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxOwnSymbolsPerColor_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxCheckForUpdates_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabPageGrid_Click(object sender, EventArgs e)
        {

        }

        private void groupBoxSelectionRectColors_Enter(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxLine10Double_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tabPageThreads_Click(object sender, EventArgs e)
        {

        }

        private void textBoxUserPalettesLocation_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void tabPagePDFSecurity_Click(object sender, EventArgs e)
        {

        }

        private void txtPDFPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtOwnerPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtOwner_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCompany_TextChanged(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}
