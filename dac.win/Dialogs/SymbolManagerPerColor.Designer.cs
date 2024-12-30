
namespace Ravlyk.SAE5.WinForms.Dialogs
{
    partial class SymbolManagerPerColor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SymbolManagerPerColor));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSymbol = new System.Windows.Forms.TabPage();
            this.scrollControlPerColor = new Ravlyk.Drawing.WinForms.ScrollControl();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageSymbol.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSymbol);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPageSymbol
            // 
            this.tabPageSymbol.Controls.Add(this.scrollControlPerColor);
            resources.ApplyResources(this.tabPageSymbol, "tabPageSymbol");
            this.tabPageSymbol.Name = "tabPageSymbol";
            this.tabPageSymbol.UseVisualStyleBackColor = true;
            // 
            // scrollControlPerColor
            // 
            this.scrollControlPerColor.Controller = null;
            resources.ApplyResources(this.scrollControlPerColor, "scrollControlPerColor");
            this.scrollControlPerColor.Name = "scrollControlPerColor";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SymbolManagerPerColor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabControl1);
            this.Name = "SymbolManagerPerColor";
            this.tabControl1.ResumeLayout(false);
            this.tabPageSymbol.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSymbol;
        private Drawing.WinForms.ScrollControl scrollControlPerColor;
        private System.Windows.Forms.Button button1;
    }
}