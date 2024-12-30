
namespace Ravlyk.SAE5.WinForms.Dialogs
{
    partial class PDFSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFSettings));
            this.panelTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbLegendLogo = new System.Windows.Forms.GroupBox();
            this.cbLogo = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new Ravlyk.Drawing.WinForms.FlatDialogButton();
            this.gbLines = new System.Windows.Forms.GroupBox();
            this.cbLine10 = new System.Windows.Forms.CheckBox();
            this.cbGitternetz = new System.Windows.Forms.CheckBox();
            this.cbLineal = new System.Windows.Forms.CheckBox();
            this.cbSchnittlinien = new System.Windows.Forms.CheckBox();
            this.gbPDFSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxPermitFullQualityPrint = new System.Windows.Forms.CheckBox();
            this.checkBoxPermitFormsFill = new System.Windows.Forms.CheckBox();
            this.checkBoxPermitAnnotations = new System.Windows.Forms.CheckBox();
            this.checkBoxPermitAccessibilityExtractContent = new System.Windows.Forms.CheckBox();
            this.checkBoxPermitExtractContent = new System.Windows.Forms.CheckBox();
            this.checkBoxPermitAssembleDocument = new System.Windows.Forms.CheckBox();
            this.checkBoxPermitModifyDocument = new System.Windows.Forms.CheckBox();
            this.checkBoxPermitPrint = new System.Windows.Forms.CheckBox();
            this.gpSymbolSize = new System.Windows.Forms.GroupBox();
            this.cbSymSizeFull = new System.Windows.Forms.CheckBox();
            this.cbSymSizeNormal = new System.Windows.Forms.CheckBox();
            this.gbVorlage = new System.Windows.Forms.GroupBox();
            this.cbLegendLeft = new System.Windows.Forms.CheckBox();
            this.checkBoxMystery = new System.Windows.Forms.CheckBox();
            this.cbLegend = new System.Windows.Forms.CheckBox();
            this.checkBoxPrintScheme = new System.Windows.Forms.CheckBox();
            this.checkBoxDeckblatt = new System.Windows.Forms.CheckBox();
            this.gbEtiketten = new System.Windows.Forms.GroupBox();
            this.cbWithoutDMC = new System.Windows.Forms.CheckBox();
            this.cbTicTac = new System.Windows.Forms.CheckBox();
            this.txtVO = new System.Windows.Forms.TextBox();
            this.txtVL = new System.Windows.Forms.TextBox();
            this.lblOben = new System.Windows.Forms.Label();
            this.lblLinks = new System.Windows.Forms.Label();
            this.checkBoxEtiUmrandung = new System.Windows.Forms.CheckBox();
            this.cbRundEti = new System.Windows.Forms.CheckBox();
            this.lblMysteryPic = new System.Windows.Forms.Label();
            this.tbMysteryPic = new System.Windows.Forms.TextBox();
            this.btnMyteryPicFile = new Ravlyk.Drawing.WinForms.FlatButton();
            this.labelLogo = new System.Windows.Forms.Label();
            this.textBoxLogo = new System.Windows.Forms.TextBox();
            this.buttonSelectLogoLocation = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonOk = new Ravlyk.Drawing.WinForms.FlatDialogButton();
            this.textBoxPictureName = new System.Windows.Forms.TextBox();
            this.labelPictureName = new System.Windows.Forms.Label();
            this.txtPDFPassword = new System.Windows.Forms.TextBox();
            this.txtOwnerPassword = new System.Windows.Forms.TextBox();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelComapny = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.gbLegendLogo.SuspendLayout();
            this.gbLines.SuspendLayout();
            this.gbPDFSettings.SuspendLayout();
            this.gpSymbolSize.SuspendLayout();
            this.gbVorlage.SuspendLayout();
            this.gbEtiketten.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Name = "panelTop";
            this.panelTop.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTop_Paint);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.PDF48;
            this.pictureBox1.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.PDF48;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.InitialImage = global::Ravlyk.SAE5.WinForms.Properties.Resources.PDF48;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gbLegendLogo);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.gbLines);
            this.panel1.Controls.Add(this.gbPDFSettings);
            this.panel1.Controls.Add(this.gpSymbolSize);
            this.panel1.Controls.Add(this.gbVorlage);
            this.panel1.Controls.Add(this.gbEtiketten);
            this.panel1.Controls.Add(this.lblMysteryPic);
            this.panel1.Controls.Add(this.tbMysteryPic);
            this.panel1.Controls.Add(this.btnMyteryPicFile);
            this.panel1.Controls.Add(this.labelLogo);
            this.panel1.Controls.Add(this.textBoxLogo);
            this.panel1.Controls.Add(this.buttonSelectLogoLocation);
            this.panel1.Controls.Add(this.buttonOk);
            this.panel1.Controls.Add(this.textBoxPictureName);
            this.panel1.Controls.Add(this.labelPictureName);
            this.panel1.Controls.Add(this.txtPDFPassword);
            this.panel1.Controls.Add(this.txtOwnerPassword);
            this.panel1.Controls.Add(this.txtOwner);
            this.panel1.Controls.Add(this.txtCompany);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.labelComapny);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // gbLegendLogo
            // 
            this.gbLegendLogo.Controls.Add(this.cbLogo);
            resources.ApplyResources(this.gbLegendLogo, "gbLegendLogo");
            this.gbLegendLogo.Name = "gbLegendLogo";
            this.gbLegendLogo.TabStop = false;
            this.gbLegendLogo.Enter += new System.EventHandler(this.gbLegendLogo_Enter);
            // 
            // cbLogo
            // 
            resources.ApplyResources(this.cbLogo, "cbLogo");
            this.cbLogo.Name = "cbLogo";
            this.cbLogo.UseVisualStyleBackColor = true;
            this.cbLogo.CheckedChanged += new System.EventHandler(this.cbLogo_CheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.White;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Delete24;
            this.buttonCancel.IsSelected = false;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // gbLines
            // 
            this.gbLines.Controls.Add(this.cbLine10);
            this.gbLines.Controls.Add(this.cbGitternetz);
            this.gbLines.Controls.Add(this.cbLineal);
            this.gbLines.Controls.Add(this.cbSchnittlinien);
            resources.ApplyResources(this.gbLines, "gbLines");
            this.gbLines.Name = "gbLines";
            this.gbLines.TabStop = false;
            this.gbLines.Enter += new System.EventHandler(this.gbLines_Enter);
            // 
            // cbLine10
            // 
            resources.ApplyResources(this.cbLine10, "cbLine10");
            this.cbLine10.Name = "cbLine10";
            this.cbLine10.UseVisualStyleBackColor = true;
            this.cbLine10.CheckedChanged += new System.EventHandler(this.cbLine10_CheckedChanged);
            // 
            // cbGitternetz
            // 
            resources.ApplyResources(this.cbGitternetz, "cbGitternetz");
            this.cbGitternetz.Name = "cbGitternetz";
            this.cbGitternetz.UseVisualStyleBackColor = true;
            this.cbGitternetz.CheckedChanged += new System.EventHandler(this.cbGitternetz_CheckedChanged);
            // 
            // cbLineal
            // 
            resources.ApplyResources(this.cbLineal, "cbLineal");
            this.cbLineal.Name = "cbLineal";
            this.cbLineal.UseVisualStyleBackColor = true;
            this.cbLineal.CheckedChanged += new System.EventHandler(this.cbLineal_CheckedChanged);
            // 
            // cbSchnittlinien
            // 
            resources.ApplyResources(this.cbSchnittlinien, "cbSchnittlinien");
            this.cbSchnittlinien.Name = "cbSchnittlinien";
            this.cbSchnittlinien.UseVisualStyleBackColor = true;
            this.cbSchnittlinien.CheckedChanged += new System.EventHandler(this.cbSchnittlinien_CheckedChanged);
            // 
            // gbPDFSettings
            // 
            this.gbPDFSettings.BackColor = System.Drawing.SystemColors.Control;
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitFullQualityPrint);
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitFormsFill);
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitAnnotations);
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitAccessibilityExtractContent);
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitExtractContent);
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitAssembleDocument);
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitModifyDocument);
            this.gbPDFSettings.Controls.Add(this.checkBoxPermitPrint);
            resources.ApplyResources(this.gbPDFSettings, "gbPDFSettings");
            this.gbPDFSettings.Name = "gbPDFSettings";
            this.gbPDFSettings.TabStop = false;
            this.gbPDFSettings.Enter += new System.EventHandler(this.gbPDFSettings_Enter);
            // 
            // checkBoxPermitFullQualityPrint
            // 
            resources.ApplyResources(this.checkBoxPermitFullQualityPrint, "checkBoxPermitFullQualityPrint");
            this.checkBoxPermitFullQualityPrint.Name = "checkBoxPermitFullQualityPrint";
            this.checkBoxPermitFullQualityPrint.UseVisualStyleBackColor = true;
            this.checkBoxPermitFullQualityPrint.CheckedChanged += new System.EventHandler(this.checkBoxPermitFullQualityPrint_CheckedChanged);
            // 
            // checkBoxPermitFormsFill
            // 
            resources.ApplyResources(this.checkBoxPermitFormsFill, "checkBoxPermitFormsFill");
            this.checkBoxPermitFormsFill.Name = "checkBoxPermitFormsFill";
            this.checkBoxPermitFormsFill.UseVisualStyleBackColor = true;
            this.checkBoxPermitFormsFill.CheckedChanged += new System.EventHandler(this.checkBoxPermitFormsFill_CheckedChanged);
            // 
            // checkBoxPermitAnnotations
            // 
            resources.ApplyResources(this.checkBoxPermitAnnotations, "checkBoxPermitAnnotations");
            this.checkBoxPermitAnnotations.Name = "checkBoxPermitAnnotations";
            this.checkBoxPermitAnnotations.UseVisualStyleBackColor = true;
            this.checkBoxPermitAnnotations.CheckedChanged += new System.EventHandler(this.checkBoxPermitAnnotations_CheckedChanged);
            // 
            // checkBoxPermitAccessibilityExtractContent
            // 
            resources.ApplyResources(this.checkBoxPermitAccessibilityExtractContent, "checkBoxPermitAccessibilityExtractContent");
            this.checkBoxPermitAccessibilityExtractContent.Name = "checkBoxPermitAccessibilityExtractContent";
            this.checkBoxPermitAccessibilityExtractContent.UseVisualStyleBackColor = true;
            this.checkBoxPermitAccessibilityExtractContent.CheckedChanged += new System.EventHandler(this.checkBoxPermitAccessibilityExtractContent_CheckedChanged);
            // 
            // checkBoxPermitExtractContent
            // 
            resources.ApplyResources(this.checkBoxPermitExtractContent, "checkBoxPermitExtractContent");
            this.checkBoxPermitExtractContent.Name = "checkBoxPermitExtractContent";
            this.checkBoxPermitExtractContent.UseVisualStyleBackColor = true;
            this.checkBoxPermitExtractContent.CheckedChanged += new System.EventHandler(this.checkBoxPermitExtractContent_CheckedChanged);
            // 
            // checkBoxPermitAssembleDocument
            // 
            resources.ApplyResources(this.checkBoxPermitAssembleDocument, "checkBoxPermitAssembleDocument");
            this.checkBoxPermitAssembleDocument.Name = "checkBoxPermitAssembleDocument";
            this.checkBoxPermitAssembleDocument.UseVisualStyleBackColor = true;
            this.checkBoxPermitAssembleDocument.CheckedChanged += new System.EventHandler(this.checkBoxPermitAssembleDocument_CheckedChanged);
            // 
            // checkBoxPermitModifyDocument
            // 
            resources.ApplyResources(this.checkBoxPermitModifyDocument, "checkBoxPermitModifyDocument");
            this.checkBoxPermitModifyDocument.Name = "checkBoxPermitModifyDocument";
            this.checkBoxPermitModifyDocument.UseVisualStyleBackColor = true;
            this.checkBoxPermitModifyDocument.CheckedChanged += new System.EventHandler(this.checkBoxPermitModifyDocument_CheckedChanged);
            // 
            // checkBoxPermitPrint
            // 
            resources.ApplyResources(this.checkBoxPermitPrint, "checkBoxPermitPrint");
            this.checkBoxPermitPrint.Name = "checkBoxPermitPrint";
            this.checkBoxPermitPrint.UseVisualStyleBackColor = true;
            this.checkBoxPermitPrint.CheckedChanged += new System.EventHandler(this.checkBoxPermitPrint_CheckedChanged);
            // 
            // gpSymbolSize
            // 
            this.gpSymbolSize.Controls.Add(this.cbSymSizeFull);
            this.gpSymbolSize.Controls.Add(this.cbSymSizeNormal);
            resources.ApplyResources(this.gpSymbolSize, "gpSymbolSize");
            this.gpSymbolSize.Name = "gpSymbolSize";
            this.gpSymbolSize.TabStop = false;
            this.gpSymbolSize.Enter += new System.EventHandler(this.gpSymbolSize_Enter);
            // 
            // cbSymSizeFull
            // 
            resources.ApplyResources(this.cbSymSizeFull, "cbSymSizeFull");
            this.cbSymSizeFull.Name = "cbSymSizeFull";
            this.cbSymSizeFull.UseVisualStyleBackColor = true;
            this.cbSymSizeFull.CheckedChanged += new System.EventHandler(this.cbSymSizeFull_CheckedChanged_1);
            // 
            // cbSymSizeNormal
            // 
            resources.ApplyResources(this.cbSymSizeNormal, "cbSymSizeNormal");
            this.cbSymSizeNormal.Checked = true;
            this.cbSymSizeNormal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSymSizeNormal.Name = "cbSymSizeNormal";
            this.cbSymSizeNormal.UseVisualStyleBackColor = true;
            this.cbSymSizeNormal.CheckedChanged += new System.EventHandler(this.cbSymSizeNormal_CheckedChanged_1);
            // 
            // gbVorlage
            // 
            this.gbVorlage.Controls.Add(this.cbLegendLeft);
            this.gbVorlage.Controls.Add(this.checkBoxMystery);
            this.gbVorlage.Controls.Add(this.cbLegend);
            this.gbVorlage.Controls.Add(this.checkBoxPrintScheme);
            this.gbVorlage.Controls.Add(this.checkBoxDeckblatt);
            resources.ApplyResources(this.gbVorlage, "gbVorlage");
            this.gbVorlage.Name = "gbVorlage";
            this.gbVorlage.TabStop = false;
            this.gbVorlage.Enter += new System.EventHandler(this.gbVorlage_Enter);
            // 
            // cbLegendLeft
            // 
            resources.ApplyResources(this.cbLegendLeft, "cbLegendLeft");
            this.cbLegendLeft.Name = "cbLegendLeft";
            this.cbLegendLeft.UseVisualStyleBackColor = true;
            this.cbLegendLeft.CheckedChanged += new System.EventHandler(this.cbLegendLeft_CheckedChanged);
            // 
            // checkBoxMystery
            // 
            resources.ApplyResources(this.checkBoxMystery, "checkBoxMystery");
            this.checkBoxMystery.Name = "checkBoxMystery";
            this.checkBoxMystery.UseVisualStyleBackColor = true;
            this.checkBoxMystery.CheckedChanged += new System.EventHandler(this.checkBoxMystery_CheckedChanged);
            // 
            // cbLegend
            // 
            resources.ApplyResources(this.cbLegend, "cbLegend");
            this.cbLegend.Name = "cbLegend";
            this.cbLegend.UseVisualStyleBackColor = true;
            this.cbLegend.CheckedChanged += new System.EventHandler(this.cbLegend_CheckedChanged);
            // 
            // checkBoxPrintScheme
            // 
            resources.ApplyResources(this.checkBoxPrintScheme, "checkBoxPrintScheme");
            this.checkBoxPrintScheme.Name = "checkBoxPrintScheme";
            this.checkBoxPrintScheme.UseVisualStyleBackColor = true;
            this.checkBoxPrintScheme.CheckedChanged += new System.EventHandler(this.checkBoxPrintScheme_CheckedChanged);
            // 
            // checkBoxDeckblatt
            // 
            resources.ApplyResources(this.checkBoxDeckblatt, "checkBoxDeckblatt");
            this.checkBoxDeckblatt.Name = "checkBoxDeckblatt";
            this.checkBoxDeckblatt.UseVisualStyleBackColor = true;
            this.checkBoxDeckblatt.CheckedChanged += new System.EventHandler(this.checkBoxDeckblatt_CheckedChanged);
            // 
            // gbEtiketten
            // 
            this.gbEtiketten.Controls.Add(this.cbWithoutDMC);
            this.gbEtiketten.Controls.Add(this.cbTicTac);
            this.gbEtiketten.Controls.Add(this.txtVO);
            this.gbEtiketten.Controls.Add(this.txtVL);
            this.gbEtiketten.Controls.Add(this.lblOben);
            this.gbEtiketten.Controls.Add(this.lblLinks);
            this.gbEtiketten.Controls.Add(this.checkBoxEtiUmrandung);
            this.gbEtiketten.Controls.Add(this.cbRundEti);
            resources.ApplyResources(this.gbEtiketten, "gbEtiketten");
            this.gbEtiketten.Name = "gbEtiketten";
            this.gbEtiketten.TabStop = false;
            this.gbEtiketten.Enter += new System.EventHandler(this.gbEtiketten_Enter);
            // 
            // cbWithoutDMC
            // 
            resources.ApplyResources(this.cbWithoutDMC, "cbWithoutDMC");
            this.cbWithoutDMC.Name = "cbWithoutDMC";
            this.cbWithoutDMC.UseVisualStyleBackColor = true;
            this.cbWithoutDMC.CheckedChanged += new System.EventHandler(this.cbWithoutDMC_CheckedChanged);
            // 
            // cbTicTac
            // 
            resources.ApplyResources(this.cbTicTac, "cbTicTac");
            this.cbTicTac.Name = "cbTicTac";
            this.cbTicTac.UseVisualStyleBackColor = true;
            this.cbTicTac.CheckedChanged += new System.EventHandler(this.cbTicTac_CheckedChanged);
            // 
            // txtVO
            // 
            resources.ApplyResources(this.txtVO, "txtVO");
            this.txtVO.Name = "txtVO";
            this.txtVO.TextChanged += new System.EventHandler(this.txtVO_TextChanged);
            // 
            // txtVL
            // 
            resources.ApplyResources(this.txtVL, "txtVL");
            this.txtVL.Name = "txtVL";
            this.txtVL.TextChanged += new System.EventHandler(this.txtVL_TextChanged);
            // 
            // lblOben
            // 
            resources.ApplyResources(this.lblOben, "lblOben");
            this.lblOben.Name = "lblOben";
            this.lblOben.Click += new System.EventHandler(this.lblOben_Click);
            // 
            // lblLinks
            // 
            resources.ApplyResources(this.lblLinks, "lblLinks");
            this.lblLinks.Name = "lblLinks";
            this.lblLinks.Click += new System.EventHandler(this.lblLinks_Click);
            // 
            // checkBoxEtiUmrandung
            // 
            resources.ApplyResources(this.checkBoxEtiUmrandung, "checkBoxEtiUmrandung");
            this.checkBoxEtiUmrandung.Name = "checkBoxEtiUmrandung";
            this.checkBoxEtiUmrandung.UseVisualStyleBackColor = true;
            this.checkBoxEtiUmrandung.CheckedChanged += new System.EventHandler(this.checkBoxEtiUmrandung_CheckedChanged);
            // 
            // cbRundEti
            // 
            resources.ApplyResources(this.cbRundEti, "cbRundEti");
            this.cbRundEti.Name = "cbRundEti";
            this.cbRundEti.UseVisualStyleBackColor = true;
            this.cbRundEti.CheckedChanged += new System.EventHandler(this.cbRundEti_CheckedChanged);
            // 
            // lblMysteryPic
            // 
            resources.ApplyResources(this.lblMysteryPic, "lblMysteryPic");
            this.lblMysteryPic.Name = "lblMysteryPic";
            this.lblMysteryPic.Click += new System.EventHandler(this.lblMysteryPic_Click);
            // 
            // tbMysteryPic
            // 
            resources.ApplyResources(this.tbMysteryPic, "tbMysteryPic");
            this.tbMysteryPic.Name = "tbMysteryPic";
            this.tbMysteryPic.TextChanged += new System.EventHandler(this.tbMysteryPic_TextChanged);
            // 
            // btnMyteryPicFile
            // 
            this.btnMyteryPicFile.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.btnMyteryPicFile, "btnMyteryPicFile");
            this.btnMyteryPicFile.FlatAppearance.BorderSize = 0;
            this.btnMyteryPicFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.btnMyteryPicFile.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Open16;
            this.btnMyteryPicFile.IsSelected = false;
            this.btnMyteryPicFile.Name = "btnMyteryPicFile";
            this.btnMyteryPicFile.UseVisualStyleBackColor = false;
            this.btnMyteryPicFile.Click += new System.EventHandler(this.btnMyteryPicFile_Click);
            // 
            // labelLogo
            // 
            resources.ApplyResources(this.labelLogo, "labelLogo");
            this.labelLogo.Name = "labelLogo";
            this.labelLogo.Click += new System.EventHandler(this.labelLogo_Click);
            // 
            // textBoxLogo
            // 
            resources.ApplyResources(this.textBoxLogo, "textBoxLogo");
            this.textBoxLogo.Name = "textBoxLogo";
            this.textBoxLogo.TextChanged += new System.EventHandler(this.textBoxLogo_TextChanged);
            // 
            // buttonSelectLogoLocation
            // 
            this.buttonSelectLogoLocation.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.buttonSelectLogoLocation, "buttonSelectLogoLocation");
            this.buttonSelectLogoLocation.FlatAppearance.BorderSize = 0;
            this.buttonSelectLogoLocation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            this.buttonSelectLogoLocation.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Open16;
            this.buttonSelectLogoLocation.IsSelected = false;
            this.buttonSelectLogoLocation.Name = "buttonSelectLogoLocation";
            this.buttonSelectLogoLocation.UseVisualStyleBackColor = false;
            this.buttonSelectLogoLocation.Click += new System.EventHandler(this.buttonSelectLogoLocation_Click);
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
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // textBoxPictureName
            // 
            resources.ApplyResources(this.textBoxPictureName, "textBoxPictureName");
            this.textBoxPictureName.Name = "textBoxPictureName";
            this.textBoxPictureName.TextChanged += new System.EventHandler(this.textBoxPictureName_TextChanged);
            // 
            // labelPictureName
            // 
            resources.ApplyResources(this.labelPictureName, "labelPictureName");
            this.labelPictureName.Name = "labelPictureName";
            this.labelPictureName.Click += new System.EventHandler(this.labelPictureName_Click);
            // 
            // txtPDFPassword
            // 
            resources.ApplyResources(this.txtPDFPassword, "txtPDFPassword");
            this.txtPDFPassword.Name = "txtPDFPassword";
            this.txtPDFPassword.TextChanged += new System.EventHandler(this.txtPDFPassword_TextChanged);
            // 
            // txtOwnerPassword
            // 
            resources.ApplyResources(this.txtOwnerPassword, "txtOwnerPassword");
            this.txtOwnerPassword.Name = "txtOwnerPassword";
            this.txtOwnerPassword.TextChanged += new System.EventHandler(this.txtOwnerPassword_TextChanged);
            // 
            // txtOwner
            // 
            resources.ApplyResources(this.txtOwner, "txtOwner");
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.TextChanged += new System.EventHandler(this.txtOwner_TextChanged);
            // 
            // txtCompany
            // 
            resources.ApplyResources(this.txtCompany, "txtCompany");
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.TextChanged += new System.EventHandler(this.txtCompany_TextChanged);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            this.label15.Click += new System.EventHandler(this.label15_Click);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // labelComapny
            // 
            resources.ApplyResources(this.labelComapny, "labelComapny");
            this.labelComapny.Name = "labelComapny";
            this.labelComapny.Click += new System.EventHandler(this.labelComapny_Click);
            // 
            // PDFSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelTop);
            this.Name = "PDFSettings";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbLegendLogo.ResumeLayout(false);
            this.gbLegendLogo.PerformLayout();
            this.gbLines.ResumeLayout(false);
            this.gbLines.PerformLayout();
            this.gbPDFSettings.ResumeLayout(false);
            this.gbPDFSettings.PerformLayout();
            this.gpSymbolSize.ResumeLayout(false);
            this.gpSymbolSize.PerformLayout();
            this.gbVorlage.ResumeLayout(false);
            this.gbVorlage.PerformLayout();
            this.gbEtiketten.ResumeLayout(false);
            this.gbEtiketten.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtPDFPassword;
        private System.Windows.Forms.TextBox txtOwnerPassword;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label labelComapny;
        private System.Windows.Forms.TextBox textBoxPictureName;
        private System.Windows.Forms.Label labelPictureName;
        private Drawing.WinForms.FlatDialogButton buttonOk;
        private Drawing.WinForms.FlatButton buttonSelectLogoLocation;
        private System.Windows.Forms.TextBox textBoxLogo;
        private System.Windows.Forms.Label labelLogo;
        private System.Windows.Forms.Label lblMysteryPic;
        private System.Windows.Forms.TextBox tbMysteryPic;
        private Drawing.WinForms.FlatButton btnMyteryPicFile;
        private System.Windows.Forms.GroupBox gpSymbolSize;
        private System.Windows.Forms.GroupBox gbVorlage;
        private System.Windows.Forms.CheckBox checkBoxMystery;
        private System.Windows.Forms.CheckBox checkBoxPrintScheme;
        private System.Windows.Forms.CheckBox checkBoxDeckblatt;
        private System.Windows.Forms.GroupBox gbEtiketten;
        private System.Windows.Forms.CheckBox cbTicTac;
        private System.Windows.Forms.TextBox txtVO;
        private System.Windows.Forms.TextBox txtVL;
        private System.Windows.Forms.Label lblOben;
        private System.Windows.Forms.Label lblLinks;
        private System.Windows.Forms.CheckBox checkBoxEtiUmrandung;
        private System.Windows.Forms.CheckBox cbRundEti;
        private System.Windows.Forms.CheckBox cbSymSizeFull;
        private System.Windows.Forms.CheckBox cbSymSizeNormal;
        private System.Windows.Forms.GroupBox gbPDFSettings;
        private System.Windows.Forms.CheckBox checkBoxPermitFullQualityPrint;
        private System.Windows.Forms.CheckBox checkBoxPermitFormsFill;
        private System.Windows.Forms.CheckBox checkBoxPermitAnnotations;
        private System.Windows.Forms.CheckBox checkBoxPermitAccessibilityExtractContent;
        private System.Windows.Forms.CheckBox checkBoxPermitExtractContent;
        private System.Windows.Forms.CheckBox checkBoxPermitAssembleDocument;
        private System.Windows.Forms.CheckBox checkBoxPermitModifyDocument;
        private System.Windows.Forms.CheckBox checkBoxPermitPrint;
        private System.Windows.Forms.GroupBox gbLines;
        private System.Windows.Forms.CheckBox cbLineal;
        private System.Windows.Forms.CheckBox cbSchnittlinien;
        private System.Windows.Forms.CheckBox cbLine10;
        private System.Windows.Forms.CheckBox cbGitternetz;
        private Drawing.WinForms.FlatDialogButton buttonCancel;
        private System.Windows.Forms.GroupBox gbLegendLogo;
        private System.Windows.Forms.CheckBox cbLogo;
        private System.Windows.Forms.CheckBox cbLegend;
        private System.Windows.Forms.CheckBox cbLegendLeft;
        private System.Windows.Forms.CheckBox cbWithoutDMC;
    }
}