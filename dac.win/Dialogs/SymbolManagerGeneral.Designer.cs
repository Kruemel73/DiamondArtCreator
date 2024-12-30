
namespace Ravlyk.SAE5.WinForms.Dialogs
{
    partial class SymbolManagerGeneral
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolManagerGeneral));
            this.labelSelectedNo = new System.Windows.Forms.Label();
            this.tabControlGeneralSymbols = new System.Windows.Forms.TabControl();
            this.tabPageGeneralSymbols = new System.Windows.Forms.TabPage();
            this.scrollControlGeneralSymbols = new Ravlyk.Drawing.WinForms.ScrollControl();
            this.labelRandom = new System.Windows.Forms.Label();
            this.textBoxRandom = new System.Windows.Forms.TextBox();
            this.flatButtonSaveSymbols = new Ravlyk.Drawing.WinForms.FlatButton();
            this.flatButtonDeselect = new Ravlyk.Drawing.WinForms.FlatButton();
            this.buttonRandom = new Ravlyk.Drawing.WinForms.FlatButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControlGeneralSymbols.SuspendLayout();
            this.tabPageGeneralSymbols.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelSelectedNo
            // 
            resources.ApplyResources(this.labelSelectedNo, "labelSelectedNo");
            this.labelSelectedNo.Name = "labelSelectedNo";
            // 
            // tabControlGeneralSymbols
            // 
            this.tabControlGeneralSymbols.Controls.Add(this.tabPageGeneralSymbols);
            resources.ApplyResources(this.tabControlGeneralSymbols, "tabControlGeneralSymbols");
            this.tabControlGeneralSymbols.Name = "tabControlGeneralSymbols";
            this.tabControlGeneralSymbols.SelectedIndex = 0;
            // 
            // tabPageGeneralSymbols
            // 
            this.tabPageGeneralSymbols.Controls.Add(this.scrollControlGeneralSymbols);
            resources.ApplyResources(this.tabPageGeneralSymbols, "tabPageGeneralSymbols");
            this.tabPageGeneralSymbols.Name = "tabPageGeneralSymbols";
            this.tabPageGeneralSymbols.UseVisualStyleBackColor = true;
            // 
            // scrollControlGeneralSymbols
            // 
            this.scrollControlGeneralSymbols.Controller = null;
            resources.ApplyResources(this.scrollControlGeneralSymbols, "scrollControlGeneralSymbols");
            this.scrollControlGeneralSymbols.Name = "scrollControlGeneralSymbols";
            // 
            // labelRandom
            // 
            resources.ApplyResources(this.labelRandom, "labelRandom");
            this.labelRandom.Name = "labelRandom";
            // 
            // textBoxRandom
            // 
            resources.ApplyResources(this.textBoxRandom, "textBoxRandom");
            this.textBoxRandom.Name = "textBoxRandom";
            this.textBoxRandom.TextChanged += new System.EventHandler(this.textBoxRandom_TextChanged);
            // 
            // flatButtonSaveSymbols
            // 
            this.flatButtonSaveSymbols.BackColor = System.Drawing.Color.White;
            this.flatButtonSaveSymbols.FlatAppearance.CheckedBackColor = System.Drawing.Color.Coral;
            this.flatButtonSaveSymbols.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSlateGray;
            this.flatButtonSaveSymbols.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.flatButtonSaveSymbols, "flatButtonSaveSymbols");
            this.flatButtonSaveSymbols.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.SymbolSpeichern_2_32;
            this.flatButtonSaveSymbols.IsSelected = false;
            this.flatButtonSaveSymbols.Name = "flatButtonSaveSymbols";
            this.flatButtonSaveSymbols.UseVisualStyleBackColor = true;
            this.flatButtonSaveSymbols.Click += new System.EventHandler(this.flatButtonSaveSymbols_Click);
            // 
            // flatButtonDeselect
            // 
            this.flatButtonDeselect.BackColor = System.Drawing.Color.White;
            this.flatButtonDeselect.FlatAppearance.CheckedBackColor = System.Drawing.Color.Coral;
            this.flatButtonDeselect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSlateGray;
            this.flatButtonDeselect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.flatButtonDeselect, "flatButtonDeselect");
            this.flatButtonDeselect.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.clear_32;
            this.flatButtonDeselect.IsSelected = false;
            this.flatButtonDeselect.Name = "flatButtonDeselect";
            this.flatButtonDeselect.UseVisualStyleBackColor = true;
            this.flatButtonDeselect.Click += new System.EventHandler(this.flatButtonDeselect_Click);
            // 
            // buttonRandom
            // 
            this.buttonRandom.BackColor = System.Drawing.Color.White;
            this.buttonRandom.FlatAppearance.CheckedBackColor = System.Drawing.Color.Coral;
            this.buttonRandom.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSlateGray;
            this.buttonRandom.FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.buttonRandom, "buttonRandom");
            this.buttonRandom.Image = global::Ravlyk.SAE5.WinForms.Properties.Resources.Random_32;
            this.buttonRandom.IsSelected = false;
            this.buttonRandom.Name = "buttonRandom";
            this.buttonRandom.UseVisualStyleBackColor = true;
            this.buttonRandom.Click += new System.EventHandler(this.buttonRandom_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flatButtonDeselect);
            this.panel1.Controls.Add(this.labelSelectedNo);
            this.panel1.Controls.Add(this.textBoxRandom);
            this.panel1.Controls.Add(this.labelRandom);
            this.panel1.Controls.Add(this.flatButtonSaveSymbols);
            this.panel1.Controls.Add(this.buttonRandom);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControlGeneralSymbols);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // SymbolManagerGeneral
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "SymbolManagerGeneral";
            this.tabControlGeneralSymbols.ResumeLayout(false);
            this.tabPageGeneralSymbols.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelSelectedNo;
        private System.Windows.Forms.TabControl tabControlGeneralSymbols;
        private System.Windows.Forms.TabPage tabPageGeneralSymbols;
        private Drawing.WinForms.ScrollControl scrollControlGeneralSymbols;
        private Drawing.WinForms.FlatButton buttonRandom;
        private System.Windows.Forms.Label labelRandom;
        private System.Windows.Forms.TextBox textBoxRandom;
        private Drawing.WinForms.FlatButton flatButtonDeselect;
        private Drawing.WinForms.FlatButton flatButtonSaveSymbols;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}