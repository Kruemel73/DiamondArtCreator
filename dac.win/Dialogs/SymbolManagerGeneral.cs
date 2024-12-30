using System;
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
    public partial class SymbolManagerGeneral : Form
    {
        public SymbolManagerGeneral()
        {
            InitializeComponent();

            scrollControlGeneralSymbols.Controller = new VisualSymbolsController(Wizard.ImageSymboler, new Size(scrollControlGeneralSymbols.Width, scrollControlGeneralSymbols.Height));
            scrollControlGeneralSymbols.Controller.VisualImageChanged += Symbols_VisualImageChanged;
            LoadSymbols();
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
            labelSelectedNo.Text = Wizard.ImageSymboler.SelectedCount + " Symbole ausgewählt";
        }

        private void LoadSymbols()
        {
            wizard.ImageSymboler.ClearAllSelection();

            filename = Settings.Default.UserPalettesLocation + @"\os.txt";

            if (File.Exists(filename))
            {
                var lineCount = File.ReadAllLines(filename).Count();
                if (lineCount == 0)
                {
                    return;
                }
                var lines = File.ReadAllLines(filename);
                int i = 0; // counters
                string[] result = new string[lineCount]; // set array
                foreach (var line in lines)
                {
                    result[i] = line;
                    string sym = result[i];
                    i++;
                }

                wizard.ImageSymboler.SetSymbolsAfterLoad(result);
            }
            else
            {
                return;
            }
        }

        private void buttonRandom_Click(object sender, EventArgs e)
        {
            using (Wizard.ImageSymboler.SuspendCallManipulations())
            {
                Wizard.ImageSymboler.ClearAllSelection();
                if (textBoxRandom.Text != "")
                {
                    Wizard.ImageSymboler.AddRandomSymbolsWithoutImage(Int32.Parse(textBoxRandom.Text));
                }
            }
        }

        private void textBoxRandom_TextChanged(object sender, EventArgs e)
        {
            if (textBoxRandom.Text == "") { return; }
            var count = Int32.Parse(textBoxRandom.Text);
            if (count > 455) { textBoxRandom.Text = "455"; }
            if (count < 0 ) { textBoxRandom.Text = "0"; }
        }

        private void flatButtonDeselect_Click(object sender, EventArgs e)
        {
            Wizard.ImageSymboler.ClearAllSelection();
        }

        private void flatButtonSaveSymbols_Click(object sender, EventArgs e)
        {
            // schreiben in Datei os
                if (!Directory.Exists(Settings.Default.UserPalettesLocation))
                {
                    Directory.CreateDirectory(Settings.Default.UserPalettesLocation);
                }
                filename = Settings.Default.UserPalettesLocation + @"\os.txt";

            if (File.Exists(filename)) { File.Delete(filename); }
            using (FileStream fs = File.OpenWrite(filename))
            {
                StreamWriter sw = new StreamWriter(fs);
                foreach (var color in wizard.ImageSymboler.SelectedSymbols)
                {
                    string exp = color.ToString();
                    sw.WriteLine(exp);
                }
                sw.Close();
            }
        }
    }

}
