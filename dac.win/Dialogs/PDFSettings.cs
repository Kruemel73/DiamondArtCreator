using Ravlyk.SAE.Drawing.Properties;
using Ravlyk.SAE5.WinForms.Properties;
using Ravlyk.SAE5.WinForms.UserControls;
using System;
using System.Windows.Forms;
//using TreeksLicensingLibrary2.EasyIntegration;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
    public partial class PDFSettings : Form
    {
        public PDFSettings()
        {
            InitializeComponent();

            isLizenzValid = Settings.Default.isLizenzValid;
            isLizenzDemo = Settings.Default.isLizenzDemo;
            isLizenzCommerc = Settings.Default.isLizenzCommerc;
            textBoxPictureName.Text = Settings.Default.PictureName;
            Settings.Default.SymNormal = true;
            Settings.Default.SymFull = false;

            //don't allow etiketten in landscape mode
            if (Settings.Default.PrintPageLandscape) 
            {
                gbEtiketten.Enabled = false;
                gbEtiketten.Text = "Etiketten - Nur im Hochformat!";
            }

            if (isLizenzValid)
            {
                if (!isLizenzCommerc)
                {
                    if (Settings.Default.FT3)
                    {
                        gbVorlage.Enabled = true;
                        gbEtiketten.Enabled = true;
                        cbRundEti.Enabled = true;
                        cbRundEti.Checked = false;
                        cbWithoutDMC.Checked = false;
                        cbWithoutDMC.Enabled = true;
                        cbTicTac.Enabled = true;
                        cbTicTac.Checked = false;
                        cbLegend.Enabled = true;                    
                        checkBoxEtiUmrandung.Enabled = true;
                        checkBoxEtiUmrandung.Checked = false;
                        checkBoxDeckblatt.Enabled = true;
                        checkBoxDeckblatt.Checked = false;
                        checkBoxPrintScheme.Enabled = true;
                        checkBoxPrintScheme.Checked = true;
                        lblLinks.Enabled = true;
                        lblOben.Enabled = true;
                        txtVL.Enabled = true;
                        txtVO.Enabled = true;
                        cbSymSizeFull.Enabled = true;
                        cbSymSizeNormal.Enabled = true;
                    }

                    if (Settings.Default.FT6)
                    {
                        cbLegend.Enabled = true;
                        label14.Enabled = false;
                        txtOwnerPassword.Enabled = false;
                        txtOwnerPassword.Enabled = false;
                        txtOwnerPassword.Text = "Pro-Version only !";
                        label15.Enabled = false;
                        txtPDFPassword.Enabled = false;
                        txtPDFPassword.Enabled = false;
                        txtPDFPassword.Text = "Pro-Version only !";
                        checkBoxMystery.Enabled = false;
                        checkBoxMystery.Checked = false;
                        checkBoxDeckblatt.Enabled = true;
                        checkBoxPrintScheme.Enabled = true;
                        lblLinks.Enabled = true;
                        lblOben.Enabled = true;
                        txtVL.Enabled = true;
                        txtVO.Enabled = true;
                        checkBoxMystery.Checked = Settings.Default.Mystery;
                        lblMysteryPic.Enabled = true;
                        btnMyteryPicFile.Enabled = true;
                        checkBoxPrintScheme.Checked = Settings.Default.PrintScheme;
                        checkBoxDeckblatt.Checked = Settings.Default.PrintDeckblatt;
                        label13.Enabled = true;
                        txtOwner.Enabled = true;
                        label14.Enabled = true;
                        label15.Enabled = true;
                        labelPictureName.Enabled = true;
                        textBoxPictureName.Enabled = true;
                        cbLegendLeft.Enabled = false;
                        cbLegendLeft.Checked = false;

                        checkBoxPermitAccessibilityExtractContent.Enabled = false;
                        checkBoxPermitAnnotations.Enabled = false;
                        checkBoxPermitAssembleDocument.Enabled = false;
                        checkBoxPermitExtractContent.Enabled = false;
                        checkBoxPermitFormsFill.Enabled = false;
                        checkBoxPermitFullQualityPrint.Enabled = false;
                        checkBoxPermitModifyDocument.Enabled = false;
                        checkBoxPermitPrint.Enabled = false;
                    }

                    if (Settings.Default.FT8)
                    {
                        cbLegendLeft.Enabled = true;
                        cbLegendLeft.Checked = false;
                    }
                    if (Settings.Default.FT10)
                    {
                        checkBoxMystery.Enabled = true;
                        checkBoxMystery.Checked = false;
                        tbMysteryPic.Enabled = true;
                        lblMysteryPic.Enabled = true;
                        btnMyteryPicFile.Enabled = true;
                        tbMysteryPic.Text = Settings.Default.MysteryPic;
                    }
                }

                cbGitternetz.Checked = GridPainterSettings.Default.ShowLines;
                cbLine10.Checked = GridPainterSettings.Default.Line10DoubleWidth;
                cbLineal.Checked = GridPainterSettings.Default.ShowRulers;
                cbSchnittlinien.Checked = GridPainterSettings.Default.CutLines;
                cbLegend.Enabled = true;

            }
            if (isLizenzValid)
            {
                if (isLizenzCommerc || isLizenzDemo || Settings.Default.FT6)
                {
                    txtCompany.Enabled = true;
                    labelComapny.Enabled = true;
                    buttonSelectLogoLocation.Enabled = true;
                    labelLogo.Enabled = true;
                    textBoxLogo.Enabled = true;
                    cbRundEti.Enabled = true;
                    cbRundEti.Checked = false;
                    cbTicTac.Enabled = true;
                    cbTicTac.Checked = false;
                    cbWithoutDMC.Enabled = true;
                    cbWithoutDMC.Checked = false;
                    checkBoxEtiUmrandung.Enabled = true;
                    checkBoxEtiUmrandung.Checked = false;
                    checkBoxDeckblatt.Enabled = true;
                    checkBoxPrintScheme.Enabled = true;
                    checkBoxMystery.Checked = Settings.Default.Mystery;
                    lblMysteryPic.Enabled = true;
                    btnMyteryPicFile.Enabled = true;
                    checkBoxPrintScheme.Checked = Settings.Default.PrintScheme;
                    lblLinks.Enabled = true;
                    lblOben.Enabled = true;
                    txtVL.Enabled = true;
                    txtVO.Enabled = true;

                    cbSymSizeFull.Enabled = true;
                    cbSymSizeNormal.Enabled = true;

                    txtOwnerPassword.Enabled = true;
                    txtOwnerPassword.Enabled = true;
                    txtOwnerPassword.Text = "";
                    txtPDFPassword.Enabled = true;
                    txtPDFPassword.Enabled = true;
                    txtPDFPassword.Text = "";
                    checkBoxMystery.Enabled = true;
                    checkBoxMystery.Checked = false;
                    cbLegendLeft.Enabled = true;
                    cbLegendLeft.Checked = false;

                    textBoxLogo.Text = Settings.Default.Logo;
                    txtCompany.Text = Settings.Default.Company;
                    txtOwner.Text = Settings.Default.Owner;
                    txtOwner.Enabled = true;
                    label13.Enabled = true;
                    label14.Enabled = true;
                    txtOwnerPassword.Text = Settings.Default.OwnerPassword;
                    label15.Enabled = true;
                    txtPDFPassword.Text = Settings.Default.PDFPassword;
                    labelPictureName.Enabled = true;
                    textBoxPictureName.Enabled = true;
                    lblMysteryPic.Enabled = true;
                    tbMysteryPic.Enabled = true;
                    checkBoxPermitAccessibilityExtractContent.Enabled = false;
                    checkBoxPermitAnnotations.Enabled = false;
                    checkBoxPermitAssembleDocument.Enabled = false;
                    checkBoxPermitExtractContent.Enabled = false;
                    checkBoxPermitFormsFill.Enabled = false;
                    checkBoxPermitFullQualityPrint.Enabled = false;
                    checkBoxPermitModifyDocument.Enabled = false;
                    checkBoxPermitPrint.Enabled = false;
                    checkBoxDeckblatt.Checked = Settings.Default.PrintDeckblatt;
                    cbRundEti.Checked = Settings.Default.PrintEtiketten;
                    checkBoxPrintScheme.Checked = Settings.Default.PrintScheme;
                    checkBoxEtiUmrandung.Checked = Settings.Default.EtiUmrandung;
                    checkBoxMystery.Checked = Settings.Default.Mystery;
                    tbMysteryPic.Text = Settings.Default.MysteryPic;

                }
            }
        }

        //public static TLLInterface tlli = new TLLInterface(SAE5.WinForms.Properties.Resources.chunk, "5ss8:,UaAUhzTE?9trSjSynsxDxTRbn");
        public bool isLizenzValid;
        public bool isLizenzDemo;
        public bool isLizenzCommerc;

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Settings.Default.Company = txtCompany.Text;
            Settings.Default.Owner = txtOwner.Text;
            if (isLizenzValid && isLizenzCommerc || Settings.Default.FT6)
            {
                Settings.Default.OwnerPassword = txtOwnerPassword.Text;
                Settings.Default.PDFPassword = txtPDFPassword.Text;
            }
            else
            {
                Settings.Default.OwnerPassword = "";
                Settings.Default.PDFPassword = "";
            }

            if (!checkBoxDeckblatt.Checked && !checkBoxPrintScheme.Checked && !cbRundEti.Checked && !cbTicTac.Checked && !cbLegend.Checked && !cbLegendLeft.Checked)
            {
                checkBoxPrintScheme.Checked = true;
            }

            Settings.Default.PictureName = textBoxPictureName.Text;
            Settings.Default.Logo = textBoxLogo.Text;
            Settings.Default.MysteryPic = tbMysteryPic.Text;
            Settings.Default.PrintDeckblatt = checkBoxDeckblatt.Checked;
            Settings.Default.PrintEtiketten = cbRundEti.Checked;
            Settings.Default.PrintScheme = checkBoxPrintScheme.Checked;
            Settings.Default.PermitPrint = checkBoxPermitPrint.Checked;
            Settings.Default.PermitModifyDocument = checkBoxPermitModifyDocument.Checked;
            Settings.Default.PermitAccessibilityExtractContent = checkBoxPermitAccessibilityExtractContent.Checked;
            Settings.Default.PermitExtractContent = checkBoxPermitExtractContent.Checked;
            Settings.Default.PermitAnnotations = checkBoxPermitAnnotations.Checked;
            Settings.Default.PermitAssembleDocument = checkBoxPermitAssembleDocument.Checked;
            Settings.Default.PermitFormsFill = checkBoxPermitFormsFill.Checked;
            Settings.Default.PermitFullQualityPrint = checkBoxPermitFullQualityPrint.Checked;
            Settings.Default.Mystery = checkBoxMystery.Checked;
            Settings.Default.MysteryPic = tbMysteryPic.Text;
            Settings.Default.S = Convert.ToInt32 (txtVO.Text);
            Settings.Default.VL = Convert.ToInt32 (txtVL.Text);
            Settings.Default.EtiUmrandung = checkBoxEtiUmrandung.Checked;
            Settings.Default.TicTacEti = cbTicTac.Checked;
            Settings.Default.RundEti = cbRundEti.Checked;
            Settings.Default.SymFull = cbSymSizeFull.Checked;
            Settings.Default.SymNormal = cbSymSizeNormal.Checked;

            Settings.Default.Line10Dbl = cbLine10.Checked;
            Settings.Default.ShowRulers = cbLineal.Checked;
            Settings.Default.ShowLines = cbGitternetz.Checked;
            Settings.Default.CutLines = cbSchnittlinien.Checked;
            Settings.Default.SchemeLegend = cbLegend.Checked;
            Settings.Default.SchemeLogo = cbLogo.Checked;
            Settings.Default.LegendLeft = cbLegendLeft.Checked;
            Settings.Default.WithoutDMC = cbWithoutDMC.Checked;
            SAEWizardSettings.Default.WithoutDMC = cbWithoutDMC.Checked;
            Dispose();
            this.Close();
        }

        private void txtOwnerPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtOwnerPassword.Text != "")
            {
                checkBoxPermitAccessibilityExtractContent.Enabled = true;
                checkBoxPermitAnnotations.Enabled = true;
                checkBoxPermitAssembleDocument.Enabled = true;
                checkBoxPermitExtractContent.Enabled = true;
                checkBoxPermitFormsFill.Enabled = true;
                checkBoxPermitFullQualityPrint.Enabled = true;
                checkBoxPermitModifyDocument.Enabled = true;
                checkBoxPermitPrint.Enabled = true;
            }
            else
            {
                checkBoxPermitAccessibilityExtractContent.Enabled = false;
                checkBoxPermitAnnotations.Enabled = false;
                checkBoxPermitAssembleDocument.Enabled = false;
                checkBoxPermitExtractContent.Enabled = false;
                checkBoxPermitFormsFill.Enabled = false;
                checkBoxPermitFullQualityPrint.Enabled = false;
                checkBoxPermitModifyDocument.Enabled = false;
                checkBoxPermitPrint.Enabled = false;
            }
        }

        private void buttonSelectLogoLocation_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = Resources.FileFilterImages;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxLogo.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnMyteryPicFile_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = Resources.FileFilterImages;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tbMysteryPic.Text = openFileDialog.FileName;
                }
            }
        }

        private void cbSymSizeNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSymSizeNormal.Checked) { cbSymSizeFull.Checked = false; }
            
        }

        private void cbSymSizeFull_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSymSizeFull.Checked) { cbSymSizeNormal.Checked = false; }
        }

        private void cbGitternetz_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbGitternetz.Checked) { cbLine10.Checked = false; }
            cbLine10.Enabled = cbGitternetz.Checked;
           
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Dispose();
            this.Close();
        }

        private void cbSymSizeFull_CheckedChanged_1(object sender, EventArgs e)
        {
            cbSymSizeNormal.Checked = !cbSymSizeFull.Checked;
        }

        private void cbSymSizeNormal_CheckedChanged_1(object sender, EventArgs e)
        {
            cbSymSizeFull.Checked = !cbSymSizeNormal.Checked;
        }

        private void gbVorlage_Enter(object sender, EventArgs e)
        {

        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void gbLegendLogo_Enter(object sender, EventArgs e)
        {

        }

        private void cbLogo_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gbLines_Enter(object sender, EventArgs e)
        {

        }

        private void cbLine10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbLineal_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbSchnittlinien_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gbPDFSettings_Enter(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitFullQualityPrint_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitFormsFill_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitAnnotations_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitAccessibilityExtractContent_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitExtractContent_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitAssembleDocument_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitModifyDocument_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPermitPrint_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void gpSymbolSize_Enter(object sender, EventArgs e)
        {

        }

        private void gbEtiketten_Enter(object sender, EventArgs e)
        {

        }

        private void cbLegendLeft_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxMystery_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbLegend_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxPrintScheme_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxDeckblatt_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbWithoutDMC_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbTicTac_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtVO_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtVL_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblOben_Click(object sender, EventArgs e)
        {

        }

        private void lblLinks_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxEtiUmrandung_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbRundEti_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lblMysteryPic_Click(object sender, EventArgs e)
        {

        }

        private void tbMysteryPic_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelLogo_Click(object sender, EventArgs e)
        {

        }

        private void textBoxLogo_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxPictureName_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelPictureName_Click(object sender, EventArgs e)
        {

        }

        private void txtPDFPassword_TextChanged(object sender, EventArgs e)
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

        private void labelComapny_Click(object sender, EventArgs e)
        {

        }
    }
}
