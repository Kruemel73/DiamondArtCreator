using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ravlyk.Common;
using Ravlyk.Drawing.ImageProcessor;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Grid;
using Ravlyk.SAE.Drawing.Processor;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE.Resources;
using Ravlyk.SAE5.WinForms.Dialogs;
using Ravlyk.SAE5.WinForms.Properties;

using SAEWizard = Ravlyk.Drawing.WinForms.SAEWizard;
using Size = Ravlyk.Common.Size;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
    public partial class SymbolManagerPerColor : Form
    {
        public SymbolManagerPerColor(IList<CodedColor> colorsList)
        {
            InitializeComponent();

            scrollControlPerColor.Controller = new VisualSymbolsController(Wizard.ImageSymboler, new Size(scrollControlPerColor.Width, scrollControlPerColor.Height));
            scrollControlPerColor.Controller.VisualImageChanged += Symbols_VisualImageChanged;
            wizard.ImageSymboler.ClearAllSelection();
            wizard.ImageSymboler.MaxSelectedSymbols = 1;
            
        }

        public SAEWizard Wizard
        {
            get
            {
                if (wizard == null)
                {
                    wizard = new SAEWizard();
                    wizard.SetPalettes(SAEResources.GetAllPalettes(Settings.Default.UserPalettesLocationSafe), "DMC");
                    wizard.SetSymbolFonts(SAEResources.GetAllFonts());
                }
                return wizard;
            }

        }

        SAEWizard wizard;
        public string filename;


        void Symbols_VisualImageChanged(object sender, EventArgs e)
        {
            //labelSelectedNo.Text = Wizard.ImageSymboler.SelectedCount + " Symbole ausgewählt";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wizard.ImageSymboler.GetSymbolKey();
            Settings.Default.Symbol = Ravlyk.SAE.Drawing.Properties.SAEWizardSettings.Default.ChoosenSymbol;
            button1.Enabled = true;
            this.Close();
        }

        public char SelectedSymbol => wizard.ImageSymboler.SelectedCount > 0 ? wizard.ImageSymboler.SelectedSymbols.First() : ' ';
    }
}
